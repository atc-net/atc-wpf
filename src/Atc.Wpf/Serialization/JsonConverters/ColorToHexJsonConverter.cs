namespace Atc.Wpf.Serialization.JsonConverters;

/// <summary>
/// JSON converter that serializes a <see cref="Color"/> as a hex string (for example <c>#FF0000FF</c>) and back.
/// </summary>
public sealed class ColorToHexJsonConverter : JsonConverter<Color?>
{
    /// <inheritdoc />
    public override Color? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var hexaColor = reader.GetString();
        return string.IsNullOrEmpty(hexaColor)
            ? null
            : ColorHelper.GetColorFromHex(hexaColor);
    }

    /// <inheritdoc />
    public override void Write(
        Utf8JsonWriter writer,
        Color? value,
        JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);

        if (string.IsNullOrEmpty(value?.ToString(GlobalizationConstants.EnglishCultureInfo)))
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue(value.Value.ToString(GlobalizationConstants.EnglishCultureInfo));
        }
    }
}