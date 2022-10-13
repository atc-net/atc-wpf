namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelDecimalBox.
/// </summary>
public partial class LabelDecimalBox : ILabelDecimalBox
{
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation),
        typeof(Orientation),
        typeof(LabelDecimalBox),
        new PropertyMetadata(default(Orientation)));

    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public static readonly DependencyProperty ShowAsteriskOnMandatoryProperty = DependencyProperty.Register(
        nameof(ShowAsteriskOnMandatory),
        typeof(bool),
        typeof(LabelDecimalBox),
        new PropertyMetadata(defaultValue: true));

    public bool ShowAsteriskOnMandatory
    {
        get => (bool)GetValue(ShowAsteriskOnMandatoryProperty);
        set => SetValue(ShowAsteriskOnMandatoryProperty, value);
    }

    public static readonly DependencyProperty IsMandatoryProperty = DependencyProperty.Register(
        nameof(IsMandatory),
        typeof(bool),
        typeof(LabelDecimalBox),
        new PropertyMetadata(defaultValue: false));

    public bool IsMandatory
    {
        get => (bool)GetValue(IsMandatoryProperty);
        set => SetValue(IsMandatoryProperty, value);
    }

    public static readonly DependencyProperty MandatoryColorProperty = DependencyProperty.Register(
        nameof(MandatoryColor),
        typeof(SolidColorBrush),
        typeof(LabelDecimalBox),
        new PropertyMetadata(new SolidColorBrush(Colors.Red)));

    public SolidColorBrush MandatoryColor
    {
        get => (SolidColorBrush)GetValue(MandatoryColorProperty);
        set => SetValue(MandatoryColorProperty, value);
    }

    public static readonly DependencyProperty LabelTextProperty = DependencyProperty.Register(
        nameof(LabelText),
        typeof(string),
        typeof(LabelDecimalBox),
        new PropertyMetadata(defaultValue: string.Empty));

    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public static readonly DependencyProperty HideValidationTextAreaProperty = DependencyProperty.Register(
        nameof(HideValidationTextArea),
        typeof(bool),
        typeof(LabelDecimalBox),
        new PropertyMetadata(defaultValue: false));

    public bool HideValidationTextArea
    {
        get => (bool)GetValue(HideValidationTextAreaProperty);
        set => SetValue(HideValidationTextAreaProperty, value);
    }

    public static readonly DependencyProperty ValidationColorProperty = DependencyProperty.Register(
        nameof(ValidationColor),
        typeof(SolidColorBrush),
        typeof(LabelDecimalBox),
        new PropertyMetadata(new SolidColorBrush(Colors.OrangeRed)));

    public SolidColorBrush ValidationColor
    {
        get => (SolidColorBrush)GetValue(ValidationColorProperty);
        set => SetValue(ValidationColorProperty, value);
    }

    public static readonly DependencyProperty ValidationTextProperty = DependencyProperty.Register(
        nameof(ValidationText),
        typeof(string),
        typeof(LabelDecimalBox),
        new PropertyMetadata(default(string)));

    public string ValidationText
    {
        get => (string)GetValue(ValidationTextProperty);
        set => SetValue(ValidationTextProperty, value);
    }

    public static readonly DependencyProperty InformationTextProperty = DependencyProperty.Register(
        nameof(InformationText),
        typeof(string),
        typeof(LabelDecimalBox),
        new PropertyMetadata(defaultValue: string.Empty));

    public string InformationText
    {
        get => (string)GetValue(InformationTextProperty);
        set => SetValue(InformationTextProperty, value);
    }

    public static readonly DependencyProperty InformationColorProperty = DependencyProperty.Register(
        nameof(InformationColor),
        typeof(Color),
        typeof(LabelDecimalBox),
        new PropertyMetadata(Colors.DodgerBlue));

    public Color InformationColor
    {
        get => (Color)GetValue(InformationColorProperty);
        set => SetValue(InformationColorProperty, value);
    }

    public static readonly DependencyProperty WatermarkTextProperty = DependencyProperty.Register(
        nameof(WatermarkText),
        typeof(string),
        typeof(LabelDecimalBox),
        new PropertyMetadata(defaultValue: string.Empty));

    public string WatermarkText
    {
        get => (string)GetValue(WatermarkTextProperty);
        set => SetValue(WatermarkTextProperty, value);
    }

    public static readonly DependencyProperty WatermarkAlignmentProperty = DependencyProperty.Register(
        nameof(WatermarkAlignment),
        typeof(TextAlignment),
        typeof(LabelDecimalBox),
        new PropertyMetadata(default(TextAlignment)));

    public TextAlignment WatermarkAlignment
    {
        get => (TextAlignment)GetValue(WatermarkAlignmentProperty);
        set => SetValue(WatermarkAlignmentProperty, value);
    }

    public static readonly DependencyProperty PrefixTextProperty = DependencyProperty.Register(
        nameof(PrefixText),
        typeof(string),
        typeof(LabelDecimalBox),
        new FrameworkPropertyMetadata(
            string.Empty,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

    public string PrefixText
    {
        get => (string)GetValue(PrefixTextProperty);
        set => SetValue(PrefixTextProperty, value);
    }

    public static readonly DependencyProperty SuffixTextProperty = DependencyProperty.Register(
        nameof(SuffixText),
        typeof(string),
        typeof(LabelDecimalBox),
        new FrameworkPropertyMetadata(
            string.Empty,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

    public string SuffixText
    {
        get => (string)GetValue(SuffixTextProperty);
        set => SetValue(SuffixTextProperty, value);
    }

    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
        nameof(Maximum),
        typeof(decimal),
        typeof(LabelDecimalBox),
        new FrameworkPropertyMetadata(
            decimal.MaxValue,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

    public decimal Maximum
    {
        get => (decimal)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
        nameof(Minimum),
        typeof(decimal),
        typeof(LabelDecimalBox),
        new FrameworkPropertyMetadata(
            decimal.MinValue,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

    public decimal Minimum
    {
        get => (decimal)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value),
        typeof(decimal),
        typeof(LabelDecimalBox),
        new FrameworkPropertyMetadata(
            default(decimal),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            propertyChangedCallback: null,
            coerceValueCallback: null,
            isAnimationProhibited: true,
            UpdateSourceTrigger.LostFocus));

    [SuppressMessage("Naming", "CA1721:Property names should not match get methods", Justification = "OK.")]
    public decimal Value
    {
        get => (decimal)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public LabelDecimalBox()
    {
        InitializeComponent();
    }
}