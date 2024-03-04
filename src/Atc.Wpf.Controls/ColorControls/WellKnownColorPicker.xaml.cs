namespace Atc.Wpf.Controls.ColorControls;

/// <summary>
/// Interaction logic for WellKnownColorPicker.
/// </summary>
[SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "OK.")]
public partial class WellKnownColorPicker
{
    public static readonly DependencyProperty ShowOnlyStandardProperty = DependencyProperty.Register(
        nameof(ShowOnlyStandard),
        typeof(bool),
        typeof(WellKnownColorPicker),
        new PropertyMetadata(
            BooleanBoxes.FalseBox,
            OnShowOnlyStandardChanged));

    public bool ShowOnlyStandard
    {
        get => (bool)GetValue(ShowOnlyStandardProperty);
        set => SetValue(ShowOnlyStandardProperty, value);
    }

    public static readonly DependencyProperty PaletteProperty = DependencyProperty.Register(
        nameof(Palette),
        typeof(List<Color>),
        typeof(WellKnownColorPicker),
        new PropertyMetadata(default(List<Color>)));

    public List<Color> Palette
    {
        get => (List<Color>)GetValue(PaletteProperty);
        set => SetValue(PaletteProperty, value);
    }

    public event EventHandler<ValueChangedEventArgs<Color>>? ColorChanged;

    public WellKnownColorPicker()
    {
        InitializeComponent();

        DataContext = this;

        Palette = [];
        Palette.AddRange(ColorHelper.GetColors());
    }

    private static void OnShowOnlyStandardChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (WellKnownColorPicker)d;

        control.Palette = [];
        control.Palette.AddRange(
            control.ShowOnlyStandard
                ? ColorHelper.GetBasicColors()
                : ColorHelper.GetColors());
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
    }
}