namespace SoundHz.Views;

/// <summary>
/// Displays entry fields for creating a new sound board.
/// </summary>
public partial class SoundBoardDetailEntryPage : ContentPage
{
        /// <summary>
        /// Initializes a new instance of the <see cref="SoundBoardDetailEntryPage"/> class.
        /// </summary>
        /// <param name="viewModel">The view model providing binding data.</param>
        public SoundBoardDetailEntryPage(SoundBoardDetailEntryViewModel viewModel)
        {
                InitializeComponent();
                BindingContext = viewModel;
        }
}
