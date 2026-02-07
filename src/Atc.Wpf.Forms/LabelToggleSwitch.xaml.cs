namespace Atc.Wpf.Forms;

public partial class LabelToggleSwitch : ILabelToggleSwitch
{
    [DependencyProperty(DefaultValue = FlowDirection.RightToLeft)]
    private FlowDirection contentDirection;

    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private string? offText;

    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private string? onText;

    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)]
    private string? offOnContent;

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

        SetCurrentValue(OffTextProperty, Word.Off);
        SetCurrentValue(OnTextProperty, Word.On);

        CultureManager.UiCultureChanged += OnUiCultureChanged;
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
            control.SetCurrentValue(OffTextProperty, Word.Off);
            control.SetCurrentValue(OnTextProperty, Word.On);
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

    private void OnUiCultureChanged(
        object? sender,
        UiCultureEventArgs e)
    {
        var oldOff = Word.ResourceManager.GetString(nameof(Word.Off), e.OldCulture) ?? "Off";
        var oldOn = Word.ResourceManager.GetString(nameof(Word.On), e.OldCulture) ?? "On";

        if (string.Equals(OffText, oldOff, StringComparison.Ordinal))
        {
            SetCurrentValue(OffTextProperty, Word.Off);
        }

        if (string.Equals(OnText, oldOn, StringComparison.Ordinal))
        {
            SetCurrentValue(OnTextProperty, Word.On);
        }
    }
}