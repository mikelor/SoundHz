using SoundHz.SoundBoard.Models;

namespace SoundHz.SoundBoard.Services;

/// <summary>
///     Represents the in-memory store for sound board state shared between views.
/// </summary>
public interface ISoundBoardStateStore
{
    /// <summary>
    ///     Gets or sets the active configuration.
    /// </summary>
    SoundBoardConfiguration Configuration { get; set; }
}
