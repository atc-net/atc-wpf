namespace Atc.Wpf.Sample.ScreenshotGeneration;

/// <summary>
/// Generates a markdown report of all captured control screenshots.
/// </summary>
internal static class ScreenshotReport
{
    public static void Generate(
        string outputPath,
        IReadOnlyList<(ScreenshotItem Item, CaptureResult? Result, string? SkipReason)> results)
    {
        var sb = new StringBuilder();

        var successes = results.Where(r => r.Result is { IsSuccess: true }).ToList();
        var failures = results.Where(r => r.Result is null or { IsSuccess: false }).ToList();

        var groupedByArea = results
            .GroupBy(r => r.Item.AssemblyArea, StringComparer.OrdinalIgnoreCase)
            .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase)
            .ToList();

        AppendHeader(sb, successes.Count, failures.Count);
        AppendTableOfContents(sb, groupedByArea);
        AppendScreenshotSections(sb, groupedByArea);
        AppendFailuresTable(sb, failures);

        var directory = Path.GetDirectoryName(outputPath);
        if (directory is not null && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(outputPath, sb.ToString());
    }

    private static void AppendHeader(
        StringBuilder sb,
        int successCount,
        int failureCount)
    {
        sb.AppendLine("# ATC-WPF Control Screenshots");
        sb.AppendLine();
        sb.AppendLine(CultureInfo.InvariantCulture, $"> Auto-generated on {DateTime.Now:yyyy-MM-dd HH:mm}. {successCount} captured, {failureCount} failed/skipped.");
        sb.AppendLine();
    }

    private static void AppendTableOfContents(
        StringBuilder sb,
        List<IGrouping<string, (ScreenshotItem Item, CaptureResult? Result, string? SkipReason)>> groupedByArea)
    {
        sb.AppendLine("## Table of Contents");
        sb.AppendLine();
        foreach (var areaGroup in groupedByArea)
        {
            var anchorName = areaGroup.Key
                .Replace(".", string.Empty, StringComparison.Ordinal)
                .ToLowerInvariant();
            var successCount = areaGroup.Count(r => r.Result is { IsSuccess: true });
            sb.AppendLine(CultureInfo.InvariantCulture, $"- [{areaGroup.Key}](#{anchorName}) ({successCount} controls)");
        }

        sb.AppendLine();
    }

    private static void AppendScreenshotSections(
        StringBuilder sb,
        List<IGrouping<string, (ScreenshotItem Item, CaptureResult? Result, string? SkipReason)>> groupedByArea)
    {
        foreach (var areaGroup in groupedByArea)
        {
            sb.AppendLine(CultureInfo.InvariantCulture, $"## {areaGroup.Key}");
            sb.AppendLine();

            var groupedByCategory = areaGroup
                .GroupBy(r => r.Item.Category, StringComparer.OrdinalIgnoreCase)
                .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase);

            foreach (var categoryGroup in groupedByCategory)
            {
                sb.AppendLine(CultureInfo.InvariantCulture, $"### {categoryGroup.Key}");
                sb.AppendLine();

                foreach (var (item, result, _) in categoryGroup
                             .OrderBy(r => r.Item.ControlName, StringComparer.OrdinalIgnoreCase))
                {
                    if (result is not { IsSuccess: true })
                    {
                        continue;
                    }

                    sb.AppendLine(CultureInfo.InvariantCulture, $"#### {item.ControlName}");
                    sb.AppendLine();
                    var imagePath = $"images/screenshots/{item.AssemblyArea}/{item.Category}/{item.ControlName}.png";
                    sb.AppendLine(CultureInfo.InvariantCulture, $"![{item.ControlName}]({imagePath})");
                    sb.AppendLine();
                }
            }
        }
    }

    private static void AppendFailuresTable(
        StringBuilder sb,
        List<(ScreenshotItem Item, CaptureResult? Result, string? SkipReason)> failures)
    {
        if (failures.Count == 0)
        {
            return;
        }

        sb.AppendLine("## Failed / Skipped Controls");
        sb.AppendLine();
        sb.AppendLine("| Control | Area | Category | Reason |");
        sb.AppendLine("|---------|------|----------|--------|");

        foreach (var (item, result, skipReason) in failures
                     .OrderBy(r => r.Item.AssemblyArea, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(r => r.Item.ControlName, StringComparer.OrdinalIgnoreCase))
        {
            var reason = skipReason ?? result?.ErrorMessage ?? "Unknown error";
            sb.AppendLine(CultureInfo.InvariantCulture, $"| {item.ControlName} | {item.AssemblyArea} | {item.Category} | {reason} |");
        }

        sb.AppendLine();
    }
}