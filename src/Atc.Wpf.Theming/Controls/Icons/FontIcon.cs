// ReSharper disable InconsistentNaming
namespace Atc.Wpf.Theming.Controls.Icons;

/// <summary>
/// Represents an icon that uses a glyph from the specified font.
/// </summary>
[DefaultStyleKey]
public sealed partial class FontIcon : IconElement
{
    /// <summary>
    /// Gets or sets the character code that identifies the icon glyph.
    /// </summary>
    /// <returns>The hexadecimal character code for the icon glyph.</returns>
    [DependencyProperty(
        DefaultValue = "",
        MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata)]
    private string glyph;

    [TemplatePartField("PART_Glyph")]
    private TextBlock? glyphPart;

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        ResolveTemplateParts();

        if (glyphPart is not null && InheritsForegroundFromVisualParent)
        {
            glyphPart.Foreground = VisualParentForeground;
        }
    }

    /// <inheritdoc />
    protected override void OnInheritsForegroundFromVisualParentPropertyChanged(
        DependencyPropertyChangedEventArgs e)
    {
        base.OnInheritsForegroundFromVisualParentPropertyChanged(e);

        if (glyphPart is not null)
        {
            if (InheritsForegroundFromVisualParent)
            {
                glyphPart.Foreground = VisualParentForeground;
            }
            else
            {
                glyphPart.ClearValue(TextBlock.ForegroundProperty);
            }
        }
    }

    /// <inheritdoc />
    protected override void OnVisualParentForegroundPropertyChanged(
        DependencyPropertyChangedEventArgs e)
    {
        base.OnVisualParentForegroundPropertyChanged(e);

        if (glyphPart is not null && InheritsForegroundFromVisualParent)
        {
            glyphPart.Foreground = e.NewValue as Brush;
        }
    }
}