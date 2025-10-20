using System.Threading;

namespace SoundHz.Views;

/// <summary>
/// Displays the list of available sound boards and orchestrates their loading lifecycle.
/// </summary>
public partial class SoundBoardsPage : ContentPage
{
        private readonly SoundBoardsViewModel viewModel;
        private readonly ILogger<SoundBoardsPage> logger;
        private CancellationTokenSource? loadCancellationTokenSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundBoardsPage"/> class.
        /// </summary>
        /// <param name="viewModel">The view model providing the page state.</param>
        /// <param name="logger">The logger used for diagnostic output.</param>
        public SoundBoardsPage(SoundBoardsViewModel viewModel, ILogger<SoundBoardsPage> logger)
        {
                ArgumentNullException.ThrowIfNull(viewModel);
                ArgumentNullException.ThrowIfNull(logger);

                InitializeComponent();

                BindingContext = this.viewModel = viewModel;
                this.logger = logger;
        }

        /// <inheritdoc />
        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
                base.OnNavigatedTo(args);

                CancelPendingLoad();
                loadCancellationTokenSource = new CancellationTokenSource();

                _ = LoadAsync(loadCancellationTokenSource.Token);
        }

        /// <inheritdoc />
        protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
        {
                CancelPendingLoad();

                base.OnNavigatedFrom(args);
        }

        private async Task LoadAsync(CancellationToken cancellationToken)
        {
                try
                {
                        await viewModel.LoadDataAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                        // Navigation triggered cancellation; ignore.
                }
                catch (Exception ex)
                {
                        logger.LogError(ex, "Failed to load sound boards.");
                }
        }

        private void CancelPendingLoad()
        {
                if (loadCancellationTokenSource is null)
                {
                        return;
                }

                if (!loadCancellationTokenSource.IsCancellationRequested)
                {
                        loadCancellationTokenSource.Cancel();
                }

                loadCancellationTokenSource.Dispose();
                loadCancellationTokenSource = null;
        }
}
