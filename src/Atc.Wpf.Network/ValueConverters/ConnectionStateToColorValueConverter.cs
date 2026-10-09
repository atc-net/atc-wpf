namespace Atc.Wpf.Network.ValueConverters;

/// <summary>
/// ValueConverter: <see cref="ConnectionState"/> to <see cref="Color"/>.
/// </summary>
/// <remarks>
/// Source of truth for the connection-state color palette.
/// <see cref="ConnectionStateToBrushValueConverter"/> wraps these same colors in frozen
/// <see cref="SolidColorBrush"/> instances.
/// <para>
/// Each per-state color is a mutable static property so consumers can override individual
/// entries at application startup, e.g.
/// <c>ConnectionStateToColorValueConverter.PulseColor = Colors.HotPink;</c>.
/// Call <see cref="ResetToDefaults"/> to restore the built-in palette.
/// </para>
/// <para>
/// Note: WPF bindings are not re-evaluated automatically when these static properties change —
/// set overrides during application startup before any binding fires.
/// </para>
/// <para>
/// Colors are drawn from the Material Design 300-800 weights and chosen to be recognisable
/// by hue on both light and dark application backgrounds. Status pips are hue-coded
/// (red = error, green = ok, amber = in-progress, blue = info) — strict WCAG 3:1 contrast
/// on both themes is not achievable for a natural status palette because amber/yellow are
/// inherently bright and dark reds are inherently dark.
/// </para>
/// </remarks>
[ValueConversion(typeof(ConnectionState), typeof(Color))]
public sealed class ConnectionStateToColorValueConverter : IValueConverter
{
    /// <summary>
    /// Gets the shared instance of the converter.
    /// </summary>
    public static readonly ConnectionStateToColorValueConverter Instance = new();

    /// <summary>The built-in color for <see cref="ConnectionState.None"/>.</summary>
    public static readonly Color DefaultNoneColor = Color.FromRgb(0x90, 0xA4, 0xAE);

    /// <summary>The built-in color for <see cref="ConnectionState.Connecting"/>.</summary>
    public static readonly Color DefaultConnectingColor = Color.FromRgb(0xFF, 0xB3, 0x00);

    /// <summary>The built-in color for <see cref="ConnectionState.Connected"/>.</summary>
    public static readonly Color DefaultConnectedColor = Color.FromRgb(0x4C, 0xAF, 0x50);

    /// <summary>The built-in color for <see cref="ConnectionState.Disconnecting"/>.</summary>
    public static readonly Color DefaultDisconnectingColor = Color.FromRgb(0xFB, 0x8C, 0x00);

    /// <summary>The built-in color for <see cref="ConnectionState.Disconnected"/>.</summary>
    public static readonly Color DefaultDisconnectedColor = Color.FromRgb(0x9E, 0x9E, 0x9E);

    /// <summary>The built-in color for <see cref="ConnectionState.ConnectionFailed"/>.</summary>
    public static readonly Color DefaultConnectionFailedColor = Color.FromRgb(0xEF, 0x53, 0x50);

    /// <summary>The built-in color for <see cref="ConnectionState.ReconnectionFailed"/>.</summary>
    public static readonly Color DefaultReconnectionFailedColor = Color.FromRgb(0xD3, 0x2F, 0x2F);

    /// <summary>The built-in color for <see cref="ConnectionState.Reconnecting"/>.</summary>
    public static readonly Color DefaultReconnectingColor = Color.FromRgb(0xFF, 0xCA, 0x28);

    /// <summary>The built-in color for <see cref="ConnectionState.Reconnected"/>.</summary>
    public static readonly Color DefaultReconnectedColor = Color.FromRgb(0x66, 0xBB, 0x6A);

    /// <summary>The built-in color for <see cref="ConnectionState.Pulse"/>.</summary>
    public static readonly Color DefaultPulseColor = Color.FromRgb(0x42, 0xA5, 0xF5);

    /// <summary>
    /// Gets or sets the color used for <see cref="ConnectionState.None"/>.
    /// </summary>
    public static Color NoneColor { get; set; } = DefaultNoneColor;

    /// <summary>
    /// Gets or sets the color used for <see cref="ConnectionState.Connecting"/>.
    /// </summary>
    public static Color ConnectingColor { get; set; } = DefaultConnectingColor;

