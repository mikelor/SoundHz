using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace SoundHz.SoundBoard.Models;

/// <summary>
///     Represents the persisted configuration for the application sound boards.
/// </summary>
public sealed class SoundBoardConfiguration
{
    /// <summary>
    ///     Gets or sets the configured sound boards.
    /// </summary>
    [Required]
    public ObservableCollection<SoundBoardDefinition> SoundBoards { get; set; } = new();
}
