using Microsoft.Maui.ApplicationModel;
using SoundHz.Messages;

namespace SoundHz.ViewModels;

/// <summary>
/// Provides the presentation logic for creating or editing a <see cref="SoundBoard"/> instance.
/// </summary>
[QueryProperty(nameof(Item), "Item")]
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
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        private string title = string.Empty;

        /// <summary>
        /// Gets or sets the description for the new sound board.
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        private string description = string.Empty;

        /// <summary>
        /// Gets or sets the sound board being edited when in edit mode.
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsInEditMode))]
        [NotifyPropertyChangedFor(nameof(SubmitButtonText))]
        [NotifyPropertyChangedFor(nameof(PageTitle))]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        private SoundBoard? item;

        /// <summary>
        /// Gets the text displayed on the submit button.
        /// </summary>
        public string SubmitButtonText => IsInEditMode ? "Update" : "Add";

        /// <summary>
        /// Gets the title displayed on the page.
        /// </summary>
        public string PageTitle => IsInEditMode ? "Edit Sound Board" : "New Sound Board";

        /// <summary>
        /// Gets a value indicating whether the view model is editing an existing sound board.
        /// </summary>
        public bool IsInEditMode => Item is not null;

        /// <summary>
        /// Adds or updates the configured sound board and persists the changes.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanSubmit))]
        private async Task SubmitAsync()
        {
                var trimmedTitle = (Title ?? string.Empty).Trim();
                var trimmedDescription = (Description ?? string.Empty).Trim();

                if (IsInEditMode)
                {
                        try
                        {
                                ArgumentNullException.ThrowIfNull(Item);
                                Item.Title = trimmedTitle;
                                Item.Description = trimmedDescription;
                                await MainThread.InvokeOnMainThreadAsync(() => Shell.Current.GoToAsync(".."));
                        }
                        catch (Exception ex)
                        {
                                logger.LogError(ex, "Failed to update sound board '{Title}'.", trimmedTitle);
                                throw;
                        }
                }
                else
                {
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
        }

        /// <summary>
        /// Cancels the creation process and returns to the previous page.
        /// </summary>
        [RelayCommand]
        private Task CancelAsync()
        {
                return MainThread.InvokeOnMainThreadAsync(() => Shell.Current.GoToAsync(".."));
        }

        private bool CanSubmit() => !string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(Description);

        partial void OnItemChanged(SoundBoard? value)
        {
                if (value is null)
                {
                        return;
                }

                Title = value.Title;
                Description = value.Description;
        }
}
