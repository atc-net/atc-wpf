namespace Atc.Wpf.Controls.ColorControls;

[SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "OK.")]
public partial class WellKnownColorPicker
{
    [DependencyProperty(DefaultValue = false, PropertyChangedCallback = nameof(OnShowOnlyStandardChanged))]
    private bool showOnlyBasicColors;

    // Note: DependencyProperty-SourceGenerator don't support "List<ColorItem>" for now
    public static readonly DependencyProperty PaletteProperty = DependencyProperty.Register(
        nameof(Palette),
        typeof(List<ColorItem>),
        typeof(WellKnownColorPicker),
        new PropertyMetadata(default(List<ColorItem>)));

    // Note: DependencyProperty-SourceGenerator don't support "List<ColorItem>" for now
    public List<ColorItem> Palette
    {
        get => (List<ColorItem>)GetValue(PaletteProperty);
        set => SetValue(PaletteProperty, value);
    }

    [DependencyProperty(DefaultValue = "Transparent")]
    private Brush colorBrush;

    public event EventHandler<ValueChangedEventArgs<Color>>? ColorChanged;

    public WellKnownColorPicker()
    {
        InitializeComponent();

        DataContext = this;

        Palette = [];
        Palette.AddRange(ColorItemHelper.GetColorItems());
    }

    private static void OnShowOnlyStandardChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (WellKnownColorPicker)d;

        control.Palette = [];
        control.Palette.AddRange(
            control.ShowOnlyBasicColors
                ? ColorItemHelper.GetBasicColorItems()
                : ColorItemHelper.GetColorItems());
    }

    private void OnPaletteColorClick(
        object sender,
        RoutedEventArgs e)
    {
        var control = (Button)sender;
        var color = ((SolidColorBrush)control.Background).Color;

        ColorChanged?.Invoke(
            this,
            new ValueChangedEventArgs<Color>(
                ControlHelper.GetIdentifier(this),
                oldValue: default,
                color));

        ColorBrush = new SolidColorBrush(color);
    }
}