using System.Resources;

namespace SoundHz.SoundBoard.Resources.Localization;

/// <summary>
///     Provides helper accessors for error message resources.
/// </summary>
public static class ErrorMessagesResourceManager
{
    /// <summary>
    ///     Gets a singleton <see cref="ResourceManager"/> instance for error messages.
    /// </summary>
    public static ResourceManager Instance { get; } = new ResourceManager("SoundHz.SoundBoard.Resources.Strings.ErrorMessages", typeof(ErrorMessagesResourceManager).Assembly);
}
