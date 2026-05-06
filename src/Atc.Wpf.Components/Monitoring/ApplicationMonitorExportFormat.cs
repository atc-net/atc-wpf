// ReSharper disable CheckNamespace
namespace Atc.Wpf.Components.Monitoring;

/// <summary>
/// File format for the Application Monitor export operation. Inferred from
/// the chosen file extension in the Save File dialog: <c>.csv</c>, <c>.json</c>,
/// or <c>.txt</c> / <c>.log</c>.
/// </summary>
public enum ApplicationMonitorExportFormat
{
    /// <summary>Comma-separated values with a header row, RFC 4180 quoting.</summary>
    Csv,

    /// <summary>Pretty-printed JSON array of entries.</summary>
    Json,

    /// <summary>Plain pipe-delimited text, one entry per line.</summary>
    Txt,
}
