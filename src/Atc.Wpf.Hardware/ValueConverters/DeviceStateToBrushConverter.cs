namespace Atc.Wpf.Hardware.ValueConverters;

/// <summary>
/// Converts a <see cref="DeviceState"/> to a status brush; non-state values use <see cref="UnknownBrush"/>.
/// </summary>
[ValueConversion(typeof(DeviceState), typeof(Brush))]
public sealed class DeviceStateToBrushConverter : IValueConverter
{
    /// <summary>
    /// The shared instance of the converter, using the default brushes.
    /// </summary>
    public static readonly DeviceStateToBrushConverter Instance = new();

    /// <summary>
    /// Gets or sets the brush for <see cref="DeviceState.Available"/> (default green).
    /// </summary>
    public Brush AvailableBrush { get; set; } = new SolidColorBrush(Color.FromRgb(0x33, 0xC7, 0x59));

    /// <summary>
    /// Gets or sets the brush for <see cref="DeviceState.JustConnected"/> (default green).
    /// </summary>
    public Brush JustConnectedBrush { get; set; } = new SolidColorBrush(Color.FromRgb(0x33, 0xC7, 0x59));

    /// <summary>
    /// Gets or sets the brush for <see cref="DeviceState.InUse"/> (default amber).
    /// </summary>
    public Brush InUseBrush { get; set; } = new SolidColorBrush(Color.FromRgb(0xE6, 0x9D, 0x17));

    /// <summary>
    /// Gets or sets the brush for <see cref="DeviceState.Disconnected"/> (default red).
    /// </summary>
    public Brush DisconnectedBrush { get; set; } = new SolidColorBrush(Color.FromRgb(0xD2, 0x3A, 0x3A));

    /// <summary>
    /// Gets or sets the brush for <see cref="DeviceState.Unknown"/> and for values that are not a <see cref="DeviceState"/> (default gray).
    /// </summary>
    public Brush UnknownBrush { get; set; } = new SolidColorBrush(Color.FromRgb(0x80, 0x80, 0x80));

    /// <inheritdoc />
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

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}