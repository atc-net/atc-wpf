// ReSharper disable CheckNamespace
namespace Atc.Wpf.Components.Monitoring.Internal;

/// <summary>
/// Writes <see cref="ApplicationEventEntry"/> sequences to disk in
/// CSV / JSON / TXT formats. Creates the parent directory when missing.
/// All output is UTF-8.
/// </summary>
internal static class ApplicationMonitorExportService
{
    public static void Export(
        IEnumerable<ApplicationEventEntry> entries,
        string filePath,
        ApplicationMonitorExportFormat format)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var dir = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var content = format switch
        {
            ApplicationMonitorExportFormat.Csv => BuildCsv(entries),
            ApplicationMonitorExportFormat.Json => BuildJson(entries),
            ApplicationMonitorExportFormat.Txt => BuildTxt(entries),
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Unknown export format."),
        };

        File.WriteAllText(filePath, content, Encoding.UTF8);
    }

    /// <summary>
    /// Maps a file extension (with or without leading dot) to an export format.
    /// Falls back to <see cref="ApplicationMonitorExportFormat.Csv"/>.
    /// </summary>
    public static ApplicationMonitorExportFormat InferFormat(string filePath)
    {
        var ext = Path.GetExtension(filePath).TrimStart('.').ToUpperInvariant();
        return ext switch
        {
            "JSON" => ApplicationMonitorExportFormat.Json,
            "TXT" or "LOG" => ApplicationMonitorExportFormat.Txt,
            _ => ApplicationMonitorExportFormat.Csv,
        };
    }

    private static string BuildCsv(IEnumerable<ApplicationEventEntry> entries)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Timestamp,Category,Area,Message");

        foreach (var entry in entries)
        {
            sb.Append(entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff", GlobalizationConstants.EnglishCultureInfo));
            sb.Append(',');
            sb.Append(CsvEscape(entry.LogCategoryType.ToString()));
            sb.Append(',');
            sb.Append(CsvEscape(entry.Area));
            sb.Append(',');
            sb.AppendLine(CsvEscape(entry.Message));
        }

        return sb.ToString();
    }

    private static string CsvEscape(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        if (value.Contains(',') ||
            value.Contains('"') ||
            value.Contains('\n') ||
            value.Contains('\r'))
        {
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        return value;
    }

    private static string BuildJson(IEnumerable<ApplicationEventEntry> entries)
    {
        var dtos = entries.Select(entry => new
        {
            timestamp = entry.Timestamp,
            category = entry.LogCategoryType.ToString(),
            area = entry.Area,
            message = entry.Message,
        });

        return System.Text.Json.JsonSerializer.Serialize(
            dtos,
            new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
            });
    }

    private static string BuildTxt(IEnumerable<ApplicationEventEntry> entries)
    {
        var sb = new StringBuilder();

        foreach (var entry in entries)
        {
            sb.Append(entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff", GlobalizationConstants.EnglishCultureInfo));
            sb.Append(" [");
            sb.Append(entry.LogCategoryType);
            sb.Append("] ");
            sb.Append(entry.Area);
            sb.Append(" | ");
            sb.AppendLine(entry.Message);
        }

        return sb.ToString();
    }
}
