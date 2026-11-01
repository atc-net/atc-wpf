namespace Atc.Wpf.Tests.Documents.TextFormatters;

[Collection(nameof(JsonColorSchemaTestGroup))]
public sealed class JsonFormatterTests
{
    private const string JsonWithNulls = """{"id":1,"name":null,"tags":[1,null,2]}""";

    [StaFact]
    public void ColorizeJson_KeepsNullPropertiesAndNullArrayElements()
    {
        var document = new FlowDocument();

        JsonFormatter.ColorizeJson(document, JsonWithNulls, ThemeMode.Light);

        Assert.Equal(JsonWithNulls, GetTextWithoutWhitespace(document));
    }

    [StaFact]
    public void ColorizeJson_RendersNullWithTheNullBrush()
    {
        var document = new FlowDocument();

        JsonFormatter.ColorizeJson(document, JsonWithNulls, ThemeMode.Light);

        var nullRuns = document.Blocks
            .OfType<Paragraph>()
            .SelectMany(p => p.Inlines.OfType<Run>())
            .Where(r => string.Equals(r.Text, "null", StringComparison.Ordinal))
            .ToList();
        Assert.Equal(2, nullRuns.Count);
        Assert.All(nullRuns, r => Assert.Same(JsonColorSchema.NullBrush, r.Foreground));
    }

    [StaFact]
    public void GetText_ReturnsPlainJson_NotRtf()
    {
        var document = new FlowDocument();
        var sut = JsonFormatter.Instance;
        sut.SetText(document, JsonWithNulls, ThemeMode.Light);

        var text = sut.GetText(document);

        Assert.DoesNotContain(@"{\rtf", text, StringComparison.Ordinal);
        Assert.Equal(JsonWithNulls, RemoveWhitespace(text));
    }

    [StaTheory]
    [InlineData("""{"path":"C:\temp","q":"say \"hi\""}""")]
    [InlineData("""{"k\"ey\\":["tab\there","line\nbreak","\u0001","\u00e6\u00f8\u00e5 \u20ac","<&>/"]}""")]
    [InlineData("""["2024-01-02T03:04:05","https://example.com/a?b=\"c\"","8a3b2c1d-1111-2222-3333-444455556666"]""")]
    [InlineData("""{"a\b":"\\","b":"\ud83d\ude00"}""")]
    public void GetText_after_SetText_parses_to_the_same_json_values(
        string json)
    {
        var document = new FlowDocument();
        var sut = JsonFormatter.Instance;
        sut.SetText(document, json, ThemeMode.Light);

        var text = sut.GetText(document);

        using var expected = JsonDocument.Parse(json);
        using var actual = JsonDocument.Parse(text);
        Assert.True(
            JsonElement.DeepEquals(expected.RootElement, actual.RootElement),
            $"Round-tripped text differs: {text}");
    }

    private static string GetTextWithoutWhitespace(FlowDocument document)
        => RemoveWhitespace(new TextRange(document.ContentStart, document.ContentEnd).Text);

    private static string RemoveWhitespace(string text)
        => string.Concat(text.Where(c => !char.IsWhiteSpace(c)));
}