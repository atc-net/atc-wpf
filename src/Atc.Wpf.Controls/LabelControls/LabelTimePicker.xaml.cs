// ReSharper disable InconsistentNaming
namespace Atc.Wpf.Controls.LabelControls;

public partial class LabelTimePicker : ILabelTimePicker
{
    [DependencyProperty(
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        PropertyChangedCallback = nameof(OnSelectedTimeChanged))]
    private DateTime? selectedTime;

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

    [DependencyProperty]
    private bool openClock;

    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<string>))]
    private static readonly RoutedEvent textChanged;

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

        control.SetCurrentValue(
            TextProperty,
            textBox.Text);

        var dateTime = ValidateText(control);

        UpdateSelectedTime(
            control,
            dateTime);
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

        var dateTime = ValidateText(
            e,
            control,
            raiseEvents: true);

        UpdateSelectedTime(
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
        var shortTime = clockPanelPicker.SelectedDateTime.Value.ToShortTimeStringUsingSpecificCulture(cultureInfo);

        if (shortTime.Equals(
                Text,
                StringComparison.Ordinal))
        {
            return;
        }

        SetCurrentValue(
            TextProperty,
            shortTime);
    }

    private static void OnSelectedTimeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelTimePicker)d;

        if (control.SelectedTime is null)
        {
            control.SetCurrentValue(
                TextProperty,
                string.Empty);
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

    private static DateTime? ValidateText(LabelTimePicker control)
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

    private static void OnLostFocusFireValidEvent(
        LabelTimePicker control,
        DependencyPropertyChangedEventArgs e)
        => DateTimePickerEventHelper.FireLostFocusValidEvent(
            control,
            e,
            control.CustomCulture,
            DateTimeHelper.TryParseShortTimeUsingSpecificCulture,
            control.LostFocusValid);

    private static void OnLostFocusFireInvalidEvent(
        LabelTimePicker control,
        DependencyPropertyChangedEventArgs e)
        => DateTimePickerEventHelper.FireLostFocusInvalidEvent(
            control,
            e,
            control.CustomCulture,
            DateTimeHelper.TryParseShortTimeUsingSpecificCulture,
            control.LostFocusInvalid);

    private static object CoerceText(
        DependencyObject d,
        object? value)
        => DateTimePickerEventHelper.CoerceText(value);

    private void SetDefaultWatermarkIfNeeded(CultureInfo cultureInfo)
    {
        if (string.IsNullOrEmpty(WatermarkText) ||
            WatermarkText.Contains(
                ':',
                StringComparison.Ordinal) ||
            WatermarkText.Contains(
                '.',
                StringComparison.Ordinal))
        {
            WatermarkText = cultureInfo.DateTimeFormat.ShortTimePattern;
        }
    }
}