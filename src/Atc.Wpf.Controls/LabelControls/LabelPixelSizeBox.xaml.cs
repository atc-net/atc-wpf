namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelPixelSizeBox.
/// </summary>
public partial class LabelPixelSizeBox : ILabelPixelSizeBox
{
    public static readonly RoutedEvent ValueWidthChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(ValueWidthChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedPropertyChangedEventHandler<int>),
        typeof(LabelPixelSizeBox));

    public event RoutedPropertyChangedEventHandler<int> ValueWidthChanged
    {
        add => AddHandler(ValueWidthChangedEvent, value);
        remove => RemoveHandler(ValueWidthChangedEvent, value);
    }

    public static readonly RoutedEvent ValueHeightChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(ValueHeightChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedPropertyChangedEventHandler<int>),
        typeof(LabelPixelSizeBox));

    public event RoutedPropertyChangedEventHandler<int> ValueHeightChanged
    {
        add => AddHandler(ValueHeightChangedEvent, value);
        remove => RemoveHandler(ValueHeightChangedEvent, value);
    }

    public static readonly DependencyProperty ValueWidthProperty = DependencyProperty.Register(
        nameof(ValueWidth),
        typeof(int),
        typeof(LabelPixelSizeBox),
        new FrameworkPropertyMetadata(
            default(int),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            OnValueWidthLostFocus,
            coerceValueCallback: null,
            isAnimationProhibited: true,
            UpdateSourceTrigger.LostFocus));

    public int ValueWidth
    {
        get => (int)GetValue(ValueWidthProperty);
        set => SetValue(ValueWidthProperty, value);
    }

    public static readonly DependencyProperty ValueHeightProperty = DependencyProperty.Register(
        nameof(ValueHeight),
        typeof(int),
        typeof(LabelPixelSizeBox),
        new FrameworkPropertyMetadata(
            default(int),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            OnValueHeightLostFocus,
            coerceValueCallback: null,
            isAnimationProhibited: true,
            UpdateSourceTrigger.LostFocus));

    public int ValueHeight
    {
        get => (int)GetValue(ValueHeightProperty);
        set => SetValue(ValueHeightProperty, value);
    }

    public event EventHandler<ChangedIntegerEventArgs>? ValueWidthLostFocus;

    public event EventHandler<ChangedIntegerEventArgs>? ValueHeightLostFocus;

    public LabelPixelSizeBox()
    {
        InitializeComponent();
    }

    private void OnValueWidthChanged(
        object sender,
        RoutedPropertyChangedEventArgs<int> e)
    {
        RaiseEvent(new RoutedPropertyChangedEventArgs<int>(e.OldValue, e.NewValue, ValueWidthChangedEvent));
    }

    private void OnValueHeightChanged(
        object sender,
        RoutedPropertyChangedEventArgs<int> e)
    {
        RaiseEvent(new RoutedPropertyChangedEventArgs<int>(e.OldValue, e.NewValue, ValueHeightChangedEvent));
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
    private static void OnValueWidthLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelPixelSizeBox)d;

        control.ValueWidthLostFocus?.Invoke(
            control,
            new ChangedIntegerEventArgs(
                ControlHelper.GetIdentifier(control),
                (int)e.OldValue,
                (int)e.NewValue));
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
    private static void OnValueHeightLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelPixelSizeBox)d;

        control.ValueHeightLostFocus?.Invoke(
            control,
            new ChangedIntegerEventArgs(
                ControlHelper.GetIdentifier(control),
                (int)e.OldValue,
                (int)e.NewValue));
    }
}