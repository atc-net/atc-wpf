namespace Atc.Wpf.Serialization.JsonConverters;

public class JsonColorToNameConverter : JsonConverter<Color>
{
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var colorName = reader.GetString();
        if (string.IsNullOrEmpty(colorName))
        {
            return Colors.Pink;
        }

        var color = ColorUtil.GetColorFromName(colorName);
        return color ?? Colors.Pink;
    }

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        if (writer is null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        if (string.IsNullOrEmpty(value.ToString(GlobalizationConstants.EnglishCultureInfo)))
        {
            writer.WriteNullValue();
        }
        else
        {
            var colorName = ColorUtil.GetColorNameFromHex(value.ToString(GlobalizationConstants.EnglishCultureInfo));
            if (string.IsNullOrEmpty(colorName))
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(colorName);
            }
        }
    }
}