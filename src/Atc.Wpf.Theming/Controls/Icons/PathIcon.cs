// ReSharper disable InconsistentNaming
namespace Atc.Wpf.Theming.Controls.Icons;

/// <summary>
/// Represents an icon that uses a vector path as its content.
/// </summary>
[TemplatePart(Name = nameof(PART_Path), Type = typeof(Path))]
public class PathIcon : IconElement
{
    private Path? PART_Path { get; set; }

    public static readonly DependencyProperty DataProperty = Path.DataProperty.AddOwner(
        typeof(PathIcon),
        new FrameworkPropertyMetadata(propertyChangedCallback: null));

    /// <summary>
    /// Gets or sets a Geometry that specifies the shape to be drawn. In XAML this can also be set using the Path Markup Syntax.
    /// </summary>
    public Geometry? Data
    {
        get => (Geometry?)GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    static PathIcon()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(PathIcon), new FrameworkPropertyMetadata(typeof(PathIcon)));
        FocusableProperty.OverrideMetadata(typeof(PathIcon), new FrameworkPropertyMetadata(false));
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        PART_Path = GetTemplateChild(nameof(PART_Path)) as Path;

        if (PART_Path is not null && InheritsForegroundFromVisualParent)
        {
            PART_Path.Fill = VisualParentForeground;
        }
    }

    protected override void OnInheritsForegroundFromVisualParentPropertyChanged(
        DependencyPropertyChangedEventArgs e)
    {
        base.OnInheritsForegroundFromVisualParentPropertyChanged(e);

        if (PART_Path is not null)
        {
            if (InheritsForegroundFromVisualParent)
            {
                PART_Path.Fill = VisualParentForeground;
            }
            else
            {
                PART_Path.ClearValue(Shape.FillProperty);
            }
        }
    }

    protected override void OnVisualParentForegroundPropertyChanged(
        DependencyPropertyChangedEventArgs e)
    {
        base.OnVisualParentForegroundPropertyChanged(e);

        if (PART_Path is not null && InheritsForegroundFromVisualParent)
        {
            PART_Path.Fill = e.NewValue as Brush;
        }
    }
}