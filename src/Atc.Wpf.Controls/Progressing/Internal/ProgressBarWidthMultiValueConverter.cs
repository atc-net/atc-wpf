namespace Atc.Wpf.Controls.Progressing.Internal;

public class ProgressBarWidthMultiValueConverter : IMultiValueConverter
{
    public object Convert(object[]? values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values is null ||
            values.Length != 2 ||
            values[0] == DependencyProperty.UnsetValue ||
            values[1] == DependencyProperty.UnsetValue)
        {
            return 0;
        }

        var contentWidth = (double)values[0];
        var parentMinWidth = (double)values[1];

        return System.Math.Max(contentWidth, parentMinWidth);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}