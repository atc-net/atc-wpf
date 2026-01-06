namespace Atc.Wpf.ValueConverters;

/// <summary>
/// ValueConverter: LogLevel To (SolidColor)Brush.
/// </summary>
[ValueConversion(typeof(LogLevel), typeof(SolidColorBrush))]
public sealed class LogLevelToBrushValueConverter : IValueConverter
{
    public static readonly LogLevelToBrushValueConverter Instance = new();

    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is null)
        {
            return Brushes.DeepPink;
        }

        if (value is not LogLevel type)
        {
            throw new UnexpectedTypeException();
        }

        return type switch
        {
            LogLevel.Critical => Brushes.Red,
            LogLevel.Error => Brushes.Crimson,
            LogLevel.Warning => Brushes.Goldenrod,
            LogLevel.Information => Brushes.DodgerBlue,
            LogLevel.Debug => Brushes.CadetBlue,
            LogLevel.Trace => Brushes.Gray,
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