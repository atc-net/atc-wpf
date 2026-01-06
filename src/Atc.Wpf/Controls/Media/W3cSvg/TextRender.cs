// ReSharper disable InconsistentNaming
// ReSharper disable ConvertIfStatementToSwitchStatement
namespace Atc.Wpf.Controls.Media.W3cSvg;

internal static class TextRender
{
    private static int dpiX;
    private static int dpiY;

    public static readonly DependencyProperty TSpanElementProperty = DependencyProperty.RegisterAttached(
        "TSpanElement",
        typeof(TextShape.TSpan.Element),
        typeof(DependencyObject));

    public static GeometryGroup BuildTextGeometry(TextShape shape)
    {
        ArgumentNullException.ThrowIfNull(shape);

        if (shape.TextSpan is not null)
        {
            return BuildTextSpan(shape);
        }

        var geometryGroup = new GeometryGroup();
        double totalWidth = 0;
        var txt = shape.Text;
        if (shape.TextStyle is not null)
        {
            geometryGroup.Children.Add(BuildGlyphRun(
                shape.TextStyle,
                txt,
                shape.X,
                shape.Y,
                ref totalWidth));
        }

        return geometryGroup;
    }

    public static GeometryGroup BuildTextSpan(TextShape shape)
    {
        ArgumentNullException.ThrowIfNull(shape);

        var x = shape.X;
        var y = shape.Y;
        var geometryGroup = new GeometryGroup();
        if (shape.TextStyle is not null && shape.TextSpan is not null)
        {
            BuildTextSpan(
                geometryGroup,
                shape.TextStyle,
                shape.TextSpan,
                ref x,
                ref y);
        }

        return geometryGroup;
    }

    public static void SetElement(
        DependencyObject d,
        TextShape.TSpan.Element value)
    {
        ArgumentNullException.ThrowIfNull(d);

        d.SetValue(TSpanElementProperty, value);
    }

    public static TextShape.TSpan.Element GetElement(DependencyObject d)
    {
        ArgumentNullException.ThrowIfNull(d);

        return (TextShape.TSpan.Element)d.GetValue(TSpanElementProperty);
    }

    private static void BuildTextSpan(
        GeometryGroup gp,
        TextStyle textStyle,
        TextShape.TSpan.Element tSpan,
        ref double x,
        ref double y)
    {
        if (tSpan.Children is null)
        {
            return;
        }

        foreach (var child in tSpan.Children)
        {
            if (child.ElementType == TextShapeElementType.Text)
            {
                if (child.TextStyle is not null)
                {
                    var txt = child.Text;
                    double totalWidth = 0;
                    var baseline = y;

                    switch (child.TextStyle.BaseLineShift)
                    {
                        case "sub":
                            baseline += child.TextStyle.FontSize * 0.5;
                            break;
                        case "super":
                            baseline -= tSpan.TextStyle!.FontSize + (child.TextStyle.FontSize * 0.25);
                            break;
                    }

                    var gm = BuildGlyphRun(
                        child.TextStyle,
                        txt,
                        x,
                        baseline,
                        ref totalWidth);
                    SetElement(
                        gm,
                        child);
                    gp.Children.Add(gm);
                    x += totalWidth;
                }

                continue;
            }

            if (child.ElementType == TextShapeElementType.Tag)
            {
                BuildTextSpan(
                    gp,
                    textStyle,
                    child,
                    ref x,
                    ref y);
            }
        }
    }

