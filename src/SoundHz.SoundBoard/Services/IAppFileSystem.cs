using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SoundHz.SoundBoard.Services;

/// <summary>
///     Provides a testable abstraction around file-system operations used by the application.
/// </summary>
public interface IAppFileSystem
{
    /// <summary>
    ///     Gets the absolute path to the application data directory.
    /// </summary>
    /// <returns>The path to the application data directory.</returns>
    string GetAppDataDirectory();

    /// <summary>
    ///     Determines whether the file exists at the given path.
    /// </summary>
    /// <param name="path">The file path to check.</param>
    /// <param name="cancellationToken">A token that cancels the operation.</param>
    /// <returns>A task returning <c>true</c> if the file exists.</returns>
    Task<bool> FileExistsAsync(string path, CancellationToken cancellationToken);

    /// <summary>
    ///     Opens a file stream for reading.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task producing a readable <see cref="Stream"/>.</returns>
    Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken);

    /// <summary>
    ///     Opens a file stream for writing.
    /// </summary>
    /// <param name="path">The file path.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task producing a writable <see cref="Stream"/>.</returns>
    Task<Stream> OpenWriteAsync(string path, CancellationToken cancellationToken);

    /// <summary>
    ///     Opens a file included within the application package.
    /// </summary>
    /// <param name="path">The relative path to the packaged file.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task producing a readable <see cref="Stream"/>.</returns>
    Task<Stream> OpenAppPackageFileAsync(string path, CancellationToken cancellationToken);
}
