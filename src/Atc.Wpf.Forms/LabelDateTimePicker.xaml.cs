// ReSharper disable InvertIf
// ReSharper disable ConvertIfStatementToSwitchStatement
namespace Atc.Wpf.Forms;

public partial class LabelDateTimePicker : ILabelDateTimePicker
{
    [DependencyProperty(DefaultValue = "DateTime.Now")]
    private DateTime displayDate;

    [DependencyProperty]
    private DateTime? displayDateStart;

    [DependencyProperty]
    private DateTime? displayDateEnd;

    [DependencyProperty(DefaultValue = DayOfWeek.Monday)]
    private DayOfWeek firstDayOfWeek;

    [DependencyProperty(DefaultValue = true)]
    private bool isTodayHighlighted;

    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        PropertyChangedCallback = nameof(OnSelectedDateChanged))]
    private DateTime? selectedDate;

    [DependencyProperty(DefaultValue = DatePickerFormat.Short)]
    private DatePickerFormat selectedDateFormat;

    [DependencyProperty]
    private CultureInfo? customCulture;

    [DependencyProperty(
        DefaultValue = "",
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        PropertyChangedCallback = nameof(OnTextLostFocus),
        CoerceValueCallback = nameof(CoerceText),
        IsAnimationProhibited = true,
        DefaultUpdateSourceTrigger = UpdateSourceTrigger.LostFocus)]
    private string textDate;

    [DependencyProperty(
        DefaultValue = "",
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        PropertyChangedCallback = nameof(OnTextLostFocus),
        CoerceValueCallback = nameof(CoerceText),
        IsAnimationProhibited = true,
        DefaultUpdateSourceTrigger = UpdateSourceTrigger.LostFocus)]
    private string textTime;

    [DependencyProperty(DefaultValue = "")]
    private string watermarkDateText;

    [DependencyProperty(DefaultValue = "")]
    private string watermarkTimeText;

    [DependencyProperty(DefaultValue = TextAlignment.Left)]
    private TextAlignment watermarkAlignment;

    [DependencyProperty(DefaultValue = TextTrimming.None)]
    private TextTrimming watermarkTrimming;

    [DependencyProperty(DefaultValue = false)]
    private bool openCalender;

    [DependencyProperty(DefaultValue = false)]
    private bool openClock;

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
        => SetDefaultWatermarkIfNeeded(CustomCulture ?? Thread.CurrentThread.CurrentUICulture);

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
                    SelectedDate
                        .Value
                        .ToShortTimeStringUsingSpecificCulture(cultureInfo));
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
        => CommonKeyDownHandler(
            sender,
            e,
            (control, textBox) => control.SetCurrentValue(
                TextDateProperty,
                textBox.Text));

    private void OnTimeTextKeyDown(
        object sender,
        KeyEventArgs e)
        => CommonKeyDownHandler(
            sender,
            e,
            (control, textBox) => control.SetCurrentValue(
                TextTimeProperty,
                textBox.Text));

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

        var dateTime = ValidateText(
            e,
            control,
            raiseEvents: true);

        UpdateSelectedDate(
            control,
            dateTime);
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
        var shortTime = clockPanelPicker
            .SelectedDateTime
            .Value
            .ToShortTimeStringUsingSpecificCulture(cultureInfo);

        if (shortTime.Equals(
                TextTime,
                StringComparison.Ordinal))
        {
            return;
        }

        SetCurrentValue(
            TextTimeProperty,
            shortTime);
    }

    private static void OnSelectedDateChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelDateTimePicker)d;

        if (control.SelectedDate is null)
        {
            control.SetCurrentValue(
                TextDateProperty,
                string.Empty);
            control.SetCurrentValue(
                TextTimeProperty,
                string.Empty);
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
                control.SelectedDate
                    .Value
                    .ToShortTimeStringUsingSpecificCulture(cultureInfo));
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

        updatePropertyAction(
            control,
            textBox);

        var dateTime = ValidateText(control);

        UpdateSelectedDate(
            control,
            dateTime);
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
                OnLostFocusFireValidEvent(
                    control,
                    e);
            }
            else
            {
                OnLostFocusFireInvalidEvent(
                    control,
                    e);
            }
        }

        return dateTime;
    }

    [SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "OK.")]
    private static DateTime? ValidateText(LabelDateTimePicker control)
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

    private static void OnLostFocusFireValidEvent(
        LabelDateTimePicker control,
        DependencyPropertyChangedEventArgs e)
        => DateTimePickerEventHelper.FireLostFocusValidEvent(
            control,
            e,
            control.CustomCulture,
            (string value, CultureInfo culture, out DateTime result) =>
                DateTimeTextBoxHelper.TryParseUsingSpecificCulture(
                    control.SelectedDateFormat,
                    value,
                    culture,
                    out result),
            control.LostFocusValid);

    private static void OnLostFocusFireInvalidEvent(
        LabelDateTimePicker control,
        DependencyPropertyChangedEventArgs e)
        => DateTimePickerEventHelper.FireLostFocusInvalidEvent(
            control,
            e,
            control.CustomCulture,
            (string value, CultureInfo culture, out DateTime result) =>
                DateTimeTextBoxHelper.TryParseUsingSpecificCulture(
                    control.SelectedDateFormat,
                    value,
                    culture,
                    out result),
            control.LostFocusInvalid);

    private static object CoerceText(
        DependencyObject d,
        object? value)
        => DateTimePickerEventHelper.CoerceText(value);

    private void SetDefaultWatermarkIfNeeded(CultureInfo cultureInfo)
    {
        if (string.IsNullOrEmpty(WatermarkDateText) ||
            WatermarkDateText.Contains(
                '/',
                StringComparison.Ordinal) ||
            WatermarkDateText.Contains(
                '.',
                StringComparison.Ordinal) ||
            WatermarkDateText.Contains(
                ',',
                StringComparison.Ordinal))
        {
            WatermarkDateText = SelectedDateFormat == DatePickerFormat.Short
                ? cultureInfo.DateTimeFormat.ShortDatePattern
                : cultureInfo.DateTimeFormat.LongDatePattern;
        }

        if (string.IsNullOrEmpty(WatermarkTimeText) ||
            WatermarkTimeText.Contains(
                ':',
                StringComparison.Ordinal) ||
            WatermarkTimeText.Contains(
                '.',
                StringComparison.Ordinal))
        {
            WatermarkTimeText = cultureInfo.DateTimeFormat.ShortTimePattern;
        }
    }
}