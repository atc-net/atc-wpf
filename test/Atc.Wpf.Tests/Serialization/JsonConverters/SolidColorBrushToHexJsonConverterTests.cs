namespace Atc.Wpf.Tests.Serialization.JsonConverters;

[Collection("Sequential")]
public sealed class SolidColorBrushToHexJsonConverterTests
{
    [StaTheory]
    [InlineData("#FFFF0000")]
    public void Read_ShouldReturnExpectedSolidColorBrush(string brushAsHex)
    {
        // Arrange
        var jsonSerializerOptions = JsonSerializerOptionsFactory.Create();
        var jsonConverter = new SolidColorBrushToHexJsonConverter();
        var json = $"\"{brushAsHex}\"";
        var utf8JsonReader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));

        utf8JsonReader.Read();

        // Act
        var result = jsonConverter.Read(ref utf8JsonReader, typeof(Color), jsonSerializerOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(brushAsHex, result.ToString(GlobalizationConstants.EnglishCultureInfo));
    }

    [StaTheory]
    [InlineData("Red")]
    public void Write_ShouldWriteSolidColorBrushNameToUtf8JsonWriter(
        string brushName)
    {
        // Arrange
        var jsonSerializerOptions = JsonSerializerOptionsFactory.Create();
        var jsonConverter = new SolidColorBrushToHexJsonConverter();
        var memoryStream = new MemoryStream();
        using var utf8JsonWriter = new Utf8JsonWriter(memoryStream);
        var solidColorBrush = SolidColorBrushHelper.GetBrushFromName(brushName, CultureInfo.InvariantCulture) ?? Brushes.Transparent;

        // Act
        jsonConverter.Write(utf8JsonWriter, solidColorBrush, jsonSerializerOptions);

        // Assert
        utf8JsonWriter.Flush();
        var result = Encoding.UTF8.GetString(memoryStream.ToArray());

        Assert.NotNull(result);
        Assert.Equal($"\"{solidColorBrush}\"", result);
    }
}