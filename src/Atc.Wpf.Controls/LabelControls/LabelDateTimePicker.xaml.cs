// ReSharper disable InvertIf
// ReSharper disable ConvertIfStatementToSwitchStatement
namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelDateTimePicker.
/// </summary>
public partial class LabelDateTimePicker : ILabelDateTimePicker
{
    public static readonly DependencyProperty DisplayDateProperty = DependencyProperty.Register(
        nameof(DisplayDate),
        typeof(DateTime),
        typeof(LabelDateTimePicker),
        new PropertyMetadata(defaultValue: DateTime.Now));

    public DateTime DisplayDate
    {
        get => (DateTime)GetValue(DisplayDateProperty);
        set => SetValue(DisplayDateProperty, value);
    }

    public static readonly DependencyProperty DisplayDateStartProperty = DependencyProperty.Register(
        nameof(DisplayDateStart),
        typeof(DateTime?),
        typeof(LabelDateTimePicker),
        new PropertyMetadata(default(DateTime?)));

    public DateTime? DisplayDateStart
    {
        get => (DateTime?)GetValue(DisplayDateStartProperty);
        set => SetValue(DisplayDateStartProperty, value);
    }

    public static readonly DependencyProperty DisplayDateEndProperty = DependencyProperty.Register(
        nameof(DisplayDateEnd),
        typeof(DateTime?),
        typeof(LabelDateTimePicker),
        new PropertyMetadata(default(DateTime?)));

    public DateTime? DisplayDateEnd
    {
        get => (DateTime?)GetValue(DisplayDateEndProperty);
        set => SetValue(DisplayDateEndProperty, value);
    }

    public static readonly DependencyProperty FirstDayOfWeekProperty = DependencyProperty.Register(
        nameof(FirstDayOfWeek),
        typeof(DayOfWeek),
        typeof(LabelDateTimePicker),
        new PropertyMetadata(defaultValue: DayOfWeek.Monday));

    public DayOfWeek FirstDayOfWeek
    {
        get => (DayOfWeek)GetValue(FirstDayOfWeekProperty);
        set => SetValue(FirstDayOfWeekProperty, value);
    }

    public static readonly DependencyProperty IsTodayHighlightedProperty = DependencyProperty.Register(
        nameof(IsTodayHighlighted),
        typeof(bool),
        typeof(LabelDateTimePicker),
        new PropertyMetadata(defaultValue: true));

    public bool IsTodayHighlighted
    {
        get => (bool)GetValue(IsTodayHighlightedProperty);
        set => SetValue(IsTodayHighlightedProperty, value);
    }

    public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register(
        nameof(SelectedDate),
        typeof(DateTime?),
        typeof(LabelDateTimePicker),
        new FrameworkPropertyMetadata(
            default(DateTime?),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            OnSelectedDateChanged));

