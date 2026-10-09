namespace Atc.Wpf.Forms.FontEditing;

/// <summary>
/// Bundle of font appearance properties: Family, Size, Weight, Style, Stretch, plus optional
/// Foreground / Background brushes.
/// </summary>
public sealed class FontDescription : IEquatable<FontDescription>
{
    /// <summary>
    /// The default font size.
    /// </summary>
    public const double DefaultSize = 12d;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontDescription"/> class with Segoe UI at the default size and normal weight, style and stretch.
    /// </summary>
    public FontDescription()
        : this(
            new FontFamily("Segoe UI"),
            DefaultSize,
            FontWeights.Normal,
            FontStyles.Normal,
            FontStretches.Normal)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FontDescription"/> class.
    /// </summary>
    /// <param name="family">The font family.</param>
    /// <param name="size">The font size.</param>
    /// <param name="weight">The font weight.</param>
    /// <param name="style">The font style.</param>
    /// <param name="stretch">The font stretch.</param>
    /// <param name="foreground">The optional foreground brush.</param>
    /// <param name="background">The optional background brush.</param>
    /// <param name="textDecorations">The optional text decorations.</param>
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

    /// <summary>
    /// Gets or sets the font family.
    /// </summary>
    public FontFamily Family { get; set; }

    /// <summary>
    /// Gets or sets the font size.
    /// </summary>
    public double Size { get; set; }

    /// <summary>
    /// Gets or sets the font weight.
    /// </summary>
    public FontWeight Weight { get; set; }

    /// <summary>
    /// Gets or sets the font style.
    /// </summary>
    public FontStyle Style { get; set; }

    /// <summary>
    /// Gets or sets the font stretch.
    /// </summary>
    public FontStretch Stretch { get; set; }

    /// <summary>
    /// Gets or sets the optional foreground brush.
    /// </summary>
    public SolidColorBrush? Foreground { get; set; }

    /// <summary>
    /// Gets or sets the optional background brush.
    /// </summary>
    public SolidColorBrush? Background { get; set; }

    /// <summary>
    /// Gets or sets the optional text decorations, such as underline or strikethrough.
    /// </summary>
    public TextDecorationCollection? TextDecorations { get; set; }

    /// <summary>
    /// Creates a font description from the font properties and solid-color brushes of a control.
    /// </summary>
    /// <param name="control">The control to read from.</param>
    /// <returns>The new font description.</returns>
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

    /// <summary>
    /// Creates a font description from the font properties, solid-color brushes and text decorations of a text block.
    /// </summary>
    /// <param name="textBlock">The text block to read from.</param>
    /// <returns>The new font description.</returns>
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

    /// <summary>
    /// Applies the font properties to a control; the brushes are applied only when set.
    /// </summary>
    /// <param name="control">The control to update.</param>
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

    /// <summary>
    /// Applies the font properties to a text block; the brushes and text decorations are applied only when set.
    /// </summary>
    /// <param name="textBlock">The text block to update.</param>
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

    /// <summary>
    /// Creates a copy of this font description with its own brushes and text decorations.
    /// </summary>
    /// <returns>The copy.</returns>
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

    /// <inheritdoc />
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
               Size.IsEqual(other.Size) &&
               Weight.Equals(other.Weight) &&
               Style.Equals(other.Style) &&
               Stretch.Equals(other.Stretch) &&
               BrushColorEquals(Foreground, other.Foreground) &&
               BrushColorEquals(Background, other.Background) &&
               TextDecorationsEquals(TextDecorations, other.TextDecorations);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is FontDescription other && Equals(other);

    /// <inheritdoc />
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

    /// <inheritdoc />
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