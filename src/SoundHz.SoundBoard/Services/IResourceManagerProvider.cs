using System.Resources;

namespace SoundHz.SoundBoard.Services;

/// <summary>
///     Provides access to localized <see cref="ResourceManager"/> instances.
/// </summary>
public interface IResourceManagerProvider
{
    /// <summary>
    ///     Gets the resource manager used for error messages.
    /// </summary>
    /// <returns>The configured <see cref="ResourceManager"/>.</returns>
    ResourceManager GetErrorResourceManager();

    /// <summary>
    ///     Gets the resource manager used for logging messages.
    /// </summary>
    /// <returns>The configured <see cref="ResourceManager"/>.</returns>
    ResourceManager GetLogResourceManager();
}
