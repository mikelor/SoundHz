namespace SoundHz.Views;

/// <summary>
/// Displays the UI that allows the user to create a new <see cref="SoundBoard"/>.
/// </summary>
public partial class SoundBoardDetailEntryPage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SoundBoardDetailEntryPage"/> class.
    /// </summary>
    /// <param name="viewModel">The view model that backs the page.</param>
    public SoundBoardDetailEntryPage(SoundBoardDetailEntryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
