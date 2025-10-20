using System;

namespace SoundHz.Views;

public partial class SoundsPage : ContentPage
{
	public SoundsPage(SoundsViewModel viewModel)
	{
		// TODO: change the source and add your own controls for playback as necessary.
		InitializeComponent();
		BindingContext = viewModel;
	}

	void Page_Unloaded(object sender, EventArgs e)
	{
		// Stop and cleanup MediaElement when we navigate away
		mediaElement.Handler?.DisconnectHandler();
	}
}
