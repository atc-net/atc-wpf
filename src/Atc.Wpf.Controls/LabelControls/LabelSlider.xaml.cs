namespace Atc.Wpf.Controls.LabelControls;

public partial class LabelSlider : ILabelSlider
{
    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
        nameof(Maximum),
        typeof(int),
        typeof(LabelSlider),
        new FrameworkPropertyMetadata(
            defaultValue: 100,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

    public int Maximum
    {
        get => (int)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
        nameof(Minimum),
        typeof(int),
        typeof(LabelSlider),
        new FrameworkPropertyMetadata(
            defaultValue: 0,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

    public int Minimum
    {
        get => (int)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public static readonly DependencyProperty AutoToolTipPlacementProperty = DependencyProperty.Register(
        nameof(AutoToolTipPlacement),
        typeof(AutoToolTipPlacement),
        typeof(LabelSlider),
        new PropertyMetadata(defaultValue: AutoToolTipPlacement.TopLeft));

    public AutoToolTipPlacement AutoToolTipPlacement
    {
        get => (AutoToolTipPlacement)GetValue(AutoToolTipPlacementProperty);
        set => SetValue(AutoToolTipPlacementProperty, value);
    }

    public static readonly DependencyProperty TickFrequencyProperty = DependencyProperty.Register(
        nameof(TickFrequency),
        typeof(int),
        typeof(LabelSlider),
        new PropertyMetadata(defaultValue: 5));

    public int TickFrequency
    {
        get => (int)GetValue(TickFrequencyProperty);
        set => SetValue(TickFrequencyProperty, value);
    }

    public static readonly DependencyProperty TickPlacementProperty = DependencyProperty.Register(
        nameof(TickPlacement),
        typeof(TickPlacement),
        typeof(LabelSlider),
        new PropertyMetadata(defaultValue: TickPlacement.BottomRight));

    public TickPlacement TickPlacement
    {
        get => (TickPlacement)GetValue(TickPlacementProperty);
        set => SetValue(TickPlacementProperty, value);
    }

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(int),
        typeof(LabelSlider),
        new FrameworkPropertyMetadata(
            default(int),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            OnValueChanged,
            coerceValueCallback: null,
            isAnimationProhibited: true,
            UpdateSourceTrigger.PropertyChanged));

    [SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

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