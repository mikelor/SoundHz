using CommunityToolkit.Mvvm.Messaging;

using SoundHz.Models;
using SoundHz.ViewModels.Messages;

namespace SoundHz.ViewModels;

/// <summary>
/// Provides presentation logic for capturing new sound board details from the user.
/// </summary>
public partial class SoundBoardDetailEntryViewModel
    (ISoundBoardStorageService storageService, IMessenger messenger, ILogger<SoundBoardDetailEntryViewModel> logger) : BaseViewModel
{
    private readonly ISoundBoardStorageService storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
    private readonly IMessenger messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
    private readonly ILogger<SoundBoardDetailEntryViewModel> logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Gets or sets the title for the new sound board.
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    private string title = string.Empty;

    /// <summary>
    /// Gets or sets the description for the new sound board.
    /// </summary>
    [ObservableProperty]
    private string description = string.Empty;

    /// <summary>
    /// Resets the entry form to its default state.
    /// </summary>
    public void Reset()
    {
        Title = string.Empty;
        Description = string.Empty;
    }

    /// <summary>
    /// Adds a new sound board using the provided form data.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanAdd))]
    private async Task AddAsync()
    {
        try
        {
            var sanitizedTitle = Title.Trim();
            Title = sanitizedTitle;
            var soundBoard = new SoundBoard(sanitizedTitle, Description);

            await storageService.AddSoundBoardAsync(soundBoard);
            messenger.Send(new SoundBoardAddedMessage(soundBoard));

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add a new sound board.");
            throw;
        }
    }

    /// <summary>
    /// Cancels entry and returns to the previous page.
    /// </summary>
    [RelayCommand]
    private static async Task CancelAsync() => await Shell.Current.GoToAsync("..");

    /// <summary>
    /// Determines whether the add operation can be executed with the current form state.
    /// </summary>
    /// <returns><see langword="true"/> when the form contains a valid title; otherwise, <see langword="false"/>.</returns>
    private bool CanAdd() => !string.IsNullOrWhiteSpace(Title);
}
