namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelDecimalXyBox.
/// </summary>
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

    public event EventHandler<ChangedDecimalEventArgs>? ValueXLostFocus;

    public event EventHandler<ChangedDecimalEventArgs>? ValueYLostFocus;

    public LabelDecimalXyBox()
    {
        InitializeComponent();
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

        control.ValueXLostFocus?.Invoke(
            control,
            new ChangedDecimalEventArgs(
                ControlHelper.GetIdentifier(control),
                (decimal)e.OldValue,
                (decimal)e.NewValue));
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
    private static void OnValueYLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelDecimalXyBox)d;

        control.ValueYLostFocus?.Invoke(
            control,
            new ChangedDecimalEventArgs(
                ControlHelper.GetIdentifier(control),
                (decimal)e.OldValue,
                (decimal)e.NewValue));
    }
}