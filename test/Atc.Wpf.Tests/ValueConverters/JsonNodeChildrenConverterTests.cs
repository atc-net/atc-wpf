namespace Atc.Wpf.Tests.ValueConverters;

public sealed class JsonNodeChildrenConverterTests
{
    [Fact]
    public void Convert_returns_object_property_children()
    {
        var root = JsonNode.Parse(@"{ ""a"": 1, ""b"": 2, ""c"": 3 }");

        var actual = JsonNodeChildrenConverter.Instance.Convert(
            root,
            typeof(IEnumerable<JsonNode>),
            parameter: null,
            CultureInfo.InvariantCulture);

        var children = Assert.IsAssignableFrom<IEnumerable<JsonNode>>(actual!);
        Assert.Equal(3, children.Count());
    }

    [Fact]
    public void Convert_returns_array_item_children()
    {
        var root = JsonNode.Parse("[10, 20, 30, 40]");

        var actual = JsonNodeChildrenConverter.Instance.Convert(
            root,
            typeof(IEnumerable<JsonNode>),
            parameter: null,
            CultureInfo.InvariantCulture);

        var children = Assert.IsAssignableFrom<IEnumerable<JsonNode>>(actual!);
        Assert.Equal(4, children.Count());
    }

    [Fact]
    public void Convert_returns_empty_for_a_value_node()
    {
        var root = JsonNode.Parse("\"plain-string\"");

        var actual = JsonNodeChildrenConverter.Instance.Convert(
            root,
            typeof(IEnumerable<JsonNode>),
            parameter: null,
            CultureInfo.InvariantCulture);

        var children = Assert.IsAssignableFrom<IEnumerable<JsonNode>>(actual!);
        Assert.Empty(children);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("not a node")]
    [InlineData(42)]
    public void Convert_returns_null_for_unsupported_input(object? input)
    {
        var actual = JsonNodeChildrenConverter.Instance.Convert(
            input,
            typeof(IEnumerable<JsonNode>),
            parameter: null,
            CultureInfo.InvariantCulture);

        Assert.Null(actual);
    }

    [Fact]
    public void ConvertBack_throws_NotSupportedException()
    {
        Assert.Throws<NotSupportedException>(() =>
            JsonNodeChildrenConverter.Instance.ConvertBack(
                Array.Empty<JsonNode>(),
                typeof(JsonNode),
                parameter: null,
                CultureInfo.InvariantCulture));
    }
}