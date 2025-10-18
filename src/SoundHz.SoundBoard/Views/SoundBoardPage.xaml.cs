using System;
using System.Threading;
using System.Threading.Tasks;
using SoundHz.SoundBoard.Services;
using SoundHz.SoundBoard.ViewModels;
using Microsoft.Maui.Controls;

namespace SoundHz.SoundBoard.Views;

/// <summary>
///     Displays a sound board and allows interaction with individual sounds.
/// </summary>
public partial class SoundBoardPage : ContentPage
{
    private readonly ISoundPlaybackService _playbackService;
    private readonly SoundBoardViewModel _viewModel;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SoundBoardPage"/> class.
    /// </summary>
    /// <param name="viewModel">The associated view model.</param>
    /// <param name="playbackService">The playback service.</param>
    public SoundBoardPage(SoundBoardViewModel viewModel, ISoundPlaybackService playbackService)
    {
        InitializeComponent();
        _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        _playbackService = playbackService ?? throw new ArgumentNullException(nameof(playbackService));
        BindingContext = _viewModel;
    }

    /// <inheritdoc />
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _playbackService.RegisterHost(MediaHost);
    }

    /// <inheritdoc />
    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        _playbackService.UnregisterHost(MediaHost);
        await _viewModel.PersistAsync(CancellationToken.None).ConfigureAwait(false);
    }
}
