namespace Atc.Wpf.Controls.ColorControls;

public partial class SaturationBrightnessPicker
{
    private readonly PointPickerAdorner adorner;

    public static readonly DependencyProperty HueProperty = DependencyProperty.Register(
        nameof(Hue),
        typeof(double),
        typeof(SaturationBrightnessPicker),
        new PropertyMetadata(defaultValue: 0d));

    public double Hue
    {
        get => (double)GetValue(HueProperty);
        set => SetValue(HueProperty, value);
    }

    public static readonly DependencyProperty SaturationProperty = DependencyProperty.Register(
        nameof(Saturation),
        typeof(double),
        typeof(SaturationBrightnessPicker),
        new PropertyMetadata(
            defaultValue: 0d,
            OnSaturationChanged));

    public double Saturation
    {
        get => (double)GetValue(SaturationProperty);
        set => SetValue(SaturationProperty, value);
    }

    public static readonly DependencyProperty BrightnessProperty = DependencyProperty.Register(
        nameof(Brightness),
        typeof(double),
        typeof(SaturationBrightnessPicker),
        new PropertyMetadata(
            defaultValue: 0d,
            OnBrightnessChanged));

    public double Brightness
    {
        get => (double)GetValue(BrightnessProperty);
        set => SetValue(BrightnessProperty, value);
    }

    public static readonly DependencyProperty ColorValueProperty = DependencyProperty.Register(
        nameof(ColorValue),
        typeof(Color),
        typeof(SaturationBrightnessPicker),
        new PropertyMetadata(Colors.Black));

    public Color? ColorValue
    {
        get => (Color?)GetValue(ColorValueProperty);
        set => SetValue(ColorValueProperty, value);
    }

    public static readonly DependencyProperty BrushValueProperty = DependencyProperty.Register(
        nameof(BrushValue),
        typeof(Brush),
        typeof(SaturationBrightnessPicker),
        new PropertyMetadata(Brushes.Black));

    public Brush BrushValue
    {
        get => (Brush)GetValue(BrushValueProperty);
        set => SetValue(BrushValueProperty, value);
    }

    public SaturationBrightnessPicker()
    {
        InitializeComponent();

        adorner = new PointPickerAdorner(this);
        Loaded += OnLoaded;
    }

    protected override void OnMouseMove(
        MouseEventArgs e)
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

    protected override void OnMouseUp(
        MouseButtonEventArgs e)
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

        var saturation = (double)e.NewValue;
        var pos = control.adorner.Position;

        control.adorner.Position = pos with
        {
            X = saturation * control.ActualWidth,
        };
    }

    private static void OnBrightnessChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (SaturationBrightnessPicker)d;

        var brightness = (double)e.NewValue;
        var pos = control.adorner.Position;

        control.adorner.Position = pos with
        {
            Y = (1 - brightness) * control.ActualHeight,
        };
    }

    private void Update(
        Point point)
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