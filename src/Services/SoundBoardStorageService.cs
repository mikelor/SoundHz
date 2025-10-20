using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using Microsoft.Maui.Storage;

namespace SoundHz.Services;

/// <summary>
/// Defines operations for retrieving and persisting <see cref="SoundBoard"/> instances.
/// </summary>
public interface ISoundBoardStorageService
{
    /// <summary>
    /// Retrieves all sound boards stored locally.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A read-only collection of <see cref="SoundBoard"/> instances.</returns>
    Task<IReadOnlyList<SoundBoard>> GetSoundBoardsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists the provided sound boards to local storage, replacing existing data.
    /// </summary>
    /// <param name="soundBoards">The collection of sound boards to persist.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SaveSoundBoardsAsync(IEnumerable<SoundBoard> soundBoards, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds the provided sound board to the persisted storage.
    /// </summary>
    /// <param name="soundBoard">The sound board to add.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task AddSoundBoardAsync(SoundBoard soundBoard, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the sound board with the specified title from storage.
    /// </summary>
    /// <param name="title">The title of the sound board to remove.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task RemoveSoundBoardAsync(string title, CancellationToken cancellationToken = default);
}

/// <summary>
/// Provides local file-system backed storage for <see cref="SoundBoard"/> collections.
/// </summary>
public sealed class SoundBoardStorageService(IFileSystem fileSystem, ILogger<SoundBoardStorageService> logger) : ISoundBoardStorageService, IDisposable
{
    private const string StorageFileName = "soundboards.json";

    private readonly IFileSystem fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    private readonly ILogger<SoundBoardStorageService> logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };
    private readonly SemaphoreSlim semaphore = new(1, 1);

    /// <inheritdoc />
    public async Task<IReadOnlyList<SoundBoard>> GetSoundBoardsAsync(CancellationToken cancellationToken = default)
    {
        await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            var storagePath = GetStorageFilePath();

            if (!File.Exists(storagePath))
            {
                var defaults = CreateDefaultSoundBoards();
                await SaveInternalAsync(defaults, storagePath, cancellationToken).ConfigureAwait(false);
                return defaults;
            }

            await using var readStream = CreateFileStream(storagePath, FileMode.Open, FileAccess.Read);
            var boards = await JsonSerializer.DeserializeAsync<List<SoundBoard>>(readStream, serializerOptions, cancellationToken).ConfigureAwait(false);

            return boards?.Select(board => board.Clone()).ToList() ?? new List<SoundBoard>();
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Failed to deserialize sound board data. Reinitializing storage file.");
            var defaults = CreateDefaultSoundBoards();
            await SaveInternalAsync(defaults, GetStorageFilePath(), cancellationToken).ConfigureAwait(false);
            return defaults;
        }
        finally
        {
            semaphore.Release();
        }
    }

    /// <inheritdoc />
    public async Task SaveSoundBoardsAsync(IEnumerable<SoundBoard> soundBoards, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(soundBoards);

        await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            var storagePath = GetStorageFilePath();
            var snapshot = soundBoards.Select(board => board.Clone()).ToList();
            await SaveInternalAsync(snapshot, storagePath, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            semaphore.Release();
        }
    }

    /// <inheritdoc />
    public async Task AddSoundBoardAsync(SoundBoard soundBoard, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(soundBoard);

        await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            var storagePath = GetStorageFilePath();
            var boards = await LoadInternalAsync(storagePath, cancellationToken).ConfigureAwait(false);
            boards.Add(soundBoard.Clone());
            await SaveInternalAsync(boards, storagePath, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            semaphore.Release();
        }
    }

    /// <inheritdoc />
    public async Task RemoveSoundBoardAsync(string title, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            var storagePath = GetStorageFilePath();
            var boards = await LoadInternalAsync(storagePath, cancellationToken).ConfigureAwait(false);
            var removedCount = boards.RemoveAll(board => string.Equals(board.Title, title, StringComparison.OrdinalIgnoreCase));

            if (removedCount == 0)
            {
                logger.LogWarning("No sound board with title '{Title}' was found for removal.", title);
                return;
            }

            await SaveInternalAsync(boards, storagePath, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            semaphore.Release();
        }
    }

    /// <summary>
    /// Disposes resources held by the service.
    /// </summary>
    public void Dispose()
    {
        semaphore.Dispose();
        GC.SuppressFinalize(this);
    }

    private static FileStream CreateFileStream(string path, FileMode fileMode, FileAccess fileAccess)
    {
        var directory = Path.GetDirectoryName(path);

        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        return new FileStream(path, fileMode, fileAccess, FileShare.None, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
    }

    private string GetStorageFilePath() => Path.Combine(fileSystem.AppDataDirectory, StorageFileName);

    private async Task<List<SoundBoard>> LoadInternalAsync(string path, CancellationToken cancellationToken)
    {
        if (!File.Exists(path))
        {
            var defaults = CreateDefaultSoundBoards();
            await SaveInternalAsync(defaults, path, cancellationToken).ConfigureAwait(false);
            return defaults;
        }

        await using var readStream = CreateFileStream(path, FileMode.Open, FileAccess.Read);
        var boards = await JsonSerializer.DeserializeAsync<List<SoundBoard>>(readStream, serializerOptions, cancellationToken).ConfigureAwait(false);
        return boards?.Select(board => board.Clone()).ToList() ?? new List<SoundBoard>();
    }

    private async Task SaveInternalAsync(IEnumerable<SoundBoard> soundBoards, string path, CancellationToken cancellationToken)
    {
        await using var writeStream = CreateFileStream(path, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(writeStream, soundBoards, serializerOptions, cancellationToken).ConfigureAwait(false);
    }

    private static List<SoundBoard> CreateDefaultSoundBoards()
    {
        return new List<SoundBoard>
        {
            new("Arcade Classics", "Relive the nostalgia of arcade cabinets with iconic sounds."),
            new("Sci-Fi Toolkit", "Spaceship bleeps, portal swirls, and synthetic ambiences."),
            new("Fantasy Adventure", "Quests, creatures, and spellcasting soundscapes for your campaign."),
        };
    }
}
