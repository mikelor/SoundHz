using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace SoundHz.ViewModels;

/// <summary>
/// Provides the presentation logic for displaying and persisting sound boards.
/// </summary>
public partial class SoundBoardsViewModel(ISoundBoardStorageService storageService, ILogger<SoundBoardsViewModel> logger) : BaseViewModel
{
    private readonly ISoundBoardStorageService storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
    private readonly ILogger<SoundBoardsViewModel> logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private ObservableCollection<SoundBoard>? items;
    private bool isRefreshing;

    /// <summary>
    /// Gets or sets a value indicating whether a refresh operation is currently in progress.
    /// </summary>
    public bool IsRefreshing
    {
        get => isRefreshing;
        set => SetProperty(ref isRefreshing, value);
    }

    /// <summary>
    /// Gets the collection of sound boards displayed in the user interface.
    /// </summary>
    public ObservableCollection<SoundBoard>? Items
    {
        get => items;
        private set
        {
            if (items == value)
            {
                return;
            }

            if (items is not null)
            {
                UnsubscribeFromCollection(items);
            }

            if (SetProperty(ref items, value) && value is not null)
            {
                SubscribeToCollection(value);
            }
        }
    }

    /// <summary>
    /// Gets the command used to trigger a refresh of the sound board list.
    /// </summary>
    public IAsyncRelayCommand RefreshingCommand => OnRefreshingCommand;

    /// <summary>
    /// Loads sound boards from persistent storage and updates the bound collection.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    public async Task LoadDataAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var soundBoards = await storageService.GetSoundBoardsAsync(cancellationToken);
            Items = new ObservableCollection<SoundBoard>(soundBoards.Select(board => new SoundBoard(board)));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load sound boards from storage.");
            throw;
        }
    }

    /// <summary>
    /// Navigates to the detail page for the provided sound board.
    /// </summary>
    /// <param name="soundBoard">The sound board whose details should be displayed.</param>
    [RelayCommand]
    private async Task GoToDetails(SoundBoard soundBoard)
    {
        ArgumentNullException.ThrowIfNull(soundBoard);

        await Shell.Current.GoToAsync(nameof(SoundBoardsDetailPage), true, new Dictionary<string, object>
        {
            { "Item", soundBoard }
        });
    }

    /// <summary>
    /// Refreshes the list of sound boards.
    /// </summary>
    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task OnRefreshing()
    {
        try
        {
            IsRefreshing = true;
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while refreshing sound boards.");
            throw;
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    private void SubscribeToCollection(ObservableCollection<SoundBoard>? collection)
    {
        if (collection is null)
        {
            return;
        }

        collection.CollectionChanged += OnItemsCollectionChanged;

        foreach (var soundBoard in collection)
        {
            soundBoard.PropertyChanged += OnSoundBoardPropertyChanged;
        }
    }

    private void UnsubscribeFromCollection(ObservableCollection<SoundBoard> collection)
    {
        collection.CollectionChanged -= OnItemsCollectionChanged;

        foreach (var soundBoard in collection)
        {
            soundBoard.PropertyChanged -= OnSoundBoardPropertyChanged;
        }
    }

    private void OnItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (var item in e.OldItems.OfType<SoundBoard>())
            {
                item.PropertyChanged -= OnSoundBoardPropertyChanged;
            }
        }

        if (e.NewItems is not null)
        {
            foreach (var item in e.NewItems.OfType<SoundBoard>())
            {
                item.PropertyChanged += OnSoundBoardPropertyChanged;
            }
        }

        if (e.Action == NotifyCollectionChangedAction.Reset && sender is ObservableCollection<SoundBoard> collection)
        {
            foreach (var soundBoard in collection)
            {
                soundBoard.PropertyChanged -= OnSoundBoardPropertyChanged;
                soundBoard.PropertyChanged += OnSoundBoardPropertyChanged;
            }
        }

        QueuePersistence();
    }

    private void OnSoundBoardPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(SoundBoard.Title) or nameof(SoundBoard.Description))
        {
            QueuePersistence();
        }
    }

    private void QueuePersistence()
    {
        _ = PersistSoundBoardsAsync();
    }

    private async Task PersistSoundBoardsAsync()
    {
        if (Items is null)
        {
            return;
        }

        var snapshot = Items.Select(item => item.Clone()).ToList();

        try
        {
            await storageService.SaveSoundBoardsAsync(snapshot).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to persist sound board changes.");
        }
    }
}
