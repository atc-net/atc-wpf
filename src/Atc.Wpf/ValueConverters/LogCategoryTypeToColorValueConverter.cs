namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: LogCategoryType To Color.
/// </summary>
[ValueConversion(typeof(LogCategoryType), typeof(Color))]
public sealed class LogCategoryTypeToColorValueConverter : IValueConverter
{
    public static readonly LogCategoryTypeToColorValueConverter Instance = new();

    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is null)
        {
            return Colors.DeepPink;
        }

        if (value is not LogCategoryType type)
        {
            throw new UnexpectedTypeException();
        }

        return type switch
        {
            LogCategoryType.Critical => Colors.Red,
            LogCategoryType.Error => Colors.Crimson,
            LogCategoryType.Warning => Colors.Goldenrod,
            LogCategoryType.Security => Colors.LightCyan,
            LogCategoryType.Audit => Colors.AntiqueWhite,
            LogCategoryType.Service => Colors.BurlyWood,
            LogCategoryType.UI => Colors.Aquamarine,
            LogCategoryType.Information => Colors.DodgerBlue,
            LogCategoryType.Debug => Colors.CadetBlue,
            LogCategoryType.Trace => Colors.Gray,
            _ => throw new SwitchCaseDefaultException(type),
        };
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException("This is a OneWay converter.");
}