// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
namespace Atc.Wpf.Options;

public sealed class RecentOpenFilesOption
{
    public const string SectionName = "RecentOpenFiles";

    public IList<RecentOpenFileOption> RecentOpenFiles { get; init; } = [];

    public override string ToString()
        => $"{nameof(RecentOpenFiles)}.Count: {RecentOpenFiles?.Count}";
}