namespace Atc.Wpf.Controls.BaseControls;

/// <summary>
/// Interaction logic for ColorPicker.
/// </summary>
public partial class ColorPicker
{
    public static readonly DependencyProperty RenderColorIndicatorTypeProperty = DependencyProperty.Register(
        nameof(RenderColorIndicatorType),
        typeof(RenderColorIndicatorType),
        typeof(ColorPicker),
        new PropertyMetadata(RenderColorIndicatorType.Square));

    public RenderColorIndicatorType RenderColorIndicatorType
    {
        get => (RenderColorIndicatorType)GetValue(RenderColorIndicatorTypeProperty);
        set => SetValue(RenderColorIndicatorTypeProperty, value);
    }

    public static readonly DependencyProperty ColorValueProperty = DependencyProperty.Register(
        nameof(ColorValue),
        typeof(Color),
        typeof(ColorPicker),
        new PropertyMetadata(
            Colors.Black,
            OnColorValueChanged));

    public Color? ColorValue
    {
        get => (Color?)GetValue(ColorValueProperty);
        set => SetValue(ColorValueProperty, value);
    }

    public static readonly DependencyProperty BrushValueProperty = DependencyProperty.Register(
        nameof(BrushValue),
        typeof(SolidColorBrush),
        typeof(ColorPicker),
        new PropertyMetadata(
            Brushes.Black,
            OnBrushValueChanged));

    public SolidColorBrush? BrushValue
    {
        get => (SolidColorBrush?)GetValue(BrushValueProperty);
        set => SetValue(BrushValueProperty, value);
    }

    public static readonly DependencyProperty DisplayHexCodeProperty = DependencyProperty.Register(
        nameof(DisplayHexCode),
        typeof(string),
        typeof(ColorPicker),
        new FrameworkPropertyMetadata(
            Brushes.Black.Color.ToString(GlobalizationConstants.EnglishCultureInfo),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public string? DisplayHexCode
    {
        get => (string?)GetValue(DisplayHexCodeProperty);
        set => SetValue(DisplayHexCodeProperty, value);
    }

    public event EventHandler<ValueChangedEventArgs<Color>>? ColorChanged;

    public ColorPicker()
    {
        InitializeComponent();

        DataContext = this;
    }

    private static void OnColorValueChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (ColorPicker)d;

        if (e.NewValue is not Color color)
        {
            return;
        }

        if (control.BrushValue?.Color != color)
        {
            control.SetCurrentValue(BrushValueProperty, new SolidColorBrush(color));
        }

        if (control.DisplayHexCode != color.ToString(GlobalizationConstants.EnglishCultureInfo))
        {
            control.SetCurrentValue(DisplayHexCodeProperty, color.ToString(GlobalizationConstants.EnglishCultureInfo));
        }
    }

    private static void OnBrushValueChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (ColorPicker)d;

        if (e.NewValue is not SolidColorBrush brush)
        {
            return;
        }

        if (control.ColorValue != brush.Color)
        {
            control.SetCurrentValue(ColorValueProperty, brush.Color);
        }

        if (control.DisplayHexCode != brush.Color.ToString(GlobalizationConstants.EnglishCultureInfo))
        {
            control.SetCurrentValue(DisplayHexCodeProperty, brush.Color.ToString(GlobalizationConstants.EnglishCultureInfo));
        }
    }

    private void OnClick(
        object sender,
        RoutedEventArgs e)
    {
        var colorDialog = new ColorPickerDialogBox(
            Application.Current.MainWindow!,
            ColorValue ?? Colors.Transparent);

        var dialogResult = colorDialog.ShowDialog();
        if (!dialogResult.HasValue || !dialogResult.Value)
        {
            return;
        }

        ColorValue = colorDialog.Color;
        BrushValue = colorDialog.ColorAsBrush;

        ColorChanged?.Invoke(
            this,
            new ValueChangedEventArgs<Color>(
                ControlHelper.GetIdentifier(this),
                oldValue: Colors.Transparent,
                colorDialog.Color));
    }
}