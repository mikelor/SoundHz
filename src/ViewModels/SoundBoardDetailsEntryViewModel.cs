using System;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.ApplicationModel;
using SoundHz.Messaging;

namespace SoundHz.ViewModels;

/// <summary>
/// Provides the presentation logic for creating a new <see cref="SoundBoard"/>.
/// </summary>
public partial class SoundBoardDetailsEntryViewModel : BaseViewModel
{
    private readonly ISoundBoardStorageService storageService;
    private readonly IWeakReferenceMessenger messenger;
    private readonly ILogger<SoundBoardDetailsEntryViewModel> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SoundBoardDetailsEntryViewModel"/> class.
    /// </summary>
    /// <param name="storageService">The service used to persist sound boards.</param>
    /// <param name="messenger">The messenger used to broadcast sound board changes.</param>
    /// <param name="logger">The logger used to record diagnostic information.</param>
    public SoundBoardDetailsEntryViewModel(ISoundBoardStorageService storageService, IWeakReferenceMessenger messenger, ILogger<SoundBoardDetailsEntryViewModel> logger)
    {
        this.storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        this.messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets or sets the title entered by the user.
    /// </summary>
    [ObservableProperty]
    private string title = string.Empty;

    /// <summary>
    /// Gets or sets the description entered by the user.
    /// </summary>
    [ObservableProperty]
    private string description = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether a save operation is in progress.
    /// </summary>
    [ObservableProperty]
    private bool isBusy;

    partial void OnTitleChanged(string value) => AddCommand.NotifyCanExecuteChanged();

    partial void OnDescriptionChanged(string value) => AddCommand.NotifyCanExecuteChanged();

    partial void OnIsBusyChanged(bool value) => AddCommand.NotifyCanExecuteChanged();

    /// <summary>
    /// Adds the newly entered sound board to storage and notifies listeners.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [RelayCommand(CanExecute = nameof(CanAddSoundBoard))]
    private async Task AddAsync()
    {
        if (!CanAddSoundBoard())
        {
            return;
        }

        try
        {
            IsBusy = true;
            var newSoundBoard = new SoundBoard(Title.Trim(), Description.Trim());
            await storageService.AddSoundBoardAsync(newSoundBoard).ConfigureAwait(false);
            messenger.Send(new SoundBoardAddedMessage(new SoundBoard(newSoundBoard)));
            await NavigateBackAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add a new sound board.");
            await ShowErrorAsync().ConfigureAwait(false);
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Cancels the creation workflow and navigates back to the previous page.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [RelayCommand]
    private Task CancelAsync() => NavigateBackAsync();

    private bool CanAddSoundBoard() => !string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(Description) && !IsBusy;

    private static Task NavigateBackAsync()
    {
        return MainThread.InvokeOnMainThreadAsync(() =>
        {
            if (Shell.Current is null)
            {
                return Task.CompletedTask;
            }

            return Shell.Current.GoToAsync("..", true);
        });
    }

    private Task ShowErrorAsync()
    {
        return MainThread.InvokeOnMainThreadAsync(() =>
        {
            if (Shell.Current is null)
            {
                return Task.CompletedTask;
            }

            return Shell.Current.DisplayAlert("Error", "We could not add the sound board. Please try again.", "OK");
        });
    }
}
