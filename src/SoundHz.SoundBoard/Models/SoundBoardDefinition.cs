using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace SoundHz.SoundBoard.Models;

/// <summary>
///     Represents a sound board containing a collection of individual sound definitions.
/// </summary>
public sealed class SoundBoardDefinition
{
    /// <summary>
    ///     Gets or sets the name of the sound board.
    /// </summary>
    [Required]
    [MinLength(1)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the description of the sound board.
    /// </summary>
    [Required]
    [MinLength(1)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the collection of sound definitions configured for this board.
    /// </summary>
    [Required]
    public ObservableCollection<SoundDefinition> Sounds { get; set; } = new();
}
