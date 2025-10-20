using CommunityToolkit.Mvvm.Messaging;
using SoundHz.ViewModels.Messages;

namespace SoundHz.ViewModels;

/// <summary>
/// Provides the interaction logic for creating a new <see cref="SoundBoard"/> instance.
/// </summary>
public partial class SoundBoardDetailEntryViewModel(ISoundBoardStorageService storageService, ILogger<SoundBoardDetailEntryViewModel> logger) : BaseViewModel
{
    private readonly ISoundBoardStorageService storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
    private readonly ILogger<SoundBoardDetailEntryViewModel> logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Gets or sets the title of the sound board being created.
    /// </summary>
    [ObservableProperty]
    private string title = string.Empty;

    /// <summary>
    /// Gets or sets the description of the sound board being created.
    /// </summary>
    [ObservableProperty]
    private string description = string.Empty;

    partial void OnTitleChanged(string value)
    {
        _ = value;
        AddCommand.NotifyCanExecuteChanged();
    }

    partial void OnDescriptionChanged(string value)
    {
        _ = value;
        AddCommand.NotifyCanExecuteChanged();
    }

    /// <summary>
    /// Clears the form inputs.
    /// </summary>
    [RelayCommand]
    private async Task CancelAsync()
    {
        Title = string.Empty;
        Description = string.Empty;
        await Shell.Current.GoToAsync("..").ConfigureAwait(false);
    }

    /// <summary>
    /// Adds the new sound board to the application and persists it.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanAddAsync))]
    private async Task AddAsync()
    {
        var soundBoard = new SoundBoard(Title.Trim(), Description.Trim());

        try
        {
            await storageService.AddSoundBoardAsync(soundBoard).ConfigureAwait(false);
            WeakReferenceMessenger.Default.Send(new SoundBoardAddedMessage(soundBoard.Clone()));
            Title = string.Empty;
            Description = string.Empty;
            await Shell.Current.GoToAsync("..").ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add a new sound board.");
            throw;
        }
    }

    private bool CanAddAsync()
    {
        return !string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(Description);
    }
}
