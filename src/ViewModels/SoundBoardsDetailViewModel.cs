namespace SoundHz.ViewModels;

/// <summary>
/// Provides the presentation model for displaying detailed information about a <see cref="SoundBoard"/>.
/// </summary>
[QueryProperty(nameof(Item), "Item")]
public partial class SoundBoardsDetailViewModel : BaseViewModel
{
    private SoundBoard? item;

    /// <summary>
    /// Gets or sets the sound board being displayed.
    /// </summary>
    public SoundBoard? Item
    {
        get => item;
        set => SetProperty(ref item, value);
    }
}
