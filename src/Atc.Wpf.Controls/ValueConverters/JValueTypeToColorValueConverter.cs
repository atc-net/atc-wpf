namespace Atc.Wpf.Controls.ValueConverters;

public sealed class JValueTypeToColorValueConverter : IValueConverter
{
    public object? Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        if (value is not JValue jValue)
        {
            return value;
        }

        return jValue.Type switch
        {
            JTokenType.String => new BrushConverter().ConvertFrom("#4e9a06"),
            JTokenType.Float => new BrushConverter().ConvertFrom("#ad7fa8"),
            JTokenType.Integer => new BrushConverter().ConvertFrom("#ad7fa8"),
            JTokenType.Boolean => new BrushConverter().ConvertFrom("#c4a000"),
            JTokenType.Null => new SolidColorBrush(Colors.OrangeRed),
            _ => value,
        };
    }

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
        => throw new NotSupportedException(GetType().Name + " can only be used for one way conversion.");
}