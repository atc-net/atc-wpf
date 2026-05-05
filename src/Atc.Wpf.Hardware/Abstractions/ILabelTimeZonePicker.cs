namespace Atc.Wpf.Hardware.Abstractions;

public interface ILabelTimeZonePicker : ILabelControl
{
    TimeZoneInfo? Value { get; set; }

    string WatermarkText { get; set; }
}