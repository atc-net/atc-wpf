// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
namespace Atc.Wpf.Options;

/// <summary>
/// Represents the list of recently opened files.
/// </summary>
public sealed class RecentOpenFilesOption
{
    /// <summary>The configuration section name.</summary>
    public const string SectionName = "RecentOpenFiles";

    /// <summary>
    /// Gets the recently opened files.
    /// </summary>
    public IList<RecentOpenFileOption> RecentOpenFiles { get; init; } = [];

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(RecentOpenFiles)}.Count: {RecentOpenFiles?.Count}";
}