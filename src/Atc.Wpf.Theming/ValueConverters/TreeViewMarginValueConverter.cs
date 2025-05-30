namespace Atc.Wpf.Theming.ValueConverters;

[ValueConversion(typeof(TreeViewItem), typeof(Thickness))]
public sealed class TreeViewMarginValueConverter : IValueConverter
{
    public static readonly TreeViewMarginValueConverter Instance = new();

    public double Length { get; set; }

    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => value is TreeViewItem item
            ? new Thickness(Length * item.GetDepth(), 0, 0, 0)
            : new Thickness(0);

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => DependencyProperty.UnsetValue;
}