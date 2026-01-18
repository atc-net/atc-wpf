namespace Atc.Wpf.Controls.ColorEditing;

public partial class SaturationBrightnessPicker
{
    private readonly PointPickerAdorner adorner;

    [DependencyProperty(DefaultValue = 0)]
    private double hue;

    [DependencyProperty(
        DefaultValue = 0,
        PropertyChangedCallback = nameof(OnSaturationChanged))]
    private double saturation;

    [DependencyProperty(
        DefaultValue = 0,
        PropertyChangedCallback = nameof(OnBrightnessChanged))]
    private double brightness;

    [DependencyProperty(
        DefaultValue = nameof(Colors.Black))]
    private Color colorValue;

    [DependencyProperty(
        DefaultValue = nameof(Colors.Black))]
    private Brush brushValue;

    public SaturationBrightnessPicker()
    {
        InitializeComponent();

        adorner = new PointPickerAdorner(this);
        Loaded += OnLoaded;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnMouseMove(e);

        if (e.LeftButton != MouseButtonState.Pressed)
        {
            return;
        }

        Mouse.Capture(this);

        var pos = e
            .GetPosition(this)
            .Clamp(this);

        Update(pos);
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnMouseUp(e);

        Mouse.Capture(element: null);

        var pos = e
            .GetPosition(this)
            .Clamp(this);

        Update(pos);
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        var adornerLayer = AdornerLayer.GetAdornerLayer(this);
        if (adornerLayer is null)
        {
            return;
        }

        var adorners = adornerLayer.GetAdorners(this);
        if (adorners is not null &&
            adorners.Length > 0)
        {
            foreach (var item in adorners)
            {
                adornerLayer.Remove(item);
            }
        }

        adorner.Position = new Point(
            Saturation * ActualWidth,
            (1 - Brightness) * ActualHeight);

        adornerLayer.Add(adorner);
    }

    private static void OnSaturationChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (SaturationBrightnessPicker)d;

        var newSaturation = (double)e.NewValue;
        var pos = control.adorner.Position;

        control.adorner.Position = pos with
        {
            X = newSaturation * control.ActualWidth,
        };
    }

    private static void OnBrightnessChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (SaturationBrightnessPicker)d;

        var newBrightness = (double)e.NewValue;
        var pos = control.adorner.Position;

        control.adorner.Position = pos with
        {
            Y = (1 - newBrightness) * control.ActualHeight,
        };
    }

    private void Update(Point point)
    {
        adorner.Position = point;
        Saturation = point.X / ActualWidth;
        Brightness = 1 - (point.Y / ActualHeight);

        var color = ColorHelper.GetColorFromHsv(
            Hue,
            Saturation,
            Brightness);

        ColorValue = color;
        BrushValue = new SolidColorBrush(color);
    }
}