namespace Atc.Wpf.Serialization.JsonConverters;

/// <summary>
/// JSON converter that serializes a <see cref="Color"/> as a known color name and back.
/// </summary>
public sealed class ColorToNameJsonConverter : JsonConverter<Color?>
{
    /// <inheritdoc />
    public override Color? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var colorName = reader.GetString();
        return string.IsNullOrEmpty(colorName)
            ? null
            : ColorHelper.GetColorFromString(colorName, GlobalizationConstants.EnglishCultureInfo);
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
            var colorName = ColorHelper.GetColorKeyFromHex(value.Value.ToString(GlobalizationConstants.EnglishCultureInfo));
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