    [SuppressMessage("Major Code Smell", "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields", Justification = "OK.")]
    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK - for now.")]
    private static Geometry BuildGlyphRun(
        TextStyle textStyle,
        string text,
        double x,
        double y,
        ref double totalWidth)
    {
        ArgumentNullException.ThrowIfNull(textStyle);

        if (string.IsNullOrEmpty(text))
        {
            return new GeometryGroup();
        }

        if (dpiX == 0 || dpiY == 0)
        {
            var sysPara = typeof(SystemParameters);
            var dpiXProperty = sysPara.GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiYProperty = sysPara.GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

            if (dpiXProperty is not null)
            {
                dpiX = (int)dpiXProperty.GetValue(null, index: null)!;
            }

            if (dpiYProperty is not null)
            {
                dpiY = (int)dpiYProperty.GetValue(null, index: null)!;
            }
        }

        var fontSize = textStyle.FontSize;
        GlyphRun? glyphs;
        var font = new Typeface(
            new FontFamily(textStyle.FontFamily ?? "Arial Unicode MS, Verdana"),
            textStyle.FontStyle,
            textStyle.FontWeight,
            FontStretch.FromOpenTypeStretch(9),
            new FontFamily("Arial Unicode MS"));

        double baseLine;
        if (font.TryGetGlyphTypeface(out var glyphFace))
        {
            var dpiScale = new DpiScale(
                dpiX,
                dpiY);
            glyphs = new GlyphRun((float)dpiScale.PixelsPerDip);
            ((ISupportInitialize)glyphs).BeginInit();
            glyphs.GlyphTypeface = glyphFace;
            glyphs.FontRenderingEmSize = fontSize;
            var textChars = new List<char>();
            var glyphIndices = new List<ushort>();
            var advanceWidths = new List<double>();
            totalWidth = 0;
            foreach (var ch in text)
            {
                int codepoint = ch;
                if (!glyphFace.CharacterToGlyphMap.TryGetValue(codepoint, out var glyphIndex))
                {
                    continue;
                }

                textChars.Add(ch);
                var glyphWidth = glyphFace.AdvanceWidths[glyphIndex];
                glyphIndices.Add(glyphIndex);
                advanceWidths.Add((glyphWidth * fontSize) + textStyle.LetterSpacing);
                if (char.IsWhiteSpace(ch))
                {
                    advanceWidths[^1] += textStyle.WordSpacing;
                }

                totalWidth += advanceWidths[^1];
            }

            glyphs.Characters = textChars.ToArray();
            glyphs.GlyphIndices = glyphIndices.ToArray();
            glyphs.AdvanceWidths = advanceWidths.ToArray();

            var alignmentOffset = textStyle.TextAlignment switch
            {
                TextAlignment.Center => totalWidth / 2,
                TextAlignment.Right => totalWidth,
                _ => 0,
            };

            baseLine = y;
            glyphs.BaselineOrigin = new Point(
                x - alignmentOffset,
                baseLine);
            ((ISupportInitialize)glyphs).EndInit();
        }
        else
        {
            return new GeometryGroup();
        }

        var geometryGroup = new GeometryGroup();
        geometryGroup.Children.Add(glyphs.BuildGeometry());
        if (textStyle.TextDecoration is null)
        {
            return geometryGroup;
        }

        double decorationPos = 0;
        double decorationThickness = 0;
        if (textStyle.TextDecoration[0].Location == TextDecorationLocation.Strikethrough)
        {
            decorationPos = baseLine - (font.StrikethroughPosition * fontSize);
            decorationThickness = font.StrikethroughThickness * fontSize;
        }

        if (textStyle.TextDecoration[0].Location == TextDecorationLocation.Underline)
        {
            decorationPos = baseLine - (font.UnderlinePosition * fontSize);
            decorationThickness = font.UnderlineThickness * fontSize;
        }

        if (textStyle.TextDecoration[0].Location == TextDecorationLocation.OverLine)
        {
            decorationPos = baseLine - fontSize;
            decorationThickness = font.StrikethroughThickness * fontSize;
        }

        var bounds = new Rect(
            geometryGroup.Bounds.Left,
            decorationPos,
            geometryGroup.Bounds.Width,
            decorationThickness);
        geometryGroup.Children.Add(new RectangleGeometry(bounds));
        return geometryGroup;
    }
}