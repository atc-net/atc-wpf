// ReSharper disable ConvertIfStatementToSwitchStatement
namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelDatePicker.
/// </summary>
public partial class LabelDatePicker : ILabelDatePicker
{
    public static readonly DependencyProperty DisplayDateProperty = DependencyProperty.Register(
        nameof(DisplayDate),
        typeof(DateTime),
        typeof(LabelDatePicker),
        new PropertyMetadata(defaultValue: DateTime.Now));

    public DateTime DisplayDate
    {
        get => (DateTime)GetValue(DisplayDateProperty);
        set => SetValue(DisplayDateProperty, value);
    }

    public static readonly DependencyProperty DisplayDateStartProperty = DependencyProperty.Register(
        nameof(DisplayDateStart),
        typeof(DateTime?),
        typeof(LabelDatePicker),
        new PropertyMetadata(default(DateTime?)));

    public DateTime? DisplayDateStart
    {
        get => (DateTime?)GetValue(DisplayDateStartProperty);
        set => SetValue(DisplayDateStartProperty, value);
    }

    public static readonly DependencyProperty DisplayDateEndProperty = DependencyProperty.Register(
        nameof(DisplayDateEnd),
        typeof(DateTime?),
        typeof(LabelDatePicker),
        new PropertyMetadata(default(DateTime?)));

    public DateTime? DisplayDateEnd
    {
        get => (DateTime?)GetValue(DisplayDateEndProperty);
        set => SetValue(DisplayDateEndProperty, value);
    }

    public static readonly DependencyProperty FirstDayOfWeekProperty = DependencyProperty.Register(
        nameof(FirstDayOfWeek),
        typeof(DayOfWeek),
        typeof(LabelDatePicker),
        new PropertyMetadata(defaultValue: DayOfWeek.Monday));

    public DayOfWeek FirstDayOfWeek
    {
        get => (DayOfWeek)GetValue(FirstDayOfWeekProperty);
        set => SetValue(FirstDayOfWeekProperty, value);
    }

    public static readonly DependencyProperty IsTodayHighlightedProperty = DependencyProperty.Register(
        nameof(IsTodayHighlighted),
        typeof(bool),
        typeof(LabelDatePicker),
        new PropertyMetadata(defaultValue: true));

    public bool IsTodayHighlighted
    {
        get => (bool)GetValue(IsTodayHighlightedProperty);
        set => SetValue(IsTodayHighlightedProperty, value);
    }

    public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register(
        nameof(SelectedDate),
        typeof(DateTime?),
        typeof(LabelDatePicker),
        new PropertyMetadata(default(DateTime?)));

    public DateTime? SelectedDate
    {
        get => (DateTime?)GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    public static readonly DependencyProperty SelectedDateFormatProperty = DependencyProperty.Register(
        nameof(SelectedDateFormat),
        typeof(DatePickerFormat),
        typeof(LabelDatePicker),
        new PropertyMetadata(defaultValue: DatePickerFormat.Short));

    public DatePickerFormat SelectedDateFormat
    {
        get => (DatePickerFormat)GetValue(SelectedDateFormatProperty);
        set => SetValue(SelectedDateFormatProperty, value);
    }

    public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(TextChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedPropertyChangedEventHandler<string>),
        typeof(LabelDatePicker));

