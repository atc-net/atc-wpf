namespace Atc.Wpf.Controls.Tests.Sample;

/// <summary>
/// Pushes a fenced C# block through MdXaml with the sample's markdown style and checks that the AvalonEdit
/// syntax colours are lifted once the editor sits on a dark background.
/// </summary>
public sealed class MarkdownCodeBlockThemingTests
{
    private const string Markdown = """
        # Title

        ```csharp
        var answer = 42; // comment
        ```
        """;

    [StaFact]
    public void FencedCodeBlock_GetsTheBehaviourFromTheMarkdownStyle()
    {
        var editor = RenderCodeBlock();

        Assert.True(MarkdownCodeBlockTheming.GetIsEnabled(editor));
    }

    [StaFact]
    public void FencedCodeBlock_OnADarkBackground_LiftsTheKeywordColour()
    {
        var editor = RenderCodeBlock();
        editor.Background = Brushes.White;
        editor.UpdateLayout();
        var onLight = ForegroundOf(editor, "var");

        editor.Background = Brushes.Black;
        editor.UpdateLayout();
        var onDark = ForegroundOf(editor, "var");

        Assert.True(Luminance(onLight) < 0.4, $"AvalonEdit's keyword colour should be dark on a light page, got {onLight}");
        Assert.True(Luminance(onDark) > Luminance(onLight), $"expected a lighter keyword on black, got {onDark}");
        Assert.True(Luminance(onDark) > 0.4, $"expected a readable keyword on black, got {onDark}");
    }

    [StaFact]
    public void FencedCodeBlock_BackToALightBackground_RestoresTheOriginalColour()
    {
        var editor = RenderCodeBlock();
        editor.Background = Brushes.White;
        editor.UpdateLayout();
        var original = ForegroundOf(editor, "var");

        editor.Background = Brushes.Black;
        editor.UpdateLayout();
        editor.Background = Brushes.White;
        editor.UpdateLayout();

        Assert.Equal(original, ForegroundOf(editor, "var"));
    }

    [StaTheory]
    [InlineData("Light.Blue")]
    [InlineData("Dark.Blue")]
    public void FencedCodeBlock_UnderTheTheme_UsesTheThemeColoursAndStaysReadable(
        string theme)
    {
        var themeResources = LoadTheme(theme);
        var editor = RenderCodeBlock(themeResources);

        // The style's DynamicResource lookups must resolve against the theme; a failed lookup would leave
        // AvalonEdit's own white background and make the contrast checks below meaningless.
        var background = BrushColor(themeResources, "AtcApps.Brushes.ThemeBackground2");
        var foreground = BrushColor(themeResources, "AtcApps.Brushes.ThemeForeground");
        Assert.Equal(background, Assert.IsType<SolidColorBrush>(editor.Background).Color);
        Assert.Equal(foreground, Assert.IsType<SolidColorBrush>(editor.Foreground).Color);

        Assert.True(
            ContrastRatio(foreground, background) >= 4.5,
            $"{theme}: code text {foreground} on {background} has contrast {ContrastRatio(foreground, background):F2}");

        var tokens = HighlightedTokens(editor);
        Assert.Contains(tokens, token => token.Text == "var" && token.Color != foreground);
        Assert.Contains(tokens, token => token.Text.StartsWith("//", StringComparison.Ordinal) && token.Color != foreground);
        foreach (var (text, color) in tokens)
        {
            Assert.True(
                ContrastRatio(color, background) >= 3,
                $"{theme}: '{text}' in {color} on {background} has contrast {ContrastRatio(color, background):F2}");
        }
    }

    [Theory]
    [InlineData(0, 0, 0, true)]
    [InlineData(30, 30, 30, true)]
    [InlineData(255, 255, 255, false)]
    [InlineData(240, 240, 240, false)]
    public void IsDarkBackground_JudgesSolidColours(
        byte r,
        byte g,
        byte b,
        bool expected)
        => Assert.Equal(expected, MarkdownCodeBlockTheming.IsDarkBackground(new SolidColorBrush(Color.FromRgb(r, g, b))));

