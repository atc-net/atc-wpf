namespace Atc.Wpf.Forms;

public partial class LabelToggleSwitch : ILabelToggleSwitch
{
    [DependencyProperty(DefaultValue = FlowDirection.RightToLeft)]
    private FlowDirection contentDirection;

    [DependencyProperty(
        DefaultValue = "Off",
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private string offText;

    [DependencyProperty(
        DefaultValue = "On",
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private string onText;

    [DependencyProperty(
        DefaultValue = false,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        PropertyChangedCallback = nameof(OnIsOnChanged))]
    private bool isOn;

    public event EventHandler<ValueChangedEventArgs<bool>>? IsOnChanged;

    static LabelToggleSwitch()
    {
        LabelPositionProperty.OverrideMetadata(
            typeof(LabelToggleSwitch),
            new FrameworkPropertyMetadata(
                LabelPosition.Left,
                OnLabelPositionChanged));
    }

    public LabelToggleSwitch()
    {
        InitializeComponent();
    }

    private static void OnLabelPositionChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelToggleSwitch)d;

        control.SetCurrentValue(LabelWidthSizeDefinitionProperty, SizeDefinitionType.Pixel);
        if (e.NewValue is LabelPosition.Right)
        {
            control.SetCurrentValue(LabelWidthNumberProperty, 50);
            control.SetCurrentValue(OffTextProperty, string.Empty);
            control.SetCurrentValue(OnTextProperty, string.Empty);
        }
        else
        {
            control.SetCurrentValue(LabelWidthNumberProperty, 120);
            control.SetCurrentValue(OffTextProperty, "Off");
            control.SetCurrentValue(OnTextProperty, "On");
        }
    }

    private static void OnIsOnChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelToggleSwitch)d;

        control.IsOnChanged?.Invoke(
            control,
            new ValueChangedEventArgs<bool>(
                control.Identifier,
                control.IsOn,
                !control.IsOn));
    }
}