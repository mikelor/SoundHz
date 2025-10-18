using System.Resources;

namespace SoundHz.SoundBoard.Resources.Localization;

/// <summary>
///     Provides helper accessors for log message resources.
/// </summary>
public static class LogMessagesResourceManager
{
    /// <summary>
    ///     Gets a singleton <see cref="ResourceManager"/> instance for log messages.
    /// </summary>
    public static ResourceManager Instance { get; } = new ResourceManager("SoundHz.SoundBoard.Resources.Strings.LogMessages", typeof(LogMessagesResourceManager).Assembly);
}
