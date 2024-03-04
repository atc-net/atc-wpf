namespace Atc.Wpf.Controls.Adorners;

public sealed class PointPickerAdorner : Adorner
{
    private static readonly Brush FillBrush = Brushes.Transparent;
    private readonly Pen selectorPen;

    private static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
        nameof(Position),
        typeof(Point),
        typeof(PointPickerAdorner),
        new FrameworkPropertyMetadata(
            default(Point),
            FrameworkPropertyMetadataOptions.AffectsRender));

    public Point Position
    {
        get => (Point)GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }

    public PointPickerAdorner(
        UIElement adornedElement)
        : base(adornedElement)
    {
        selectorPen = new(
            ThemeManagerHelper.GetPrimaryAccentBrush(),
            1);

        IsHitTestVisible = false;
    }

    protected override void OnRender(
        DrawingContext drawingContext)
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