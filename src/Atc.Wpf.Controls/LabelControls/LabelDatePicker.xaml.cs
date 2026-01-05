// ReSharper disable ConvertIfStatementToSwitchStatement
// ReSharper disable InconsistentNaming
// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.LabelControls;

public partial class LabelDatePicker : ILabelDatePicker
{
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<string>))]
    private static readonly RoutedEvent textChanged;

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
    private string text;

    [DependencyProperty(DefaultValue = "")]
    private string watermarkText;

    [DependencyProperty(DefaultValue = TextAlignment.Left)]
    private TextAlignment watermarkAlignment;

    [DependencyProperty(DefaultValue = TextTrimming.None)]
    private TextTrimming watermarkTrimming;

    [DependencyProperty(DefaultValue = false)]
    private bool openCalender;

    public event EventHandler<ValueChangedEventArgs<DateTime?>>? LostFocusValid;

    public event EventHandler<ValueChangedEventArgs<DateTime?>>? LostFocusInvalid;

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
                TextProperty,
                DateTimeTextBoxHelper.GetSelectedDateAsText(
                    SelectedDate.Value,
                    SelectedDateFormat,
                    cultureInfo));
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

    private void OnDateTextKeyDown(
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

        var control = VisualTreeHelperEx.FindParent<LabelDatePicker>(parent);
        if (control is null ||
            sender is not TextBox textBox)
        {
            return;
        }

        control.SetCurrentValue(TextProperty, textBox.Text);

        var dateTime = ValidateText(control);

        UpdateSelectedDate(control, dateTime);
    }

    private static void OnTextLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelDatePicker)d;

        if (StackTraceHelper.ContainsPropertyName(nameof(OnSelectedDateChanged)))
        {
            return;
        }

        if (StackTraceHelper.ContainsPropertyName("ClearControl"))
        {
            control.Text = string.Empty;
        }

        var dateTime = ValidateText(e, control, raiseEvents: true);

        UpdateSelectedDate(control, dateTime);
    }

    private static void OnSelectedDateChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelDatePicker)d;

        if (control.SelectedDate is null)
        {
            control.SetCurrentValue(TextProperty, string.Empty);
        }
        else
        {
            var cultureInfo = control.CustomCulture ?? Thread.CurrentThread.CurrentUICulture;

            control.SetCurrentValue(
                TextProperty,
                DateTimeTextBoxHelper.GetSelectedDateAsText(
                    control.SelectedDate.Value,
                    control.SelectedDateFormat,
                    cultureInfo));
        }
    }

    private static void UpdateSelectedDate(
        LabelDatePicker control,
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
        LabelDatePicker control,
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
        LabelDatePicker control)
    {
        var isDateEmpty = string.IsNullOrWhiteSpace(control.Text);

        if (control.IsMandatory)
        {
            if (isDateEmpty)
            {
                control.ValidationText = Validations.FieldIsRequired;
                return null;
            }
        }
        else
        {
            if (isDateEmpty)
            {
                control.ValidationText = string.Empty;
                return null;
            }
        }

        var cultureInfo = control.CustomCulture ?? Thread.CurrentThread.CurrentUICulture;

        if (DateTimeTextBoxHelper.TryParseUsingSpecificCulture(
                control.SelectedDateFormat,
                control.Text,
                cultureInfo,
                out var dateTime))
        {
            control.ValidationText = string.Empty;
            return dateTime;
        }

        control.ValidationText = control.SelectedDateFormat == DatePickerFormat.Short
            ? string.Format(
                CultureInfo.CurrentUICulture,
                Validations.InvalidDate1,
                cultureInfo.DateTimeFormat.ShortDatePattern)
            : string.Format(
                CultureInfo.CurrentUICulture,
                Validations.InvalidDate1,
                cultureInfo.DateTimeFormat.LongDatePattern);

        return null;
    }

    private static void OnLostFocusFireValidEvent(
        LabelDatePicker control,
        DependencyPropertyChangedEventArgs e)
        => DateTimePickerEventHelper.FireLostFocusValidEvent(
            control,
            e,
            control.CustomCulture,
            (string value, CultureInfo culture, out DateTime result) =>
                DateTimeTextBoxHelper.TryParseUsingSpecificCulture(control.SelectedDateFormat, value, culture, out result),
            control.LostFocusValid);

    private static void OnLostFocusFireInvalidEvent(
        LabelDatePicker control,
        DependencyPropertyChangedEventArgs e)
        => DateTimePickerEventHelper.FireLostFocusInvalidEvent(
            control,
            e,
            control.CustomCulture,
            (string value, CultureInfo culture, out DateTime result) =>
                DateTimeTextBoxHelper.TryParseUsingSpecificCulture(control.SelectedDateFormat, value, culture, out result),
            control.LostFocusInvalid);

    private static object CoerceText(
        DependencyObject d,
        object? value)
        => DateTimePickerEventHelper.CoerceText(value);

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
    }
}