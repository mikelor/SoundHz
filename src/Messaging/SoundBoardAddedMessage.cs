namespace SoundHz.Messaging;

/// <summary>
/// Represents a notification that a new sound board has been added.
/// </summary>
public sealed class SoundBoardAddedMessage : ValueChangedMessage<SoundBoard>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SoundBoardAddedMessage"/> class.
    /// </summary>
    /// <param name="value">The sound board that was added.</param>
    public SoundBoardAddedMessage(SoundBoard value)
        : base(value)
    {
    }
}
