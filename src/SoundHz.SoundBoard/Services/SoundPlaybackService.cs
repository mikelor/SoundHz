using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SoundHz.SoundBoard.Models;
using SoundHz.SoundBoard.Resources.Localization;
using CommunityToolkit.Maui.Views;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace SoundHz.SoundBoard.Services;

/// <summary>
///     Provides managed playback of sound definitions using <see cref="MediaElement"/> controls.
/// </summary>
public sealed class SoundPlaybackService(IMediaElementFactory mediaElementFactory, ILogger<SoundPlaybackService> logger) : ISoundPlaybackService
{
    private readonly List<WeakReference<Layout>> _hosts = new();
    private readonly ILogger<SoundPlaybackService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMediaElementFactory _mediaElementFactory = mediaElementFactory ?? throw new ArgumentNullException(nameof(mediaElementFactory));
    private readonly List<MediaElement> _activeElements = new();
    private readonly object _syncLock = new();

    /// <inheritdoc />
    public void RegisterHost(Layout layout)
    {
        ArgumentNullException.ThrowIfNull(layout);
        lock (_syncLock)
        {
            _hosts.Add(new WeakReference<Layout>(layout));
        }
    }

    /// <inheritdoc />
    public void UnregisterHost(Layout layout)
    {
        ArgumentNullException.ThrowIfNull(layout);
        lock (_syncLock)
        {
            _hosts.RemoveAll(reference => reference.TryGetTarget(out var target) && target == layout);
        }
    }

    /// <inheritdoc />
    public async Task PlayAsync(SoundDefinition sound, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(sound);
        cancellationToken.ThrowIfCancellationRequested();
        if (!Uri.TryCreate(sound.FilePath, UriKind.RelativeOrAbsolute, out var uri))
        {
            throw new InvalidOperationException(ErrorMessagesResourceManager.Instance.GetString("InvalidSoundDefinition", CultureInfo.CurrentCulture));
        }

        var host = GetActiveHost();
        if (host is null)
        {
            _logger.LogWarning("No host registered for sound playback; ignoring request.");
            return;
        }

        var mediaElement = _mediaElementFactory.Create(uri);
        mediaElement.MediaEnded += HandleCompleted;
        mediaElement.MediaFailed += HandleCompleted;

        void HandleCompleted(object? sender, EventArgs args)
        {
            mediaElement.MediaEnded -= HandleCompleted;
            mediaElement.MediaFailed -= HandleCompleted;
            _ = MainThread.InvokeOnMainThreadAsync(() =>
            {
                lock (_syncLock)
                {
                    if (host.Children.Contains(mediaElement))
                    {
                        host.Children.Remove(mediaElement);
                    }

                    _activeElements.Remove(mediaElement);
                }

                mediaElement.Handler?.DisconnectHandler();
                mediaElement.Source = null;
            });
        }

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            lock (_syncLock)
            {
                host.Children.Add(mediaElement);
                _activeElements.Add(mediaElement);
            }

            mediaElement.Play();
        }).ConfigureAwait(false);
    }

    private Layout? GetActiveHost()
    {
        lock (_syncLock)
        {
            foreach (var reference in _hosts)
            {
                if (reference.TryGetTarget(out var target))
                {
                    return target;
                }
            }
        }

        return null;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        lock (_syncLock)
        {
            foreach (var element in _activeElements.ToList())
            {
                element.Handler?.DisconnectHandler();
                element.Source = null;
            }

            _activeElements.Clear();
            _hosts.Clear();
        }
    }
}
