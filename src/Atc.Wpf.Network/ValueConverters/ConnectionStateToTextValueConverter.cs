namespace Atc.Wpf.Network.ValueConverters;

/// <summary>
/// Converts ConnectionState to localized text.
/// </summary>
[ValueConversion(typeof(ConnectionState), typeof(string))]
public sealed class ConnectionStateToTextValueConverter : IValueConverter
{
    public static ConnectionStateToTextValueConverter Instance { get; } = new();

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is ConnectionState connectionState)
        {
            return connectionState.GetDescription();
        }

        return "Unknown";
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException();
}