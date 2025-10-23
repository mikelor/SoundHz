namespace SoundHz;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
                Routing.RegisterRoute(nameof(SoundBoardsDetailPage), typeof(SoundBoardsDetailPage));
                Routing.RegisterRoute(nameof(SoundBoardDetailEntryPage), typeof(SoundBoardDetailEntryPage));
        }
}
