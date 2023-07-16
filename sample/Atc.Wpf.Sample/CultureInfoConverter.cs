using System.Text.Json.Serialization;

namespace Atc.Wpf.Sample;

// TODO: Move to Atc
public class CultureInfoConverter : JsonConverter<CultureInfo>
{
    public override CultureInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var name = reader.GetString();
        return string.IsNullOrEmpty(name)
            ? GlobalizationConstants.EnglishCultureInfo
            : new CultureInfo(name);
    }

    public override void Write(Utf8JsonWriter writer, CultureInfo value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Name);
    }
}