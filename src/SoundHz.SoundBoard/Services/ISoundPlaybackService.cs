using System;
using System.Threading;
using System.Threading.Tasks;
using SoundHz.SoundBoard.Models;
using Microsoft.Maui.Controls;

namespace SoundHz.SoundBoard.Services;

/// <summary>
///     Provides high level orchestration for playing sound definitions via <see cref="MediaElement"/> controls.
/// </summary>
public interface ISoundPlaybackService : IDisposable
{
    /// <summary>
    ///     Registers a layout to host generated media elements.
    /// </summary>
    /// <param name="layout">The layout that will contain hidden media elements.</param>
    void RegisterHost(Layout layout);

    /// <summary>
    ///     Unregisters a previously registered host.
    /// </summary>
    /// <param name="layout">The layout to unregister.</param>
    void UnregisterHost(Layout layout);

    /// <summary>
    ///     Plays the specified sound.
    /// </summary>
    /// <param name="sound">The sound definition to play.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An awaitable task.</returns>
    Task PlayAsync(SoundDefinition sound, CancellationToken cancellationToken);
}
