namespace Atc.Wpf.Controls.LabelControls;

public class LabelControlBase : UserControl, ILabelControlBase
{
    public static readonly DependencyProperty HideAsteriskAreaProperty = DependencyProperty.Register(
        nameof(HideAsteriskArea),
        typeof(bool),
        typeof(LabelControlBase),
        new PropertyMetadata(defaultValue: false));

    public bool HideAsteriskArea
    {
        get => (bool)GetValue(HideAsteriskAreaProperty);
        set => SetValue(HideAsteriskAreaProperty, value);
    }

    public static readonly DependencyProperty HideInformationAreaProperty = DependencyProperty.Register(
        nameof(HideInformationArea),
        typeof(bool),
        typeof(LabelControlBase),
        new PropertyMetadata(defaultValue: false));

    public bool HideInformationArea
    {
        get => (bool)GetValue(HideInformationAreaProperty);
        set => SetValue(HideInformationAreaProperty, value);
    }

    public static readonly DependencyProperty HideValidationTextAreaProperty = DependencyProperty.Register(
        nameof(HideValidationTextArea),
        typeof(bool),
        typeof(LabelControlBase),
        new PropertyMetadata(defaultValue: false));

    public bool HideValidationTextArea
    {
        get => (bool)GetValue(HideValidationTextAreaProperty);
        set => SetValue(HideValidationTextAreaProperty, value);
    }

    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation),
        typeof(Orientation),
        typeof(LabelControlBase),
        new PropertyMetadata(default(Orientation)));

    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public static readonly DependencyProperty LabelWidthNumberProperty = DependencyProperty.Register(
        nameof(LabelWidthNumber),
        typeof(int),
        typeof(LabelControlBase),
        new PropertyMetadata(120));

    public int LabelWidthNumber
    {
        get => (int)GetValue(LabelWidthNumberProperty);
        set => SetValue(LabelWidthNumberProperty, value);
    }

    public static readonly DependencyProperty LabelWidthSizeDefinitionProperty = DependencyProperty.Register(
        nameof(LabelWidthSizeDefinition),
        typeof(SizeDefinitionType),
        typeof(LabelControlBase),
        new PropertyMetadata(SizeDefinitionType.Pixel));

    public SizeDefinitionType LabelWidthSizeDefinition
    {
        get => (SizeDefinitionType)GetValue(LabelWidthSizeDefinitionProperty);
        set => SetValue(LabelWidthSizeDefinitionProperty, value);
    }

    public static readonly DependencyProperty InformationTextProperty = DependencyProperty.Register(
        nameof(InformationText),
        typeof(string),
        typeof(LabelControlBase),
        new PropertyMetadata(defaultValue: string.Empty));

    public string InformationText
    {
        get => (string)GetValue(InformationTextProperty);
        set => SetValue(InformationTextProperty, value);
    }

    public static readonly DependencyProperty InformationColorProperty = DependencyProperty.Register(
        nameof(InformationColor),
        typeof(Color),
        typeof(LabelControlBase),
        new PropertyMetadata(Colors.DodgerBlue));

    public Color InformationColor
    {
        get => (Color)GetValue(InformationColorProperty);
        set => SetValue(InformationColorProperty, value);
    }
}