// ReSharper disable PossibleLossOfFraction
namespace Atc.Wpf.Controls.ColorControls.Internal;

internal class SliderPickerAdorner : Adorner
{
    private static readonly Pen Pen = new(Brushes.Black, 1);
    private Brush brush = Brushes.Red;

    private static readonly DependencyProperty VerticalPercentProperty = DependencyProperty.Register(
        nameof(VerticalPercent),
        typeof(double),
        typeof(SliderPickerAdorner),
        new FrameworkPropertyMetadata(
            0.0,
            FrameworkPropertyMetadataOptions.AffectsRender));

    private static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
        nameof(Color),
        typeof(Color),
        typeof(SliderPickerAdorner),
        new FrameworkPropertyMetadata(
            Colors.Red,
            FrameworkPropertyMetadataOptions.AffectsRender));

    public SliderPickerAdorner(UIElement adornedElement)
        : base(adornedElement)
    {
        IsHitTestVisible = false;
    }

    public double VerticalPercent
    {
        get => (double)GetValue(VerticalPercentProperty);
        set => SetValue(VerticalPercentProperty, value);
    }

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set
        {
            SetValue(ColorProperty, value);
            brush = new SolidColorBrush(value);
        }
    }

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

        drawingContext.DrawGeometry(brush, Pen, triangleGeometry);
        drawingContext.DrawGeometry(brush, Pen, rightTri);
    }
}