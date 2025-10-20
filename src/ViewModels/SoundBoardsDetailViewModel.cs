namespace SoundHz.ViewModels;

[QueryProperty(nameof(Item), "Item")]
public partial class SoundBoardsDetailViewModel : BaseViewModel
{
	[ObservableProperty]
	public partial SampleItem? Item { get; set; }
}
