// ReSharper disable InvertIf
// ReSharper disable RedundantJumpStatement
namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelTextBox.
/// </summary>
public partial class LabelTextBox : ILabelTextBox
{
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
        new PropertyMetadata(defaultValue: true));

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
        new PropertyMetadata(defaultValue: true));

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
            OnTextPropertyChanged,
            CoerceText,
            isAnimationProhibited: true,
            UpdateSourceTrigger.LostFocus));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public LabelTextBox()
    {
        InitializeComponent();
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

    private static void OnTextPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var labelTextBox = (LabelTextBox)d;

        if (labelTextBox.IsMandatory &&
            string.IsNullOrWhiteSpace(labelTextBox.Text))
        {
            labelTextBox.ValidationText = "Field is required";
            return;
        }

        if (labelTextBox.Text.Length < labelTextBox.MinLength)
        {
            labelTextBox.ValidationText = $"Min length: {labelTextBox.MinLength}";
            return;
        }

        if (labelTextBox.Text.Length > labelTextBox.MaxLength)
        {
            labelTextBox.ValidationText = $"Max length: {labelTextBox.MaxLength}";
            return;
        }

        if (!string.IsNullOrEmpty(labelTextBox.CharactersNotAllowed)
            && labelTextBox.CharactersNotAllowed.Any(x => labelTextBox.Text.Contains(x, StringComparison.OrdinalIgnoreCase)))
        {
            labelTextBox.ValidationText = $"Not allowed: {GetOnlyUsedNotAllowedCharacters(labelTextBox.CharactersNotAllowed, labelTextBox.Text)}";
            return;
        }

        labelTextBox.ValidationText = string.Empty;
    }
}