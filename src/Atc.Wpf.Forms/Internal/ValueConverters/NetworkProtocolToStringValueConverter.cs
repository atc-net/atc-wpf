namespace Atc.Wpf.Forms.Internal.ValueConverters;

/// <summary>
/// Internal copy of the public <see cref="Atc.Wpf.Controls.ValueConverters.NetworkProtocolToStringValueConverter"/>.
/// </summary>
/// <remarks>
/// <para>
/// Functionally identical to <see cref="Atc.Wpf.Controls.ValueConverters.NetworkProtocolToStringValueConverter"/>.
/// Prefer the public converter in <c>Atc.Wpf.Controls</c> when accessible. This internal copy
/// exists so labelled form controls in <c>Atc.Wpf.Forms</c> can reference it without leaking the
/// type to the public Forms API surface.
/// </para>
/// <para>
/// If both converters' bodies ever diverge, update <see cref="Atc.Wpf.Controls.ValueConverters.NetworkProtocolToStringValueConverter"/>
/// first (it is the canonical source) and mirror the change here.
/// </para>
/// </remarks>
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