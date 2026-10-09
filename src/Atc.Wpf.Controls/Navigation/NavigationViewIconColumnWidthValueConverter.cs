namespace Atc.Wpf.Controls.Navigation;

/// <summary>
/// Converts <see cref="NavigationView.CompactPaneLength"/> to the width of the icon column inside an item,
/// which is the compact pane minus the item's horizontal margin. Returns a <see cref="GridLength"/>
/// for column definitions and a <see cref="double"/> otherwise.
/// </summary>
[ValueConversion(typeof(double), typeof(GridLength))]
public sealed class NavigationViewIconColumnWidthValueConverter : IValueConverter
{
    /// <summary>
    /// The horizontal margin of an item on each side.
    /// </summary>
    public const double ItemMargin = 4;

    /// <summary>The shared instance of the converter.</summary>
    public static readonly NavigationViewIconColumnWidthValueConverter Instance = new();

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        var compactPaneLength = value is double length ? length : 48d;
        var width = System.Math.Max(0, compactPaneLength - (2 * ItemMargin));

        return targetType == typeof(GridLength)
            ? new GridLength(width)
            : width;
    }

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}