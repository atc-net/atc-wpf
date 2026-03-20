namespace Atc.Wpf.Controls.Zoom.ValueConverters;

public class ZoomAdjustValueConverter : MarkupExtension, IValueConverter
{
    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;

    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is double d)
        {
            return System.Math.Log(d);
        }

        return null;
    }

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is double d)
        {
            return System.Math.Exp(d);
        }

        return null;
    }
}