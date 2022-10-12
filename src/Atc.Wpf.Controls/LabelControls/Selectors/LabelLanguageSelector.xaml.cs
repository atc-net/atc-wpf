// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelLanguageSelector.
/// </summary>
public partial class LabelLanguageSelector : ILabelControlBase
{
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
    nameof(Orientation),
    typeof(Orientation),
    typeof(LabelLanguageSelector),
    new PropertyMetadata(default(Orientation)));

    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public static readonly DependencyProperty HideValidationTextAreaProperty = DependencyProperty.Register(
        nameof(HideValidationTextArea),
        typeof(bool),
        typeof(LabelLanguageSelector),
        new PropertyMetadata(defaultValue: true));

    public bool HideValidationTextArea
    {
        get => (bool)GetValue(HideValidationTextAreaProperty);
        set => SetValue(HideValidationTextAreaProperty, value);
    }

    public static readonly DependencyProperty InformationTextProperty = DependencyProperty.Register(
        nameof(InformationText),
        typeof(string),
        typeof(LabelLanguageSelector),
        new PropertyMetadata(defaultValue: string.Empty));

    public string InformationText
    {
        get => (string)GetValue(InformationTextProperty);
        set => SetValue(InformationTextProperty, value);
    }

    public static readonly DependencyProperty InformationColorProperty = DependencyProperty.Register(
        nameof(InformationColor),
        typeof(Color),
        typeof(LabelLanguageSelector),
        new PropertyMetadata(Colors.DodgerBlue));

    public Color InformationColor
    {
        get => (Color)GetValue(InformationColorProperty);
        set => SetValue(InformationColorProperty, value);
    }

    public LabelLanguageSelector()
    {
        InitializeComponent();

        DataContext = this;
    }
}