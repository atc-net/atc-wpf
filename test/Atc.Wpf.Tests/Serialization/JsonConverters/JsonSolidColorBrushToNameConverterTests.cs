namespace Atc.Wpf.Tests.Serialization.JsonConverters;

[Collection("Sequential")]
public class JsonSolidColorBrushToNameConverterTests
{
    [StaTheory]
    [InlineData("Red")]
    public void Read_ShouldReturnExpectedSolidColorBrush(string brushName)
    {
        // Arrange
        var jsonSerializerOptions = JsonSerializerOptionsFactory.Create();
        var jsonConverter = new JsonSolidColorBrushToNameConverter();
        var json = $"\"{brushName}\"";
        var utf8JsonReader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));

        utf8JsonReader.Read();

        // Act
        var result = jsonConverter.Read(ref utf8JsonReader, typeof(SolidColorBrush), jsonSerializerOptions);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(
            SolidColorBrushHelper.GetBrushFromName(brushName, CultureInfo.InvariantCulture)!.ToString(GlobalizationConstants.EnglishCultureInfo),
            result.ToString(GlobalizationConstants.EnglishCultureInfo));
    }

    [StaTheory]
    [InlineData("Red")]
    public void Write_ShouldWriteSolidColorBrushNameToUtf8JsonWriter(string colorName)
    {
        // Arrange
        var jsonSerializerOptions = JsonSerializerOptionsFactory.Create();
        var jsonConverter = new JsonSolidColorBrushToNameConverter();
        var memoryStream = new MemoryStream();
        using var utf8JsonWriter = new Utf8JsonWriter(memoryStream);
        var solidColorBrush = SolidColorBrushHelper.GetBrushFromName(colorName, CultureInfo.InvariantCulture) ?? Brushes.Transparent;

        // Act
        jsonConverter.Write(utf8JsonWriter, solidColorBrush, jsonSerializerOptions);

        // Assert
        utf8JsonWriter.Flush();
        var result = Encoding.UTF8.GetString(memoryStream.ToArray());

        Assert.NotNull(result);
        Assert.Equal(
            $"\"{SolidColorBrushHelper.GetBrushNameFromBrush(solidColorBrush)}\"",
            result);
    }
}