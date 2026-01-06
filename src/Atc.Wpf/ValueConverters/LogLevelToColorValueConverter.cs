namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: LogLevel To Color.
/// </summary>
[ValueConversion(typeof(LogLevel), typeof(Color))]
public sealed class LogLevelToColorValueConverter : IValueConverter
{
    public static readonly LogLevelToColorValueConverter Instance = new();

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

        if (value is not LogLevel type)
        {
            throw new UnexpectedTypeException();
        }

        return type switch
        {
            LogLevel.Critical => Colors.Red,
            LogLevel.Error => Colors.Crimson,
            LogLevel.Warning => Colors.Goldenrod,
            LogLevel.Information => Colors.DodgerBlue,
            LogLevel.Debug => Colors.CadetBlue,
            LogLevel.Trace => Colors.Gray,
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