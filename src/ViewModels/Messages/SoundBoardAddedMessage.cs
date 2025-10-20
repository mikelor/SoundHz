using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SoundHz.ViewModels.Messages;

/// <summary>
/// Represents a notification that a new <see cref="SoundBoard"/> has been created.
/// </summary>
public sealed class SoundBoardAddedMessage(SoundBoard value) : ValueChangedMessage<SoundBoard>(value)
{
}
