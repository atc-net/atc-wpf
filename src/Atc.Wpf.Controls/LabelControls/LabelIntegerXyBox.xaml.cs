namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelIntegerXyBox.
/// </summary>
public partial class LabelIntegerXyBox : ILabelIntegerXyBox
{
    public static readonly RoutedEvent ValueXChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(ValueXChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedPropertyChangedEventHandler<int>),
        typeof(LabelIntegerXyBox));

    public event RoutedPropertyChangedEventHandler<int> ValueXChanged
    {
        add => AddHandler(ValueXChangedEvent, value);
        remove => RemoveHandler(ValueXChangedEvent, value);
    }

    public static readonly RoutedEvent ValueYChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(ValueYChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedPropertyChangedEventHandler<int>),
        typeof(LabelIntegerXyBox));

    public event RoutedPropertyChangedEventHandler<int> ValueYChanged
    {
        add => AddHandler(ValueYChangedEvent, value);
        remove => RemoveHandler(ValueYChangedEvent, value);
    }

    public static readonly DependencyProperty PrefixTextXProperty = DependencyProperty.Register(
        nameof(PrefixTextX),
        typeof(string),
        typeof(LabelIntegerXyBox),
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
        typeof(LabelIntegerXyBox),
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
        typeof(LabelIntegerXyBox),
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
        typeof(int),
        typeof(LabelIntegerXyBox),
        new FrameworkPropertyMetadata(
            default(int),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            OnValueXLostFocus,
            coerceValueCallback: null,
            isAnimationProhibited: true,
            UpdateSourceTrigger.LostFocus));

    public int ValueX
    {
        get => (int)GetValue(ValueXProperty);
        set => SetValue(ValueXProperty, value);
    }

    public static readonly DependencyProperty ValueYProperty = DependencyProperty.Register(
        nameof(ValueY),
        typeof(int),
        typeof(LabelIntegerXyBox),
        new FrameworkPropertyMetadata(
            default(int),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            OnValueYLostFocus,
            coerceValueCallback: null,
            isAnimationProhibited: true,
            UpdateSourceTrigger.LostFocus));

    public int ValueY
    {
        get => (int)GetValue(ValueYProperty);
        set => SetValue(ValueYProperty, value);
    }

    public event EventHandler<ChangedIntegerEventArgs>? ValueXLostFocus;

    public event EventHandler<ChangedIntegerEventArgs>? ValueYLostFocus;

    public LabelIntegerXyBox()
    {
        InitializeComponent();
    }

    private void OnValueXChanged(
        object sender,
        RoutedPropertyChangedEventArgs<int> e)
    {
        RaiseEvent(new RoutedPropertyChangedEventArgs<int>(e.OldValue, e.NewValue, ValueXChangedEvent));
    }

    private void OnValueYChanged(
        object sender,
        RoutedPropertyChangedEventArgs<int> e)
    {
        RaiseEvent(new RoutedPropertyChangedEventArgs<int>(e.OldValue, e.NewValue, ValueYChangedEvent));
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
    private static void OnValueXLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelIntegerXyBox)d;

        control.ValueXLostFocus?.Invoke(
            control,
            new ChangedIntegerEventArgs(
                ControlHelper.GetIdentifier(control),
                (int)e.OldValue,
                (int)e.NewValue));
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
    private static void OnValueYLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelIntegerXyBox)d;

        control.ValueYLostFocus?.Invoke(
            control,
            new ChangedIntegerEventArgs(
                ControlHelper.GetIdentifier(control),
                (int)e.OldValue,
                (int)e.NewValue));
    }
}