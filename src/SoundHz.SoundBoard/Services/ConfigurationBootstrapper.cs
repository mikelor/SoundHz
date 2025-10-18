using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SoundHz.SoundBoard.Services;

/// <summary>
///     Ensures configuration files exist within the writable app data directory.
/// </summary>
public sealed class ConfigurationBootstrapper(IAppFileSystem fileSystem)
{
    private readonly IAppFileSystem _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

    /// <summary>
    ///     Gets the resolved configuration path.
    /// </summary>
    public string ConfigurationPath => Path.Combine(_fileSystem.GetAppDataDirectory(), "soundboards.json");

    /// <summary>
    ///     Ensures the configuration exists; if not, seeds from the packaged defaults.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An awaitable task.</returns>
    public async Task EnsureSeedConfigurationAsync(CancellationToken cancellationToken)
    {
        if (await _fileSystem.FileExistsAsync(ConfigurationPath, cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        await using var destinationStream = await _fileSystem.OpenWriteAsync(ConfigurationPath, cancellationToken).ConfigureAwait(false);
        await using var sourceStream = await _fileSystem.OpenAppPackageFileAsync("appsettings.json", cancellationToken).ConfigureAwait(false);
        await sourceStream.CopyToAsync(destinationStream, cancellationToken).ConfigureAwait(false);
    }
}
