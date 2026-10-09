namespace Atc.Wpf.Theming.ValueConverters;

/// <summary>
/// ValueConverter: <see cref="TreeViewItem"/> → left-indent <see cref="Thickness"/> proportional
/// to the item's depth in the tree. Multiplies depth by <see cref="Length"/> to compute the left
/// margin; top/right/bottom remain zero. Non-<see cref="TreeViewItem"/> values return
/// <see cref="Thickness"/> zero.
/// </summary>
[ValueConversion(typeof(TreeViewItem), typeof(Thickness))]
public sealed class TreeViewMarginValueConverter : IValueConverter
{
    /// <summary>
    /// Gets the shared instance of the converter.
    /// </summary>
    public static readonly TreeViewMarginValueConverter Instance = new();

    /// <summary>
    /// Gets or sets the indent applied per depth level.
    /// </summary>
    public double Length { get; set; }

    /// <inheritdoc />
    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is TreeViewItem item
            ? new Thickness(Length * item.GetDepth(), 0, 0, 0)
            : new Thickness(0);

    /// <inheritdoc />
    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => DependencyProperty.UnsetValue;
}