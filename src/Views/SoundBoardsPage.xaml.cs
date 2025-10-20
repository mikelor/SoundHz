namespace SoundHz.Views;

public partial class SoundBoardsPage : ContentPage
{
	SoundBoardsViewModel ViewModel;

	public SoundBoardsPage(SoundBoardsViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = ViewModel = viewModel;
	}

	protected override async void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		await ViewModel.LoadDataAsync();
	}
}
