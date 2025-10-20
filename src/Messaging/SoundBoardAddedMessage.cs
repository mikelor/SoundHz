using CommunityToolkit.Mvvm.Messaging.Messages;
using SoundHz.Models;

namespace SoundHz.Messaging;

/// <summary>
/// Message published when a new <see cref="SoundBoard"/> should be added to the collection.
/// </summary>
public sealed class SoundBoardAddedMessage : ValueChangedMessage<SoundBoard>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SoundBoardAddedMessage"/> class.
    /// </summary>
    /// <param name="soundBoard">The sound board that was added.</param>
    public SoundBoardAddedMessage(SoundBoard soundBoard)
        : base(soundBoard ?? throw new ArgumentNullException(nameof(soundBoard)))
    {
    }
}
