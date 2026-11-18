// ReSharper disable InconsistentNaming
namespace Atc.Wpf.Theming.Controls.Icons;

/// <summary>
/// Represents an icon that uses a vector path as its content.
/// </summary>
[DefaultStyleKey]
public sealed partial class PathIcon : IconElement
{
    /// <summary>
    /// Gets or sets a Geometry that specifies the shape to be drawn. In XAML this can also be set using the Path Markup Syntax.
    /// </summary>
    [DependencyProperty(AddOwnerOf = typeof(Path), MetadataType = DependencyPropertyMetadataType.FrameworkPropertyMetadata)]
    private Geometry? data;

    [TemplatePartField("PART_Path")]
    private Path? pathPart;

    static PathIcon()
    {
        OverrideDefaultStyleKey();
        FocusableProperty.OverrideMetadata(typeof(PathIcon), new FrameworkPropertyMetadata(false));
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        ResolveTemplateParts();

        if (pathPart is not null && InheritsForegroundFromVisualParent)
        {
            pathPart.Fill = VisualParentForeground;
        }
    }

    /// <inheritdoc />
    protected override void OnInheritsForegroundFromVisualParentPropertyChanged(
        DependencyPropertyChangedEventArgs e)
    {
        base.OnInheritsForegroundFromVisualParentPropertyChanged(e);

        if (pathPart is not null)
        {
            if (InheritsForegroundFromVisualParent)
            {
                pathPart.Fill = VisualParentForeground;
            }
            else
            {
                pathPart.ClearValue(Shape.FillProperty);
            }
        }
    }

    /// <inheritdoc />
    protected override void OnVisualParentForegroundPropertyChanged(
        DependencyPropertyChangedEventArgs e)
    {
        base.OnVisualParentForegroundPropertyChanged(e);

        if (pathPart is not null && InheritsForegroundFromVisualParent)
        {
            pathPart.Fill = e.NewValue as Brush;
        }
    }
}