using SoundHz.SoundBoard.CommandHandling;
using SoundHz.SoundBoard.Models;
using SoundHz.SoundBoard.Services;
using SoundHz.SoundBoard.ViewModels;
using SoundHz.SoundBoard.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SoundHz.SoundBoard;

/// <summary>
///     Configures the Maui application and dependency container.
/// </summary>
public static class MauiProgram
{
    /// <summary>
    ///     Creates the Maui application builder.
    /// </summary>
    /// <returns>A configured <see cref="MauiApp"/> instance.</returns>
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>()
               .UseMauiCommunityToolkit();

        builder.Services.AddLogging(logging => logging.AddDebug());
        builder.Services.AddSingleton<IResourceManagerProvider, ResourceManagerProvider>();
        builder.Services.AddSingleton<IAppFileSystem, AppFileSystem>();
        builder.Services.AddSingleton<IJsonSerializer, JsonSerializerService>();
        builder.Services.AddSingleton<IMediaElementFactory, MediaElementFactory>();
        builder.Services.AddSingleton<ISoundPlaybackService, SoundPlaybackService>();
        builder.Services.AddSingleton<ISoundBoardRepository, SoundBoardRepository>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<ISoundBoardStateStore, SoundBoardStateStore>();

        builder.Services.AddSingleton<CommandHandler<SoundBoardConfiguration>>(provider =>
        {
            var repository = provider.GetRequiredService<ISoundBoardRepository>();
            return new SaveSoundBoardCommandHandler(repository);
        });

        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<SoundBoardViewModel>();
        builder.Services.AddTransient<EditSoundViewModel>();

        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<SoundBoardPage>();
        builder.Services.AddTransient<EditSoundPage>();

        builder.Services.AddSingleton(provider => new NavigationPage(provider.GetRequiredService<MainPage>()));

        ConfigureConfiguration(builder);

        return builder.Build();
    }

    private static void ConfigureConfiguration(MauiAppBuilder builder)
    {
        using var serviceProvider = builder.Services.BuildServiceProvider();
        var fileSystem = serviceProvider.GetRequiredService<IAppFileSystem>();
        var repository = new ConfigurationBootstrapper(fileSystem);
        repository.EnsureSeedConfigurationAsync(CancellationToken.None).GetAwaiter().GetResult();

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(repository.ConfigurationPath, optional: false, reloadOnChange: true)
            .Build();

        builder.Configuration.AddConfiguration(configuration);
    }
}
