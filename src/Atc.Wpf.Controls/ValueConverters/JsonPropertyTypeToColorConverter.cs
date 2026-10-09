namespace Atc.Wpf.Controls.ValueConverters;

/// <summary>
/// Converts a JsonPropertyNode's value type to a color brush for syntax highlighting.
/// </summary>
public sealed class JsonPropertyTypeToColorConverter : IValueConverter
{
    /// <summary>The shared instance of the converter.</summary>
    public static readonly JsonPropertyTypeToColorConverter Instance = new();

    /// <inheritdoc />
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        if (value is not JsonPropertyNode propertyNode)
        {
            return value;
        }

        return propertyNode.ValueType switch
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

    /// <inheritdoc />
    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture)
        => throw new NotSupportedException(GetType().Name + " can only be used for one way conversion.");
}