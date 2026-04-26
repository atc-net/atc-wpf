namespace Atc.Wpf.Tests.ValueConverters;

public sealed class JsonValueDisplayConverterTests
{
    private static JsonValueNode ParseValueNode(string json)
    {
        var node = JsonNode.Parse(json);
        return Assert.IsType<JsonValueNode>(node);
    }

    [Fact]
    public void Convert_returns_DisplayValue_for_a_string_JsonValueNode()
    {
        var valueNode = ParseValueNode("\"hello\"");

        var actual = JsonValueDisplayConverter.Instance.Convert(
            valueNode,
            typeof(string),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(valueNode.DisplayValue, actual);
    }

    [Fact]
    public void Convert_returns_DisplayValue_for_a_numeric_JsonValueNode()
    {
        var valueNode = ParseValueNode("42");

        var actual = JsonValueDisplayConverter.Instance.Convert(
            valueNode,
            typeof(string),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(valueNode.DisplayValue, actual);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("plain string passes through")]
    [InlineData(42)]
    public void Convert_passes_non_JsonValueNode_input_through_unchanged(
        object? input)
    {
        var actual = JsonValueDisplayConverter.Instance.Convert(
            input,
            typeof(string),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Equal(input, actual);
    }

    [Fact]
    public void ConvertBack_throws_NotSupportedException()
    {
        Assert.Throws<NotSupportedException>(() =>
            JsonValueDisplayConverter.Instance.ConvertBack(
                "anything",
                typeof(JsonValueNode),
                parameter: null,
                CultureInfo.InvariantCulture));
    }
}