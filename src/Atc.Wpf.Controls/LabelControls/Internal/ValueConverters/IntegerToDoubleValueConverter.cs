namespace Atc.Wpf.Controls.LabelControls.Internal.ValueConverters;

/// <summary>
/// ValueConverter: Integer to Double.
/// </summary>
[ValueConversion(typeof(int), typeof(double))]
[ValueConversion(typeof(decimal), typeof(double))]
public class IntegerToDoubleValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value switch
        {
            int i => i,
            decimal d => d,
            _ => 0D,
        };

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value switch
        {
            double d => d,
            _ => 0,
        };
}