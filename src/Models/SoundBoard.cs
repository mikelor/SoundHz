namespace SoundHz.Models;

/// <summary>
/// Represents a sound board that groups related sounds together.
/// </summary>
public sealed class SoundBoard : ObservableObject
{
    private string title = string.Empty;
    private string description = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="SoundBoard"/> class.
    /// </summary>
    public SoundBoard()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SoundBoard"/> class with the specified values.
    /// </summary>
    /// <param name="title">The display title for the sound board.</param>
    /// <param name="description">The descriptive text for the sound board.</param>
    public SoundBoard(string title, string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentNullException.ThrowIfNull(description);

        this.title = title;
        this.description = description;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SoundBoard"/> class by copying another instance.
    /// </summary>
    /// <param name="other">The sound board to copy.</param>
    public SoundBoard(SoundBoard other)
    {
        ArgumentNullException.ThrowIfNull(other);

        title = other.Title;
        description = other.Description;
    }

    /// <summary>
    /// Gets or sets the display title of the sound board.
    /// </summary>
    public string Title
    {
        get => title;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            SetProperty(ref title, value);
        }
    }

    /// <summary>
    /// Gets or sets descriptive text about the sound board.
    /// </summary>
    public string Description
    {
        get => description;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            SetProperty(ref description, value);
        }
    }

    /// <summary>
    /// Creates a new copy of the current <see cref="SoundBoard"/>.
    /// </summary>
    /// <returns>A new <see cref="SoundBoard"/> containing the same data.</returns>
    public SoundBoard Clone() => new(Title, Description);
}
