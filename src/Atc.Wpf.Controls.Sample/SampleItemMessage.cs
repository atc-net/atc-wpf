namespace Atc.Wpf.Controls.Sample;

public sealed class SampleItemMessage : MessageBase
{
    public SampleItemMessage(
        string? header,
        string? sampleItemPath)
    {
        Header = header;
        SampleItemPath = sampleItemPath;
    }

    public string? Header { get; }

    public string? SampleItemPath { get; }

    public override string ToString()
        => $"{nameof(Header)}: {Header}, {nameof(SampleItemPath)}: {SampleItemPath}";
}