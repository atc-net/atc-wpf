namespace Atc.Wpf.Theming.Controls;

[TemplatePart(Name = UnderlineBorderPartName, Type = typeof(Border))]
public sealed class Underline : ContentControl
{
    public const string UnderlineBorderPartName = "PART_UnderlineBorder";
    private Border? underlineBorder;

    public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register(
        nameof(Placement),
        typeof(Dock),
        typeof(Underline),
        new PropertyMetadata(
            default(Dock),
            (o, _) => { (o as Underline)?.ApplyBorderProperties(); }));

    public Dock Placement
    {
        get => (Dock)GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }

    public static readonly DependencyProperty LineThicknessProperty = DependencyProperty.Register(
        nameof(LineThickness),
        typeof(double),
        typeof(Underline),
        new PropertyMetadata(
            1d,
            (o, _) => { (o as Underline)?.ApplyBorderProperties(); }));

    public double LineThickness
    {
        get => (double)GetValue(LineThicknessProperty);
        set => SetValue(LineThicknessProperty, value);
    }

    public static readonly DependencyProperty LineExtentProperty = DependencyProperty.Register(
        nameof(LineExtent),
        typeof(double),
        typeof(Underline),
        new PropertyMetadata(
            double.NaN,
            (o, _) => { (o as Underline)?.ApplyBorderProperties(); }));

    public double LineExtent
    {
        get => (double)GetValue(LineExtentProperty);
        set => SetValue(LineExtentProperty, value);
    }

    static Underline()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Underline), new FrameworkPropertyMetadata(typeof(Underline)));
    }

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