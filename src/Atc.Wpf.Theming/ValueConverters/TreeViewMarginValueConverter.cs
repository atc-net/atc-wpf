namespace Atc.Wpf.Theming.ValueConverters;

[ValueConversion(typeof(TreeViewItem), typeof(Thickness))]
public class TreeViewMarginValueConverter : IValueConverter
{
    public double Length { get; set; }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is TreeViewItem item
            ? new Thickness(Length * item.GetDepth(), 0, 0, 0)
            : new Thickness(0);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}