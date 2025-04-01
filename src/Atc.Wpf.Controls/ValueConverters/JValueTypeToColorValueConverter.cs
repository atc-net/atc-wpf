namespace Atc.Wpf.Controls.ValueConverters;

public sealed class JValueTypeToColorValueConverter : IValueConverter
{
    public static readonly JValueTypeToColorValueConverter Instance = new();

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
            JTokenType.String => JsonColorSchema.StringBrush,
            JTokenType.Float => JsonColorSchema.FloatBrush,
            JTokenType.Integer => JsonColorSchema.IntegerBrush,
            JTokenType.Boolean => JsonColorSchema.BooleanBrush,
            JTokenType.Null => JsonColorSchema.NullBrush,
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