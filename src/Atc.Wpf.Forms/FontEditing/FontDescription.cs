namespace Atc.Wpf.Forms.FontEditing;

/// <summary>
/// Bundle of font appearance properties: Family, Size, Weight, Style, Stretch, plus optional
/// Foreground / Background brushes.
/// </summary>
public sealed class FontDescription : IEquatable<FontDescription>
{
    public const double DefaultSize = 12d;

    public FontDescription()
        : this(
            new FontFamily("Segoe UI"),
            DefaultSize,
            FontWeights.Normal,
            FontStyles.Normal,
            FontStretches.Normal)
    {
    }

    public FontDescription(
        FontFamily family,
        double size,
        FontWeight weight,
        FontStyle style,
        FontStretch stretch,
        SolidColorBrush? foreground = null,
        SolidColorBrush? background = null,
        TextDecorationCollection? textDecorations = null)
    {
        ArgumentNullException.ThrowIfNull(family);

        Family = family;
        Size = size;
        Weight = weight;
        Style = style;
        Stretch = stretch;
        Foreground = foreground;
        Background = background;
        TextDecorations = textDecorations;
    }

    public FontFamily Family { get; set; }

    public double Size { get; set; }

    public FontWeight Weight { get; set; }

    public FontStyle Style { get; set; }

    public FontStretch Stretch { get; set; }

    public SolidColorBrush? Foreground { get; set; }

    public SolidColorBrush? Background { get; set; }

    public TextDecorationCollection? TextDecorations { get; set; }

    public static FontDescription FromControl(Control control)
    {
        ArgumentNullException.ThrowIfNull(control);

        return new FontDescription(
            control.FontFamily,
            control.FontSize,
            control.FontWeight,
            control.FontStyle,
            control.FontStretch,
            control.Foreground as SolidColorBrush,
            control.Background as SolidColorBrush);
    }

    public static FontDescription FromTextBlock(TextBlock textBlock)
    {
        ArgumentNullException.ThrowIfNull(textBlock);

        return new FontDescription(
            textBlock.FontFamily,
            textBlock.FontSize,
            textBlock.FontWeight,
            textBlock.FontStyle,
            textBlock.FontStretch,
            textBlock.Foreground as SolidColorBrush,
            textBlock.Background as SolidColorBrush,
            textBlock.TextDecorations);
    }

    public void ApplyTo(Control control)
    {
        ArgumentNullException.ThrowIfNull(control);

        control.FontFamily = Family;
        control.FontSize = Size;
        control.FontWeight = Weight;
        control.FontStyle = Style;
        control.FontStretch = Stretch;

        if (Foreground is not null)
        {
            control.Foreground = Foreground;
        }

        if (Background is not null)
        {
            control.Background = Background;
        }
    }

    public void ApplyTo(TextBlock textBlock)
    {
        ArgumentNullException.ThrowIfNull(textBlock);

        textBlock.FontFamily = Family;
        textBlock.FontSize = Size;
        textBlock.FontWeight = Weight;
        textBlock.FontStyle = Style;
        textBlock.FontStretch = Stretch;

        if (Foreground is not null)
        {
            textBlock.Foreground = Foreground;
        }

        if (Background is not null)
        {
            textBlock.Background = Background;
        }

        if (TextDecorations is not null)
        {
            textBlock.TextDecorations = TextDecorations;
        }
    }

    public FontDescription Clone()
        => new(
            Family,
            Size,
            Weight,
            Style,
            Stretch,
            Foreground is null ? null : new SolidColorBrush(Foreground.Color),
            Background is null ? null : new SolidColorBrush(Background.Color),
            TextDecorations is null ? null : new TextDecorationCollection(TextDecorations));

    public bool Equals(FontDescription? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return string.Equals(Family.Source, other.Family.Source, StringComparison.Ordinal) &&
               Size.Equals(other.Size) &&
               Weight.Equals(other.Weight) &&
               Style.Equals(other.Style) &&
               Stretch.Equals(other.Stretch) &&
               BrushColorEquals(Foreground, other.Foreground) &&
               BrushColorEquals(Background, other.Background) &&
               TextDecorationsEquals(TextDecorations, other.TextDecorations);
    }

    public override bool Equals(object? obj)
        => obj is FontDescription other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(
            Family.Source,
            Size,
            Weight,
            Style,
            Stretch,
            Foreground?.Color,
            Background?.Color,
            TextDecorations?.Count ?? 0);

    private static bool TextDecorationsEquals(
        TextDecorationCollection? a,
        TextDecorationCollection? b)
    {
        if (a is null && b is null)
        {
            return true;
        }

        if (a is null || b is null)
        {
            return a?.Count == 0 || b?.Count == 0;
        }

        if (a.Count != b.Count)
        {
            return false;
        }

        for (var i = 0; i < a.Count; i++)
        {
            if (a[i].Location != b[i].Location)
            {
                return false;
            }
        }

        return true;
    }

    public override string ToString()
        => $"{Family.Source} {Size}pt {Weight} {Style} {Stretch}";

    private static bool BrushColorEquals(
        SolidColorBrush? a,
        SolidColorBrush? b)
    {
        if (a is null && b is null)
        {
            return true;
        }

        if (a is null || b is null)
        {
            return false;
        }

        return a.Color.Equals(b.Color);
    }
}