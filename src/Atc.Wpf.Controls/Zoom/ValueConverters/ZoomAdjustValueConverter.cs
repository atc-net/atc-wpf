namespace Atc.Wpf.Controls.Zoom.ValueConverters;

/// <summary>
/// ValueConverter: maps zoom factor between linear and logarithmic scales.
/// Convert applies <see cref="System.Math.Log(double)"/>; ConvertBack applies
/// <see cref="System.Math.Exp(double)"/>. Used to drive a zoom slider with logarithmic
/// pacing so that 1×, 2×, 4×, 8× are evenly spaced.
/// </summary>
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