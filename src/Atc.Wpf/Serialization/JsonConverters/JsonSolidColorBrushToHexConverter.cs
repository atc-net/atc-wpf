namespace Atc.Wpf.Serialization.JsonConverters;

public class JsonSolidColorBrushToHexConverter : JsonConverter<SolidColorBrush?>
{
    public override SolidColorBrush? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var hexaColor = reader.GetString();
        return string.IsNullOrEmpty(hexaColor)
            ? null
            : SolidColorBrushHelper.GetBrushFromHex(hexaColor);
    }

    public override void Write(
        Utf8JsonWriter writer,
        SolidColorBrush? value,
        JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);

        ArgumentNullException.ThrowIfNull(writer);

        if (string.IsNullOrEmpty(value?.ToString(GlobalizationConstants.EnglishCultureInfo)))
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue(value.ToString(GlobalizationConstants.EnglishCultureInfo));
        }
    }
}