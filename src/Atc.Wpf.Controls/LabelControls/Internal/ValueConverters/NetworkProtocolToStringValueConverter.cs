namespace Atc.Wpf.Controls.LabelControls.Internal.ValueConverters;

[ValueConversion(typeof(NetworkProtocolType), typeof(string))]
internal sealed class NetworkProtocolToStringValueConverter : IValueConverter
{
    public static readonly NetworkProtocolToStringValueConverter Instance = new();

    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value switch
        {
            null => string.Empty,
            NetworkProtocolType protocolType => NetworkProtocolHelper.GetSchemeFromProtocol(protocolType),
            string s => s,
            _ => Binding.DoNothing,
        };

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value switch
        {
            null => NetworkProtocolType.None,
            NetworkProtocolType protocolType => protocolType,
            string s when string.IsNullOrWhiteSpace(s) => NetworkProtocolType.None,
            string s => NetworkProtocolHelper.GetProtocolFromScheme(s.Trim()),
            _ => Binding.DoNothing,
        };
}