    /// <summary>
    /// Gets or sets the color used for <see cref="ConnectionState.Connected"/>.
    /// </summary>
    public static Color ConnectedColor { get; set; } = DefaultConnectedColor;

    /// <summary>
    /// Gets or sets the color used for <see cref="ConnectionState.Disconnecting"/>.
    /// </summary>
    public static Color DisconnectingColor { get; set; } = DefaultDisconnectingColor;

    /// <summary>
    /// Gets or sets the color used for <see cref="ConnectionState.Disconnected"/>.
    /// </summary>
    public static Color DisconnectedColor { get; set; } = DefaultDisconnectedColor;

    /// <summary>
    /// Gets or sets the color used for <see cref="ConnectionState.ConnectionFailed"/>.
    /// </summary>
    public static Color ConnectionFailedColor { get; set; } = DefaultConnectionFailedColor;

    /// <summary>
    /// Gets or sets the color used for <see cref="ConnectionState.ReconnectionFailed"/>.
    /// </summary>
    public static Color ReconnectionFailedColor { get; set; } = DefaultReconnectionFailedColor;

    /// <summary>
    /// Gets or sets the color used for <see cref="ConnectionState.Reconnecting"/>.
    /// </summary>
    public static Color ReconnectingColor { get; set; } = DefaultReconnectingColor;

    /// <summary>
    /// Gets or sets the color used for <see cref="ConnectionState.Reconnected"/>.
    /// </summary>
    public static Color ReconnectedColor { get; set; } = DefaultReconnectedColor;

    /// <summary>
    /// Gets or sets the color used for <see cref="ConnectionState.Pulse"/>.
    /// </summary>
    public static Color PulseColor { get; set; } = DefaultPulseColor;

    /// <summary>
    /// Gets the <see cref="Color"/> currently configured for the given <paramref name="state"/>.
    /// </summary>
    public static Color GetColor(ConnectionState state)
        => state switch
        {
            ConnectionState.None => NoneColor,
            ConnectionState.Connecting => ConnectingColor,
            ConnectionState.Connected => ConnectedColor,
            ConnectionState.Disconnecting => DisconnectingColor,
            ConnectionState.Disconnected => DisconnectedColor,
            ConnectionState.ConnectionFailed => ConnectionFailedColor,
            ConnectionState.ReconnectionFailed => ReconnectionFailedColor,
            ConnectionState.Reconnecting => ReconnectingColor,
            ConnectionState.Reconnected => ReconnectedColor,
            ConnectionState.Pulse => PulseColor,
            _ => NoneColor,
        };

    /// <summary>
    /// Sets the <see cref="Color"/> override for a single <paramref name="state"/>.
    /// </summary>
    public static void SetColor(
        ConnectionState state,
        Color color)
    {
        switch (state)
        {
            case ConnectionState.None:
                NoneColor = color;
                break;
            case ConnectionState.Connecting:
                ConnectingColor = color;
                break;
            case ConnectionState.Connected:
                ConnectedColor = color;
                break;
            case ConnectionState.Disconnecting:
                DisconnectingColor = color;
                break;
            case ConnectionState.Disconnected:
                DisconnectedColor = color;
                break;
            case ConnectionState.ConnectionFailed:
                ConnectionFailedColor = color;
                break;
            case ConnectionState.ReconnectionFailed:
                ReconnectionFailedColor = color;
                break;
            case ConnectionState.Reconnecting:
                ReconnectingColor = color;
                break;
            case ConnectionState.Reconnected:
                ReconnectedColor = color;
                break;
            case ConnectionState.Pulse:
                PulseColor = color;
                break;
            default:
                throw new SwitchCaseDefaultException(state);
        }
    }

    /// <summary>
    /// Restores the built-in default palette (clears all consumer overrides).
    /// </summary>
    public static void ResetToDefaults()
    {
        NoneColor = DefaultNoneColor;
        ConnectingColor = DefaultConnectingColor;
        ConnectedColor = DefaultConnectedColor;
        DisconnectingColor = DefaultDisconnectingColor;
        DisconnectedColor = DefaultDisconnectedColor;
        ConnectionFailedColor = DefaultConnectionFailedColor;
        ReconnectionFailedColor = DefaultReconnectionFailedColor;
        ReconnectingColor = DefaultReconnectingColor;
        ReconnectedColor = DefaultReconnectedColor;
        PulseColor = DefaultPulseColor;
    }

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is ConnectionState state
            ? GetColor(state)
            : NoneColor;

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}