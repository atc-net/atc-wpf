// ReSharper disable ConvertIfStatementToReturnStatement
namespace Atc.Wpf.Controls.ValueConverters;

public sealed class JPropertyTypeToColorValueConverter : IValueConverter
{
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not JProperty jProperty)
        {
            return value;
        }

        return jProperty.Value.Type switch
        {
            JTokenType.String => GetStringTypeColor(jProperty.Value.ToString()),
            JTokenType.Float => JsonColorSchema.FloatBrush,
            JTokenType.Integer => JsonColorSchema.IntegerBrush,
            JTokenType.Boolean => JsonColorSchema.BooleanBrush,
            JTokenType.Date => JsonColorSchema.DateBrush,
            JTokenType.Guid => JsonColorSchema.GuidBrush,
            JTokenType.Uri => JsonColorSchema.UriBrush,
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

    private static object GetStringTypeColor(string value)
    {
        if (Guid.TryParse(value, out _))
        {
            return JsonColorSchema.GuidBrush;
        }

        if (Uri.IsWellFormedUriString(value, UriKind.Absolute))
        {
            return JsonColorSchema.UriBrush;
        }

        return JsonColorSchema.StringBrush;
    }
}