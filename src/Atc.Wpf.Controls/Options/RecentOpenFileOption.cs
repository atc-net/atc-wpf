namespace Atc.Wpf.Controls.Options;

public sealed class RecentOpenFileOption
{
    public const string SectionName = "RecentOpenFile";

    public DateTime TimeStamp { get; set; }

    public string FilePath { get; set; } = string.Empty;

    public override string ToString()
        => $"{nameof(TimeStamp)}: {TimeStamp}, {nameof(FilePath)}: {FilePath}";
}