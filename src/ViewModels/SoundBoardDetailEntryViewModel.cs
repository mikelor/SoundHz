using SoundHz.Messaging;

namespace SoundHz.ViewModels;

/// <summary>
/// Provides the presentation logic for capturing details of a new sound board entry.
/// </summary>
public partial class SoundBoardDetailEntryViewModel(ISoundBoardStorageService storageService, ILogger<SoundBoardDetailEntryViewModel> logger, IWeakReferenceMessenger messenger) : BaseViewModel
{
    private readonly ISoundBoardStorageService storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
    private readonly ILogger<SoundBoardDetailEntryViewModel> logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IWeakReferenceMessenger messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));

    /// <summary>
    /// Gets or sets the title entered for the new sound board.
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    private string title = string.Empty;

    /// <summary>
    /// Gets or sets the description entered for the new sound board.
    /// </summary>
    [ObservableProperty]
    private string description = string.Empty;

    /// <summary>
    /// Adds the sound board defined by the user to persistent storage.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanAdd))]
    private async Task AddAsync()
    {
        try
        {
            var description = Description ?? string.Empty;
            var newSoundBoard = new SoundBoard(Title.Trim(), description);
            await storageService.AddSoundBoardAsync(newSoundBoard).ConfigureAwait(false);
            messenger.Send(new SoundBoardAddedMessage(newSoundBoard));
            await Shell.Current.GoToAsync("..", true).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add a new sound board entry.");
            throw;
        }
    }

    /// <summary>
    /// Cancels the creation of a new sound board and returns to the previous page.
    /// </summary>
    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..", true).ConfigureAwait(false);
    }

    private bool CanAdd() => !string.IsNullOrWhiteSpace(Title);
}
