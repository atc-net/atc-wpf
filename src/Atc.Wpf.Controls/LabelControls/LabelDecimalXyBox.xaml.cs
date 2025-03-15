// ReSharper disable PreferConcreteValueOverDefault
namespace Atc.Wpf.Controls.LabelControls;

public partial class LabelDecimalXyBox : ILabelDecimalXyBox
{
    public static readonly RoutedEvent ValueXChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(ValueXChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedPropertyChangedEventHandler<decimal>),
        typeof(LabelDecimalXyBox));

    public event RoutedPropertyChangedEventHandler<decimal> ValueXChanged
    {
        add => AddHandler(ValueXChangedEvent, value);
        remove => RemoveHandler(ValueXChangedEvent, value);
    }

    public static readonly RoutedEvent ValueYChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(ValueYChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedPropertyChangedEventHandler<decimal>),
        typeof(LabelDecimalXyBox));

    public event RoutedPropertyChangedEventHandler<decimal> ValueYChanged
    {
        add => AddHandler(ValueYChangedEvent, value);
        remove => RemoveHandler(ValueYChangedEvent, value);
    }

    public static readonly DependencyProperty PrefixTextXProperty = DependencyProperty.Register(
        nameof(PrefixTextX),
        typeof(string),
        typeof(LabelDecimalXyBox),
        new FrameworkPropertyMetadata(
            string.Empty,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

    public string PrefixTextX
    {
        get => (string)GetValue(PrefixTextXProperty);
        set => SetValue(PrefixTextXProperty, value);
    }

    public static readonly DependencyProperty PrefixTextYProperty = DependencyProperty.Register(
        nameof(PrefixTextY),
        typeof(string),
        typeof(LabelDecimalXyBox),
        new FrameworkPropertyMetadata(
            string.Empty,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

    public string PrefixTextY
    {
        get => (string)GetValue(PrefixTextYProperty);
        set => SetValue(PrefixTextYProperty, value);
    }

    public static readonly DependencyProperty SuffixTextProperty = DependencyProperty.Register(
        nameof(SuffixText),
        typeof(string),
        typeof(LabelDecimalXyBox),
        new FrameworkPropertyMetadata(
            string.Empty,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

    public string SuffixText
    {
        get => (string)GetValue(SuffixTextProperty);
        set => SetValue(SuffixTextProperty, value);
    }

    public static readonly DependencyProperty ValueXProperty = DependencyProperty.Register(
        nameof(ValueX),
        typeof(decimal),
        typeof(LabelDecimalXyBox),
        new FrameworkPropertyMetadata(
            default(decimal),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            OnValueXLostFocus,
            coerceValueCallback: null,
            isAnimationProhibited: true,
            UpdateSourceTrigger.LostFocus));

    public decimal ValueX
    {
        get => (decimal)GetValue(ValueXProperty);
        set => SetValue(ValueXProperty, value);
    }

    public static readonly DependencyProperty ValueYProperty = DependencyProperty.Register(
        nameof(ValueY),
        typeof(decimal),
        typeof(LabelDecimalXyBox),
        new FrameworkPropertyMetadata(
            default(decimal),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            OnValueYLostFocus,
            coerceValueCallback: null,
            isAnimationProhibited: true,
            UpdateSourceTrigger.LostFocus));

    public decimal ValueY
    {
        get => (decimal)GetValue(ValueYProperty);
        set => SetValue(ValueYProperty, value);
    }

    public event EventHandler<ValueChangedEventArgs<decimal?>>? ValueXLostFocus;

    public event EventHandler<ValueChangedEventArgs<decimal?>>? ValueYLostFocus;

    public LabelDecimalXyBox()
    {
        InitializeComponent();
    }

    public override bool IsValid()
    {
        return string.IsNullOrEmpty(ValidationText);
    }

    private void OnValueXChanged(
        object sender,
        RoutedPropertyChangedEventArgs<decimal> e)
    {
        RaiseEvent(new RoutedPropertyChangedEventArgs<decimal>(e.OldValue, e.NewValue, ValueXChangedEvent));
    }

    private void OnValueYChanged(
        object sender,
        RoutedPropertyChangedEventArgs<decimal> e)
    {
        RaiseEvent(new RoutedPropertyChangedEventArgs<decimal>(e.OldValue, e.NewValue, ValueYChangedEvent));
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
    private static void OnValueXLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelDecimalXyBox)d;

        if (e.NewValue is not decimal newValue)
        {
            control.ValidationText = Validations.ValueShouldBeADecimal;
            return;
        }

        if (e.OldValue is not decimal oldValue)
        {
            return;
        }

        control.ValueXLostFocus?.Invoke(
            control,
            new ValueChangedEventArgs<decimal?>(
                ControlHelper.GetIdentifier(control),
                oldValue,
                newValue));
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
    private static void OnValueYLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelDecimalXyBox)d;

        if (e.NewValue is not decimal newValue)
        {
            control.ValidationText = Validations.ValueShouldBeADecimal;
            return;
        }

        if (e.OldValue is not decimal oldValue)
        {
            return;
        }

        control.ValueYLostFocus?.Invoke(
            control,
            new ValueChangedEventArgs<decimal?>(
                ControlHelper.GetIdentifier(control),
                oldValue,
                newValue));
    }
}