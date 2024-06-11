// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
namespace Atc.Wpf.Controls.Documents.TextFormatters.SourceCode;

/// <summary>
/// Formats the RichTextBox text as colored Xaml.
/// </summary>
public class XamlFormatter : ITextFormatter
{
    /// <summary>
    /// The instance.
    /// </summary>
    public static readonly XamlFormatter Instance = new();

    /// <summary>
    /// Colorizes the xaml.
    /// </summary>
    /// <param name="xamlText">The xaml text.</param>
    /// <param name="targetDoc">The target document.</param>
    /// <param name="themeMode">The ThemeMode.</param>
    /// <returns>The flowDocument.</returns>
    public static FlowDocument ColorizeXaml(string xamlText, FlowDocument targetDoc, ThemeMode themeMode)
    {
        ArgumentNullException.ThrowIfNull(xamlText);
        ArgumentNullException.ThrowIfNull(targetDoc);

        var tokenizer = new XmlTokenizer();
        var mode = XmlTokenizerMode.OutsideElement;

        var tokens = tokenizer.Tokenize(xamlText, ref mode);
        var tokenTexts = new List<string>(tokens.Count);
        var colors = new List<Color>(tokens.Count);
        var position = 0;
        foreach (var token in tokens)
        {
            var tokenText = xamlText.Substring(position, token.Length);
            tokenTexts.Add(tokenText);
            var color = themeMode == ThemeMode.Light
                ? LightColorForToken(token)
                : DarkColorForToken(token);
            colors.Add(color);
            position += token.Length;
        }

        var p = new Paragraph();

        // Loop through tokens
        for (var i = 0; i < tokenTexts.Count; i++)
        {
            var run = new Run(tokenTexts[i])
            {
                Foreground = new SolidColorBrush(colors[i]),
            };
            p.Inlines.Add(run);
        }

        targetDoc.Blocks.Add(p);

        return targetDoc;
    }

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
    public void SetText(FlowDocument document, string text, ThemeMode themeMode)
    {
        ArgumentNullException.ThrowIfNull(document);

        document.Blocks.Clear();
        document.SetCurrentValue(FlowDocument.PageWidthProperty, 2500D);
        ColorizeXaml(text, document, themeMode);
    }

    private static Color LightColorForToken(XmlToken token)
    {
        var color = Color.FromRgb(231, 84, 128);
        switch (token.Kind)
        {
            case XmlTokenKind.Open:
            case XmlTokenKind.OpenClose:
            case XmlTokenKind.Close:
            case XmlTokenKind.SelfClose:
            case XmlTokenKind.CommentBegin:
            case XmlTokenKind.CommentEnd:
            case XmlTokenKind.CDataBegin:
            case XmlTokenKind.CDataEnd:
            case XmlTokenKind.Equals:
            case XmlTokenKind.OpenProcessingInstruction:
            case XmlTokenKind.CloseProcessingInstruction:
            case XmlTokenKind.AttributeValue:
                color = Color.FromRgb(0, 0, 255);
                break;
            case XmlTokenKind.ElementName:
                color = Color.FromRgb(163, 21, 21);
                break;
            case XmlTokenKind.TextContent:
                break;
            case XmlTokenKind.AttributeName:
            case XmlTokenKind.Entity:
                color = Color.FromRgb(255, 0, 0);
                break;
            case XmlTokenKind.CommentText:
                color = Color.FromRgb(0, 128, 0);
                break;
        }

        return color;
    }

    private static Color DarkColorForToken(XmlToken token)
    {
        var color = Color.FromRgb(231, 84, 128);
        switch (token.Kind)
        {
            case XmlTokenKind.Open:
            case XmlTokenKind.OpenClose:
            case XmlTokenKind.Close:
            case XmlTokenKind.SelfClose:
            case XmlTokenKind.CommentBegin:
            case XmlTokenKind.CommentEnd:
            case XmlTokenKind.CDataBegin:
            case XmlTokenKind.CDataEnd:
            case XmlTokenKind.Equals:
            case XmlTokenKind.OpenProcessingInstruction:
            case XmlTokenKind.CloseProcessingInstruction:
            case XmlTokenKind.AttributeValue:
                color = Color.FromRgb(86, 156, 214);
                break;
            case XmlTokenKind.ElementName:
                color = Color.FromRgb(255, 255, 255);
                break;
            case XmlTokenKind.TextContent:
                break;
            case XmlTokenKind.AttributeName:
            case XmlTokenKind.Entity:
                color = Color.FromRgb(146, 202, 244);
                break;
            case XmlTokenKind.CommentText:
                color = Color.FromRgb(87, 166, 74);
                break;
        }

        return color;
    }
}