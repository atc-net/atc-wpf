namespace Atc.Wpf.Hardware.ValueConverters;

/// <summary>
/// Converts a <see cref="DeviceState"/> to a localized status text; <see cref="DeviceState.Unknown"/> and non-state values become an empty string.
/// </summary>
[ValueConversion(typeof(DeviceState), typeof(string))]
public sealed class DeviceStateToTextConverter : IValueConverter
{
    /// <summary>
    /// The shared instance of the converter.
    /// </summary>
    public static readonly DeviceStateToTextConverter Instance = new();

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not DeviceState state)
        {
            return string.Empty;
        }

        return state switch
        {
            DeviceState.Available => Miscellaneous.Available,
            DeviceState.JustConnected => Miscellaneous.New,
            DeviceState.InUse => Miscellaneous.InUse,
            DeviceState.Disconnected => Miscellaneous.Disconnected,
            _ => string.Empty,
        };
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}