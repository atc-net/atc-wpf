// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls.ValueConverters;

/// <summary>
/// Maps a bool to a <see cref="Atc.Wpf.Controls.DataDisplay.PopoverTriggerMode"/>: <c>true</c> → Hover,
/// <c>false</c> → Manual. Used to disable a hover-popover when its content
/// would be empty (e.g. the prerequisites popover when no items are missing),
/// so hover events do not pop a useless empty/redundant tooltip.
/// </summary>
[ValueConversion(typeof(bool), typeof(DataDisplay.PopoverTriggerMode))]
public sealed class BoolToPopoverTriggerModeValueConverter : IValueConverter
{
    public static readonly BoolToPopoverTriggerModeValueConverter Instance = new();

    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is true
            ? DataDisplay.PopoverTriggerMode.Hover
            : DataDisplay.PopoverTriggerMode.Manual;

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}