namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: LogCategoryType To (SolidColor)Brush.
/// </summary>
[ValueConversion(typeof(LogCategoryType), typeof(SolidColorBrush))]
public sealed class LogCategoryTypeToBrushValueConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return Brushes.DeepPink;
        }

        if (value is not LogCategoryType type)
        {
            throw new UnexpectedTypeException();
        }

        return type switch
        {
            LogCategoryType.Critical => Brushes.Red,
            LogCategoryType.Error => Brushes.Crimson,
            LogCategoryType.Warning => Brushes.Goldenrod,
            LogCategoryType.Security => Brushes.LightCyan,
            LogCategoryType.Audit => Brushes.AntiqueWhite,
            LogCategoryType.Service => Brushes.BurlyWood,
            LogCategoryType.UI => Brushes.Aquamarine,
            LogCategoryType.Information => Brushes.DodgerBlue,
            LogCategoryType.Debug => Brushes.CadetBlue,
            LogCategoryType.Trace => Brushes.Gray,
            _ => throw new SwitchCaseDefaultException(type),
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("This is a OneWay converter.");
    }
}