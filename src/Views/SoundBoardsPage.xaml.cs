using CommunityToolkit.Maui.Views;
using Microsoft.Extensions.DependencyInjection;

namespace SoundHz.Views;

/// <summary>
/// Displays the list of sound boards and entry points for creating new boards.
/// </summary>
public partial class SoundBoardsPage : ContentPage
{
        private readonly SoundBoardsViewModel ViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundBoardsPage"/> class.
        /// </summary>
        public SoundBoardsPage()
                : this(ResolveViewModel())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundBoardsPage"/> class.
        /// </summary>
        /// <param name="viewModel">The view model that supplies state and commands for the page.</param>
        public SoundBoardsPage(SoundBoardsViewModel viewModel)
        {
                InitializeComponent();

                BindingContext = ViewModel = viewModel;
                SoundBoardsTabView.SelectedIndex = 0;
        }

        /// <inheritdoc />
        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
                base.OnNavigatedTo(args);

                await ViewModel.LoadDataAsync();
        }

        private async void OnTabSelectionChanged(object? sender, TabViewSelectionChangedEventArgs e)
        {
                if (sender is not TabView tabView)
                {
                        return;
                }

                if (tabView.SelectedIndex != 1)
                {
                        return;
                }

                if (ViewModel.AddNewSoundBoardCommand.CanExecute(null))
                {
                        await ViewModel.AddNewSoundBoardCommand.ExecuteAsync(null);
                }

                tabView.SelectedIndex = 0;
        }

        private static SoundBoardsViewModel ResolveViewModel()
        {
                var services = Application.Current?.Handler?.MauiContext?.Services
                        ?? throw new InvalidOperationException("Unable to resolve application services.");

                return services.GetRequiredService<SoundBoardsViewModel>();
        }
}
