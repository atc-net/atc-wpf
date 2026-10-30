namespace Atc.Wpf.Tests.Documents.TextFormatters;

/// <summary>
/// Pins the exact runs (text and brush) <see cref="JsonFormatter.ColorizeJson"/> produces.
/// Each run is written as <c>text|brush</c>, with line breaks shown as <c>\n</c> and
/// <c>-</c> for a run without its own foreground.
/// </summary>
public sealed class JsonFormatterOutputTests
{
    [StaFact]
    public void ColorizeJson_Object_RendersIndentedRunsWithBrushes()
    {
        var runs = Colorize("""{"a":1,"b":"x","c":true,"d":false,"e":null}""");

        Assert.Equal(
            [
                @"{\n|Default",
                "  |-",
                "\"a\": |Key",
                "1|Number",
                @",\n|Default",
                "  |-",
                "\"b\": |Key",
                "\"|String",
                "x|String",
                "\"|String",
                @",\n|Default",
                "  |-",
                "\"c\": |Key",
                "true|Boolean",
                @",\n|Default",
                "  |-",
                "\"d\": |Key",
                "false|Boolean",
                @",\n|Default",
                "  |-",
                "\"e\": |Key",
                "null|Null",
                @"\n|Default",
                "|-",
                "}|Default",
            ],
            runs);
    }

    [StaFact]
    public void ColorizeJson_NestedContainers_IndentsEachLevel()
    {
        var runs = Colorize("""{"a":[1,{"b":[]}],"c":{}}""");

        Assert.Equal(
            [
                @"{\n|Default",
                "  |-",
                "\"a\": |Key",
                @"[\n|Default",
                "    |-",
                "1|Number",
                @",\n|Default",
                "    |-",
                @"{\n|Default",
                "      |-",
                "\"b\": |Key",
                @"[\n|Default",
                "      |-",
                "]|Default",
                @"\n|Default",
                "    |-",
                "}|Default",
                @"\n|Default",
                "  |-",
                "]|Default",
                @",\n|Default",
                "  |-",
                "\"c\": |Key",
                @"{\n|Default",
                "  |-",
                "}|Default",
                @"\n|Default",
                "|-",
                "}|Default",
            ],
            runs);
    }

    [StaTheory]
    [InlineData("1.50", "1.50")]
    [InlineData("1e10", "1e10")]
    [InlineData("-0", "-0")]
    [InlineData("1E+2", "1E+2")]
    [InlineData("12345678901234567890123", "12345678901234567890123")]
    public void ColorizeJson_Number_KeepsTheRawText(
        string json,
        string expected)
    {
        var runs = Colorize($"[{json}]");

        Assert.Contains($"{expected}|Number", runs, StringComparer.Ordinal);
    }

    [StaFact]
    public void ColorizeJson_EscapedKeyAndValue_ShowsThemUnescaped()
    {
        var runs = Colorize("""{"k\u00e9\"y":"v\u00e9\"\\\n<&>"}""");

        Assert.Contains("\"ké\"y\": |Key", runs, StringComparer.Ordinal);
        Assert.Contains("vé\"\\" + @"\n" + "<&>|String", runs, StringComparer.Ordinal);
    }

    [StaFact]
    public void ColorizeJson_ClassifiedStrings_UseTheDateGuidAndUriBrushes()
    {
        var runs = Colorize("""["2024-01-02T03:04:05","8a3b2c1d-1111-2222-3333-444455556666","https://example.com/a?b=c","plain"]""");

        Assert.Contains("2024-01-02T03:04:05|Date", runs, StringComparer.Ordinal);
        Assert.Contains("8a3b2c1d-1111-2222-3333-444455556666|Guid", runs, StringComparer.Ordinal);
        Assert.Contains("https://example.com/a?b=c|Uri", runs, StringComparer.Ordinal);
        Assert.Contains("plain|String", runs, StringComparer.Ordinal);
    }

    [StaTheory]
    [InlineData("42", "42|Number")]
    [InlineData("true", "true|Boolean")]
    [InlineData("\"text\"", "text|String")]
    public void ColorizeJson_TopLevelScalar_RendersTheScalar(
        string json,
        string expectedRun)
    {
        var runs = Colorize(json);

        Assert.Contains(expectedRun, runs, StringComparer.Ordinal);
    }

