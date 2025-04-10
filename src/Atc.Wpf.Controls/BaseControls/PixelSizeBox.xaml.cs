namespace Atc.Wpf.Controls.BaseControls;

public partial class PixelSizeBox
{
    public static readonly RoutedEvent ValueWidthChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(ValueWidthChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedPropertyChangedEventHandler<int>),
        typeof(PixelSizeBox));

    public event RoutedPropertyChangedEventHandler<int> ValueWidthChanged
    {
        add => AddHandler(ValueWidthChangedEvent, value);
        remove => RemoveHandler(ValueWidthChangedEvent, value);
    }

    public static readonly RoutedEvent ValueHeightChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(ValueHeightChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedPropertyChangedEventHandler<int>),
        typeof(PixelSizeBox));

    public event RoutedPropertyChangedEventHandler<int> ValueHeightChanged
    {
        add => AddHandler(ValueHeightChangedEvent, value);
        remove => RemoveHandler(ValueHeightChangedEvent, value);
    }

    public static readonly DependencyProperty HideUpDownButtonsProperty = DependencyProperty.Register(
        nameof(HideUpDownButtons),
        typeof(bool),
        typeof(PixelSizeBox),
        new PropertyMetadata(BooleanBoxes.FalseBox));

    public bool HideUpDownButtons
    {
        get => (bool)GetValue(HideUpDownButtonsProperty);
        set => SetValue(HideUpDownButtonsProperty, value);
    }

    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
        nameof(Maximum),
        typeof(int),
        typeof(PixelSizeBox),
        new FrameworkPropertyMetadata(
            int.MaxValue,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

    public int Maximum
    {
        get => (int)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public static readonly DependencyProperty ValueWidthProperty = DependencyProperty.Register(
        nameof(ValueWidth),
        typeof(int),
        typeof(PixelSizeBox),
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
        typeof(PixelSizeBox),
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

    public event EventHandler<ValueChangedEventArgs<int?>>? ValueWidthLostFocus;

    public event EventHandler<ValueChangedEventArgs<int?>>? ValueHeightLostFocus;

    public PixelSizeBox()
    {
        InitializeComponent();
    }

    private void OnValueWidthChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double?> e)
    {
        if (e.OldValue is null || e.NewValue is null)
        {
            return;
        }

        RaiseEvent(new RoutedPropertyChangedEventArgs<int>((int)e.OldValue, (int)e.NewValue, ValueWidthChangedEvent));
    }

    private void OnValueHeightChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double?> e)
    {
        if (e.OldValue is null || e.NewValue is null)
        {
            return;
        }

        RaiseEvent(new RoutedPropertyChangedEventArgs<int>((int)e.OldValue, (int)e.NewValue, ValueHeightChangedEvent));
    }

    private static void OnValueWidthLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (PixelSizeBox)d;

        control.ValueWidthLostFocus?.Invoke(
            control,
            new ValueChangedEventArgs<int?>(
                ControlHelper.GetIdentifier(control),
                (int)e.OldValue,
                (int)e.NewValue));
    }

    private static void OnValueHeightLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (PixelSizeBox)d;

        control.ValueHeightLostFocus?.Invoke(
            control,
            new ValueChangedEventArgs<int?>(
                ControlHelper.GetIdentifier(control),
                (int)e.OldValue,
                (int)e.NewValue));
    }
}