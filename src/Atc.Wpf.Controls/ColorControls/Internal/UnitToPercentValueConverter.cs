namespace Atc.Wpf.Controls.ColorControls.Internal;

/// <summary>
/// Converts from range [0 - 1] to percent.
/// </summary>
internal class UnitToPercentValueConverter : IValueConverter
{
    public static readonly UnitToPercentValueConverter Instance = new();

    public object Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is null)
        {
            return 0;
        }

        return (int)((double)value * 100);
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is null)
        {
            return 0;
        }

        return (int)value / 100.0;
    }
}