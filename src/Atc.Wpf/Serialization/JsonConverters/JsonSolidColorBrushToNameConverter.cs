namespace Atc.Wpf.Serialization.JsonConverters;

public class JsonSolidColorBrushToNameConverter : JsonConverter<SolidColorBrush?>
{
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
            var brushName = SolidColorBrushHelper.GetBaseBrushNameFromBrush(value);
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