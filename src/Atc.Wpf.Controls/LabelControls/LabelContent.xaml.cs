namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelContent.
/// </summary>
public partial class LabelContent : ILabelTextControl
{
    public static readonly DependencyProperty HideAsteriskAreaProperty = DependencyProperty.Register(
        nameof(HideAsteriskArea),
        typeof(bool),
        typeof(LabelContent),
        new PropertyMetadata(defaultValue: false));

    public bool HideAsteriskArea
    {
        get => (bool)GetValue(HideAsteriskAreaProperty);
        set => SetValue(HideAsteriskAreaProperty, value);
    }

    public static readonly DependencyProperty HideInformationAreaProperty = DependencyProperty.Register(
        nameof(HideInformationArea),
        typeof(bool),
        typeof(LabelContent),
        new PropertyMetadata(defaultValue: false));

    public bool HideInformationArea
    {
        get => (bool)GetValue(HideInformationAreaProperty);
        set => SetValue(HideInformationAreaProperty, value);
    }

    public static readonly DependencyProperty HideValidationTextAreaProperty = DependencyProperty.Register(
        nameof(HideValidationTextArea),
        typeof(bool),
        typeof(LabelContent),
        new PropertyMetadata(defaultValue: false));

    public bool HideValidationTextArea
    {
        get => (bool)GetValue(HideValidationTextAreaProperty);
        set => SetValue(HideValidationTextAreaProperty, value);
    }

    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation),
        typeof(Orientation),
        typeof(LabelContent),
        new PropertyMetadata(default(Orientation)));

    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public static readonly DependencyProperty ShowAsteriskOnMandatoryProperty = DependencyProperty.Register(
        nameof(ShowAsteriskOnMandatory),
        typeof(bool),
        typeof(LabelContent),
        new PropertyMetadata(defaultValue: true));

    public bool ShowAsteriskOnMandatory
    {
        get => (bool)GetValue(ShowAsteriskOnMandatoryProperty);
        set => SetValue(ShowAsteriskOnMandatoryProperty, value);
    }

    public static readonly DependencyProperty IsMandatoryProperty = DependencyProperty.Register(
        nameof(IsMandatory),
        typeof(bool),
        typeof(LabelContent),
        new PropertyMetadata(defaultValue: false));

    public bool IsMandatory
    {
        get => (bool)GetValue(IsMandatoryProperty);
        set => SetValue(IsMandatoryProperty, value);
    }

    public static readonly DependencyProperty MandatoryColorProperty = DependencyProperty.Register(
        nameof(MandatoryColor),
        typeof(SolidColorBrush),
        typeof(LabelContent),
        new PropertyMetadata(new SolidColorBrush(Colors.Red)));

    public SolidColorBrush MandatoryColor
    {
        get => (SolidColorBrush)GetValue(MandatoryColorProperty);
        set => SetValue(MandatoryColorProperty, value);
    }

    public static readonly DependencyProperty LabelWidthNumberProperty = DependencyProperty.Register(
        nameof(LabelWidthNumber),
        typeof(int),
        typeof(LabelContent),
        new PropertyMetadata(30));

    public int LabelWidthNumber
    {
        get => (int)GetValue(LabelWidthNumberProperty);
        set => SetValue(LabelWidthNumberProperty, value);
    }

    public static readonly DependencyProperty LabelWidthSizeDefinitionProperty = DependencyProperty.Register(
        nameof(LabelWidthSizeDefinition),
        typeof(SizeDefinitionType),
        typeof(LabelContent),
        new PropertyMetadata(SizeDefinitionType.Percentage));

    public SizeDefinitionType LabelWidthSizeDefinition
    {
        get => (SizeDefinitionType)GetValue(LabelWidthSizeDefinitionProperty);
        set => SetValue(LabelWidthSizeDefinitionProperty, value);
    }

    public static readonly DependencyProperty LabelTextProperty = DependencyProperty.Register(
        nameof(LabelText),
        typeof(string),
        typeof(LabelContent),
        new PropertyMetadata(defaultValue: string.Empty));

    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public static readonly DependencyProperty ValidationColorProperty = DependencyProperty.Register(
        nameof(ValidationColor),
        typeof(SolidColorBrush),
        typeof(LabelContent),
        new PropertyMetadata(new SolidColorBrush(Colors.OrangeRed)));

    public SolidColorBrush ValidationColor
    {
        get => (SolidColorBrush)GetValue(ValidationColorProperty);
        set => SetValue(ValidationColorProperty, value);
    }

    public static readonly DependencyProperty ValidationTextProperty = DependencyProperty.Register(
        nameof(ValidationText),
        typeof(string),
        typeof(LabelContent),
        new PropertyMetadata(default(string)));

    public string ValidationText
    {
        get => (string)GetValue(ValidationTextProperty);
        set => SetValue(ValidationTextProperty, value);
    }

    public static readonly DependencyProperty InformationTextProperty = DependencyProperty.Register(
        nameof(InformationText),
        typeof(string),
        typeof(LabelContent),
        new PropertyMetadata(defaultValue: string.Empty));

    public string InformationText
    {
        get => (string)GetValue(InformationTextProperty);
        set => SetValue(InformationTextProperty, value);
    }

    public static readonly DependencyProperty InformationColorProperty = DependencyProperty.Register(
        nameof(InformationColor),
        typeof(Color),
        typeof(LabelContent),
        new PropertyMetadata(Colors.DodgerBlue));

    public Color InformationColor
    {
        get => (Color)GetValue(InformationColorProperty);
        set => SetValue(InformationColorProperty, value);
    }

    public LabelContent()
    {
        InitializeComponent();
    }
}