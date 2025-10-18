using System.Threading;
using System.Threading.Tasks;
using SoundHz.SoundBoard.Models;

namespace SoundHz.SoundBoard.Services;

/// <summary>
///     Provides persistence operations for sound board definitions.
/// </summary>
public interface ISoundBoardRepository
{
    /// <summary>
    ///     Loads the sound board configuration.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task producing the configuration.</returns>
    Task<SoundBoardConfiguration> LoadAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Persists the provided sound board configuration.
    /// </summary>
    /// <param name="configuration">The configuration to persist.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An awaitable task.</returns>
    Task SaveAsync(SoundBoardConfiguration configuration, CancellationToken cancellationToken);
}
