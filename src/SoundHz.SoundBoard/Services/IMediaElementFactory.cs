using CommunityToolkit.Maui.Views;

namespace SoundHz.SoundBoard.Services;

/// <summary>
///     Creates configured <see cref="MediaElement"/> instances for playback.
/// </summary>
public interface IMediaElementFactory
{
    /// <summary>
    ///     Creates a new <see cref="MediaElement"/> for the provided source.
    /// </summary>
    /// <param name="source">The media source URI.</param>
    /// <returns>A configured media element.</returns>
    MediaElement Create(Uri source);
}
