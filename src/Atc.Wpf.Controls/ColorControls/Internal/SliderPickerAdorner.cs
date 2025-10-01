// ReSharper disable PossibleLossOfFraction
namespace Atc.Wpf.Controls.ColorControls.Internal;

internal partial class SliderPickerAdorner : Adorner
{
    [DependencyProperty(
        DefaultValue = 0.0,
        Flags = FrameworkPropertyMetadataOptions.AffectsRender)]
    private double verticalPercent;

    [DependencyProperty(
        DefaultValue = nameof(Colors.Black),
        Flags = FrameworkPropertyMetadataOptions.AffectsRender)]
    private Color color;

    [DependencyProperty(
        DefaultValue = nameof(Brushes.Red),
        Flags = FrameworkPropertyMetadataOptions.AffectsRender)]
    private Brush brush;

    public SliderPickerAdorner(UIElement adornedElement)
        : base(adornedElement)
        => IsHitTestVisible = false;

    public Rect ElementSize { get; set; }

    protected override void OnRender(
        DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        const int width = 10;
        var y = ElementSize.Height * VerticalPercent;
        const int x = 5;

        var triangleGeometry = new StreamGeometry();
        using (var context = triangleGeometry.Open())
        {
            context.BeginFigure(
                new Point(x, y + (width / 2)),
                isFilled: true,
                isClosed: true);

            context.LineTo(
                new Point(x + width, y),
                isStroked: true,
                isSmoothJoin: false);

            context.LineTo(
                new Point(x, y - (width / 2)),
                isStroked: true,
                isSmoothJoin: false);
        }

        var rightTri = triangleGeometry.Clone();
        var transformGroup = new TransformGroup();
        transformGroup.Children.Add(new ScaleTransform(-1, 1));
        transformGroup.Children.Add(new TranslateTransform(ElementSize.Width, 0));
        rightTri.Transform = transformGroup;

        var pen = new Pen(
            new SolidColorBrush(Color),
            1);

        drawingContext.DrawGeometry(Brush, pen, triangleGeometry);
        drawingContext.DrawGeometry(Brush, pen, rightTri);
    }
}