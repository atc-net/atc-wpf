namespace Atc.Wpf.Tests.ValueConverters;

public sealed class JsonArrayLengthConverterTests
{
    [Fact]
    public void Convert_returns_bracketed_length_for_a_JsonArrayNode()
    {
        var arrayNode = (JsonArrayNode)JsonNode.Parse("[1, 2, 3, 4]");

        var actual = JsonArrayLengthConverter.Instance.Convert(
            arrayNode,
            typeof(string),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal("[4]", actual);
    }

    [Fact]
    public void Convert_returns_padded_bracketed_length_for_a_property_holding_an_array()
    {
        var root = (JsonObjectNode)JsonNode.Parse(@"{ ""items"": [10, 20, 30] }");
        var propertyNode = (JsonPropertyNode)root.Children().First();

        var actual = JsonArrayLengthConverter.Instance.Convert(
            propertyNode,
            typeof(string),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal("[ 3 ]", actual);
    }

    [Fact]
    public void Convert_returns_empty_string_for_a_property_whose_value_is_not_an_array()
    {
        var root = (JsonObjectNode)JsonNode.Parse(@"{ ""name"": ""Alice"" }");
        var propertyNode = (JsonPropertyNode)root.Children().First();

        var actual = JsonArrayLengthConverter.Instance.Convert(
            propertyNode,
            typeof(string),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(string.Empty, actual);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("not a json node")]
    [InlineData(42)]
    public void Convert_returns_empty_string_for_unsupported_input(
        object? input)
    {
        var actual = JsonArrayLengthConverter.Instance.Convert(
            input,
            typeof(string),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(string.Empty, actual);
    }

    [Fact]
    public void ConvertBack_throws_NotSupportedException()
    {
        Assert.Throws<NotSupportedException>(() =>
            JsonArrayLengthConverter.Instance.ConvertBack(
                "[3]",
                typeof(JsonArrayNode),
                parameter: null,
                CultureInfo.InvariantCulture));
    }
}