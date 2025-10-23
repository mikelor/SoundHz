namespace SoundHz.Views;

/// <summary>
/// Represents the page that allows users to enter details for a new sound board.
/// </summary>
public partial class SoundBoardDetailEntryPage : ContentPage
{
        /// <summary>
        /// Initializes a new instance of the <see cref="SoundBoardDetailEntryPage"/> class.
        /// </summary>
        /// <param name="viewModel">The view model providing state and commands for the page.</param>
        public SoundBoardDetailEntryPage(SoundBoardDetailEntryViewModel viewModel)
        {
                ArgumentNullException.ThrowIfNull(viewModel);

                InitializeComponent();
                BindingContext = viewModel;
        }
}
