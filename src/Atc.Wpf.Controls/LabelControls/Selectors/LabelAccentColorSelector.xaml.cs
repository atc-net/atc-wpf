// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls.LabelControls;

/// <summary>
/// Interaction logic for LabelAccentColorSelector.
/// </summary>
public partial class LabelAccentColorSelector : ILabelControlBase
{
    public static readonly DependencyProperty HideAsteriskAreaProperty = DependencyProperty.Register(
        nameof(HideAsteriskArea),
        typeof(bool),
        typeof(LabelAccentColorSelector),
        new PropertyMetadata(defaultValue: false));

    public bool HideAsteriskArea
    {
        get => (bool)GetValue(HideAsteriskAreaProperty);
        set => SetValue(HideAsteriskAreaProperty, value);
    }

    public static readonly DependencyProperty HideInformationAreaProperty = DependencyProperty.Register(
        nameof(HideInformationArea),
        typeof(bool),
        typeof(LabelAccentColorSelector),
        new PropertyMetadata(defaultValue: false));

    public bool HideInformationArea
    {
        get => (bool)GetValue(HideInformationAreaProperty);
        set => SetValue(HideInformationAreaProperty, value);
    }

    public static readonly DependencyProperty HideValidationTextAreaProperty = DependencyProperty.Register(
        nameof(HideValidationTextArea),
        typeof(bool),
        typeof(LabelAccentColorSelector),
        new PropertyMetadata(defaultValue: false));

    public bool HideValidationTextArea
    {
        get => (bool)GetValue(HideValidationTextAreaProperty);
        set => SetValue(HideValidationTextAreaProperty, value);
    }

    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation),
        typeof(Orientation),
        typeof(LabelAccentColorSelector),
        new PropertyMetadata(default(Orientation)));

    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public static readonly DependencyProperty LabelWidthNumberProperty = DependencyProperty.Register(
        nameof(LabelWidthNumber),
        typeof(int),
        typeof(LabelAccentColorSelector),
        new PropertyMetadata(30));

    public int LabelWidthNumber
    {
        get => (int)GetValue(LabelWidthNumberProperty);
        set => SetValue(LabelWidthNumberProperty, value);
    }

    public static readonly DependencyProperty LabelWidthSizeDefinitionProperty = DependencyProperty.Register(
        nameof(LabelWidthSizeDefinition),
        typeof(SizeDefinitionType),
        typeof(LabelAccentColorSelector),
        new PropertyMetadata(SizeDefinitionType.Percentage));

    public SizeDefinitionType LabelWidthSizeDefinition
    {
        get => (SizeDefinitionType)GetValue(LabelWidthSizeDefinitionProperty);
        set => SetValue(LabelWidthSizeDefinitionProperty, value);
    }

    public static readonly DependencyProperty InformationTextProperty = DependencyProperty.Register(
        nameof(InformationText),
        typeof(string),
        typeof(LabelAccentColorSelector),
        new PropertyMetadata(defaultValue: string.Empty));

    public string InformationText
    {
        get => (string)GetValue(InformationTextProperty);
        set => SetValue(InformationTextProperty, value);
    }

    public static readonly DependencyProperty InformationColorProperty = DependencyProperty.Register(
        nameof(InformationColor),
        typeof(Color),
        typeof(LabelAccentColorSelector),
        new PropertyMetadata(Colors.DodgerBlue));

    public Color InformationColor
    {
        get => (Color)GetValue(InformationColorProperty);
        set => SetValue(InformationColorProperty, value);
    }

    public static readonly DependencyProperty RenderColorIndicatorTypeProperty = DependencyProperty.Register(
        nameof(RenderColorIndicatorType),
        typeof(RenderColorIndicatorType),
        typeof(LabelAccentColorSelector),
        new PropertyMetadata(RenderColorIndicatorType.Square));

    public RenderColorIndicatorType RenderColorIndicatorType
    {
        get => (RenderColorIndicatorType)GetValue(RenderColorIndicatorTypeProperty);
        set => SetValue(RenderColorIndicatorTypeProperty, value);
    }

    public LabelAccentColorSelector()
    {
        InitializeComponent();
    }
}