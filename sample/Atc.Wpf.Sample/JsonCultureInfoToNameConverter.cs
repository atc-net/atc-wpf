using System.Text.Json.Serialization;

// ReSharper disable CheckNamespace
namespace Atc.Serialization.JsonConverters;

// TODO: Move to Atc
public class JsonCultureInfoToNameConverter : JsonConverter<CultureInfo?>
{
    public override CultureInfo? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var name = reader.GetString();
        return string.IsNullOrEmpty(name)
            ? null
            : new CultureInfo(name);
    }

    public override void Write(Utf8JsonWriter writer, CultureInfo? value, JsonSerializerOptions options)
    {
        if (writer is null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        if (string.IsNullOrEmpty(value?.Name))
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue(value.Name);
        }
    }
}