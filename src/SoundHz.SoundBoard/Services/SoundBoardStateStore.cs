using SoundHz.SoundBoard.Models;

namespace SoundHz.SoundBoard.Services;

/// <summary>
///     Provides a default implementation of <see cref="ISoundBoardStateStore"/>.
/// </summary>
public sealed class SoundBoardStateStore : ISoundBoardStateStore
{
    /// <inheritdoc />
    public SoundBoardConfiguration Configuration { get; set; } = new();
}
