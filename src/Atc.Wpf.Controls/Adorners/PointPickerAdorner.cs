namespace Atc.Wpf.Controls.Adorners;

/// <summary>An adorner that draws a circular point marker at <see cref="Position"/> on top of the adorned element.</summary>
public sealed partial class PointPickerAdorner : Adorner
{
    private static readonly Brush FillBrush = Brushes.Transparent;
    private readonly Pen selectorPen;

    [DependencyProperty(
        DefaultValue = "default(Point)",
        Flags = FrameworkPropertyMetadataOptions.AffectsRender)]
    private Point position;

    /// <summary>Initializes a new instance of the <see cref="PointPickerAdorner"/> class.</summary>
    public PointPickerAdorner(UIElement adornedElement)
        : base(adornedElement)
    {
        selectorPen = new Pen(
            ThemeManagerHelper.GetPrimaryAccentBrush(),
            1);

        IsHitTestVisible = false;
    }

    /// <inheritdoc />
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