namespace Atc.Wpf.Network.ValueConverters;

/// <summary>
/// ValueConverter: <see cref="ConnectionState"/> to <see cref="SolidColorBrush"/>.
/// </summary>
/// <remarks>
/// Returns frozen <see cref="SolidColorBrush"/> instances built from the palette defined on
/// <see cref="ConnectionStateToColorValueConverter"/> (single source of truth for the colors).
/// <para>
/// To override a brush for a single state, set the corresponding color on the
/// <see cref="ConnectionStateToColorValueConverter"/> — e.g.
/// <c>ConnectionStateToColorValueConverter.PulseColor = Colors.HotPink;</c>.
/// The brush converter detects the change and rebuilds (and re-caches) the affected brush.
/// </para>
/// </remarks>
[ValueConversion(typeof(ConnectionState), typeof(SolidColorBrush))]
public sealed class ConnectionStateToBrushValueConverter : IValueConverter
{
    public static readonly ConnectionStateToBrushValueConverter Instance = new();

    private static readonly ConcurrentDictionary<ConnectionState, SolidColorBrush> Cache = new();

    /// <summary>
    /// Gets a frozen <see cref="SolidColorBrush"/> for the given <paramref name="state"/>.
    /// Brushes are cached and rebuilt automatically when their underlying color changes.
    /// </summary>
    public static SolidColorBrush GetBrush(ConnectionState state)
    {
        var color = ConnectionStateToColorValueConverter.GetColor(state);
        return Cache.AddOrUpdate(
            state,
            _ => CreateFrozen(color),
            (_, existing) => existing.Color == color
                ? existing
                : CreateFrozen(color));
    }

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is ConnectionState state
            ? GetBrush(state)
            : GetBrush(ConnectionState.None);

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");

    private static SolidColorBrush CreateFrozen(Color color)
    {
        var brush = new SolidColorBrush(color);
        brush.Freeze();
        return brush;
    }
}