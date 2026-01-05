namespace Atc.Wpf.Controls.ValueConverters;

/// <summary>
/// Converts a JsonValueNode's type to a color brush for syntax highlighting.
/// </summary>
public sealed class JsonValueTypeToColorConverter : IValueConverter
{
    public static readonly JsonValueTypeToColorConverter Instance = new();

    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not JsonValueNode valueNode)
        {
            return value;
        }

        return valueNode.Type switch
        {
            JsonNodeType.String => JsonColorSchema.StringBrush,
            JsonNodeType.Float => JsonColorSchema.FloatBrush,
            JsonNodeType.Integer => JsonColorSchema.IntegerBrush,
            JsonNodeType.Boolean => JsonColorSchema.BooleanBrush,
            JsonNodeType.Date => JsonColorSchema.DateBrush,
            JsonNodeType.Guid => JsonColorSchema.GuidBrush,
            JsonNodeType.Uri => JsonColorSchema.UriBrush,
            JsonNodeType.Null => JsonColorSchema.NullBrush,
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
