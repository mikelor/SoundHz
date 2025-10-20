using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.ApplicationModel;
using SoundHz.ViewModels.Messages;

namespace SoundHz.ViewModels;

/// <summary>
/// Provides the presentation logic for displaying and persisting sound boards.
/// </summary>
public partial class SoundBoardsViewModel
    (ISoundBoardStorageService storageService, ILogger<SoundBoardsViewModel> logger, IMessenger messenger) : BaseViewModel
{
    private readonly ISoundBoardStorageService storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
    private readonly ILogger<SoundBoardsViewModel> logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMessenger messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
    private bool isMessengerRegistered;

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
    /// Loads sound boards from persistent storage and updates the bound collection.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    public async Task LoadDataAsync(CancellationToken cancellationToken = default)
    {
        EnsureMessengerRegistration();

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
    /// Navigates to the page for creating a new sound board entry.
    /// </summary>
    [RelayCommand]
    private static async Task ShowAddNewAsync() => await Shell.Current.GoToAsync(nameof(SoundBoardDetailEntryPage));

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

    /// <summary>
    /// Handles notifications that a new sound board has been created.
    /// </summary>
    /// <param name="message">The message containing the created sound board.</param>
    private void OnSoundBoardAdded(SoundBoardAddedMessage message)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var items = Items ??= new ObservableCollection<SoundBoard>();
            items.Add(new SoundBoard(message.Value));
        });
    }

    /// <summary>
    /// Ensures that the view model is registered to receive notifications for newly created sound boards.
    /// </summary>
    private void EnsureMessengerRegistration()
    {
        if (isMessengerRegistered)
        {
            return;
        }

        messenger.Register<SoundBoardsViewModel, SoundBoardAddedMessage>(this, static (recipient, message) => recipient.OnSoundBoardAdded(message));
        isMessengerRegistered = true;
    }
}