    public DateTime? SelectedDate
    {
        get => (DateTime?)GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    public static readonly DependencyProperty SelectedDateFormatProperty = DependencyProperty.Register(
        nameof(SelectedDateFormat),
        typeof(DatePickerFormat),
        typeof(LabelDateTimePicker),
        new PropertyMetadata(defaultValue: DatePickerFormat.Short));

    public DatePickerFormat SelectedDateFormat
    {
        get => (DatePickerFormat)GetValue(SelectedDateFormatProperty);
        set => SetValue(SelectedDateFormatProperty, value);
    }

    public static readonly DependencyProperty CustomCultureProperty = DependencyProperty.Register(
        nameof(CustomCulture),
        typeof(CultureInfo),
        typeof(LabelDateTimePicker),
        new PropertyMetadata(default(CultureInfo?)));

    public CultureInfo? CustomCulture
    {
        get => (CultureInfo?)GetValue(CustomCultureProperty);
        set => SetValue(CustomCultureProperty, value);
    }

    public static readonly DependencyProperty TextDateProperty = DependencyProperty.Register(
        nameof(TextDate),
        typeof(string),
        typeof(LabelDateTimePicker),
        new FrameworkPropertyMetadata(
            string.Empty,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            OnTextLostFocus,
            CoerceText,
            isAnimationProhibited: true,
            UpdateSourceTrigger.LostFocus));

    public string TextDate
    {
        get => (string)GetValue(TextDateProperty);
        set => SetValue(TextDateProperty, value);
    }

    public static readonly DependencyProperty TextTimeProperty = DependencyProperty.Register(
        nameof(TextTime),
        typeof(string),
        typeof(LabelDateTimePicker),
        new FrameworkPropertyMetadata(
            string.Empty,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            OnTextLostFocus,
            CoerceText,
            isAnimationProhibited: true,
            UpdateSourceTrigger.LostFocus));

    public string TextTime
    {
        get => (string)GetValue(TextTimeProperty);
        set => SetValue(TextTimeProperty, value);
    }

    public static readonly DependencyProperty WatermarkTextProperty = DependencyProperty.Register(
        nameof(WatermarkText),
        typeof(string),
        typeof(LabelDateTimePicker),
        new PropertyMetadata(defaultValue: string.Empty));

    public string WatermarkText
    {
        get => (string)GetValue(WatermarkTextProperty);
        set => SetValue(WatermarkTextProperty, value);
    }

    public static readonly DependencyProperty WatermarkTimeTextProperty = DependencyProperty.Register(
        nameof(WatermarkTimeText),
        typeof(string),
        typeof(LabelDateTimePicker),
        new PropertyMetadata(defaultValue: string.Empty));

    public string WatermarkTimeText
    {
        get => (string)GetValue(WatermarkTimeTextProperty);
        set => SetValue(WatermarkTimeTextProperty, value);
    }

    public static readonly DependencyProperty WatermarkAlignmentProperty = DependencyProperty.Register(
        nameof(WatermarkAlignment),
        typeof(TextAlignment),
        typeof(LabelDateTimePicker),
        new PropertyMetadata(default(TextAlignment)));

    public TextAlignment WatermarkAlignment
    {
        get => (TextAlignment)GetValue(WatermarkAlignmentProperty);
        set => SetValue(WatermarkAlignmentProperty, value);
    }

    public static readonly DependencyProperty WatermarkTrimmingProperty = DependencyProperty.Register(
        nameof(WatermarkTrimming),
        typeof(TextTrimming),
        typeof(LabelDateTimePicker),
        new PropertyMetadata(default(TextTrimming)));

    public TextTrimming WatermarkTrimming
    {
        get => (TextTrimming)GetValue(WatermarkTrimmingProperty);
        set => SetValue(WatermarkTrimmingProperty, value);
    }

    public static readonly DependencyProperty OpenCalenderProperty = DependencyProperty.Register(
        nameof(OpenCalender),
        typeof(bool),
        typeof(LabelDateTimePicker),
        new PropertyMetadata(default(bool)));

    public bool OpenCalender
    {
        get => (bool)GetValue(OpenCalenderProperty);
        set => SetValue(OpenCalenderProperty, value);
    }

    public static readonly DependencyProperty OpenClockProperty = DependencyProperty.Register(
        nameof(OpenClock),
        typeof(bool),
        typeof(LabelDateTimePicker),
        new PropertyMetadata(default(bool)));

    public bool OpenClock
    {
        get => (bool)GetValue(OpenClockProperty);
        set => SetValue(OpenClockProperty, value);
    }

    public event EventHandler<ValueChangedEventArgs<DateTime?>>? LostFocusValid;

    public event EventHandler<ValueChangedEventArgs<DateTime?>>? LostFocusInvalid;

    public LabelDateTimePicker()
    {
        InitializeComponent();

        Loaded += OnLoaded;
        CultureManager.UiCultureChanged += OnUiCultureChanged;
        ThemeManager.Current.ThemeChanged += OnThemeChanged;
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

        if (SelectedDate.HasValue)
        {
            SetCurrentValue(
                TextDateProperty,
                DateTimeTextBoxHelper.GetSelectedDateAsText(
                    SelectedDate.Value,
                    SelectedDateFormat,
                    cultureInfo));

            if (SelectedDate.HasValue)
            {
                SetCurrentValue(
                    TextTimeProperty,
                    SelectedDate.Value.ToShortTimeStringUsingSpecificCulture(cultureInfo));
            }
        }
    }

    private void OnThemeChanged(
        object? sender,
        ThemeChangedEventArgs e)
    {
        var datePickerImages = this.FindChildren<SvgImage>();
        foreach (var datePickerImage in datePickerImages)
        {
            datePickerImage.OverrideColor = (Color)FindResource("AtcApps.Colors.Accent");
            datePickerImage.ReRenderSvg();
        }
    }

    private void OnDateTextKeyDown(
        object sender,
        KeyEventArgs e)
    {
        CommonKeyDownHandler(sender, e, (control, textBox) =>
        {
            control.SetCurrentValue(TextDateProperty, textBox.Text);
        });
    }

    private void OnTimeTextKeyDown(
        object sender,
        KeyEventArgs e)
    {
        CommonKeyDownHandler(sender, e, (control, textBox) =>
        {
            control.SetCurrentValue(TextTimeProperty, textBox.Text);
        });
    }

    private static void OnTextLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelDateTimePicker)d;

