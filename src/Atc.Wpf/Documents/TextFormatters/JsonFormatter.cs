// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault

namespace Atc.Wpf.Documents.TextFormatters;

/// <summary>
/// Formats the FlowDocument text as Json.
/// </summary>
public sealed class JsonFormatter : ITextFormatter
{
    /// <summary>
    /// The instance.
    /// </summary>
    public static readonly JsonFormatter Instance = new();

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <returns>The text.</returns>
    public string GetText(FlowDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        return new TextRange(document.ContentStart, document.ContentEnd).Text;
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
            // One parse; the walk below writes the indented layout itself.
            using var jsonDocument = JsonDocument.Parse(text);
            var rootElement = jsonDocument.RootElement;

            // A top-level JSON null is shown as plain text, not as a colored literal.
            if (rootElement.ValueKind != JsonValueKind.Null)
            {
                var paragraph = new Paragraph();
                var indentLevel = 0;

                ParseJsonElement(
                    rootElement,
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

    private static void TrimTrailingComma(InlineCollection inlines)
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

    private static void ParseJsonElement(
        JsonElement jsonElement,
        InlineCollection inlines,
        ref int indentLevel)
    {
        switch (jsonElement.ValueKind)
        {
            case JsonValueKind.Object:
            {
                ParseJsonObject(inlines, ref indentLevel, jsonElement);
                break;
            }

            case JsonValueKind.Array:
            {
                ParseJsonArray(inlines, ref indentLevel, jsonElement);
                break;
            }

            default:
            {
                ParseJsonValue(inlines, jsonElement);
                break;
            }
        }
    }

    private static void ParseJsonObject(
        InlineCollection inlines,
        ref int indentLevel,
        JsonElement jsonObject)
    {
        inlines.Add(new Run("{\n") { Foreground = JsonColorSchema.DefaultBrush });
        indentLevel++;

        // Duplicate property names are not rendered (the caller falls back to the plain text).
        var propertyNames = new HashSet<string>(StringComparer.Ordinal);

        foreach (var property in jsonObject.EnumerateObject())
        {
            if (!propertyNames.Add(property.Name))
            {
                throw new JsonException($"Duplicate property name '{property.Name}'.");
            }

            AddIndent(inlines, indentLevel);
            inlines.Add(new Run($"\"{property.Name}\": ") { Foreground = JsonColorSchema.KeyBrush });
            ParseJsonElement(property.Value, inlines, ref indentLevel);
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
        JsonElement jsonArray)
    {
        inlines.Add(new Run("[\n") { Foreground = JsonColorSchema.DefaultBrush });
        indentLevel++;

        foreach (var element in jsonArray.EnumerateArray())
        {
            AddIndent(inlines, indentLevel);
            ParseJsonElement(element, inlines, ref indentLevel);
            inlines.Add(new Run(",\n") { Foreground = JsonColorSchema.DefaultBrush });
        }

        TrimTrailingComma(inlines);
        indentLevel--;
        AddIndent(inlines, indentLevel);
        inlines.Add(new Run("]") { Foreground = JsonColorSchema.DefaultBrush });
    }

    private static void ParseJsonValue(
        InlineCollection inlines,
        JsonElement jsonValue)
    {
        switch (jsonValue.ValueKind)
        {
            case JsonValueKind.String:

                var strValue = jsonValue.GetString() ?? string.Empty;
                inlines.Add(new Run("\"") { Foreground = JsonColorSchema.StringBrush });

                if (DateTimeHelper.TryParseUsingSpecificCulture(strValue, GlobalizationConstants.EnglishCultureInfo, out _))
                {
                    inlines.Add(new Run(strValue) { Foreground = JsonColorSchema.DateBrush });
                }
                else if (Guid.TryParse(strValue, out _))
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
                inlines.Add(new Run(jsonValue.GetRawText()) { Foreground = JsonColorSchema.NumberBrush });
                break;
            case JsonValueKind.True:
            case JsonValueKind.False:
                inlines.Add(new Run(jsonValue.GetRawText()) { Foreground = JsonColorSchema.BooleanBrush });
                break;
            case JsonValueKind.Null:
                inlines.Add(new Run("null") { Foreground = JsonColorSchema.NullBrush });
                break;
            default:
                inlines.Add(new Run(jsonValue.GetRawText()) { Foreground = JsonColorSchema.DefaultBrush });
                break;
        }
    }
}