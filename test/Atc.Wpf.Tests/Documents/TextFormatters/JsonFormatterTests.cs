namespace Atc.Wpf.Tests.Documents.TextFormatters;

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

    private static string GetTextWithoutWhitespace(FlowDocument document)
        => RemoveWhitespace(new TextRange(document.ContentStart, document.ContentEnd).Text);

    private static string RemoveWhitespace(string text)
        => string.Concat(text.Where(c => !char.IsWhiteSpace(c)));
}