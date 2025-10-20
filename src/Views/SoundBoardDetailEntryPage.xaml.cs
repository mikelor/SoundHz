namespace SoundHz.Views;

/// <summary>
/// Presents a form for creating a new <see cref="SoundBoard"/> entry.
/// </summary>
public partial class SoundBoardDetailEntryPage : ContentPage
{
        private readonly SoundBoardDetailEntryViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundBoardDetailEntryPage"/> class.
        /// </summary>
        /// <param name="viewModel">The view model providing page behavior.</param>
        public SoundBoardDetailEntryPage(SoundBoardDetailEntryViewModel viewModel)
        {
                InitializeComponent();
                BindingContext = this.viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        /// <inheritdoc />
        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
                base.OnNavigatedTo(args);

                viewModel.Reset();
        }
}
