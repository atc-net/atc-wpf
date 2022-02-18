namespace Atc.Wpf.Controls.W3cSvg;

internal class TextStyle
{
    public TextStyle(Shape owner)
    {
        if (owner == null)
        {
            throw new ArgumentNullException(nameof(owner));
        }

        this.FontFamily = "Arial Unicode MS, Verdana";
        this.FontSize = 12;
        this.FontWeight = FontWeights.Normal;
        this.FontStyle = FontStyles.Normal;
        this.TextAlignment = TextAlignment.Left;
        this.WordSpacing = 0;
        this.LetterSpacing = 0;
        this.BaseLineShift = string.Empty;
        if (owner.Parent != null)
        {
            this.Copy(owner.Parent.TextStyle);
        }
    }

    public TextStyle(TextStyle aCopy)
    {
        this.Copy(aCopy);
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
        if (aCopy == null)
        {
            return;
        }

        this.FontFamily = aCopy.FontFamily;
        this.FontSize = aCopy.FontSize;
        this.FontWeight = aCopy.FontWeight;
        this.FontStyle = aCopy.FontStyle;
        this.TextAlignment = aCopy.TextAlignment;
        this.WordSpacing = aCopy.WordSpacing;
        this.LetterSpacing = aCopy.LetterSpacing;
        this.BaseLineShift = aCopy.BaseLineShift;
    }
}