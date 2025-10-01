namespace Atc.Wpf.Controls.LabelControls;

public partial class LabelSlider : ILabelSlider
{
    [DependencyProperty(
        DefaultValue = 0,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private int minimum;

    [DependencyProperty(
        DefaultValue = 100,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private int maximum;

    [DependencyProperty(
        DefaultValue = AutoToolTipPlacement.TopLeft,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private AutoToolTipPlacement autoToolTipPlacement;

    [DependencyProperty(DefaultValue = 5)]
    private int tickFrequency;

    [DependencyProperty(
        DefaultValue = TickPlacement.BottomRight,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal)]
    private TickPlacement tickPlacement;

    [DependencyProperty(
        DefaultValue = 0,
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        PropertyChangedCallback = nameof(OnValueChanged),
        IsAnimationProhibited = true,
        DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged)]
    private int value;

    public event EventHandler<ValueChangedEventArgs<int?>>? ValueChanged;

    public LabelSlider()
    {
        InitializeComponent();
    }

    public override bool IsValid()
    {
        ValidateValue(default, this, raiseEvents: false);
        return string.IsNullOrEmpty(ValidationText);
    }

    private static void OnValueChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelSlider)d;

        ValidateValue(e, control, raiseEvents: true);
    }

    private static void ValidateValue(
        DependencyPropertyChangedEventArgs e,
        LabelSlider control,
        bool raiseEvents)
    {
        if (e.NewValue is not int newValue)
        {
            control.ValidationText = Validations.ValueShouldBeAInteger;
            return;
        }

        if (e.OldValue is not int oldValue)
        {
            return;
        }

        if (raiseEvents)
        {
            control.ValueChanged?.Invoke(
                control,
                new ValueChangedEventArgs<int?>(
                    control.Identifier,
                    oldValue,
                    newValue));
        }
    }
}