        if (StackTraceHelper.ContainsPropertyName(nameof(OnSelectedDateChanged)))
        {
            return;
        }

        if (StackTraceHelper.ContainsPropertyName("ClearControl"))
        {
            control.TextDate = string.Empty;
            control.TextTime = string.Empty;
        }

        var dateTime = ValidateText(e, control, raiseEvents: true);

        UpdateSelectedDate(control, dateTime);
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

        if (shortTime.Equals(TextTime, StringComparison.Ordinal))
        {
            return;
        }

        SetCurrentValue(TextTimeProperty, shortTime);
    }

    private static void OnSelectedDateChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelDateTimePicker)d;

        if (control.SelectedDate is null)
        {
            control.SetCurrentValue(TextDateProperty, string.Empty);
            control.SetCurrentValue(TextTimeProperty, string.Empty);
        }
        else
        {
            var cultureInfo = control.CustomCulture ?? Thread.CurrentThread.CurrentUICulture;

            control.SetCurrentValue(
                TextDateProperty,
                DateTimeTextBoxHelper.GetSelectedDateAsText(
                    control.SelectedDate.Value,
                    control.SelectedDateFormat,
                    cultureInfo));

            control.SetCurrentValue(
                TextTimeProperty,
                control.SelectedDate.Value.ToShortTimeStringUsingSpecificCulture(cultureInfo));
        }
    }

    private static void CommonKeyDownHandler(
        object sender,
        KeyEventArgs e,
        Action<LabelDateTimePicker, TextBox> updatePropertyAction)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        if (sender is not DependencyObject parent)
        {
            return;
        }

        var control = VisualTreeHelperEx.FindParent<LabelDateTimePicker>(parent);
        if (control is null ||
            sender is not TextBox textBox)
        {
            return;
        }

        updatePropertyAction(control, textBox);

        var dateTime = ValidateText(control);

        UpdateSelectedDate(control, dateTime);
    }

    private static void UpdateSelectedDate(
        LabelDateTimePicker control,
        DateTime? dateTime)
    {
        if (control.SelectedDate == dateTime)
        {
            return;
        }

        if (dateTime is not null &&
            string.IsNullOrEmpty(control.ValidationText))
        {
            control.SelectedDate = dateTime;
        }
        else
        {
            control.SelectedDate = null;
        }
    }

    private static DateTime? ValidateText(
        DependencyPropertyChangedEventArgs e,
        LabelDateTimePicker control,
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

    [SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "OK.")]
    private static DateTime? ValidateText(
        LabelDateTimePicker control)
    {
        var isDateEmpty = string.IsNullOrWhiteSpace(control.TextDate);
        var isTimeEmpty = string.IsNullOrWhiteSpace(control.TextTime);

        if (control.IsMandatory)
        {
            if (isDateEmpty || isTimeEmpty)
            {
                control.ValidationText = Validations.FieldIsRequired;
                return null;
            }
        }
        else
        {
            if (isDateEmpty && isTimeEmpty)
            {
                control.ValidationText = string.Empty;
                return null;
            }
        }

        var cultureInfo = control.CustomCulture ?? Thread.CurrentThread.CurrentUICulture;

        if (DateTimeTextBoxHelper.TryParseUsingSpecificCulture(
                control.TextDate,
                control.TextTime,
                cultureInfo,
                out var dateTime))
        {
            control.ValidationText = string.Empty;
            return dateTime;
        }

        control.ValidationText = control.SelectedDateFormat == DatePickerFormat.Short
            ? string.Format(
                CultureInfo.CurrentUICulture,
                Validations.InvalidDate2,
                cultureInfo.DateTimeFormat.ShortDatePattern,
                cultureInfo.DateTimeFormat.ShortTimePattern)
            : string.Format(
                CultureInfo.CurrentUICulture,
                Validations.InvalidDate2,
                cultureInfo.DateTimeFormat.LongDatePattern,
                cultureInfo.DateTimeFormat.ShortTimePattern);

        return null;
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
    private static void OnLostFocusFireValidEvent(
        LabelDateTimePicker control,
        DependencyPropertyChangedEventArgs e)
    {
        var cultureInfo = control.CustomCulture ?? Thread.CurrentThread.CurrentUICulture;

        DateTime? oldValue = null;
        if (e.OldValue is not null &&
            DateTimeTextBoxHelper.TryParseUsingSpecificCulture(
                control.SelectedDateFormat,
                e.OldValue.ToString()!,
                cultureInfo,
                out var resultOld))
        {
            oldValue = resultOld;
        }

        DateTime? newValue = null;
        if (e.NewValue is not null &&
            DateTimeTextBoxHelper.TryParseUsingSpecificCulture(
                control.SelectedDateFormat,
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
        LabelDateTimePicker control,
        DependencyPropertyChangedEventArgs e)
    {
        var cultureInfo = control.CustomCulture ?? Thread.CurrentThread.CurrentUICulture;

        DateTime? oldValue = null;
        if (e.OldValue is not null &&
            DateTimeTextBoxHelper.TryParseUsingSpecificCulture(
                control.SelectedDateFormat,
                e.OldValue.ToString()!,
                cultureInfo,
                out var resultOld))
        {
            oldValue = resultOld;
        }

        DateTime? newValue = null;
        if (e.NewValue is not null &&
            DateTimeTextBoxHelper.TryParseUsingSpecificCulture(
                control.SelectedDateFormat,
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
            WatermarkText.Contains('/', StringComparison.Ordinal) ||
            WatermarkText.Contains('.', StringComparison.Ordinal) ||
            WatermarkText.Contains(',', StringComparison.Ordinal))
        {
            WatermarkText = SelectedDateFormat == DatePickerFormat.Short
                ? cultureInfo.DateTimeFormat.ShortDatePattern
                : cultureInfo.DateTimeFormat.LongDatePattern;
        }

        if (string.IsNullOrEmpty(WatermarkTimeText) ||
            WatermarkTimeText.Contains(':', StringComparison.Ordinal) ||
            WatermarkTimeText.Contains('.', StringComparison.Ordinal))
        {
            WatermarkTimeText = cultureInfo.DateTimeFormat.ShortTimePattern;
        }
    }
}