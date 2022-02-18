namespace Atc.Wpf.Controls.W3cSvg;

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
        if (shape is null)
        {
            throw new ArgumentNullException(nameof(shape));
        }

        if (shape.TextSpan is not null)
        {
            return BuildTextSpan(shape);
        }

        var geometryGroup = new GeometryGroup();
        double totalWidth = 0;
        string txt = shape.Text;
        if (shape.TextStyle is not null)
        {
            geometryGroup.Children.Add(BuildGlyphRun(shape.TextStyle, txt, shape.X, shape.Y, ref totalWidth));
        }

        return geometryGroup;
    }

    public static GeometryGroup BuildTextSpan(TextShape shape)
    {
        if (shape is null)
        {
            throw new ArgumentNullException(nameof(shape));
        }

        double x = shape.X;
        double y = shape.Y;
        var geometryGroup = new GeometryGroup();
        if (shape.TextStyle is not null && shape.TextSpan is not null)
        {
            BuildTextSpan(geometryGroup, shape.TextStyle, shape.TextSpan, ref x, ref y);
        }

        return geometryGroup;
    }

    public static void SetElement(DependencyObject obj, TextShape.TSpan.Element value)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        obj.SetValue(TSpanElementProperty, value);
    }

    public static TextShape.TSpan.Element GetElement(DependencyObject obj)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        return (TextShape.TSpan.Element)obj.GetValue(TSpanElementProperty);
    }

    private static void BuildTextSpan(GeometryGroup gp, TextStyle textStyle, TextShape.TSpan.Element tSpan, ref double x, ref double y)
    {
        if (tSpan.Children is null)
        {
            return;
        }

        foreach (TextShape.TSpan.Element child in tSpan.Children)
        {
            if (child.ElementType == TextShapeElementType.Text)
            {
                if (child.TextStyle is not null)
                {
                    string txt = child.Text;
                    double totalWidth = 0;
                    double baseline = y;

                    switch (child.TextStyle.BaseLineShift)
                    {
                        case "sub":
                            baseline += child.TextStyle.FontSize * 0.5;
                            break;
                        case "super":
                            baseline -= tSpan.TextStyle!.FontSize + (child.TextStyle.FontSize * 0.25);
                            break;
                    }

                    Geometry gm = BuildGlyphRun(child.TextStyle, txt, x, baseline, ref totalWidth);
                    SetElement(gm, child);
                    gp.Children.Add(gm);
                    x += totalWidth;
                }

                continue;
            }

            if (child.ElementType == TextShapeElementType.Tag)
            {
                BuildTextSpan(gp, textStyle, child, ref x, ref y);
            }
        }
    }

    [SuppressMessage("Major Code Smell", "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields", Justification = "OK.")]
    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK - for now.")]
    private static Geometry BuildGlyphRun(TextStyle textStyle, string text, double x, double y, ref double totalWidth)
    {
        if (textStyle is null)
        {
            throw new ArgumentNullException(nameof(textStyle));
        }

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

        double fontSize = textStyle.FontSize;
        GlyphRun? glyphs;
        Typeface font = new Typeface(
            new FontFamily(textStyle.FontFamily ?? "Arial Unicode MS, Verdana"),
            textStyle.FontStyle,
            textStyle.FontWeight,
            FontStretch.FromOpenTypeStretch(9),
            new FontFamily("Arial Unicode MS"));

        double baseLine;
        if (font.TryGetGlyphTypeface(out GlyphTypeface glyphFace))
        {
            var dpiScale = new DpiScale(dpiX, dpiY);
            glyphs = new GlyphRun((float)dpiScale.PixelsPerDip);
            ((System.ComponentModel.ISupportInitialize)glyphs).BeginInit();
            glyphs.GlyphTypeface = glyphFace;
            glyphs.FontRenderingEmSize = fontSize;
            List<char> textChars = new List<char>();
            List<ushort> glyphIndices = new List<ushort>();
            List<double> advanceWidths = new List<double>();
            totalWidth = 0;
            foreach (char ch in text)
            {
                int codepoint = ch;
                if (!glyphFace.CharacterToGlyphMap.TryGetValue(codepoint, out var glyphIndex))
                {
                    continue;
                }

                textChars.Add(ch);
                double glyphWidth = glyphFace.AdvanceWidths[glyphIndex];
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

            double alignmentOffset = textStyle.TextAlignment switch
            {
                TextAlignment.Center => totalWidth / 2,
                TextAlignment.Right => totalWidth,
                _ => 0
            };

            baseLine = y;
            glyphs.BaselineOrigin = new Point(x - alignmentOffset, baseLine);
            ((System.ComponentModel.ISupportInitialize)glyphs).EndInit();
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

        Rect bounds = new Rect(geometryGroup.Bounds.Left, decorationPos, geometryGroup.Bounds.Width, decorationThickness);
        geometryGroup.Children.Add(new RectangleGeometry(bounds));
        return geometryGroup;
    }
}