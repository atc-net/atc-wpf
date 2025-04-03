namespace Atc.Wpf.Controls.LabelControls;

public partial class LabelToggleSwitch : ILabelToggleSwitch
{
    public static readonly DependencyProperty ContentDirectionProperty = DependencyProperty.Register(
        nameof(ContentDirection),
        typeof(FlowDirection),
        typeof(LabelToggleSwitch),
        new PropertyMetadata(FlowDirection.RightToLeft));

    public FlowDirection ContentDirection
    {
        get => (FlowDirection)GetValue(ContentDirectionProperty);
        set => SetValue(ContentDirectionProperty, value);
    }

    public static readonly DependencyProperty OffTextProperty = DependencyProperty.Register(
        nameof(OffText),
        typeof(string),
        typeof(LabelToggleSwitch),
        new FrameworkPropertyMetadata(
            "Off",
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            propertyChangedCallback: null));

    public string OffText
    {
        get => (string)GetValue(OffTextProperty);
        set => SetValue(OffTextProperty, value);
    }

    public static readonly DependencyProperty OnTextProperty = DependencyProperty.Register(
        nameof(OnText),
        typeof(string),
        typeof(LabelToggleSwitch),
        new FrameworkPropertyMetadata(
            "On",
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            propertyChangedCallback: null));

    public string OnText
    {
        get => (string)GetValue(OnTextProperty);
        set => SetValue(OnTextProperty, value);
    }

    public static readonly DependencyProperty IsOnProperty = DependencyProperty.Register(
        nameof(IsOn),
        typeof(bool),
        typeof(LabelToggleSwitch),
        new FrameworkPropertyMetadata(
            defaultValue: false,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            OnIsOnChanged));

    public bool IsOn
    {
        get => (bool)GetValue(IsOnProperty);
        set => SetValue(IsOnProperty, value);
    }

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