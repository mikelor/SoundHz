using Microsoft.Extensions.DependencyInjection;

namespace SoundHz.Views;

/// <summary>
/// Provides the user interface for creating a new sound board.
/// </summary>
public partial class SoundBoardDetailEntryPage : ContentPage
{
        /// <summary>
        /// Initializes a new instance of the <see cref="SoundBoardDetailEntryPage"/> class.
        /// </summary>
        public SoundBoardDetailEntryPage()
                : this(ResolveViewModel())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundBoardDetailEntryPage"/> class.
        /// </summary>
        /// <param name="viewModel">The view model backing the page.</param>
        public SoundBoardDetailEntryPage(SoundBoardDetailEntryViewModel viewModel)
        {
                InitializeComponent();
                BindingContext = viewModel;
        }

        private static SoundBoardDetailEntryViewModel ResolveViewModel()
        {
                var services = Application.Current?.Handler?.MauiContext?.Services
                        ?? throw new InvalidOperationException("Unable to resolve application services.");

                return services.GetRequiredService<SoundBoardDetailEntryViewModel>();
        }
}
