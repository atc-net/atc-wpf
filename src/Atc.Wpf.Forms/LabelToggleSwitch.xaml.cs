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

    public LabelToggleSwitch()
    {
        InitializeComponent();
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