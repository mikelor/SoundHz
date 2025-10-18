using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;

namespace SoundHz.SoundBoard.Services;

/// <summary>
///     Provides a default implementation of <see cref="IMediaElementFactory"/>.
/// </summary>
public sealed class MediaElementFactory : IMediaElementFactory
{
    /// <inheritdoc />
    public MediaElement Create(Uri source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return new MediaElement
        {
            ShowsPlaybackControls = false,
            ShouldAutoPlay = true,
            Source = MediaSource.FromUri(source),
            Aspect = CommunityToolkit.Maui.Core.Primitives.MediaAspect.AspectFit,
            IsLooping = false
        };
    }
}
