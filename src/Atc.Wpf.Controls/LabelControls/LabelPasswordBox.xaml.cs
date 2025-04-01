namespace Atc.Wpf.Controls.LabelControls;

public partial class LabelPasswordBox : ILabelPasswordBox
{
    public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(TextChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedPropertyChangedEventHandler<string>),
        typeof(LabelPasswordBox));

    public event RoutedPropertyChangedEventHandler<string> TextChanged
    {
        add => AddHandler(TextChangedEvent, value);
        remove => RemoveHandler(TextChangedEvent, value);
    }

    public static readonly DependencyProperty WatermarkTextProperty = DependencyProperty.Register(
        nameof(WatermarkText),
        typeof(string),
        typeof(LabelPasswordBox),
        new PropertyMetadata(defaultValue: string.Empty));

    public string WatermarkText
    {
        get => (string)GetValue(WatermarkTextProperty);
        set => SetValue(WatermarkTextProperty, value);
    }

    public static readonly DependencyProperty WatermarkAlignmentProperty = DependencyProperty.Register(
        nameof(WatermarkAlignment),
        typeof(TextAlignment),
        typeof(LabelPasswordBox),
        new PropertyMetadata(default(TextAlignment)));

    public TextAlignment WatermarkAlignment
    {
        get => (TextAlignment)GetValue(WatermarkAlignmentProperty);
        set => SetValue(WatermarkAlignmentProperty, value);
    }

    public static readonly DependencyProperty WatermarkTrimmingProperty = DependencyProperty.Register(
        nameof(WatermarkTrimming),
        typeof(TextTrimming),
        typeof(LabelPasswordBox),
        new PropertyMetadata(default(TextTrimming)));

    public TextTrimming WatermarkTrimming
    {
        get => (TextTrimming)GetValue(WatermarkTrimmingProperty);
        set => SetValue(WatermarkTrimmingProperty, value);
    }

    public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register(
        nameof(MaxLength),
        typeof(uint),
        typeof(LabelPasswordBox),
        new PropertyMetadata(100U));

    public uint MaxLength
    {
        get => (uint)GetValue(MaxLengthProperty);
        set => SetValue(MaxLengthProperty, value);
    }

    public static readonly DependencyProperty MinLengthProperty = DependencyProperty.Register(
        nameof(MinLength),
        typeof(uint),
        typeof(LabelPasswordBox),
        new PropertyMetadata(0U));

    public uint MinLength
    {
        get => (uint)GetValue(MinLengthProperty);
        set => SetValue(MinLengthProperty, value);
    }

    public static readonly DependencyProperty ShowClearTextButtonProperty = DependencyProperty.Register(
        nameof(ShowClearTextButton),
        typeof(bool),
        typeof(LabelPasswordBox),
        new PropertyMetadata(defaultValue: BooleanBoxes.TrueBox));

    public bool ShowClearTextButton
    {
        get => (bool)GetValue(ShowClearTextButtonProperty);
        set => SetValue(ShowClearTextButtonProperty, value);
    }

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(LabelPasswordBox),
        new FrameworkPropertyMetadata(
            string.Empty,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            OnTextLostFocus,
            CoerceText,
            isAnimationProhibited: true,
            UpdateSourceTrigger.LostFocus));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public event EventHandler<ValueChangedEventArgs<string?>>? TextLostFocusValid;

    public event EventHandler<ValueChangedEventArgs<string?>>? TextLostFocusInvalid;

    public LabelPasswordBox()
    {
        InitializeComponent();
    }

    public override bool IsValid()
    {
        ValidateText(default, this, raiseEvents: false);
        return string.IsNullOrEmpty(ValidationText);
    }

    private static object CoerceText(
        DependencyObject d,
        object? value)
        => value ?? string.Empty;

    private void OnTextChanged(
        object sender,
        RoutedEventArgs e)
    {
        var control = (PasswordBox)sender;

        Text = control.Password;

        RaiseEvent(new RoutedPropertyChangedEventArgs<string>(string.Empty, control.Password, TextChangedEvent));
    }

    private static void OnTextLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelPasswordBox)d;

        ValidateText(e, control, raiseEvents: true);
    }

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
    [SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "OK.")]
    private static void ValidateText(
        DependencyPropertyChangedEventArgs e,
        LabelPasswordBox control,
        bool raiseEvents)
    {
        if (control.IsMandatory &&
            string.IsNullOrWhiteSpace(control.Text))
        {
            control.ValidationText = Validations.FieldIsRequired;
            if (raiseEvents)
            {
                OnTextLostFocusFireInvalidEvent(control, e);
            }

            return;
        }

        if (control.Text.Length < control.MinLength)
        {
            control.ValidationText = string.Format(
                CultureInfo.CurrentUICulture,
                Validations.MinValueFormat1,
                control.MinLength);

            if (raiseEvents)
            {
                OnTextLostFocusFireInvalidEvent(control, e);
            }

            return;
        }

        if (control.Text.Length > control.MaxLength)
        {
            control.ValidationText = string.Format(
                CultureInfo.CurrentUICulture,
                Validations.MaxValueFormat1,
                control.MaxLength);

            if (raiseEvents)
            {
                OnTextLostFocusFireInvalidEvent(control, e);
            }

            return;
        }

        control.ValidationText = string.Empty;
        if (raiseEvents)
        {
            OnTextLostFocusFireValidEvent(control, e);
        }
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
    private static void OnTextLostFocusFireValidEvent(
        LabelPasswordBox control,
        DependencyPropertyChangedEventArgs e)
    {
        var newValue = e.NewValue is null
            ? string.Empty
            : e.NewValue.ToString();

        control.TextLostFocusValid?.Invoke(
            control,
            new ValueChangedEventArgs<string?>(
                ControlHelper.GetIdentifier(control),
                oldValue: null,
                newValue));
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
    private static void OnTextLostFocusFireInvalidEvent(
        LabelPasswordBox control,
        DependencyPropertyChangedEventArgs e)
    {
        var oldValue = e.OldValue is null
            ? string.Empty
            : e.OldValue.ToString();

        var newValue = e.NewValue is null
            ? string.Empty
            : e.NewValue.ToString();

        control.TextLostFocusInvalid?.Invoke(
            control,
            new ValueChangedEventArgs<string?>(
                ControlHelper.GetIdentifier(control),
                oldValue,
                newValue));
    }
}