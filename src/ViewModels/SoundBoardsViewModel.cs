using CommunityToolkit.Mvvm.Messaging;
using SoundHz.Messaging;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace SoundHz.ViewModels;

/// <summary>
/// Provides the presentation logic for displaying and persisting sound boards.
/// </summary>
public partial class SoundBoardsViewModel : BaseViewModel, IRecipient<SoundBoardAddedMessage>
{
    private const int BrowseTabIndex = 0;
    private const int AddTabIndex = 1;

    private readonly ISoundBoardStorageService storageService;
    private readonly ILogger<SoundBoardsViewModel> logger;
    private readonly IWeakReferenceMessenger messenger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SoundBoardsViewModel"/> class.
    /// </summary>
    /// <param name="storageService">The service that persists sound boards.</param>
    /// <param name="logger">The logger used to record diagnostic information.</param>
    /// <param name="messenger">The messenger used to receive notifications from other view models.</param>
    public SoundBoardsViewModel(ISoundBoardStorageService storageService, ILogger<SoundBoardsViewModel> logger, IWeakReferenceMessenger messenger)
    {
        this.storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));

        this.messenger.Register<SoundBoardsViewModel, SoundBoardAddedMessage>(this);
    }

    /// <summary>
    /// Gets or sets the collection of sound boards displayed in the user interface.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<SoundBoard>? items;

    /// <summary>
    /// Gets or sets a value indicating whether a refresh operation is currently in progress.
    /// </summary>
    [ObservableProperty]
    private bool isRefreshing;

    /// <summary>
    /// Gets or sets the currently selected tab index for the page-level tab bar.
    /// </summary>
    [ObservableProperty]
    private int selectedTabIndex = BrowseTabIndex;

    /// <summary>
    /// Called prior to updating the <see cref="Items"/> collection so that change listeners can be detached.
    /// </summary>
    /// <param name="value">The incoming collection instance.</param>
    partial void OnItemsChanging(ObservableCollection<SoundBoard>? value)
    {
        _ = value;

        if (Items is not null)
        {
            UnsubscribeFromCollection(Items);
        }
    }

    /// <summary>
    /// Called after the <see cref="Items"/> collection has changed to attach listeners to the new instance.
    /// </summary>
    /// <param name="value">The new collection instance.</param>
    partial void OnItemsChanged(ObservableCollection<SoundBoard>? value)
    {
        if (value is not null)
        {
            SubscribeToCollection(value);
        }
    }

    /// <summary>
    /// Handles updates when the <see cref="SelectedTabIndex"/> property changes.
    /// </summary>
    /// <param name="value">The updated selected index.</param>
    partial void OnSelectedTabIndexChanged(int value)
    {
        if (value == AddTabIndex)
        {
            AddSoundBoardCommand.Execute(null);
            SelectedTabIndex = BrowseTabIndex;
        }
    }

    /// <summary>
    /// Loads sound boards from persistent storage and updates the bound collection.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    public async Task LoadDataAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var soundBoards = await storageService.GetSoundBoardsAsync(cancellationToken).ConfigureAwait(false);
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
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// Navigates to the sound board entry page to create a new sound board.
    /// </summary>
    [RelayCommand]
    private async Task AddSoundBoardAsync()
    {
        await Shell.Current.GoToAsync(nameof(SoundBoardDetailEntryPage), true).ConfigureAwait(false);
    }

    /// <summary>
    /// Refreshes the list of sound boards.
    /// </summary>
    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task RefreshingAsync()
    {
        try
        {
            IsRefreshing = true;
            await LoadDataAsync().ConfigureAwait(false);
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

    /// <inheritdoc />
    public void Receive(SoundBoardAddedMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);

        Items ??= new ObservableCollection<SoundBoard>();
        Items.Add(new SoundBoard(message.Value));

        QueuePersistence();
    }

    private void SubscribeToCollection(ObservableCollection<SoundBoard> collection)
    {
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
