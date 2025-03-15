namespace Atc.Wpf.Controls.LabelControls;

public partial class LabelTimePicker : ILabelTimePicker
{
    public static readonly DependencyProperty SelectedTimeProperty = DependencyProperty.Register(
        nameof(SelectedTime),
        typeof(DateTime?),
        typeof(LabelTimePicker),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            OnSelectedTimeChanged));

    public DateTime? SelectedTime
    {
        get => (DateTime?)GetValue(SelectedTimeProperty);
        set => SetValue(SelectedTimeProperty, value);
    }

    public static readonly DependencyProperty CustomCultureProperty = DependencyProperty.Register(
        nameof(CustomCulture),
        typeof(CultureInfo),
        typeof(LabelTimePicker),
        new PropertyMetadata(default(CultureInfo?)));

    public CultureInfo? CustomCulture
    {
        get => (CultureInfo?)GetValue(CustomCultureProperty);
        set => SetValue(CustomCultureProperty, value);
    }

    public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(TextChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedPropertyChangedEventHandler<string>),
        typeof(LabelTimePicker));

    public event RoutedPropertyChangedEventHandler<string> TextChanged
    {
        add => AddHandler(TextChangedEvent, value);
        remove => RemoveHandler(TextChangedEvent, value);
    }

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(LabelTimePicker),
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

    public static readonly DependencyProperty WatermarkTextProperty = DependencyProperty.Register(
        nameof(WatermarkText),
        typeof(string),
        typeof(LabelTimePicker),
        new PropertyMetadata(defaultValue: string.Empty));

    public string WatermarkText
    {
        get => (string)GetValue(WatermarkTextProperty);
        set => SetValue(WatermarkTextProperty, value);
    }

    public static readonly DependencyProperty WatermarkAlignmentProperty = DependencyProperty.Register(
        nameof(WatermarkAlignment),
        typeof(TextAlignment),
        typeof(LabelTimePicker),
        new PropertyMetadata(default(TextAlignment)));

    public TextAlignment WatermarkAlignment
    {
        get => (TextAlignment)GetValue(WatermarkAlignmentProperty);
        set => SetValue(WatermarkAlignmentProperty, value);
    }

    public static readonly DependencyProperty WatermarkTrimmingProperty = DependencyProperty.Register(
        nameof(WatermarkTrimming),
        typeof(TextTrimming),
        typeof(LabelTimePicker),
        new PropertyMetadata(default(TextTrimming)));

    public TextTrimming WatermarkTrimming
    {
        get => (TextTrimming)GetValue(WatermarkTrimmingProperty);
        set => SetValue(WatermarkTrimmingProperty, value);
    }

    public static readonly DependencyProperty OpenClockProperty = DependencyProperty.Register(
        nameof(OpenClock),
        typeof(bool),
        typeof(LabelTimePicker),
        new PropertyMetadata(default(bool)));

    public bool OpenClock
    {
        get => (bool)GetValue(OpenClockProperty);
        set => SetValue(OpenClockProperty, value);
    }

    public event EventHandler<ValueChangedEventArgs<DateTime?>>? LostFocusValid;

    public event EventHandler<ValueChangedEventArgs<DateTime?>>? LostFocusInvalid;

    public LabelTimePicker()
    {
        InitializeComponent();

        Loaded += OnLoaded;
        CultureManager.UiCultureChanged += OnUiCultureChanged;
    }

    private void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        var cultureInfo = CustomCulture ?? Thread.CurrentThread.CurrentUICulture;

        SetDefaultWatermarkIfNeeded(cultureInfo);
    }

    private void OnUiCultureChanged(
        object? sender,
        UiCultureEventArgs e)
    {
        var cultureInfo = CustomCulture ?? Thread.CurrentThread.CurrentUICulture;

        SetDefaultWatermarkIfNeeded(cultureInfo);

        if (SelectedTime.HasValue)
        {
            SetCurrentValue(
                TextProperty,
                SelectedTime.Value.ToShortTimeStringUsingSpecificCulture(cultureInfo));
        }
    }

    private void OnTimeTextKeyDown(
        object sender,
        KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        if (sender is not DependencyObject parent)
        {
            return;
        }

        var control = VisualTreeHelperEx.FindParent<LabelTimePicker>(parent);
        if (control is null ||
            sender is not TextBox textBox)
        {
            return;
        }

        control.SetCurrentValue(TextProperty, textBox.Text);

        var dateTime = ValidateText(control);

        UpdateSelectedTime(control, dateTime);
    }

    private static void OnTextLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelTimePicker)d;

        if (StackTraceHelper.ContainsPropertyName(nameof(OnSelectedTimeChanged)))
        {
            return;
        }

        if (StackTraceHelper.ContainsPropertyName("ClearControl"))
        {
            control.Text = string.Empty;
        }

        var dateTime = ValidateText(e, control, raiseEvents: true);

        UpdateSelectedTime(control, dateTime);
    }

    private void OnClockPanelPickerSelectedClockChanged(
        object? sender,
        RoutedEventArgs e)
    {
        var clockPanelPicker = (ClockPanelPicker)sender!;

        if (!clockPanelPicker.SelectedDateTime.HasValue)
        {
            return;
        }

        var cultureInfo = CustomCulture ?? Thread.CurrentThread.CurrentUICulture;
        var shortTime = clockPanelPicker.SelectedDateTime.Value.ToShortTimeStringUsingSpecificCulture(cultureInfo);

        if (shortTime.Equals(Text, StringComparison.Ordinal))
        {
            return;
        }

        SetCurrentValue(TextProperty, shortTime);
    }

    private static void OnSelectedTimeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelTimePicker)d;

        if (control.SelectedTime is null)
        {
            control.SetCurrentValue(TextProperty, string.Empty);
        }
        else
        {
            var cultureInfo = control.CustomCulture ?? Thread.CurrentThread.CurrentUICulture;

            control.SetCurrentValue(
                TextProperty,
                control.SelectedTime.Value.ToShortTimeStringUsingSpecificCulture(cultureInfo));
        }
    }

    private static void UpdateSelectedTime(
        LabelTimePicker control,
        DateTime? dateTime)
    {
        if (control.SelectedTime == dateTime)
        {
            return;
        }

        if (dateTime is not null &&
            string.IsNullOrEmpty(control.ValidationText))
        {
            control.SelectedTime = dateTime;
        }
        else
        {
            control.SelectedTime = null;
        }
    }

    private static DateTime? ValidateText(
        DependencyPropertyChangedEventArgs e,
        LabelTimePicker control,
        bool raiseEvents)
    {
        var dateTime = ValidateText(control);

        if (raiseEvents)
        {
            if (string.IsNullOrEmpty(control.ValidationText))
            {
                OnLostFocusFireValidEvent(control, e);
            }
            else
            {
                OnLostFocusFireInvalidEvent(control, e);
            }
        }

        return dateTime;
    }

    private static DateTime? ValidateText(
        LabelTimePicker control)
    {
        var isTimeEmpty = string.IsNullOrWhiteSpace(control.Text);

        if (control.IsMandatory)
        {
            if (isTimeEmpty)
            {
                control.ValidationText = Validations.FieldIsRequired;
                return null;
            }
        }
        else
        {
            if (isTimeEmpty)
            {
                control.ValidationText = string.Empty;
                return null;
            }
        }

        var cultureInfo = control.CustomCulture ?? Thread.CurrentThread.CurrentUICulture;

        if (DateTimeHelper.TryParseShortTimeUsingSpecificCulture(
                control.Text,
                cultureInfo,
                out var dateTime))
        {
            control.ValidationText = string.Empty;
            return dateTime;
        }

        control.ValidationText = Validations.InvalidTime;

        return null;
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
    private static void OnLostFocusFireValidEvent(
        LabelTimePicker control,
        DependencyPropertyChangedEventArgs e)
    {
        var cultureInfo = control.CustomCulture ?? Thread.CurrentThread.CurrentUICulture;

        DateTime? oldValue = null;
        if (e.OldValue is not null &&
            DateTimeHelper.TryParseShortTimeUsingSpecificCulture(
                e.OldValue.ToString()!,
                cultureInfo,
                out var resultOld))
        {
            oldValue = resultOld;
        }

        DateTime? newValue = null;
        if (e.NewValue is not null &&
            DateTimeHelper.TryParseShortTimeUsingSpecificCulture(
                e.NewValue.ToString()!,
                cultureInfo,
                out var resultNew))
        {
            newValue = resultNew;
        }

        control.LostFocusValid?.Invoke(
            control,
            new ValueChangedEventArgs<DateTime?>(
                ControlHelper.GetIdentifier(control),
                oldValue,
                newValue));
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
    private static void OnLostFocusFireInvalidEvent(
        LabelTimePicker control,
        DependencyPropertyChangedEventArgs e)
    {
        var cultureInfo = control.CustomCulture ?? Thread.CurrentThread.CurrentUICulture;

        DateTime? oldValue = null;
        if (e.OldValue is not null &&
            DateTimeHelper.TryParseShortTimeUsingSpecificCulture(
                e.OldValue.ToString()!,
                cultureInfo,
                out var resultOld))
        {
            oldValue = resultOld;
        }

        DateTime? newValue = null;
        if (e.NewValue is not null &&
            DateTimeHelper.TryParseShortTimeUsingSpecificCulture(
                e.NewValue.ToString()!,
                cultureInfo,
                out var resultNew))
        {
            newValue = resultNew;
        }

        control.LostFocusInvalid?.Invoke(
            control,
            new ValueChangedEventArgs<DateTime?>(
                ControlHelper.GetIdentifier(control),
                oldValue,
                newValue));
    }

    private static object CoerceText(
        DependencyObject d,
        object? value)
        => value ?? string.Empty;

    private void SetDefaultWatermarkIfNeeded(
        CultureInfo cultureInfo)
    {
        if (string.IsNullOrEmpty(WatermarkText) ||
            WatermarkText.Contains(':', StringComparison.Ordinal) ||
            WatermarkText.Contains('.', StringComparison.Ordinal))
        {
            WatermarkText = cultureInfo.DateTimeFormat.ShortTimePattern;
        }
    }
}