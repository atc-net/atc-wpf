namespace Atc.Wpf.Options;

/// <summary>
/// Represents a recently opened file.
/// </summary>
public sealed class RecentOpenFileOption
{
    /// <summary>The configuration section name.</summary>
    public const string SectionName = "RecentOpenFile";

    /// <summary>
    /// Gets or sets the time the file was opened.
    /// </summary>
    public DateTime TimeStamp { get; set; }

    /// <summary>
    /// Gets or sets the path of the file.
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(TimeStamp)}: {TimeStamp}, {nameof(FilePath)}: {FilePath}";
}