namespace Atc.Wpf.Tests.Serialization.JsonConverters;

public class JsonComplexDataTests
{
    [Fact]
    public void ToJsonHex()
    {
        // Arrange
        var jsonSerializerOptions = JsonSerializerOptionsFactory.Create();
        jsonSerializerOptions.Converters.Add(new ColorToHexJsonConverter());
        jsonSerializerOptions.Converters.Add(new SolidColorBrushToHexJsonConverter());

        var data = new ComplexData
        {
            MyColor = Colors.Red,
            MyBrush = Brushes.AntiqueWhite,
        };

        var expected = "{\r\n" +
                       "  \"myColor\": \"#FFFF0000\",\r\n" +
                       "  \"myBrush\": \"#FFFAEBD7\"\r\n" +
                       "}".EnsureEnvironmentNewLines();

        // Atc
        var actual = JsonSerializer.Serialize(data, jsonSerializerOptions);

        // Assert
        Assert.NotNull(actual);
        if (OperatingSystem.IsWindows())
        {
            Assert.Equal(expected, actual);
        }
    }

    [Fact]
    public void FromJsonHex()
    {
        // Arrange
        var jsonSerializerOptions = JsonSerializerOptionsFactory.Create();
        jsonSerializerOptions.Converters.Add(new ColorToHexJsonConverter());
        jsonSerializerOptions.Converters.Add(new SolidColorBrushToHexJsonConverter());

        const string data = "{\r\n" +
                            "  \"myColor\": \"#FFFF0000\",\r\n" +
                            "  \"myBrush\": \"#FFFAEBD7\"\r\n" +
                            "}";

        var expected = new ComplexData
        {
            MyColor = Colors.Red,
            MyBrush = Brushes.AntiqueWhite,
        };

        // Atc
        var actual = JsonSerializer.Deserialize<ComplexData>(data, jsonSerializerOptions);

        // Assert
        Assert.NotNull(actual);
        if (OperatingSystem.IsWindows())
        {
            Assert.Equal(expected.ToString(), actual.ToString());
        }
    }

    [Fact]
    public void ToJsonName()
    {
        // Arrange
        var jsonSerializerOptions = JsonSerializerOptionsFactory.Create();
        jsonSerializerOptions.Converters.Add(new ColorToNameJsonConverter());
        jsonSerializerOptions.Converters.Add(new SolidColorBrushToNameJsonConverter());

        var data = new ComplexData
        {
            MyColor = Colors.Red,
            MyBrush = Brushes.AntiqueWhite,
        };

        var expected = "{\r\n" +
                       "  \"myColor\": \"Red\",\r\n" +
                       "  \"myBrush\": \"AntiqueWhite\"\r\n" +
                       "}".EnsureEnvironmentNewLines();

        // Atc
        var actual = JsonSerializer.Serialize(data, jsonSerializerOptions);

        // Assert
        Assert.NotNull(actual);
        if (OperatingSystem.IsWindows())
        {
            Assert.Equal(expected, actual);
        }
    }

    [Fact]
    public void FromJsonName()
    {
        // Arrange
        var jsonSerializerOptions = JsonSerializerOptionsFactory.Create();
        jsonSerializerOptions.Converters.Add(new ColorToNameJsonConverter());
        jsonSerializerOptions.Converters.Add(new SolidColorBrushToNameJsonConverter());

        const string data = "{\r\n" +
                            "  \"myColor\": \"Red\",\r\n" +
                            "  \"myBrush\": \"AntiqueWhite\"\r\n" +
                            "}";

        var expected = new ComplexData
        {
            MyColor = Colors.Red,
            MyBrush = Brushes.AntiqueWhite,
        };

        // Atc
        var actual = JsonSerializer.Deserialize<ComplexData>(data, jsonSerializerOptions);

        // Assert
        Assert.NotNull(actual);
        if (OperatingSystem.IsWindows())
        {
            Assert.Equal(expected.ToString(), actual.ToString());
        }
    }
}