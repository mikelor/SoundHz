namespace SoundHz.ViewModels;

public partial class SoundsViewModel : BaseViewModel
{
	[ObservableProperty]
	public partial string Source { get; set; } = "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4";
}
