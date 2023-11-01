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
            JTokenType.String => GetStringTypeColor(jProperty.Value.ToString()!),
            JTokenType.Float => Constants.JTokenColorFloat,
            JTokenType.Integer => Constants.JTokenColorInteger,
            JTokenType.Boolean => Constants.JTokenColorBoolean,
            JTokenType.Date => Constants.JTokenColorDate,
            JTokenType.Guid => Constants.JTokenColorGuid,
            JTokenType.Uri => Constants.JTokenColorUri,
            JTokenType.Null => Constants.JTokenColorNull,
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
            return Constants.JTokenColorGuid;
        }

        if (Uri.IsWellFormedUriString(value, UriKind.Absolute))
        {
            return Constants.JTokenColorUri;
        }

        return Constants.JTokenColorString;
    }
}