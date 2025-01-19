namespace Atc.Wpf.Tests.Serialization.JsonConverters;

[Collection("Sequential")]
public class ColorToHexJsonConverterTests
{
    [StaTheory]
    [InlineData("#FFFF0000")]
    public void Read_ShouldReturnExpectedColor(string colorAsHex)
    {
        // Arrange
        var jsonSerializerOptions = JsonSerializerOptionsFactory.Create();
        var jsonConverter = new ColorToHexJsonConverter();
        var json = $"\"{colorAsHex}\"";
        var utf8JsonReader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));

        utf8JsonReader.Read();

        // Act
        var result = jsonConverter.Read(ref utf8JsonReader, typeof(Color), jsonSerializerOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(colorAsHex, result.ToString());
    }

    [StaTheory]
    [InlineData("Red")]
    public void Write_ShouldWriteColorNameToUtf8JsonWriter(string colorName)
    {
        // Arrange
        var jsonSerializerOptions = JsonSerializerOptionsFactory.Create();
        var jsonConverter = new ColorToHexJsonConverter();
        var memoryStream = new MemoryStream();
        using var utf8JsonWriter = new Utf8JsonWriter(memoryStream);
        var color = ColorHelper.GetColorFromName(colorName, CultureInfo.InvariantCulture) ?? Colors.Transparent;

        // Act
        jsonConverter.Write(utf8JsonWriter, color, jsonSerializerOptions);

        // Assert
        utf8JsonWriter.Flush();
        var result = Encoding.UTF8.GetString(memoryStream.ToArray());

        Assert.NotNull(result);
        Assert.Equal($"\"{color}\"", result);
    }
}