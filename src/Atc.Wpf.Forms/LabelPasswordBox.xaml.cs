namespace Atc.Wpf.Forms;

public partial class LabelPasswordBox : ILabelPasswordBox
{
    [RoutedEvent(
        RoutingStrategy.Bubble,
        HandlerType = typeof(RoutedPropertyChangedEventHandler<string>))]
    private static readonly RoutedEvent textChanged;

    [DependencyProperty(DefaultValue = "")]
    private string watermarkText;

    [DependencyProperty(DefaultValue = TextAlignment.Left)]
    private TextAlignment watermarkAlignment;

    [DependencyProperty(DefaultValue = TextTrimming.None)]
    private TextTrimming watermarkTrimming;

    [DependencyProperty(DefaultValue = 100)]
    private uint maxLength;

    [DependencyProperty(DefaultValue = 0)]
    private uint minLength;

    [DependencyProperty(DefaultValue = true)]
    private bool showClearTextButton;

    [DependencyProperty(
        DefaultValue = "",
        PropertyChangedCallback = nameof(OnTextLostFocus),
        CoerceValueCallback = nameof(CoerceText),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        IsAnimationProhibited = true,
        DefaultUpdateSourceTrigger = UpdateSourceTrigger.LostFocus)]
    private string text;

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