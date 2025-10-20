using CommunityToolkit.Mvvm.Messaging.Messages;

using SoundHz.Models;

namespace SoundHz.ViewModels.Messages;

/// <summary>
/// Represents a notification indicating that a new <see cref="SoundBoard"/> has been created.
/// </summary>
public sealed class SoundBoardAddedMessage(SoundBoard value) : ValueChangedMessage<SoundBoard>(value)
{
}
