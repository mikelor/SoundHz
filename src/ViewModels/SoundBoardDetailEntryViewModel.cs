using Microsoft.Maui.ApplicationModel;
using SoundHz.Messages;

namespace SoundHz.ViewModels;

/// <summary>
/// Provides the presentation logic for creating a new <see cref="SoundBoard"/> instance.
/// </summary>
public partial class SoundBoardDetailEntryViewModel(
        ISoundBoardStorageService storageService,
        ILogger<SoundBoardDetailEntryViewModel> logger,
        IMessenger messenger) : BaseViewModel
{
        private readonly ISoundBoardStorageService soundBoardStorageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        private readonly ILogger<SoundBoardDetailEntryViewModel> logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IMessenger messageBus = messenger ?? throw new ArgumentNullException(nameof(messenger));

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
        [NotifyCanExecuteChangedFor(nameof(AddCommand))]
        private string description = string.Empty;

        /// <summary>
        /// Adds the configured sound board to storage and notifies subscribers of the addition.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanAddSoundBoard))]
        private async Task AddAsync()
        {
                var trimmedTitle = (Title ?? string.Empty).Trim();
                var trimmedDescription = (Description ?? string.Empty).Trim();
                var soundBoard = new SoundBoard(trimmedTitle, trimmedDescription);

                try
                {
                        await soundBoardStorageService.AddSoundBoardAsync(soundBoard).ConfigureAwait(false);
                        messageBus.Send(new SoundBoardAddedMessage(new SoundBoard(soundBoard)));
                        await MainThread.InvokeOnMainThreadAsync(() => Shell.Current.GoToAsync(".."));
                }
                catch (Exception ex)
                {
                        logger.LogError(ex, "Failed to add a new sound board.");
                        throw;
                }
        }

        /// <summary>
        /// Cancels the creation process and returns to the previous page.
        /// </summary>
        [RelayCommand]
        private Task CancelAsync()
        {
                return MainThread.InvokeOnMainThreadAsync(() => Shell.Current.GoToAsync(".."));
        }

        private bool CanAddSoundBoard() => !string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(Description);
}
