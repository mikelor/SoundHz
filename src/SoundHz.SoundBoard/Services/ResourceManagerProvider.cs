using System.Reflection;
using System.Resources;
using SoundHz.SoundBoard.Resources.Localization;

namespace SoundHz.SoundBoard.Services;

/// <summary>
///     Provides typed access to localized resource managers.
/// </summary>
public sealed class ResourceManagerProvider : IResourceManagerProvider
{
    private readonly ResourceManager _errorResourceManager = new(nameof(ErrorMessages), typeof(ErrorMessages).GetTypeInfo().Assembly);
    private readonly ResourceManager _logResourceManager = new(nameof(LogMessages), typeof(LogMessages).GetTypeInfo().Assembly);

    /// <inheritdoc />
    public ResourceManager GetErrorResourceManager() => _errorResourceManager;

    /// <inheritdoc />
    public ResourceManager GetLogResourceManager() => _logResourceManager;
}
