namespace Atc.Wpf.Controls.LabelControls;

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
            0,
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
            0,
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

    public LabelIntegerXyBox()
    {
        InitializeComponent();
    }

    public override bool IsValid()
    {
        return string.IsNullOrEmpty(ValidationText);
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

    private static void OnValueXLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelIntegerXyBox)d;

        if (e.NewValue is not int newValue)
        {
            control.ValidationText = Validations.ValueShouldBeAInteger;
            return;
        }

        if (e.OldValue is not int oldValue)
        {
            return;
        }

        control.ValueXLostFocus?.Invoke(
            control,
            new ValueChangedEventArgs<int?>(
                ControlHelper.GetIdentifier(control),
                oldValue,
                newValue));
    }

    private static void OnValueYLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelIntegerXyBox)d;

        if (e.NewValue is not int newValue)
        {
            control.ValidationText = Validations.ValueShouldBeAInteger;
            return;
        }

        if (e.OldValue is not int oldValue)
        {
            return;
        }

        control.ValueYLostFocus?.Invoke(
            control,
            new ValueChangedEventArgs<int?>(
                ControlHelper.GetIdentifier(control),
                oldValue,
                newValue));
    }
}