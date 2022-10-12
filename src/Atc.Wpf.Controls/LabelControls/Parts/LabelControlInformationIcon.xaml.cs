namespace Atc.Wpf.Controls.LabelControls.Parts;

/// <summary>
/// Interaction logic for LabelControlInformationIcon.
/// </summary>
public partial class LabelControlInformationIcon
{
    public static readonly DependencyProperty InformationTextProperty = DependencyProperty.Register(
        nameof(InformationText),
        typeof(string),
        typeof(LabelControlInformationIcon),
        new PropertyMetadata(defaultValue: string.Empty));

    public string InformationText
    {
        get => (string)GetValue(InformationTextProperty);
        set => SetValue(InformationTextProperty, value);
    }

    public static readonly DependencyProperty InformationColorProperty = DependencyProperty.Register(
        nameof(InformationColor),
        typeof(Color),
        typeof(LabelControlInformationIcon),
        new PropertyMetadata(Colors.DodgerBlue));

    public Color InformationColor
    {
        get => (Color)GetValue(InformationColorProperty);
        set => SetValue(InformationColorProperty, value);
    }

    public LabelControlInformationIcon()
    {
        InitializeComponent();

        DataContext = this;
    }
}