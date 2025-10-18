using System.ComponentModel.DataAnnotations;

namespace SoundHz.SoundBoard.Models;

/// <summary>
///     Represents a configured sound entry that can be played from the sound board.
/// </summary>
public sealed class SoundDefinition
{
    /// <summary>
    ///     Gets or sets the display name of the sound.
    /// </summary>
    [Required]
    [MinLength(1)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the description of the sound entry.
    /// </summary>
    [Required]
    [MinLength(1)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the path to the sound file associated with the entry.
    /// </summary>
    [Required]
    [MinLength(1)]
    public string FilePath { get; set; } = string.Empty;
}
