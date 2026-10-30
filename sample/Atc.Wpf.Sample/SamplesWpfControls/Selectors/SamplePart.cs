namespace Atc.Wpf.Sample.SamplesWpfControls.Selectors;

/// <summary>
/// A spare part row for the large <see cref="MultiColumnComboBox"/> sample.
/// </summary>
public sealed record SamplePart(
    string PartNumber,
    string Description,
    string Category,
    int Stock);