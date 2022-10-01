namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelTextBox.
/// </summary>
public partial class LabelTextBox : ILabelTextBox
{
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation),
        typeof(Orientation),
        typeof(LabelTextBox),
        new PropertyMetadata(default(Orientation)));

    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public static readonly DependencyProperty ShowAsteriskOnMandatoryProperty = DependencyProperty.Register(
        nameof(ShowAsteriskOnMandatory),
        typeof(bool),
        typeof(LabelTextBox),
        new PropertyMetadata(defaultValue: true));

    public bool ShowAsteriskOnMandatory
    {
        get => (bool)GetValue(ShowAsteriskOnMandatoryProperty);
        set => SetValue(ShowAsteriskOnMandatoryProperty, value);
    }

    public static readonly DependencyProperty IsMandatoryProperty = DependencyProperty.Register(
        nameof(IsMandatory),
        typeof(bool),
        typeof(LabelTextBox),
        new PropertyMetadata(defaultValue: false));

    public bool IsMandatory
    {
        get => (bool)GetValue(IsMandatoryProperty);
        set => SetValue(IsMandatoryProperty, value);
    }

    public static readonly DependencyProperty MandatoryColorProperty = DependencyProperty.Register(
        nameof(MandatoryColor),
        typeof(SolidColorBrush),
        typeof(LabelTextBox),
        new PropertyMetadata(new SolidColorBrush(Colors.Red)));

    public SolidColorBrush MandatoryColor
    {
        get => (SolidColorBrush)GetValue(MandatoryColorProperty);
        set => SetValue(MandatoryColorProperty, value);
    }

    public static readonly DependencyProperty LabelTextProperty = DependencyProperty.Register(
        nameof(LabelText),
        typeof(string),
        typeof(LabelTextBox),
        new PropertyMetadata(string.Empty));

    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public static readonly DependencyProperty ValidationColorProperty = DependencyProperty.Register(
        nameof(ValidationColor),
        typeof(SolidColorBrush),
        typeof(LabelTextBox),
        new PropertyMetadata(new SolidColorBrush(Colors.OrangeRed)));

    public SolidColorBrush ValidationColor
    {
        get => (SolidColorBrush)GetValue(ValidationColorProperty);
        set => SetValue(ValidationColorProperty, value);
    }

    public static readonly DependencyProperty InformationTextProperty = DependencyProperty.Register(
        nameof(InformationText),
        typeof(string),
        typeof(LabelTextBox),
        new PropertyMetadata(string.Empty));

    public string InformationText
    {
        get => (string)GetValue(InformationTextProperty);
        set => SetValue(InformationTextProperty, value);
    }

    public static readonly DependencyProperty InformationColorProperty = DependencyProperty.Register(
        nameof(InformationColor),
        typeof(Color),
        typeof(LabelTextBox),
        new PropertyMetadata(Colors.DodgerBlue));

    public Color InformationColor
    {
        get => (Color)GetValue(InformationColorProperty);
        set => SetValue(InformationColorProperty, value);
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

    public static readonly DependencyProperty UseDefaultNotAllowedCharactersProperty = DependencyProperty.Register(
        nameof(UseDefaultNotAllowedCharacters),
        typeof(bool),
        typeof(LabelTextBox),
        new PropertyMetadata(default(bool)));

    public bool UseDefaultNotAllowedCharacters
    {
        get => (bool)GetValue(UseDefaultNotAllowedCharactersProperty);
        set => SetValue(UseDefaultNotAllowedCharactersProperty, value);
    }

    public static readonly DependencyProperty CharactersNotAllowedProperty = DependencyProperty.Register(
        nameof(CharactersNotAllowed),
        typeof(string),
        typeof(LabelTextBox),
        new PropertyMetadata(string.Empty));

    public string CharactersNotAllowed
    {
        get => (string)GetValue(CharactersNotAllowedProperty);
        set => SetValue(CharactersNotAllowedProperty, value);
    }

    public static readonly DependencyProperty ControlValueProperty = DependencyProperty.Register(
        nameof(ControlValue),
        typeof(string),
        typeof(LabelTextBox),
        new PropertyMetadata(string.Empty));

    public string ControlValue
    {
        get => (string)GetValue(ControlValueProperty);
        set => SetValue(ControlValueProperty, value);
    }

    public static readonly DependencyProperty WatermarkEnableProperty = DependencyProperty.Register(
        nameof(WatermarkEnable),
        typeof(bool),
        typeof(LabelTextBox),
        new PropertyMetadata(default(bool)));

    public bool WatermarkEnable
    {
        get => (bool)GetValue(WatermarkEnableProperty);
        set => SetValue(WatermarkEnableProperty, value);
    }

    public static readonly DependencyProperty WatermarkLabelProperty = DependencyProperty.Register(
        nameof(WatermarkLabel),
        typeof(string),
        typeof(LabelTextBox),
        new PropertyMetadata(string.Empty));

    public string WatermarkLabel
    {
        get => (string)GetValue(WatermarkLabelProperty);
        set => SetValue(WatermarkLabelProperty, value);
    }

    public LabelTextBox()
    {
        InitializeComponent();

        DataContext = this;
    }
}