// ReSharper disable InvertIf
// ReSharper disable RedundantJumpStatement
// ReSharper disable ConvertIfStatementToReturnStatement
namespace Atc.Wpf.Controls.LabelControls;

[SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
[SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "OK.")]
public partial class LabelTextBox : ILabelTextBox
{
    public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(TextChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedPropertyChangedEventHandler<string>),
        typeof(LabelTextBox));

    public event RoutedPropertyChangedEventHandler<string> TextChanged
    {
        add => AddHandler(TextChangedEvent, value);
        remove => RemoveHandler(TextChangedEvent, value);
    }

    public static readonly DependencyProperty WatermarkTextProperty = DependencyProperty.Register(
        nameof(WatermarkText),
        typeof(string),
        typeof(LabelTextBox),
        new PropertyMetadata(defaultValue: string.Empty));

    public string WatermarkText
    {
        get => (string)GetValue(WatermarkTextProperty);
        set => SetValue(WatermarkTextProperty, value);
    }

    public static readonly DependencyProperty WatermarkAlignmentProperty = DependencyProperty.Register(
        nameof(WatermarkAlignment),
        typeof(TextAlignment),
        typeof(LabelTextBox),
        new PropertyMetadata(default(TextAlignment)));

    public TextAlignment WatermarkAlignment
    {
        get => (TextAlignment)GetValue(WatermarkAlignmentProperty);
        set => SetValue(WatermarkAlignmentProperty, value);
    }

    public static readonly DependencyProperty WatermarkTrimmingProperty = DependencyProperty.Register(
        nameof(WatermarkTrimming),
        typeof(TextTrimming),
        typeof(LabelTextBox),
        new PropertyMetadata(default(TextTrimming)));

    public TextTrimming WatermarkTrimming
    {
        get => (TextTrimming)GetValue(WatermarkTrimmingProperty);
        set => SetValue(WatermarkTrimmingProperty, value);
    }

    public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register(
        nameof(MaxLength),
        typeof(uint),
        typeof(LabelTextBox),
        new PropertyMetadata(100U));

    public uint MaxLength
    {
        get => (uint)GetValue(MaxLengthProperty);
        set => SetValue(MaxLengthProperty, value);
    }

    public static readonly DependencyProperty MinLengthProperty = DependencyProperty.Register(
        nameof(MinLength),
        typeof(uint),
        typeof(LabelTextBox),
        new PropertyMetadata(0U));

    public uint MinLength
    {
        get => (uint)GetValue(MinLengthProperty);
        set => SetValue(MinLengthProperty, value);
    }

    public static readonly DependencyProperty UseDefaultNotAllowedCharactersProperty = DependencyProperty.Register(
        nameof(UseDefaultNotAllowedCharacters),
        typeof(bool),
        typeof(LabelTextBox),
        new PropertyMetadata(defaultValue: BooleanBoxes.TrueBox));

    public bool UseDefaultNotAllowedCharacters
    {
        get => (bool)GetValue(UseDefaultNotAllowedCharactersProperty);
        set => SetValue(UseDefaultNotAllowedCharactersProperty, value);
    }

    public static readonly DependencyProperty CharactersNotAllowedProperty = DependencyProperty.Register(
        nameof(CharactersNotAllowed),
        typeof(string),
        typeof(LabelTextBox),
        new PropertyMetadata(defaultValue: string.Empty));

    public string CharactersNotAllowed
    {
        get => (string)GetValue(CharactersNotAllowedProperty);
        set => SetValue(CharactersNotAllowedProperty, value);
    }

    public static readonly DependencyProperty RegexPatternProperty = DependencyProperty.Register(
        nameof(RegexPattern),
        typeof(string),
        typeof(LabelTextBox),
        new PropertyMetadata(defaultValue: null));

    public string? RegexPattern
    {
        get => (string?)GetValue(RegexPatternProperty);
        set => SetValue(RegexPatternProperty, value);
    }

    public static readonly DependencyProperty ShowClearTextButtonProperty = DependencyProperty.Register(
        nameof(ShowClearTextButton),
        typeof(bool),
        typeof(LabelTextBox),
        new PropertyMetadata(defaultValue: BooleanBoxes.TrueBox));

    public bool ShowClearTextButton
    {
        get => (bool)GetValue(ShowClearTextButtonProperty);
        set => SetValue(ShowClearTextButtonProperty, value);
    }

    public static readonly DependencyProperty TriggerOnlyOnLostFocusProperty =
        DependencyProperty.Register(
            nameof(TriggerOnlyOnLostFocus),
            typeof(bool),
            typeof(LabelTextBox),
            new PropertyMetadata(defaultValue: BooleanBoxes.TrueBox));

    public bool TriggerOnlyOnLostFocus
    {
        get => (bool)GetValue(TriggerOnlyOnLostFocusProperty);
        set => SetValue(TriggerOnlyOnLostFocusProperty, value);
    }

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(LabelTextBox),
        new FrameworkPropertyMetadata(
            string.Empty,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            OnTextChanged,
            CoerceText,
            isAnimationProhibited: true,
            UpdateSourceTrigger.PropertyChanged));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty ValidationFormatProperty = DependencyProperty.Register(
        nameof(ValidationFormat),
        typeof(TextBoxValidationRuleType),
        typeof(LabelTextBox),
        new PropertyMetadata(TextBoxValidationRuleType.None));

    public TextBoxValidationRuleType ValidationFormat
    {
        get => (TextBoxValidationRuleType)GetValue(ValidationFormatProperty);
        set => SetValue(ValidationFormatProperty, value);
    }

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