// ReSharper disable ConvertIfStatementToSwitchStatement
// ReSharper disable InvertIf
namespace Atc.Wpf.Controls.LabelControls;

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
        typeof(LabelDatePicker),
        new PropertyMetadata(defaultValue: DatePickerFormat.Short));

    public DatePickerFormat SelectedDateFormat
    {
        get => (DatePickerFormat)GetValue(SelectedDateFormatProperty);
        set => SetValue(SelectedDateFormatProperty, value);
    }

    public static readonly DependencyProperty CustomCultureProperty = DependencyProperty.Register(
        nameof(CustomCulture),
        typeof(CultureInfo),
        typeof(LabelDatePicker),
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
        new PropertyMetadata(BooleanBoxes.FalseBox));

    public bool OpenCalender
    {
        get => (bool)GetValue(OpenCalenderProperty);
        set => SetValue(OpenCalenderProperty, value);
    }

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

    private static void OnLostFocusFireInvalidEvent(
        LabelDatePicker control,
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
    }
}