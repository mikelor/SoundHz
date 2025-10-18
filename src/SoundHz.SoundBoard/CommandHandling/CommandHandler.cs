using System.Threading;
using System.Threading.Tasks;

namespace SoundHz.SoundBoard.CommandHandling;

/// <summary>
///     Defines a base contract for command handlers that execute operations based on an options payload.
/// </summary>
/// <typeparam name="TOptions">The type of options handled by the command.</typeparam>
public abstract class CommandHandler<TOptions>
{
    /// <summary>
    ///     Executes the command with the specified options and cancellation token.
    /// </summary>
    /// <param name="options">The command options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An awaitable task.</returns>
    public abstract Task ExecuteAsync(TOptions options, CancellationToken cancellationToken);
}
