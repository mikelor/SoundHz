using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SoundHz.SoundBoard.Services;

/// <summary>
///     Provides a <see cref="System.Text.Json"/> based implementation of <see cref="IJsonSerializer"/>.
/// </summary>
public sealed class JsonSerializerService : IJsonSerializer
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    /// <inheritdoc />
    public async Task<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(stream);
        return await JsonSerializer.DeserializeAsync<T>(stream, SerializerOptions, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task SerializeAsync<T>(Stream stream, T value, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(stream);
        await JsonSerializer.SerializeAsync(stream, value, SerializerOptions, cancellationToken).ConfigureAwait(false);
    }
}
