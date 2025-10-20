namespace SoundHz.Views;

/// <summary>
/// Presents the user interface for creating a new sound board.
/// </summary>
public partial class SoundBoardDetailsEntryPage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SoundBoardDetailsEntryPage"/> class.
    /// </summary>
    /// <param name="viewModel">The view model providing presentation logic.</param>
    public SoundBoardDetailsEntryPage(SoundBoardDetailsEntryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
    }
}
