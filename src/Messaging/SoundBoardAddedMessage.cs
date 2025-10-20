using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SoundHz.Messaging;

/// <summary>
/// Represents a notification that a new <see cref="SoundBoard"/> has been created.
/// </summary>
public sealed class SoundBoardAddedMessage : ValueChangedMessage<SoundBoard>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SoundBoardAddedMessage"/> class.
    /// </summary>
    /// <param name="value">The newly created sound board.</param>
    public SoundBoardAddedMessage(SoundBoard value)
        : base(value ?? throw new ArgumentNullException(nameof(value)))
    {
    }
}
