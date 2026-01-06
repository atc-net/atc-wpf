namespace Atc.Wpf.Controls.Adorners;

public sealed partial class PointPickerAdorner : Adorner
{
    private static readonly Brush FillBrush = Brushes.Transparent;
    private readonly Pen selectorPen;

    [DependencyProperty(
        DefaultValue = "default(Point)",
        Flags = FrameworkPropertyMetadataOptions.AffectsRender)]
    private Point position;

    public PointPickerAdorner(UIElement adornedElement)
        : base(adornedElement)
    {
        selectorPen = new Pen(
            ThemeManagerHelper.GetPrimaryAccentBrush(),
            1);

        IsHitTestVisible = false;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        ArgumentNullException.ThrowIfNull(drawingContext);

        base.OnRender(drawingContext);

        drawingContext.DrawEllipse(
            FillBrush,
            selectorPen,
            Position,
            radiusX: 5,
            radiusY: 5);
    }
}