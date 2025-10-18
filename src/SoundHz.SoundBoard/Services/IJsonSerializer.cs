using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SoundHz.SoundBoard.Services;

/// <summary>
///     Provides serialization services for JSON payloads.
/// </summary>
public interface IJsonSerializer
{
    /// <summary>
    ///     Deserializes a JSON payload into a strongly typed instance.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="stream">The JSON stream.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task producing the deserialized value.</returns>
    Task<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken);

    /// <summary>
    ///     Serializes a value to JSON and writes it to the provided stream.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="stream">The target stream.</param>
    /// <param name="value">The value to serialize.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An awaitable task.</returns>
    Task SerializeAsync<T>(Stream stream, T value, CancellationToken cancellationToken);
}
