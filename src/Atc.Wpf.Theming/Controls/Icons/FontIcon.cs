// ReSharper disable InconsistentNaming
namespace Atc.Wpf.Theming.Controls.Icons;

/// <summary>
/// Represents an icon that uses a glyph from the specified font.
/// </summary>
[TemplatePart(Name = nameof(PART_Glyph), Type = typeof(TextBlock))]
public class FontIcon : IconElement
{
    public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register(
        nameof(Glyph),
        typeof(string),
        typeof(FontIcon),
        new FrameworkPropertyMetadata(string.Empty));

    /// <summary>
    /// Gets or sets the character code that identifies the icon glyph.
    /// </summary>
    /// <returns>The hexadecimal character code for the icon glyph.</returns>
    public string Glyph
    {
        get => (string)GetValue(GlyphProperty);
        set => SetValue(GlyphProperty, value);
    }

    static FontIcon()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(FontIcon), new FrameworkPropertyMetadata(typeof(FontIcon)));
    }

    private TextBlock? PART_Glyph { get; set; }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        PART_Glyph = GetTemplateChild(nameof(PART_Glyph)) as TextBlock;

        if (PART_Glyph is not null && InheritsForegroundFromVisualParent)
        {
            PART_Glyph.Foreground = VisualParentForeground;
        }
    }

    protected override void OnInheritsForegroundFromVisualParentPropertyChanged(
        DependencyPropertyChangedEventArgs e)
    {
        base.OnInheritsForegroundFromVisualParentPropertyChanged(e);

        if (PART_Glyph is not null)
        {
            if (InheritsForegroundFromVisualParent)
            {
                PART_Glyph.Foreground = VisualParentForeground;
            }
            else
            {
                PART_Glyph.ClearValue(TextBlock.ForegroundProperty);
            }
        }
    }

    protected override void OnVisualParentForegroundPropertyChanged(
        DependencyPropertyChangedEventArgs e)
    {
        base.OnVisualParentForegroundPropertyChanged(e);

        if (PART_Glyph is not null && InheritsForegroundFromVisualParent)
        {
            PART_Glyph.Foreground = e.NewValue as Brush;
        }
    }
}