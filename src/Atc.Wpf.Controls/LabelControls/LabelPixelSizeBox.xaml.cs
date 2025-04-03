namespace Atc.Wpf.Controls.LabelControls;

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
            0,
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
            0,
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

    public LabelPixelSizeBox()
    {
        InitializeComponent();
    }

    public override bool IsValid()
    {
        return string.IsNullOrEmpty(ValidationText);
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

    private static void OnValueWidthLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelPixelSizeBox)d;

        if (e.NewValue is not int newValue)
        {
            control.ValidationText = Validations.ValueShouldBeAInteger;
            return;
        }

        if (e.OldValue is not int oldValue)
        {
            return;
        }

        control.ValueWidthLostFocus?.Invoke(
            control,
            new ValueChangedEventArgs<int?>(
                ControlHelper.GetIdentifier(control),
                oldValue,
                newValue));
    }

    private static void OnValueHeightLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelPixelSizeBox)d;

        if (e.NewValue is not int newValue)
        {
            control.ValidationText = Validations.ValueShouldBeAInteger;
            return;
        }

        if (e.OldValue is not int oldValue)
        {
            return;
        }

        control.ValueHeightLostFocus?.Invoke(
            control,
            new ValueChangedEventArgs<int?>(
                ControlHelper.GetIdentifier(control),
                oldValue,
                newValue));
    }
}