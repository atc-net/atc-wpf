namespace Atc.Wpf.Hardware.ValueConverters;

[ValueConversion(typeof(DeviceState), typeof(string))]
public sealed class DeviceStateToTextConverter : IValueConverter
{
    public static readonly DeviceStateToTextConverter Instance = new();

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

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException();
}