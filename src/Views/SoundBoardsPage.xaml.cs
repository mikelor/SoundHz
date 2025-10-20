using CommunityToolkit.Maui.Views;
using System.Linq;

namespace SoundHz.Views;

/// <summary>
/// Displays the collection of sound boards and provides multiple entry points for adding new boards.
/// </summary>
public partial class SoundBoardsPage : ContentPage
{
        private readonly SoundBoardsViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundBoardsPage"/> class.
        /// </summary>
        /// <param name="viewModel">The view model supplying the presentation logic.</param>
        public SoundBoardsPage(SoundBoardsViewModel viewModel)
        {
                InitializeComponent();
                BindingContext = this.viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        /// <inheritdoc />
        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
                base.OnNavigatedTo(args);

                await viewModel.LoadDataAsync();
        }

        /// <summary>
        /// Handles selection changes on the add tab, invoking the add command when chosen.
        /// </summary>
        /// <param name="sender">The originating tab view.</param>
        /// <param name="e">The event data describing the new and old selections.</param>
        private void OnAddTabSelectionChanged(object? sender, TabViewSelectionChangedEventArgs e)
        {
                if (sender is not TabView tabView)
                {
                        return;
                }

                if (e.NewItem is TabViewItem newItem && string.Equals(newItem.Text, "Add New SoundBoard...", StringComparison.Ordinal))
                {
                        if (viewModel.GoToCreateSoundBoardCommand.CanExecute(null))
                        {
                                viewModel.GoToCreateSoundBoardCommand.Execute(null);
                        }

                        tabView.SelectedItem = e.OldItem ?? tabView.TabItems.FirstOrDefault();
                }
        }
}
