namespace Atc.Wpf.Controls.LabelControls;

public partial class LabelColorPicker : ILabelColorPicker
{
    [DependencyProperty(DefaultValue = RenderColorIndicatorType.Square)]
    private RenderColorIndicatorType renderColorIndicatorType;

    // Note: DependencyProperty-SourceGenerator don't support "typeof(Color) as null" for now
    public static readonly DependencyProperty ColorValueProperty = DependencyProperty.Register(
        nameof(ColorValue),
        typeof(Color),
        typeof(LabelColorPicker),
        new PropertyMetadata(
            Colors.Black,
            OnColorValueChanged));

    // Note: DependencyProperty-SourceGenerator don't support "typeof(Color) as null" for now
    public Color? ColorValue
    {
        get => (Color?)GetValue(ColorValueProperty);
        set => SetValue(ColorValueProperty, value);
    }

    // Note: DependencyProperty-SourceGenerator don't support "typeof(SolidColorBrush) as null" for now
    public static readonly DependencyProperty BrushValueProperty = DependencyProperty.Register(
        nameof(BrushValue),
        typeof(SolidColorBrush),
        typeof(LabelColorPicker),
        new PropertyMetadata(
            Brushes.Black,
            OnBrushValueChanged));

    // Note: DependencyProperty-SourceGenerator don't support "typeof(SolidColorBrush) as null" for now
    public SolidColorBrush? BrushValue
    {
        get => (SolidColorBrush?)GetValue(BrushValueProperty);
        set => SetValue(BrushValueProperty, value);
    }

    public event EventHandler<ValueChangedEventArgs<Color>>? ColorChanged;

    public LabelColorPicker()
    {
        InitializeComponent();

        DataContext = this;

        var colorPickers = this.FindChildren<ColorPicker>().ToList();
        if (colorPickers.Count != 1)
        {
            throw new UnexpectedTypeException($"{nameof(LabelColorPicker)} should only contains one {nameof(ColorPicker)}");
        }

        colorPickers[0].ColorChanged += OnColorChanged;
    }

    private static void OnColorValueChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelColorPicker)d;

        if (e.NewValue is not Color color)
        {
            return;
        }

        if (control.BrushValue?.Color != color)
        {
            control.SetCurrentValue(BrushValueProperty, new SolidColorBrush(color));
        }
    }

    private static void OnBrushValueChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelColorPicker)d;

        if (e.NewValue is not SolidColorBrush brush)
        {
            return;
        }

        if (control.ColorValue != brush.Color)
        {
            control.SetCurrentValue(ColorValueProperty, brush.Color);
        }
    }

    private void OnColorChanged(
        object? sender,
        ValueChangedEventArgs<Color> e)
    {
        ArgumentNullException.ThrowIfNull(e);

        var solidColorBrush = SolidColorBrushHelper.GetBrushFromHex(e.NewValue.ToString(GlobalizationConstants.EnglishCultureInfo))!;

        SetCurrentValue(BrushValueProperty, solidColorBrush);
        SetCurrentValue(ColorValueProperty, solidColorBrush.Color);

        ColorChanged?.Invoke(
            this,
            new ValueChangedEventArgs<Color>(
                ControlHelper.GetIdentifier(this),
                oldValue: default,
                solidColorBrush.Color));
    }
}