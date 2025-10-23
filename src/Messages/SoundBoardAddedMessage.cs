using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SoundHz.Messages;

/// <summary>
/// Represents a message indicating that a new <see cref="SoundBoard"/> has been created.
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
