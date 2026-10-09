namespace Atc.Wpf.Theming.Controls;

/// <summary>
/// A content control that draws a line along one side of its content.
/// </summary>
[TemplatePart(Name = UnderlineBorderPartName, Type = typeof(Border))]
public sealed class Underline : ContentControl
{
    /// <summary>
    /// The name of the template part that draws the line.
    /// </summary>
    public const string UnderlineBorderPartName = "PART_UnderlineBorder";
    private Border? underlineBorder;

    /// <summary>Identifies the <see cref="Placement"/> dependency property.</summary>
    public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register(
        nameof(Placement),
        typeof(Dock),
        typeof(Underline),
        new PropertyMetadata(
            default(Dock),
            (o, _) => { (o as Underline)?.ApplyBorderProperties(); }));

    /// <summary>
    /// Gets or sets the side of the content where the line is drawn.
    /// </summary>
    public Dock Placement
    {
        get => (Dock)GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }

    /// <summary>Identifies the <see cref="LineThickness"/> dependency property.</summary>
    public static readonly DependencyProperty LineThicknessProperty = DependencyProperty.Register(
        nameof(LineThickness),
        typeof(double),
        typeof(Underline),
        new PropertyMetadata(
            1d,
            (o, _) => { (o as Underline)?.ApplyBorderProperties(); }));

    /// <summary>
    /// Gets or sets the thickness of the line.
    /// </summary>
    public double LineThickness
    {
        get => (double)GetValue(LineThicknessProperty);
        set => SetValue(LineThicknessProperty, value);
    }

    /// <summary>Identifies the <see cref="LineExtent"/> dependency property.</summary>
    public static readonly DependencyProperty LineExtentProperty = DependencyProperty.Register(
        nameof(LineExtent),
        typeof(double),
        typeof(Underline),
        new PropertyMetadata(
            double.NaN,
            (o, _) => { (o as Underline)?.ApplyBorderProperties(); }));

    /// <summary>
    /// Gets or sets the size of the line border across the placement side
    /// (height for top/bottom, width for left/right); <see cref="double.NaN"/> sizes it automatically.
    /// </summary>
    public double LineExtent
    {
        get => (double)GetValue(LineExtentProperty);
        set => SetValue(LineExtentProperty, value);
    }

    static Underline()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Underline), new FrameworkPropertyMetadata(typeof(Underline)));
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        underlineBorder = GetTemplateChild(UnderlineBorderPartName) as Border;

        ApplyBorderProperties();
    }

    private void ApplyBorderProperties()
    {
        if (underlineBorder is null)
        {
            return;
        }

        void Execute()
        {
            underlineBorder.Height = double.NaN;
            underlineBorder.Width = double.NaN;
            underlineBorder.BorderThickness = new Thickness(0);
            switch (Placement)
            {
                case Dock.Left:
                    underlineBorder.Width = LineExtent;
                    underlineBorder.BorderThickness = new Thickness(LineThickness, 0d, 0d, 0d);
                    break;
                case Dock.Top:
                    underlineBorder.Height = LineExtent;
                    underlineBorder.BorderThickness = new Thickness(0d, LineThickness, 0d, 0d);
                    break;
                case Dock.Right:
                    underlineBorder.Width = LineExtent;
                    underlineBorder.BorderThickness = new Thickness(0d, 0d, LineThickness, 0d);
                    break;
                case Dock.Bottom:
                    underlineBorder.Height = LineExtent;
                    underlineBorder.BorderThickness = new Thickness(0d, 0d, 0d, LineThickness);
                    break;
                default:
                    throw new SwitchCaseDefaultException(Placement);
            }

            InvalidateVisual();
        }

        this.ExecuteWhenLoaded(Execute);
    }
}