namespace SoundHz.Views;

public partial class SoundBoardsDetailPage : ContentPage
{
	public SoundBoardsDetailPage(SoundBoardsDetailViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
