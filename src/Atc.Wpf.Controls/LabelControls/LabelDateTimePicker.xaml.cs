// ReSharper disable InvertIf
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
        new PropertyMetadata(default(DateTime?)));

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

    public event EventHandler<ChangedDateTimeEventArgs>? LostFocusValid;

    public event EventHandler<ChangedDateTimeEventArgs>? LostFocusInvalid;

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
        SetDefaultWatermarkIfNeeded();
    }

    private void OnUiCultureChanged(
        object? sender,
        UiCultureEventArgs e)
    {
        SetDefaultWatermarkIfNeeded();

        if (SelectedDate.HasValue)
        {
            TextDate = GetSelectedDateAsText(SelectedDate.Value);
            if (SelectedDate.HasValue)
            {
                TextTime = SelectedDate.Value.ToShortTimeStringUsingCurrentUiCulture();
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

    private void OnTextDateChanged(
        object sender,
        TextChangedEventArgs e)
    {
        var control = (TextBox)sender;

        if (DateTimeTextBoxHelper.TryParseUsingCurrentUiCulture(
                SelectedDateFormat,
                $"{control.Text} {TextTime}",
                out var result))
        {
            SelectedDate = result;
        }
        else
        {
            SelectedDate = null;
        }
    }

    private void OnTextTimeChanged(
        object sender,
        TextChangedEventArgs e)
    {
        var control = (TextBox)sender;

        if (!DateTimeTextBoxHelper.HandlePrerequisiteForOnTextTimeChanged(control, e))
        {
            return;
        }

        if (DateTimeTextBoxHelper.TryParseUsingCurrentUiCulture(
                SelectedDateFormat,
                $"{TextDate} {control.Text}",
                out var result))
        {
            SelectedDate = result;
        }
        else
        {
            SelectedDate = null;
        }
    }

    private static void OnTextLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelDateTimePicker)d;

        ValidateText(e, control, raiseEvents: true);

        if (string.IsNullOrEmpty(control.ValidationText))
        {
            return;
        }

        control.SelectedDate = null;
    }

    private void OnCalendarSelectedDatesChanged(
        object? sender,
        SelectionChangedEventArgs e)
    {
        var calender = (System.Windows.Controls.Calendar)sender!;

        if (calender.SelectedDate.HasValue)
        {
            TextDate = GetSelectedDateAsText(calender.SelectedDate.Value);
        }
    }

    private void OnClockPickerSelectedClockChanged(
        object? sender,
        RoutedEventArgs e)
    {
        var clockPicker = (ClockPicker)sender!;

        if (clockPicker.SelectedDateTime.HasValue)
        {
            TextTime = clockPicker.SelectedDateTime.Value.ToShortTimeStringUsingCurrentUiCulture();
        }
    }

    [SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "OK.")]
    private static void ValidateText(
        DependencyPropertyChangedEventArgs e,
        LabelDateTimePicker control,
        bool raiseEvents)
    {
        if (control.IsMandatory &&
            string.IsNullOrWhiteSpace(control.TextDate))
        {
            control.ValidationText = Validations.FieldIsRequired;
            if (raiseEvents)
            {
                OnLostFocusFireInvalidEvent(control, e);
            }

            return;
        }

        if (DateTimeTextBoxHelper.TryParseUsingCurrentUiCulture(
                control.SelectedDateFormat,
                control.TextDate,
                out _))
        {
            control.ValidationText = string.Empty;
            OnLostFocusFireValidEvent(control, e);
        }
        else
        {
            control.ValidationText = control.SelectedDateFormat == DatePickerFormat.Short
                ? string.Format(
                    CultureInfo.CurrentUICulture,
                    Validations.InvalidDate2,
                    Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern,
                    Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortTimePattern)
                : string.Format(
                    CultureInfo.CurrentUICulture,
                    Validations.InvalidDate2,
                    Thread.CurrentThread.CurrentUICulture.DateTimeFormat.LongDatePattern,
                    Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortTimePattern);

            if (raiseEvents)
            {
                OnLostFocusFireInvalidEvent(control, e);
            }
        }
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
    private static void OnLostFocusFireValidEvent(
        LabelDateTimePicker control,
        DependencyPropertyChangedEventArgs e)
    {
        DateTime? oldValue = null;
        if (e.OldValue is not null &&
            DateTimeTextBoxHelper.TryParseUsingCurrentUiCulture(
                control.SelectedDateFormat,
                e.OldValue.ToString()!,
                out var resultOld))
        {
            oldValue = resultOld;
        }

        DateTime? newValue = null;
        if (e.NewValue is not null &&
            DateTimeTextBoxHelper.TryParseUsingCurrentUiCulture(
                control.SelectedDateFormat,
                e.NewValue.ToString()!,
                out var resultNew))
        {
            newValue = resultNew;
        }

        control.LostFocusValid?.Invoke(
            control,
            new ChangedDateTimeEventArgs(
                ControlHelper.GetIdentifier(control),
                oldValue,
                newValue));
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
    private static void OnLostFocusFireInvalidEvent(
        LabelDateTimePicker control,
        DependencyPropertyChangedEventArgs e)
    {
        DateTime? oldValue = null;
        if (e.OldValue is not null &&
            DateTimeTextBoxHelper.TryParseUsingCurrentUiCulture(
                control.SelectedDateFormat,
                e.OldValue.ToString()!,
                out var resultOld))
        {
            oldValue = resultOld;
        }

        DateTime? newValue = null;
        if (e.NewValue is not null &&
            DateTimeTextBoxHelper.TryParseUsingCurrentUiCulture(
                control.SelectedDateFormat,
                e.NewValue.ToString()!,
                out var resultNew))
        {
            newValue = resultNew;
        }

        control.LostFocusInvalid?.Invoke(
            control,
            new ChangedDateTimeEventArgs(
                ControlHelper.GetIdentifier(control),
                oldValue,
                newValue));
    }

    private static object CoerceText(
        DependencyObject d,
        object? value)
        => value ?? string.Empty;

    private void SetDefaultWatermarkIfNeeded()
    {
        if (string.IsNullOrEmpty(WatermarkText) ||
            WatermarkText.Contains('/', StringComparison.Ordinal) ||
            WatermarkText.Contains('.', StringComparison.Ordinal) ||
            WatermarkText.Contains(',', StringComparison.Ordinal))
        {
            WatermarkText = SelectedDateFormat == DatePickerFormat.Short
                ? Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern
                : Thread.CurrentThread.CurrentUICulture.DateTimeFormat.LongDatePattern;
        }

        if (string.IsNullOrEmpty(WatermarkTimeText) ||
            WatermarkTimeText.Contains(':', StringComparison.Ordinal) ||
            WatermarkTimeText.Contains('.', StringComparison.Ordinal))
        {
            WatermarkTimeText = Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortTimePattern;
        }
    }

    [SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "OK.")]
    private string GetSelectedDateAsText(
        DateTime dateTime)
    {
        if (SelectedDateFormat == DatePickerFormat.Short ||
            (SelectedDateFormat == DatePickerFormat.Long &&
             Thread.CurrentThread.CurrentUICulture.LCID == GlobalizationConstants.EnglishCultureInfo.LCID))
        {
            return dateTime.ToString(
                SelectedDateFormat == DatePickerFormat.Short
                    ? Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern
                    : Thread.CurrentThread.CurrentUICulture.DateTimeFormat.LongDatePattern);
        }

        var s = dateTime.ToString(Thread.CurrentThread.CurrentUICulture.DateTimeFormat.LongDatePattern);

        if (s.StartsWith(nameof(DayOfWeek.Sunday), StringComparison.Ordinal))
        {
            s = s.Replace(nameof(DayOfWeek.Sunday), DayOfWeekHelper.GetDescription(DayOfWeek.Sunday, Thread.CurrentThread.CurrentUICulture), StringComparison.Ordinal);
        }
        else if (s.StartsWith(nameof(DayOfWeek.Monday), StringComparison.Ordinal))
        {
            s = s.Replace(nameof(DayOfWeek.Monday), DayOfWeekHelper.GetDescription(DayOfWeek.Monday, Thread.CurrentThread.CurrentUICulture), StringComparison.Ordinal);
        }
        else if (s.StartsWith(nameof(DayOfWeek.Tuesday), StringComparison.Ordinal))
        {
            s = s.Replace(nameof(DayOfWeek.Tuesday), DayOfWeekHelper.GetDescription(DayOfWeek.Tuesday, Thread.CurrentThread.CurrentUICulture), StringComparison.Ordinal);
        }
        else if (s.StartsWith(nameof(DayOfWeek.Wednesday), StringComparison.Ordinal))
        {
            s = s.Replace(nameof(DayOfWeek.Wednesday), DayOfWeekHelper.GetDescription(DayOfWeek.Wednesday, Thread.CurrentThread.CurrentUICulture), StringComparison.Ordinal);
        }
        else if (s.StartsWith(nameof(DayOfWeek.Thursday), StringComparison.Ordinal))
        {
            s = s.Replace(nameof(DayOfWeek.Thursday), DayOfWeekHelper.GetDescription(DayOfWeek.Thursday, Thread.CurrentThread.CurrentUICulture), StringComparison.Ordinal);
        }
        else if (s.StartsWith(nameof(DayOfWeek.Friday), StringComparison.Ordinal))
        {
            s = s.Replace(nameof(DayOfWeek.Friday), DayOfWeekHelper.GetDescription(DayOfWeek.Friday, Thread.CurrentThread.CurrentUICulture), StringComparison.Ordinal);
        }
        else if (s.StartsWith(nameof(DayOfWeek.Saturday), StringComparison.Ordinal))
        {
            s = s.Replace(nameof(DayOfWeek.Saturday), DayOfWeekHelper.GetDescription(DayOfWeek.Saturday, Thread.CurrentThread.CurrentUICulture), StringComparison.Ordinal);
        }

        return s;
    }
}