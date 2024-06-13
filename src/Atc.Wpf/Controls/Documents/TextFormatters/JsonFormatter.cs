// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
namespace Atc.Wpf.Controls.Documents.TextFormatters;

/// <summary>
/// Formats the FlowDocument text as Json.
/// </summary>
public class JsonFormatter : ITextFormatter
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    /// <summary>
    /// The instance.
    /// </summary>
    public static readonly JsonFormatter Instance = new();

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <returns>The text.</returns>
    public string GetText(
        FlowDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        var tr = new TextRange(document.ContentStart, document.ContentEnd);
        using var memoryStream = new MemoryStream();
        tr.Save(memoryStream, DataFormats.Rtf);
        return Encoding.Default.GetString(memoryStream.ToArray());
    }

    /// <summary>
    /// Sets the text.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="text">The text.</param>
    /// <param name="themeMode">The ThemeMode.</param>
    public void SetText(
        FlowDocument document,
        string text,
        ThemeMode themeMode)
    {
        ArgumentNullException.ThrowIfNull(document);

        document.Blocks.Clear();
        document.SetCurrentValue(FlowDocument.PageWidthProperty, 2500D);
        ColorizeJson(document, text, themeMode);
    }

    /// <summary>
    /// Colorizes the xaml.
    /// </summary>
    /// <param name="document">The target document.</param>
    /// <param name="text">The text.</param>
    /// <param name="themeMode">The ThemeMode.</param>
    /// <returns>The flowDocument.</returns>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK")]
    public static FlowDocument ColorizeJson(
        FlowDocument document,
        string text,
        ThemeMode themeMode)
    {
        ArgumentNullException.ThrowIfNull(document);

        JsonColorSchema.SetThemeMode(themeMode);

        document.Blocks.Clear();

        try
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(text);
            var formattedJson = JsonSerializer.Serialize(jsonElement, JsonSerializerOptions);

            var paragraph = new Paragraph();
            var indentLevel = 0;
            var jsonNode = JsonNode.Parse(formattedJson);

            if (jsonNode is not null)
            {
                ParseJsonNode(
                    jsonNode,
                    paragraph.Inlines,
                    ref indentLevel);

                document.Blocks.Add(paragraph);

                return document;
            }
        }
        catch
        {
            // Ignore
        }

        new TextRange(document.ContentStart, document.ContentEnd).Text = text;
        return document;
    }

    private static void TrimTrailingComma(
        InlineCollection inlines)
    {
        if (inlines.LastInline is Run run &&
            run.Text.EndsWith(",\n", StringComparison.Ordinal))
        {
            run.Text = run.Text[..^2] + "\n";
        }
    }

    private static void AddIndent(
        InlineCollection inlines,
        int indentLevel)
    {
        inlines.Add(new Run(new string(' ', indentLevel * 2)));
    }

    private static void ParseJsonNode(
         JsonNode jsonNode,
         InlineCollection inlines,
         ref int indentLevel)
    {
        switch (jsonNode)
        {
            case JsonObject jsonObject:
            {
                ParseJsonObject(inlines, ref indentLevel, jsonObject);
                break;
            }

            case JsonArray jsonArray:
            {
                ParseJsonArray(inlines, ref indentLevel, jsonArray);
                break;
            }

            case JsonValue jsonValue:
            {
                ParseJsonValue(inlines, jsonValue);
                break;
            }
        }
    }

    private static void ParseJsonObject(
        InlineCollection inlines,
        ref int indentLevel,
        JsonObject jsonObject)
    {
        inlines.Add(new Run("{\n") { Foreground = JsonColorSchema.DefaultBrush });
        indentLevel++;

        foreach (var kvp in jsonObject)
        {
            if (kvp.Value is null)
            {
                continue;
            }

            AddIndent(inlines, indentLevel);
            inlines.Add(new Run($"\"{kvp.Key}\": ") { Foreground = JsonColorSchema.KeyBrush });
            ParseJsonNode(kvp.Value, inlines, ref indentLevel);
            inlines.Add(new Run(",\n") { Foreground = JsonColorSchema.DefaultBrush });
        }

        TrimTrailingComma(inlines);
        indentLevel--;
        AddIndent(inlines, indentLevel);
        inlines.Add(new Run("}") { Foreground = JsonColorSchema.DefaultBrush });
    }

    private static void ParseJsonArray(
        InlineCollection inlines,
        ref int indentLevel,
        JsonArray jsonArray)
    {
        inlines.Add(new Run("[\n") { Foreground = JsonColorSchema.DefaultBrush });
        indentLevel++;

        foreach (var element in jsonArray)
        {
            if (element is null)
            {
                continue;
            }

            AddIndent(inlines, indentLevel);
            ParseJsonNode(element, inlines, ref indentLevel);
            inlines.Add(new Run(",\n") { Foreground = JsonColorSchema.DefaultBrush });
        }

        TrimTrailingComma(inlines);
        indentLevel--;
        AddIndent(inlines, indentLevel);
        inlines.Add(new Run("]") { Foreground = JsonColorSchema.DefaultBrush });
    }

    private static void ParseJsonValue(
        InlineCollection inlines,
        JsonValue jsonValue)
    {
        var jsonValueKind = jsonValue.GetValue<JsonElement>().ValueKind;

        switch (jsonValueKind)
        {
            case JsonValueKind.String:

                var strValue = jsonValue.ToString();
                inlines.Add(new Run("\"") { Foreground = JsonColorSchema.StringBrush });

                if (DateTimeHelper.TryParseUsingSpecificCulture(strValue, GlobalizationConstants.EnglishCultureInfo, out _))
                {
                    inlines.Add(new Run(strValue) { Foreground = JsonColorSchema.DateBrush });
                }
                else if (Guid.TryParse(jsonValue.ToString(), out _))
                {
                    inlines.Add(new Run(strValue) { Foreground = JsonColorSchema.GuidBrush });
                }
                else if (Uri.IsWellFormedUriString(strValue, UriKind.Absolute))
                {
                    inlines.Add(new Run(strValue) { Foreground = JsonColorSchema.UriBrush });
                }
                else
                {
                    inlines.Add(new Run(strValue) { Foreground = JsonColorSchema.StringBrush });
                }

                inlines.Add(new Run("\"") { Foreground = JsonColorSchema.StringBrush });

                break;
            case JsonValueKind.Number:
                inlines.Add(new Run(jsonValue.ToString()) { Foreground = JsonColorSchema.NumberBrush });
                break;
            case JsonValueKind.True:
            case JsonValueKind.False:
                inlines.Add(new Run(jsonValue.ToString()) { Foreground = JsonColorSchema.BooleanBrush });
                break;
            case JsonValueKind.Null:
                inlines.Add(new Run("null") { Foreground = JsonColorSchema.NullBrush });
                break;
            default:
                inlines.Add(new Run(jsonValue.ToString()) { Foreground = JsonColorSchema.DefaultBrush });
                break;
        }
    }
}