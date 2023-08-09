using Atc.Wpf.Controls.BaseControls;
using System.Windows.Media.TextFormatting;

namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelTimePicker.
/// </summary>
public partial class LabelTimePicker : ILabelTimePicker
{
    public static readonly DependencyProperty SelectedTimeProperty = DependencyProperty.Register(
        nameof(SelectedTime),
        typeof(DateTime?),
        typeof(LabelTimePicker),
        new PropertyMetadata(default(DateTime?)));

    public DateTime? SelectedTime
    {
        get => (DateTime?)GetValue(SelectedTimeProperty);
        set => SetValue(SelectedTimeProperty, value);
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

    public event EventHandler<ChangedDateTimeEventArgs>? LostFocusValid;

    public event EventHandler<ChangedDateTimeEventArgs>? LostFocusInvalid;

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
        SetDefaultWatermarkIfNeeded();
    }

    private void OnUiCultureChanged(
        object? sender,
        UiCultureEventArgs e)
    {
        SetDefaultWatermarkIfNeeded();

        if (SelectedTime.HasValue)
        {
            Text = GetSelectedTimeAsText(SelectedTime.Value);
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

        if (string.IsNullOrWhiteSpace(control.Text) ||
            !DateTime.TryParse(
                $"{DateTime.Now.ToShortDateString()} {control.Text}",
                Thread.CurrentThread.CurrentUICulture.DateTimeFormat,
                DateTimeStyles.None,
                out var result))
        {
            SelectedTime = null;
        }
        else
        {
            SelectedTime = result;
        }
    }

    private static void OnTextLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelTimePicker)d;

        ValidateText(e, control, raiseEvents: true);

        if (string.IsNullOrEmpty(control.ValidationText))
        {
            return;
        }

        control.SelectedTime = null;
    }

    private void OnClockPickerSelectedClockChanged(
        object? sender,
        RoutedEventArgs e)
    {
        var clockPicker = (ClockPicker)sender!;

        if (clockPicker.SelectedDateTime.HasValue)
        {
            Text = GetSelectedTimeAsText(clockPicker.SelectedDateTime.Value);
        }
    }

    private static void ValidateText(
        DependencyPropertyChangedEventArgs e,
        LabelTimePicker control,
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

        if (!string.IsNullOrWhiteSpace(control.Text) &&
            !DateTime.TryParse(
                $"{DateTime.Now.ToShortDateString()} {control.Text}",
                Thread.CurrentThread.CurrentUICulture.DateTimeFormat,
                DateTimeStyles.None,
                out _))
        {
            control.ValidationText = Validations.InvalidTime;

            if (raiseEvents)
            {
                OnLostFocusFireInvalidEvent(control, e);
            }

            return;
        }

        control.ValidationText = string.Empty;
        OnLostFocusFireValidEvent(control, e);
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
    private static void OnLostFocusFireValidEvent(
        LabelTimePicker control,
        DependencyPropertyChangedEventArgs e)
    {
        DateTime? oldValue = null;
        if (e.OldValue is not null &&
            DateTime.TryParse(
                e.OldValue.ToString(),
                Thread.CurrentThread.CurrentUICulture.DateTimeFormat,
                DateTimeStyles.None,
                out var resultOld))
        {
            oldValue = resultOld;
        }

        DateTime? newValue = null;
        if (e.NewValue is not null &&
            DateTime.TryParse(
                e.NewValue.ToString(),
                Thread.CurrentThread.CurrentUICulture.DateTimeFormat,
                DateTimeStyles.None,
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
        LabelTimePicker control,
        DependencyPropertyChangedEventArgs e)
    {
        DateTime? oldValue = null;
        if (e.OldValue is not null &&
            DateTime.TryParse(
                e.OldValue.ToString(),
                Thread.CurrentThread.CurrentUICulture.DateTimeFormat,
                DateTimeStyles.None,
                out var resultOld))
        {
            oldValue = resultOld;
        }

        DateTime? newValue = null;
        if (e.NewValue is not null &&
            DateTime.TryParse(
                e.NewValue.ToString(),
                Thread.CurrentThread.CurrentUICulture.DateTimeFormat,
                DateTimeStyles.None,
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
            WatermarkText.Contains(':', StringComparison.Ordinal) ||
            WatermarkText.Contains('.', StringComparison.Ordinal))
        {
            WatermarkText = Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortTimePattern;
        }
    }

    [SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "OK.")]
    private static string GetSelectedTimeAsText(
        DateTime dateTime)
        => dateTime.ToString(Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortTimePattern);
}