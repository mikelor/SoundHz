using System.Threading;
using System.Threading.Tasks;
using SoundHz.SoundBoard.Models;
using SoundHz.SoundBoard.Services;

namespace SoundHz.SoundBoard.CommandHandling;

/// <summary>
///     Saves the provided sound board configuration via <see cref="ISoundBoardRepository"/>.
/// </summary>
public sealed class SaveSoundBoardCommandHandler(ISoundBoardRepository repository) : CommandHandler<SoundBoardConfiguration>
{
    private readonly ISoundBoardRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    /// <inheritdoc />
    public override async Task ExecuteAsync(SoundBoardConfiguration options, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(options);
        await _repository.SaveAsync(options, cancellationToken).ConfigureAwait(false);
    }
}
