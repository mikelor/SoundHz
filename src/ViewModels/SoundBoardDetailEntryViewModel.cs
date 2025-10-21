using CommunityToolkit.Mvvm.Messaging;

using SoundHz.Models;
using SoundHz.ViewModels.Messages;

namespace SoundHz.ViewModels;

/// <summary>
/// Provides presentation logic for capturing and editing sound board details.
/// </summary>
public partial class SoundBoardDetailEntryViewModel
    (ISoundBoardStorageService storageService, IMessenger messenger, ILogger<SoundBoardDetailEntryViewModel> logger) : BaseViewModel
{
    private readonly ISoundBoardStorageService storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
    private readonly IMessenger messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
    private readonly ILogger<SoundBoardDetailEntryViewModel> logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private SoundBoard? editingSoundBoard;

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
    /// Gets or sets the title displayed in the page chrome.
    /// </summary>
    [ObservableProperty]
    private string pageTitle = "Add New SoundBoard";

    /// <summary>
    /// Gets or sets the label for the primary action button.
    /// </summary>
    [ObservableProperty]
    private string primaryActionText = "Add";

    /// <summary>
    /// Resets the entry form to its default state.
    /// </summary>
    public void PrepareForCreate()
    {
        editingSoundBoard = null;
        Title = string.Empty;
        Description = string.Empty;
        PageTitle = "Add New SoundBoard";
        PrimaryActionText = "Add";
    }

    /// <summary>
    /// Initializes the form for editing the provided sound board instance.
    /// </summary>
    /// <param name="soundBoard">The sound board being edited.</param>
    public void PrepareForEdit(SoundBoard soundBoard)
    {
        ArgumentNullException.ThrowIfNull(soundBoard);

        editingSoundBoard = soundBoard;
        Title = soundBoard.Title;
        Description = soundBoard.Description;
        PageTitle = "Edit SoundBoard";
        PrimaryActionText = "Update";
    }

    /// <summary>
    /// Adds or updates a sound board using the provided form data.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanAdd))]
    private async Task AddAsync()
    {
        try
        {
            var sanitizedTitle = Title.Trim();
            Title = sanitizedTitle;
            if (editingSoundBoard is null)
            {
                var soundBoard = new SoundBoard(sanitizedTitle, Description);

                await storageService.AddSoundBoardAsync(soundBoard).ConfigureAwait(false);
                messenger.Send(new SoundBoardAddedMessage(soundBoard));
            }
            else
            {
                editingSoundBoard.Title = sanitizedTitle;
                editingSoundBoard.Description = Description;
            }

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, editingSoundBoard is null ? "Failed to add a new sound board." : "Failed to update the sound board.");
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
