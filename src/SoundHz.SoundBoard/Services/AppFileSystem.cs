using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace SoundHz.SoundBoard.Services;

/// <summary>
///     Provides a Maui-backed implementation of <see cref="IAppFileSystem"/>.
/// </summary>
public sealed class AppFileSystem : IAppFileSystem
{
    /// <inheritdoc />
    public string GetAppDataDirectory() => FileSystem.Current.AppDataDirectory;

    /// <inheritdoc />
    public async Task<bool> FileExistsAsync(string path, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        return await Task.Run(() => File.Exists(path), cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<Stream> OpenReadAsync(string path, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        return await Task.Run(() => (Stream)File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read), cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<Stream> OpenWriteAsync(string path, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        return await Task.Run(() => (Stream)File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None), cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<Stream> OpenAppPackageFileAsync(string path, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        return await FileSystem.Current.OpenAppPackageFileAsync(path).WaitAsync(cancellationToken).ConfigureAwait(false);
    }
}
