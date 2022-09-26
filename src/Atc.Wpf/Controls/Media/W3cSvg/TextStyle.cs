namespace Atc.Wpf.Controls.Media.W3cSvg;

internal class TextStyle
{
    public TextStyle(Shape owner)
    {
        ArgumentNullException.ThrowIfNull(owner);

        FontFamily = "Arial Unicode MS, Verdana";
        FontSize = 12;
        FontWeight = FontWeights.Normal;
        FontStyle = FontStyles.Normal;
        TextAlignment = TextAlignment.Left;
        WordSpacing = 0;
        LetterSpacing = 0;
        BaseLineShift = string.Empty;
        if (owner.Parent is not null)
        {
            Copy(owner.Parent.TextStyle);
        }
    }

    public TextStyle(TextStyle aCopy)
    {
        Copy(aCopy);
    }

    public string? FontFamily { get; set; }

    public double FontSize { get; set; }

    public FontWeight FontWeight { get; set; }

    public FontStyle FontStyle { get; set; }

    public TextDecorationCollection? TextDecoration { get; set; }

    public TextAlignment TextAlignment { get; set; }

    public double WordSpacing { get; set; }

    public double LetterSpacing { get; set; }

    public string? BaseLineShift { get; set; }

    public void Copy(TextStyle? aCopy)
    {
        if (aCopy is null)
        {
            return;
        }

        FontFamily = aCopy.FontFamily;
        FontSize = aCopy.FontSize;
        FontWeight = aCopy.FontWeight;
        FontStyle = aCopy.FontStyle;
        TextAlignment = aCopy.TextAlignment;
        WordSpacing = aCopy.WordSpacing;
        LetterSpacing = aCopy.LetterSpacing;
        BaseLineShift = aCopy.BaseLineShift;
    }
}