using Microsoft.Maui.Storage;

namespace SoundHz;

/// <summary>
/// Configures the MAUI application and its dependencies.
/// </summary>
public static class MauiProgram
{
        /// <summary>
        /// Creates the MAUI application instance.
        /// </summary>
        /// <returns>The configured <see cref="MauiApp"/>.</returns>
        public static MauiApp CreateMauiApp()
        {
                var builder = MauiApp.CreateBuilder();
                builder
                        .UseMauiApp<App>()
                        .UseMauiCommunityToolkitMediaElement()
                        .UseMauiCommunityToolkit()
                        .ConfigureFonts(fonts =>
                        {
                                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                        });
#if DEBUG
                builder.Logging.AddDebug();
#endif
                builder.Services.AddSingleton<IFileSystem>(_ => FileSystem.Current);
                builder.Services.AddSingleton<ISoundBoardStorageService, SoundBoardStorageService>();
                builder.Services.AddTransient<SoundBoardsDetailViewModel>();
                builder.Services.AddTransient<SoundBoardDetailEntryViewModel>();
                builder.Services.AddSingleton<SoundBoardsViewModel>();
                builder.Services.AddSingleton<AboutViewModel>();
                builder.Services.AddTransient<SoundBoardDetailEntryPage>();

                return builder.Build();
        }
}
