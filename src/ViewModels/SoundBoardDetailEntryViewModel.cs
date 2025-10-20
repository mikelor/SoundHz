using CommunityToolkit.Mvvm.Messaging;
using SoundHz.Messaging;

namespace SoundHz.ViewModels;

/// <summary>
/// Provides the state and commands required for creating a new <see cref="SoundBoard"/>.
/// </summary>
public partial class SoundBoardDetailEntryViewModel : BaseViewModel
{
    /// <summary>
    /// Gets or sets the title provided by the user for the new sound board.
    /// </summary>
    [ObservableProperty]
    private string title = string.Empty;

    /// <summary>
    /// Gets or sets the description provided by the user for the new sound board.
    /// </summary>
    [ObservableProperty]
    private string description = string.Empty;

    /// <summary>
    /// Handles updates when the <see cref="Title"/> property changes to update command availability.
    /// </summary>
    /// <param name="value">The updated title text.</param>
    partial void OnTitleChanged(string value)
    {
        _ = value;
        AddCommand.NotifyCanExecuteChanged();
    }

    /// <summary>
    /// Handles updates when the <see cref="Description"/> property changes to update command availability.
    /// </summary>
    /// <param name="value">The updated description text.</param>
    partial void OnDescriptionChanged(string value)
    {
        _ = value;
        AddCommand.NotifyCanExecuteChanged();
    }

    /// <summary>
    /// Adds the new sound board and navigates back to the previous page.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [RelayCommand(CanExecute = nameof(CanAddSoundBoard))]
    private async Task AddAsync()
    {
        var trimmedTitle = Title.Trim();
        var trimmedDescription = Description.Trim();

        var newSoundBoard = new SoundBoard(trimmedTitle, trimmedDescription);
        WeakReferenceMessenger.Default.Send(new SoundBoardAddedMessage(newSoundBoard));

        Title = string.Empty;
        Description = string.Empty;

        await Shell.Current.GoToAsync("..", true).ConfigureAwait(false);
    }

    /// <summary>
    /// Cancels the creation of a sound board and returns to the previous page.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    [RelayCommand]
    private async Task CancelAsync()
    {
        Title = string.Empty;
        Description = string.Empty;

        await Shell.Current.GoToAsync("..", true).ConfigureAwait(false);
    }

    private bool CanAddSoundBoard() =>
        !string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(Description);
}
