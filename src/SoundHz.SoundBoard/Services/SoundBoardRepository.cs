using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SoundHz.SoundBoard.Models;
using SoundHz.SoundBoard.Resources.Localization;
using Microsoft.Extensions.Logging;

namespace SoundHz.SoundBoard.Services;

/// <summary>
///     Provides persistence for <see cref="SoundBoardConfiguration"/> instances.
/// </summary>
public sealed class SoundBoardRepository(IAppFileSystem fileSystem, IJsonSerializer jsonSerializer, ILogger<SoundBoardRepository> logger) : ISoundBoardRepository
{
    private readonly IAppFileSystem _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    private readonly IJsonSerializer _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
    private readonly ILogger<SoundBoardRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private string ConfigurationFilePath => Path.Combine(_fileSystem.GetAppDataDirectory(), "soundboards.json");

    /// <inheritdoc />
    public async Task<SoundBoardConfiguration> LoadAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (!await _fileSystem.FileExistsAsync(ConfigurationFilePath, cancellationToken).ConfigureAwait(false))
            {
                return new SoundBoardConfiguration();
            }

            await using var stream = await _fileSystem.OpenReadAsync(ConfigurationFilePath, cancellationToken).ConfigureAwait(false);
            var configuration = await _jsonSerializer.DeserializeAsync<SoundBoardConfiguration>(stream, cancellationToken).ConfigureAwait(false)
                                ?? new SoundBoardConfiguration();
            ValidateConfiguration(configuration);
            _logger.LogInformation(LogMessagesResourceManager.Instance.GetString("SoundBoardLoaded", CultureInfo.CurrentCulture), ConfigurationFilePath);
            return configuration;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessagesResourceManager.Instance.GetString("ConfigurationLoadFailed", CultureInfo.CurrentCulture));
            throw;
        }
    }

    /// <inheritdoc />
    public async Task SaveAsync(SoundBoardConfiguration configuration, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ValidateConfiguration(configuration);

        try
        {
            await using var stream = await _fileSystem.OpenWriteAsync(ConfigurationFilePath, cancellationToken).ConfigureAwait(false);
            await _jsonSerializer.SerializeAsync(stream, configuration, cancellationToken).ConfigureAwait(false);
            _logger.LogInformation(LogMessagesResourceManager.Instance.GetString("SoundBoardSaved", CultureInfo.CurrentCulture), ConfigurationFilePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessagesResourceManager.Instance.GetString("ConfigurationSaveFailed", CultureInfo.CurrentCulture));
            throw;
        }
    }

    private static void ValidateConfiguration(SoundBoardConfiguration configuration)
    {
        var context = new ValidationContext(configuration);
        Validator.ValidateObject(configuration, context, validateAllProperties: true);

        foreach (var board in configuration.SoundBoards)
        {
            var boardContext = new ValidationContext(board);
            Validator.ValidateObject(board, boardContext, true);

            foreach (var sound in board.Sounds)
            {
                var soundContext = new ValidationContext(sound);
                Validator.ValidateObject(sound, soundContext, true);
            }
        }
    }
}