    public event RoutedPropertyChangedEventHandler<string> TextChanged
    {
        add => AddHandler(TextChangedEvent, value);
        remove => RemoveHandler(TextChangedEvent, value);
    }

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(LabelDatePicker),
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
        typeof(LabelDatePicker),
        new PropertyMetadata(defaultValue: string.Empty));

    public string WatermarkText
    {
        get => (string)GetValue(WatermarkTextProperty);
        set => SetValue(WatermarkTextProperty, value);
    }

    public static readonly DependencyProperty WatermarkAlignmentProperty = DependencyProperty.Register(
        nameof(WatermarkAlignment),
        typeof(TextAlignment),
        typeof(LabelDatePicker),
        new PropertyMetadata(default(TextAlignment)));

    public TextAlignment WatermarkAlignment
    {
        get => (TextAlignment)GetValue(WatermarkAlignmentProperty);
        set => SetValue(WatermarkAlignmentProperty, value);
    }

    public static readonly DependencyProperty WatermarkTrimmingProperty = DependencyProperty.Register(
        nameof(WatermarkTrimming),
        typeof(TextTrimming),
        typeof(LabelDatePicker),
        new PropertyMetadata(default(TextTrimming)));

    public TextTrimming WatermarkTrimming
    {
        get => (TextTrimming)GetValue(WatermarkTrimmingProperty);
        set => SetValue(WatermarkTrimmingProperty, value);
    }

    public static readonly DependencyProperty OpenCalenderProperty = DependencyProperty.Register(
        nameof(OpenCalender),
        typeof(bool),
        typeof(LabelDatePicker),
        new PropertyMetadata(default(bool)));

    public bool OpenCalender
    {
        get => (bool)GetValue(OpenCalenderProperty);
        set => SetValue(OpenCalenderProperty, value);
    }

    public event EventHandler<ChangedDateTimeEventArgs>? LostFocusValid;

    public event EventHandler<ChangedDateTimeEventArgs>? LostFocusInvalid;

    public LabelDatePicker()
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
            Text = GetSelectedDateAsText(SelectedDate.Value);
        }
    }

    private void OnThemeChanged(
        object? sender,
        ThemeChangedEventArgs e)
    {
        var datePickerImage = this.FindChild<SvgImage>()!;
        datePickerImage.OverrideColor = (Color)FindResource("AtcApps.Colors.Accent");
        datePickerImage.ReRenderSvg();
    }

    private void OnTextDateChanged(
        object sender,
        TextChangedEventArgs e)
    {
        var control = (TextBox)sender;

        if (DateTimeTextBoxHelper.TryParseUsingCurrentUiCulture(
                SelectedDateFormat,
                control.Text,
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
        var control = (LabelDatePicker)d;

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
            Text = GetSelectedDateAsText(calender.SelectedDate.Value);
        }
    }

    [SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "OK.")]
    private static void ValidateText(
        DependencyPropertyChangedEventArgs e,
        LabelDatePicker control,
        bool raiseEvents)
    {
        if (control.IsMandatory &&
            string.IsNullOrWhiteSpace(control.Text))
        {
            control.ValidationText = Validations.FieldIsRequired;
            if (raiseEvents)
            {
                OnLostFocusFireInvalidEvent(control, e);
            }

            return;
        }

        if (!control.IsMandatory &&
            string.IsNullOrWhiteSpace(control.Text))
        {
            control.ValidationText = string.Empty;
            if (raiseEvents)
            {
                OnLostFocusFireValidEvent(control, e);
            }

            return;
        }

        if (DateTimeTextBoxHelper.TryParseUsingCurrentUiCulture(
                control.SelectedDateFormat,
                control.Text,
                out _))
        {
            control.ValidationText = string.Empty;
            if (raiseEvents)
            {
                OnLostFocusFireValidEvent(control, e);
            }

            return;
        }

        control.ValidationText = control.SelectedDateFormat == DatePickerFormat.Short
            ? string.Format(
                CultureInfo.CurrentUICulture,
                Validations.InvalidDate1,
                Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern)
            : string.Format(
                CultureInfo.CurrentUICulture,
                Validations.InvalidDate1,
                Thread.CurrentThread.CurrentUICulture.DateTimeFormat.LongDatePattern);

        if (raiseEvents)
        {
            OnLostFocusFireInvalidEvent(control, e);
        }
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
    private static void OnLostFocusFireValidEvent(
        LabelDatePicker control,
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
        LabelDatePicker control,
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