    [Fact]
    public void IsDarkBackground_NonSolidOrMissingBrush_CountsAsLight()
    {
        Assert.False(MarkdownCodeBlockTheming.IsDarkBackground(brush: null));
        Assert.False(MarkdownCodeBlockTheming.IsDarkBackground(new LinearGradientBrush(Colors.Black, Colors.Black, 0)));
        Assert.False(MarkdownCodeBlockTheming.IsDarkBackground(Brushes.Transparent));
    }

    private static TextEditor RenderCodeBlock(
        ResourceDictionary? themeResources = null)
    {
        RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);
        var styles = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Atc.Wpf.Controls.Sample;component/Styles/MarkdownDocumentStyle.xaml", UriKind.Absolute),
        };

        var viewer = new MarkdownScrollViewer
        {
            MarkdownStyle = (Style)styles["MarkdownDocumentStyle"],
            Markdown = Markdown,
        };

        if (themeResources is not null)
        {
            viewer.Resources.MergedDictionaries.Add(themeResources);
        }

        viewer.Measure(new Size(800, 600));
        viewer.Arrange(new Rect(0, 0, 800, 600));
        viewer.UpdateLayout();

        var editor = viewer.Document.Blocks
            .OfType<BlockUIContainer>()
            .Select(block => block.Child)
            .OfType<TextEditor>()
            .FirstOrDefault();

        Assert.NotNull(editor);
        return editor;
    }

    private static Color ForegroundOf(
        TextEditor editor,
        string word)
    {
        var textView = editor.TextArea.TextView;
        textView.EnsureVisualLines();
        var line = textView.VisualLines[0];
        var lineStart = line.FirstDocumentLine.Offset;

        foreach (var element in line.Elements)
        {
            var text = editor.Document.GetText(lineStart + element.RelativeTextOffset, element.DocumentLength);
            if (string.Equals(text, word, StringComparison.Ordinal) &&
                element.TextRunProperties.ForegroundBrush is SolidColorBrush { Color: var color })
            {
                return color;
            }
        }

        throw new Xunit.Sdk.XunitException($"no highlighted element for '{word}' found");
    }

    private static ResourceDictionary LoadTheme(string theme)
    {
        // The theme dictionaries use the StaticResource markup type from Atc.Wpf.
        _ = typeof(Atc.Wpf.MarkupExtensions.StaticResourceExtension).Assembly;
        RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);
        return new ResourceDictionary
        {
            Source = new Uri($"pack://application:,,,/Atc.Wpf.Theming;component/Styles/Themes/{theme}.xaml", UriKind.Absolute),
        };
    }

    private static Color BrushColor(
        ResourceDictionary resources,
        string key)
        => Assert.IsType<SolidColorBrush>(resources[key]).Color;

    private static List<(string Text, Color Color)> HighlightedTokens(
        TextEditor editor)
    {
        var textView = editor.TextArea.TextView;
        textView.EnsureVisualLines();
        var line = textView.VisualLines[0];
        var lineStart = line.FirstDocumentLine.Offset;

        return line.Elements
            .Select(element => (
                Text: editor.Document.GetText(lineStart + element.RelativeTextOffset, element.DocumentLength),
                Brush: element.TextRunProperties.ForegroundBrush))
            .Where(token => !string.IsNullOrWhiteSpace(token.Text) && token.Brush is SolidColorBrush)
            .Select(token => (token.Text, ((SolidColorBrush)token.Brush).Color))
            .ToList();
    }

    // WCAG 2 contrast ratio between two opaque sRGB colours (1 = identical, 21 = black on white).
    private static double ContrastRatio(
        Color first,
        Color second)
    {
        var a = RelativeLuminance(first);
        var b = RelativeLuminance(second);
        return (System.Math.Max(a, b) + 0.05) / (System.Math.Min(a, b) + 0.05);
    }

    private static double RelativeLuminance(Color color)
        => (0.2126 * Linear(color.R)) + (0.7152 * Linear(color.G)) + (0.0722 * Linear(color.B));

    private static double Linear(byte channel)
    {
        var value = channel / 255d;
        return value <= 0.04045
            ? value / 12.92
            : System.Math.Pow((value + 0.055) / 1.055, 2.4);
    }

    private static double Luminance(Color color)
        => ((0.2126 * color.R) + (0.7152 * color.G) + (0.0722 * color.B)) / 255d;
}