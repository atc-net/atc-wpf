namespace Atc.Wpf.Controls.ValueConverters;

public sealed class JValueConverter : IValueConverter
{
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not JValue jValue)
        {
            return value;
        }

        return jValue.Type switch
        {
            JTokenType.String => "\"" + jValue.Value + "\"",
            JTokenType.Null => "null",
            _ => value,
        };
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException(GetType().Name + " can only be used for one way conversion.");
}