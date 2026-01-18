// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable InconsistentNaming
// ReSharper disable InvertIf
// ReSharper disable RedundantJumpStatement
namespace Atc.Wpf.Forms;

[SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
[SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "OK.")]
public partial class LabelTextBox : ILabelTextBox
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
    private bool useDefaultNotAllowedCharacters;

    [DependencyProperty(DefaultValue = "")]
    private string charactersNotAllowed;

    [DependencyProperty]
    private string? regexPattern;

    [DependencyProperty(DefaultValue = true)]
    private bool showClearTextButton;

    [DependencyProperty(DefaultValue = true)]
    private bool triggerOnlyOnLostFocus;

    [DependencyProperty(DefaultValue = TextBoxValidationRuleType.None)]
    private TextBoxValidationRuleType validationFormat;

    [DependencyProperty(
        DefaultValue = "",
        PropertyChangedCallback = nameof(OnTextChanged),
        CoerceValueCallback = nameof(CoerceText),
        Flags = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
        IsAnimationProhibited = true,
        DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged)]
    private string text;

    public event EventHandler<ValueChangedEventArgs<string?>>? TextLostFocusValid;

    public event EventHandler<ValueChangedEventArgs<string?>>? TextLostFocusInvalid;

    public LabelTextBox()
    {
        InitializeComponent();
    }

    public override bool IsValid()
    {
        ValidateText(
            default,
            this,
            isCalledFromLostFocus: false,
            isCalledFromIsValid: true);

        return string.IsNullOrEmpty(ValidationText);
    }

    private static string GetOnlyUsedNotAllowedCharacters(
        string charactersNotAllowed,
        string text)
    {
        var sb = new StringBuilder();

        foreach (var ch in charactersNotAllowed.Where(ch => text.Contains(ch, StringComparison.OrdinalIgnoreCase)))
        {
            if (sb.Length != 0)
            {
                sb.Append(' ');
            }

            sb.Append(ch);
        }

        return sb.ToString();
    }

    private static object CoerceText(
        DependencyObject d,
        object? value)
    {
        if (value is not null &&
            value.ToString()!.Length == 0)
        {
            var control = (LabelTextBox)d;
            if (!string.IsNullOrEmpty(control.ValidationText) &&
                string.IsNullOrEmpty(control.Text))
            {
                control.ValidationText = string.Empty;
            }
        }

        return value ?? string.Empty;
    }

    private static void OnTextChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelTextBox)d;

        ValidateText(
            e,
            control,
            isCalledFromLostFocus: false,
            isCalledFromIsValid: false);

        var oldValue = e.OldValue is null
            ? string.Empty
            : e.OldValue.ToString();

        control.RaiseEvent(
            new RoutedPropertyChangedEventArgs<string>(
                oldValue!,
                control.Text,
                TextChangedEvent));
    }

    private void OnLostFocus(
        object sender,
        RoutedEventArgs e)
    {
        ValidateText(
            default,
            this,
            isCalledFromLostFocus: true,
            isCalledFromIsValid: false);
    }

    private static void ValidateText(
        DependencyPropertyChangedEventArgs e,
        LabelTextBox control,
        bool isCalledFromLostFocus,
        bool isCalledFromIsValid)
    {
        if (!isCalledFromIsValid &&
            !isCalledFromLostFocus &&
            control.TriggerOnlyOnLostFocus &&
            string.IsNullOrEmpty(control.ValidationText))
        {
            return;
        }

        switch (control.IsMandatory)
        {
            case true when
                string.IsNullOrWhiteSpace(control.Text):
                {
                    if (ControlStackTraceHelper.IsCalledFromClearCommand())
                    {
                        control.ValidationText = string.Empty;
                        return;
                    }

                    control.ValidationText = Validations.FieldIsRequired;
                    FireTextLostFocusFireInvalidEventIfNeeded(e, control, isCalledFromLostFocus);
                    return;
                }

            case false when
                string.IsNullOrWhiteSpace(control.Text):
                {
                    control.ValidationText = string.Empty;
                    return;
                }
        }

        if (control.Text.Length < control.MinLength)
        {
            if (ControlStackTraceHelper.IsCalledFromClearCommand())
            {
                control.ValidationText = string.Empty;
                return;
            }

            control.ValidationText = string.Format(
                CultureInfo.CurrentUICulture,
                Validations.MinValueFormat1,
                control.MinLength);

            FireTextLostFocusFireInvalidEventIfNeeded(e, control, isCalledFromLostFocus);
            return;
        }

        if (control.Text.Length > control.MaxLength)
        {
            control.ValidationText = string.Format(
                CultureInfo.CurrentUICulture,
                Validations.MaxValueFormat1,
                control.MaxLength);

            FireTextLostFocusFireInvalidEventIfNeeded(e, control, isCalledFromLostFocus);
            return;
        }

        if (!string.IsNullOrEmpty(control.CharactersNotAllowed) &&
            control.CharactersNotAllowed.Any(x => control.Text.Contains(x, StringComparison.OrdinalIgnoreCase)))
        {
            control.ValidationText = string.Format(
                CultureInfo.CurrentUICulture,
                Validations.NotAllowedFormat1,
                GetOnlyUsedNotAllowedCharacters(control.CharactersNotAllowed, control.Text));

            FireTextLostFocusFireInvalidEventIfNeeded(e, control, isCalledFromLostFocus);
            return;
        }

        if (!string.IsNullOrEmpty(control.RegexPattern))
        {
            var regex = new Regex(control.RegexPattern, RegexOptions.Multiline | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));
            if (!regex.Match(control.Text).Success)
            {
                control.ValidationText = Validations.RegexPatternDontMatch;
                FireTextLostFocusFireInvalidEventIfNeeded(e, control, isCalledFromLostFocus);
                return;
            }
        }

        var (isValid, errorMessage) = TextBoxValidationHelper.Validate(
            control.ValidationFormat,
            control.Text);

        if (isValid)
        {
            control.ValidationText = string.Empty;
            if (isCalledFromLostFocus)
            {
                OnTextLostFocusFireValidEvent(control, e);
            }
        }
        else
        {
            control.ValidationText = errorMessage;
            FireTextLostFocusFireInvalidEventIfNeeded(e, control, isCalledFromLostFocus);
        }
    }

    private static void FireTextLostFocusFireInvalidEventIfNeeded(
        DependencyPropertyChangedEventArgs e,
        LabelTextBox control,
        bool isCalledFromLostFocus)
    {
        if (!isCalledFromLostFocus)
        {
            return;
        }

        OnTextLostFocusFireInvalidEvent(control, e);
    }

    private static void OnTextLostFocusFireValidEvent(
        LabelTextBox control,
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
        LabelTextBox control,
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