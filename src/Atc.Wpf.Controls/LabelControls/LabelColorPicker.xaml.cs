namespace Atc.Wpf.Controls.LabelControls;

public partial class LabelColorPicker : ILabelColorPicker
{
    [DependencyProperty(DefaultValue = RenderColorIndicatorType.Square)]
    private RenderColorIndicatorType renderColorIndicatorType;

    [DependencyProperty(
        DefaultValue = "Black",
        PropertyChangedCallback = nameof(OnColorValueChanged))]
    private Color? colorValue;

    [DependencyProperty(
        DefaultValue = "Black",
        PropertyChangedCallback = nameof(OnBrushValueChanged))]
    private SolidColorBrush? brushValue;

    public event EventHandler<ValueChangedEventArgs<Color>>? ColorChanged;

    public LabelColorPicker()
    {
        InitializeComponent();

        DataContext = this;

        var colorPickers = this
            .FindChildren<ColorPicker>()
            .ToList();
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
            control.SetCurrentValue(
                BrushValueProperty,
                new SolidColorBrush(color));
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
            control.SetCurrentValue(
                ColorValueProperty,
                brush.Color);
        }
    }

    private void OnColorChanged(
        object? sender,
        ValueChangedEventArgs<Color> e)
    {
        ArgumentNullException.ThrowIfNull(e);

        var solidColorBrush = SolidColorBrushHelper.GetBrushFromHex(e.NewValue.ToString(GlobalizationConstants.EnglishCultureInfo))!;

        SetCurrentValue(
            BrushValueProperty,
            solidColorBrush);
        SetCurrentValue(
            ColorValueProperty,
            solidColorBrush.Color);

        ColorChanged?.Invoke(
            this,
            new ValueChangedEventArgs<Color>(
                ControlHelper.GetIdentifier(this),
                oldValue: default,
                solidColorBrush.Color));
    }
}