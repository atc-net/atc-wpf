namespace Atc.Wpf.Controls.BaseControls;

public partial class ColorPicker
{
    [DependencyProperty(
        DefaultValue = RenderColorIndicatorType.Square)]
    private RenderColorIndicatorType renderColorIndicatorType;

    [DependencyProperty(
        DefaultValue = nameof(Brushes.Black),
        PropertyChangedCallback = nameof(OnColorValueChanged))]
    private Color? colorValue;

    [DependencyProperty(
        DefaultValue = nameof(Brushes.Black),
        PropertyChangedCallback = nameof(OnBrushValueChanged))]
    private SolidColorBrush? brushValue;

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