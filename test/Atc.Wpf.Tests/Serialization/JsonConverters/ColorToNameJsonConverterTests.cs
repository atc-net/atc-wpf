namespace Atc.Wpf.Tests.Serialization.JsonConverters;

[Collection("Sequential")]
public class ColorToNameJsonConverterTests
{
    [StaTheory]
    [InlineData("Red")]
    public void Read_ShouldReturnExpectedColor(string colorName)
    {
        // Arrange
        var jsonSerializerOptions = JsonSerializerOptionsFactory.Create();
        var jsonConverter = new ColorToNameJsonConverter();
        var json = $"\"{colorName}\"";
        var utf8JsonReader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));

        utf8JsonReader.Read();

        // Act
        var result = jsonConverter.Read(ref utf8JsonReader, typeof(Color), jsonSerializerOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ColorHelper.GetColorFromName(colorName, CultureInfo.InvariantCulture), result);
    }

    [StaTheory]
    [InlineData("Red")]
    public void Write_ShouldWriteColorNameToUtf8JsonWriter(string colorName)
    {
        // Arrange
        var jsonSerializerOptions = JsonSerializerOptionsFactory.Create();
        var jsonConverter = new ColorToNameJsonConverter();
        var memoryStream = new MemoryStream();
        using var utf8JsonWriter = new Utf8JsonWriter(memoryStream);
        var color = ColorHelper.GetColorFromName(colorName, CultureInfo.InvariantCulture) ?? Colors.Transparent;

        // Act
        jsonConverter.Write(utf8JsonWriter, color, jsonSerializerOptions);

        // Assert
        utf8JsonWriter.Flush();
        var result = Encoding.UTF8.GetString(memoryStream.ToArray());

        Assert.NotNull(result);
        Assert.Equal($"\"{ColorHelper.GetColorNameFromColor(color)}\"", result);
    }
}