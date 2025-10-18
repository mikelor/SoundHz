using System.Globalization;
using SoundHz.SoundBoard.Resources.Localization;
using SoundHz.SoundBoard.Views;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;

namespace SoundHz.SoundBoard;

/// <summary>
///     Represents the root application for the SoundHz sound board experience.
/// </summary>
public partial class App(ILogger<App> logger, IServiceProvider serviceProvider) : Application
{
    private readonly ILogger<App> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    /// <inheritdoc />
    protected override void OnStart()
    {
        base.OnStart();
        _logger.LogInformation(LogMessagesResourceManager.Instance.GetString("AppStarted", CultureInfo.CurrentCulture));
    }

    /// <inheritdoc />
    protected override Window CreateWindow(IActivationState? activationState)
    {
        var mainPage = _serviceProvider.GetRequiredService<NavigationPage>();
        return new Window(mainPage);
    }
}
