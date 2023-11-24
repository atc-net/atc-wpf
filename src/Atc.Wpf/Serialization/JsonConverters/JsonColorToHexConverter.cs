namespace Atc.Wpf.Serialization.JsonConverters;

public class JsonColorToHexConverter : JsonConverter<Color>
{
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var hexaColor = reader.GetString();
        return string.IsNullOrEmpty(hexaColor)
            ? Colors.Pink
            : ColorUtil.GetColorFromHex(hexaColor);
    }

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);

        if (string.IsNullOrEmpty(value.ToString(GlobalizationConstants.EnglishCultureInfo)))
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue(value.ToString(GlobalizationConstants.EnglishCultureInfo));
        }
    }
}