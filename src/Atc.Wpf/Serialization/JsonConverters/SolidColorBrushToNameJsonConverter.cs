namespace Atc.Wpf.Serialization.JsonConverters;

/// <summary>
/// JSON converter that serializes a <see cref="SolidColorBrush"/> as a known brush name and back.
/// </summary>
public sealed class SolidColorBrushToNameJsonConverter : JsonConverter<SolidColorBrush?>
{
    /// <inheritdoc />
    public override SolidColorBrush? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var colorName = reader.GetString();
        return string.IsNullOrEmpty(colorName)
            ? null
            : SolidColorBrushHelper.GetBrushFromName(colorName, CultureInfo.InvariantCulture);
    }

    /// <inheritdoc />
    public override void Write(
        Utf8JsonWriter writer,
        SolidColorBrush? value,
        JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);

        if (string.IsNullOrEmpty(value?.ToString(GlobalizationConstants.EnglishCultureInfo)))
        {
            writer.WriteNullValue();
        }
        else
        {
            var brushName = SolidColorBrushHelper.GetBrushKeyFromBrush(value);
            if (string.IsNullOrEmpty(brushName))
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(brushName);
            }
        }
    }
}