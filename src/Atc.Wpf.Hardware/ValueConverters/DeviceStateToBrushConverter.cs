namespace Atc.Wpf.Hardware.ValueConverters;

[ValueConversion(typeof(DeviceState), typeof(Brush))]
public sealed class DeviceStateToBrushConverter : IValueConverter
{
    public static readonly DeviceStateToBrushConverter Instance = new();

    public Brush AvailableBrush { get; set; } = new SolidColorBrush(Color.FromRgb(0x33, 0xC7, 0x59));

    public Brush JustConnectedBrush { get; set; } = new SolidColorBrush(Color.FromRgb(0x33, 0xC7, 0x59));

    public Brush InUseBrush { get; set; } = new SolidColorBrush(Color.FromRgb(0xE6, 0x9D, 0x17));

    public Brush DisconnectedBrush { get; set; } = new SolidColorBrush(Color.FromRgb(0xD2, 0x3A, 0x3A));

    public Brush UnknownBrush { get; set; } = new SolidColorBrush(Color.FromRgb(0x80, 0x80, 0x80));

    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not DeviceState state)
        {
            return UnknownBrush;
        }

        return state switch
        {
            DeviceState.Available => AvailableBrush,
            DeviceState.JustConnected => JustConnectedBrush,
            DeviceState.InUse => InUseBrush,
            DeviceState.Disconnected => DisconnectedBrush,
            _ => UnknownBrush,
        };
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException();
}