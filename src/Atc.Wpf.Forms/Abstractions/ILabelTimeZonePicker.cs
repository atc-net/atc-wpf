namespace Atc.Wpf.Forms.Abstractions;

public interface ILabelTimeZonePicker : ILabelControl
{
    TimeZoneInfo? Value { get; set; }

    string WatermarkText { get; set; }
}