    [StaTheory]
    [InlineData("null")]
    [InlineData("""{"a":1,"a":2}""")]
    [InlineData("""{"a":1,}""")]
    [InlineData("""{"a":1 /* comment */}""")]
    [InlineData("""{"a":""")]
    [InlineData("not json")]
    [InlineData("   ")]
    [InlineData("")]
    [InlineData("""["\ud800"]""")]
    public void ColorizeJson_InputThatIsNotRendered_FallsBackToThePlainText(
        string json)
    {
        var document = new FlowDocument();

        JsonFormatter.ColorizeJson(document, json, ThemeMode.Light);

        Assert.DoesNotContain(GetRuns(document), r => r.ReadLocalValue(TextElement.ForegroundProperty) != DependencyProperty.UnsetValue);
        Assert.Equal(json, new TextRange(document.ContentStart, document.ContentEnd).Text.TrimEnd('\r', '\n'));
    }

    [StaFact]
    public void ColorizeJson_NestingAtTheDepthLimit_IsRendered()
    {
        var json = new string('[', 64) + new string(']', 64);

        var runs = Colorize(json);

        Assert.Equal(64, runs.Count(r => string.Equals(r, @"[\n|Default", StringComparison.Ordinal)));
    }

    [StaFact]
    public void ColorizeJson_NestingBeyondTheDepthLimit_FallsBackToThePlainText()
    {
        var json = new string('[', 65) + new string(']', 65);
        var document = new FlowDocument();

        JsonFormatter.ColorizeJson(document, json, ThemeMode.Light);

        Assert.Equal(json, new TextRange(document.ContentStart, document.ContentEnd).Text.TrimEnd('\r', '\n'));
    }

    [StaFact]
    public void ColorizeJson_DarkTheme_UsesTheDarkBrushes()
    {
        var document = new FlowDocument();

        JsonFormatter.ColorizeJson(document, """{"a":1}""", ThemeMode.Dark);

        var runs = GetRuns(document).ToList();
        Assert.Same(JsonColorSchema.KeyBrush, runs[2].Foreground);
        Assert.Same(JsonColorSchema.NumberBrush, runs[3].Foreground);
        Assert.Equal(Colors.White, ((SolidColorBrush)runs[0].Foreground).Color);
    }

    [StaFact]
    public void ColorizeJson_RendersOneParagraph()
    {
        var document = new FlowDocument();

        JsonFormatter.ColorizeJson(document, """{"a":[1,2]}""", ThemeMode.Light);

        Assert.IsType<Paragraph>(Assert.Single(document.Blocks));
    }

    private static List<string> Colorize(string json)
    {
        var document = new FlowDocument();

        JsonFormatter.ColorizeJson(document, json, ThemeMode.Light);

        return GetRuns(document)
            .Select(r => $"{r.Text.Replace("\n", @"\n", StringComparison.Ordinal)}|{BrushName(r)}")
            .ToList();
    }

    private static IEnumerable<Run> GetRuns(FlowDocument document)
        => document.Blocks
            .OfType<Paragraph>()
            .SelectMany(p => p.Inlines.OfType<Run>());

    private static string BrushName(Run run)
    {
        if (run.ReadLocalValue(TextElement.ForegroundProperty) == DependencyProperty.UnsetValue)
        {
            return "-";
        }

        var brush = run.Foreground;
        return brush switch
        {
            _ when ReferenceEquals(brush, JsonColorSchema.DefaultBrush) => "Default",
            _ when ReferenceEquals(brush, JsonColorSchema.KeyBrush) => "Key",
            _ when ReferenceEquals(brush, JsonColorSchema.StringBrush) => "String",
            _ when ReferenceEquals(brush, JsonColorSchema.NumberBrush) => "Number",
            _ when ReferenceEquals(brush, JsonColorSchema.BooleanBrush) => "Boolean",
            _ when ReferenceEquals(brush, JsonColorSchema.NullBrush) => "Null",
            _ when ReferenceEquals(brush, JsonColorSchema.DateBrush) => "Date",
            _ when ReferenceEquals(brush, JsonColorSchema.GuidBrush) => "Guid",
            _ when ReferenceEquals(brush, JsonColorSchema.UriBrush) => "Uri",
            _ => "Other",
        };
    }
}