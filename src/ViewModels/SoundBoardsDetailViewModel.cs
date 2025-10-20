namespace SoundHz.ViewModels;

/// <summary>
/// Provides the presentation model for displaying detailed information about a <see cref="SoundBoard"/>.
/// </summary>
[QueryProperty(nameof(Item), "Item")]
public partial class SoundBoardsDetailViewModel : BaseViewModel
{
    /// <summary>
    /// Gets or sets the sound board being displayed.
    /// </summary>
    [ObservableProperty]
    private SoundBoard? item;
}
