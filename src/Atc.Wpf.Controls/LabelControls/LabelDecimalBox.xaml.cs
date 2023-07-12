namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelDecimalBox.
/// </summary>
public partial class LabelDecimalBox : ILabelDecimalBox
{
    public static readonly DependencyProperty WatermarkTextProperty = DependencyProperty.Register(
        nameof(WatermarkText),
        typeof(string),
        typeof(LabelDecimalBox),
        new PropertyMetadata(defaultValue: string.Empty));

    public string WatermarkText
    {
        get => (string)GetValue(WatermarkTextProperty);
        set => SetValue(WatermarkTextProperty, value);
    }

    public static readonly DependencyProperty WatermarkAlignmentProperty = DependencyProperty.Register(
        nameof(WatermarkAlignment),
        typeof(TextAlignment),
        typeof(LabelDecimalBox),
        new PropertyMetadata(default(TextAlignment)));

    public TextAlignment WatermarkAlignment
    {
        get => (TextAlignment)GetValue(WatermarkAlignmentProperty);
        set => SetValue(WatermarkAlignmentProperty, value);
    }

    public static readonly DependencyProperty PrefixTextProperty = DependencyProperty.Register(
        nameof(PrefixText),
        typeof(string),
        typeof(LabelDecimalBox),
        new FrameworkPropertyMetadata(
            string.Empty,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

    public string PrefixText
    {
        get => (string)GetValue(PrefixTextProperty);
        set => SetValue(PrefixTextProperty, value);
    }

    public static readonly DependencyProperty SuffixTextProperty = DependencyProperty.Register(
        nameof(SuffixText),
        typeof(string),
        typeof(LabelDecimalBox),
        new FrameworkPropertyMetadata(
            string.Empty,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

    public string SuffixText
    {
        get => (string)GetValue(SuffixTextProperty);
        set => SetValue(SuffixTextProperty, value);
    }

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(decimal),
        typeof(LabelDecimalBox),
        new FrameworkPropertyMetadata(
            default(decimal),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            OnValueLostFocus,
            coerceValueCallback: null,
            isAnimationProhibited: true,
            UpdateSourceTrigger.LostFocus));

    [SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
    public decimal Value
    {
        get => (decimal)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public event EventHandler<ChangedDecimalEventArgs>? ValueChanged;

    public LabelDecimalBox()
    {
        InitializeComponent();
    }

    public override bool IsValid()
    {
        ValidateValue(default, this, raiseEvents: false);
        return string.IsNullOrEmpty(ValidationText);
    }

    private static void OnValueLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelDecimalBox)d;

        ValidateValue(e, control, raiseEvents: true);
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
    private static void ValidateValue(
        DependencyPropertyChangedEventArgs e,
        LabelDecimalBox control,
        bool raiseEvents)
    {
        if (e.NewValue is not decimal newValue)
        {
            control.ValidationText = Validations.ValueShouldBeADecimal;
            return;
        }

        if (e.OldValue is not decimal oldValue)
        {
            return;
        }

        if (raiseEvents)
        {
            control.ValueChanged?.Invoke(
                control,
                new ChangedDecimalEventArgs(
                    control.Identifier,
                    oldValue,
                    newValue));
        }
    }
}