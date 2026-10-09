namespace Atc.Wpf.Controls.Sample;

/// <summary>
/// Message sent when a sample is selected in the sample tree, telling the sample viewer which sample to show.
/// </summary>
public sealed class SampleItemMessage : MessageBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SampleItemMessage"/> class.
    /// </summary>
    /// <param name="header">The header of the selected sample.</param>
    /// <param name="sampleItemPath">The path of the selected sample, or <see langword="null"/> when no sample is selected.</param>
    public SampleItemMessage(
        string? header,
        string? sampleItemPath)
    {
        Header = header;
        SampleItemPath = sampleItemPath;
    }

    /// <summary>
    /// Gets the header of the selected sample.
    /// </summary>
    public string? Header { get; }

    /// <summary>
    /// Gets the path of the selected sample, used to locate the sample type.
    /// </summary>
    public string? SampleItemPath { get; }

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(Header)}: {Header}, {nameof(SampleItemPath)}: {SampleItemPath}";
}