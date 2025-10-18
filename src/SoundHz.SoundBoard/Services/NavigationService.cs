using System;
using System.Threading.Tasks;
using SoundHz.SoundBoard.ViewModels;
using SoundHz.SoundBoard.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.ApplicationModel;

namespace SoundHz.SoundBoard.Services;

/// <summary>
///     Provides a navigation service built on top of <see cref="NavigationPage"/>.
/// </summary>
public sealed class NavigationService(NavigationPage navigationPage, IServiceProvider serviceProvider) : INavigationService
{
    private readonly NavigationPage _navigationPage = navigationPage ?? throw new ArgumentNullException(nameof(navigationPage));
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    /// <inheritdoc />
    public async Task NavigateToSoundBoardAsync(SoundBoardViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        var page = _serviceProvider.GetRequiredService<SoundBoardPage>();
        page.BindingContext = viewModel;
        await MainThread.InvokeOnMainThreadAsync(async () => await _navigationPage.PushAsync(page).ConfigureAwait(false)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task NavigateToEditSoundAsync(EditSoundViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        var page = _serviceProvider.GetRequiredService<EditSoundPage>();
        page.BindingContext = viewModel;
        await MainThread.InvokeOnMainThreadAsync(async () => await _navigationPage.PushAsync(page).ConfigureAwait(false)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task GoBackAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(async () => await _navigationPage.PopAsync().ConfigureAwait(false)).ConfigureAwait(false);
    }
}
