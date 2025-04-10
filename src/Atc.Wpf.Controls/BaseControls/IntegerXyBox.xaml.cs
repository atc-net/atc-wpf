namespace Atc.Wpf.Controls.BaseControls;

public partial class IntegerXyBox
{
    public static readonly RoutedEvent ValueXChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(ValueXChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedPropertyChangedEventHandler<int>),
        typeof(IntegerXyBox));

    public event RoutedPropertyChangedEventHandler<int> ValueXChanged
    {
        add => AddHandler(ValueXChangedEvent, value);
        remove => RemoveHandler(ValueXChangedEvent, value);
    }

    public static readonly RoutedEvent ValueYChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(ValueYChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedPropertyChangedEventHandler<int>),
        typeof(IntegerXyBox));

    public event RoutedPropertyChangedEventHandler<int> ValueYChanged
    {
        add => AddHandler(ValueYChangedEvent, value);
        remove => RemoveHandler(ValueYChangedEvent, value);
    }

    public static readonly DependencyProperty HideUpDownButtonsProperty = DependencyProperty.Register(
        nameof(HideUpDownButtons),
        typeof(bool),
        typeof(IntegerXyBox),
        new PropertyMetadata(BooleanBoxes.FalseBox));

    public bool HideUpDownButtons
    {
        get => (bool)GetValue(HideUpDownButtonsProperty);
        set => SetValue(HideUpDownButtonsProperty, value);
    }

    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
        nameof(Maximum),
        typeof(int),
        typeof(IntegerXyBox),
        new FrameworkPropertyMetadata(
            int.MaxValue,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

    public int Maximum
    {
        get => (int)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
        nameof(Minimum),
        typeof(int),
        typeof(IntegerXyBox),
        new FrameworkPropertyMetadata(
            int.MinValue,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

    public int Minimum
    {
        get => (int)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public static readonly DependencyProperty PrefixTextXProperty = DependencyProperty.Register(
        nameof(PrefixTextX),
        typeof(string),
        typeof(IntegerXyBox),
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
        typeof(IntegerXyBox),
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
        typeof(IntegerXyBox),
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
        typeof(IntegerXyBox),
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
        typeof(IntegerXyBox),
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

    public event EventHandler<ValueChangedEventArgs<int?>>? ValueXLostFocus;

    public event EventHandler<ValueChangedEventArgs<int?>>? ValueYLostFocus;

    public IntegerXyBox()
    {
        InitializeComponent();
    }

    private void OnValueXChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double?> e)
    {
        if (e.OldValue is null || e.NewValue is null)
        {
            return;
        }

        RaiseEvent(new RoutedPropertyChangedEventArgs<int>((int)e.OldValue, (int)e.NewValue, ValueXChangedEvent));
    }

    private void OnValueYChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double?> e)
    {
        if (e.OldValue is null || e.NewValue is null)
        {
            return;
        }

        RaiseEvent(new RoutedPropertyChangedEventArgs<int>((int)e.OldValue, (int)e.NewValue, ValueYChangedEvent));
    }

    private static void OnValueXLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (IntegerXyBox)d;

        control.ValueXLostFocus?.Invoke(
            control,
            new ValueChangedEventArgs<int?>(
                ControlHelper.GetIdentifier(control),
                (int)e.OldValue,
                (int)e.NewValue));
    }

    private static void OnValueYLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (IntegerXyBox)d;

        control.ValueYLostFocus?.Invoke(
            control,
            new ValueChangedEventArgs<int?>(
                ControlHelper.GetIdentifier(control),
                (int)e.OldValue,
                (int)e.NewValue));
    }
}