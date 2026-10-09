namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Represents a labeled time zone picker control.
/// </summary>
public interface ILabelTimeZonePicker : ILabelControl
{
    /// <summary>
    /// Gets or sets the selected time zone.
    /// </summary>
    TimeZoneInfo? Value { get; set; }

    /// <summary>
    /// Gets or sets the watermark text shown when no time zone is selected.
    /// </summary>
    string WatermarkText { get; set; }
}