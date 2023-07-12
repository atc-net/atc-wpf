// ReSharper disable InvertIf
// ReSharper disable RedundantJumpStatement
// ReSharper disable ConvertIfStatementToReturnStatement
namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelTextBox.
/// </summary>
public partial class LabelTextBox : ILabelTextBox
{
    public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent(
        nameof(TextChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedPropertyChangedEventHandler<string>),
        typeof(LabelPixelSizeBox));

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

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(LabelTextBox),
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

    public event EventHandler<ChangedStringEventArgs>? TextLostFocusValid;

    public event EventHandler<ChangedStringEventArgs>? TextLostFocusInvalid;

    public LabelTextBox()
    {
        InitializeComponent();
    }

    public override bool IsValid()
    {
        ValidateText(default, this, raiseEvents: false);
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
        => value ?? string.Empty;

    private void OnTextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        var control = (TextBox)sender;

        RaiseEvent(new RoutedPropertyChangedEventArgs<string>(string.Empty, control.Text, TextChangedEvent));
    }

    private static void OnTextLostFocus(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var control = (LabelTextBox)d;

        ValidateText(e, control, raiseEvents: true);
    }

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
    [SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "OK.")]
    private static void ValidateText(
        DependencyPropertyChangedEventArgs e,
        LabelTextBox control,
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
            control.ValidationText = string.Format(CultureInfo.CurrentUICulture, Validations.MinValueFormat1, control.MinLength);
            if (raiseEvents)
            {
                OnTextLostFocusFireInvalidEvent(control, e);
            }

            return;
        }

        if (control.Text.Length > control.MaxLength)
        {
            control.ValidationText = string.Format(CultureInfo.CurrentUICulture, Validations.MaxValueFormat1, control.MaxLength);

            if (raiseEvents)
            {
                OnTextLostFocusFireInvalidEvent(control, e);
            }

            return;
        }

        if (!string.IsNullOrEmpty(control.CharactersNotAllowed) &&
            control.CharactersNotAllowed.Any(x => control.Text.Contains(x, StringComparison.OrdinalIgnoreCase)))
        {
            control.ValidationText = string.Format(CultureInfo.CurrentUICulture, Validations.NotAllowedFormat1, GetOnlyUsedNotAllowedCharacters(control.CharactersNotAllowed, control.Text));
            if (raiseEvents)
            {
                OnTextLostFocusFireInvalidEvent(control, e);
            }

            return;
        }

        var (isValid, errorMessage) = TextBoxValidationHelper.Validate(
            control.ValidationFormat,
            control.Text);

        if (isValid)
        {
            control.ValidationText = string.Empty;
            if (raiseEvents)
            {
                OnTextLostFocusFireValidEvent(control, e);
            }
        }
        else
        {
            control.ValidationText = errorMessage;
            if (raiseEvents)
            {
                OnTextLostFocusFireInvalidEvent(control, e);
            }
        }
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
    private static void OnTextLostFocusFireValidEvent(
        LabelTextBox control,
        DependencyPropertyChangedEventArgs e)
    {
        var newValue = e.NewValue is null
            ? string.Empty
            : e.NewValue.ToString();

        control.TextLostFocusValid?.Invoke(
            control,
            new ChangedStringEventArgs(
                ControlHelper.GetIdentifier(control),
                oldValue: null,
                newValue));
    }

    [SuppressMessage("Usage", "MA0091:Sender should be 'this' for instance events", Justification = "OK - 'this' cant be used in a static method.")]
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
            new ChangedStringEventArgs(
                ControlHelper.GetIdentifier(control),
                oldValue,
                newValue));
    }
}