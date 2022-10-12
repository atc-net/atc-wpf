namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelTextInfo.
/// </summary>
public partial class LabelTextInfo : ILabelTextInfo
{
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation),
        typeof(Orientation),
        typeof(LabelTextInfo),
        new PropertyMetadata(default(Orientation)));

    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public static readonly DependencyProperty LabelTextProperty = DependencyProperty.Register(
        nameof(LabelText),
        typeof(string),
        typeof(LabelTextInfo),
        new PropertyMetadata(defaultValue: string.Empty));

    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public static readonly DependencyProperty HideValidationTextAreaProperty = DependencyProperty.Register(
        nameof(HideValidationTextArea),
        typeof(bool),
        typeof(LabelTextInfo),
        new PropertyMetadata(defaultValue: false));

    public bool HideValidationTextArea
    {
        get => (bool)GetValue(HideValidationTextAreaProperty);
        set => SetValue(HideValidationTextAreaProperty, value);
    }

    public static readonly DependencyProperty InformationTextProperty = DependencyProperty.Register(
        nameof(InformationText),
        typeof(string),
        typeof(LabelTextInfo),
        new PropertyMetadata(defaultValue: string.Empty));

    public string InformationText
    {
        get => (string)GetValue(InformationTextProperty);
        set => SetValue(InformationTextProperty, value);
    }

    public static readonly DependencyProperty InformationColorProperty = DependencyProperty.Register(
        nameof(InformationColor),
        typeof(Color),
        typeof(LabelTextInfo),
        new PropertyMetadata(Colors.DodgerBlue));

    public Color InformationColor
    {
        get => (Color)GetValue(InformationColorProperty);
        set => SetValue(InformationColorProperty, value);
    }

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(LabelTextInfo),
        new PropertyMetadata(defaultValue: string.Empty));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public LabelTextInfo()
    {
        InitializeComponent();
    }
}