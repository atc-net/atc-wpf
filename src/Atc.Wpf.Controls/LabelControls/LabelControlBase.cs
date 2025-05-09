namespace Atc.Wpf.Controls.LabelControls;

public class LabelControlBase : UserControl, ILabelControlBase
{
    public string Identifier
        => LabelText == Constants.DefaultLabelControlLabel
            ? LabelText
            : ControlHelper.GetIdentifier(this, LabelText.PascalCase(removeSeparators: true));

    public static readonly DependencyProperty GroupIdentifierProperty = DependencyProperty.Register(
        nameof(GroupIdentifier),
        typeof(string),
        typeof(LabelControlBase),
        new PropertyMetadata(default(string?)));

    public string? GroupIdentifier
    {
        get => (string?)GetValue(GroupIdentifierProperty);
        set => SetValue(GroupIdentifierProperty, value);
    }

    public static readonly DependencyProperty InputDataTypeProperty = DependencyProperty.Register(
        nameof(InputDataType),
        typeof(Type),
        typeof(LabelControlBase),
        new PropertyMetadata(default(Type?)));

    public Type? InputDataType
    {
        get => (Type?)GetValue(InputDataTypeProperty);
        set => SetValue(InputDataTypeProperty, value);
    }

    public static readonly DependencyProperty HideAreasProperty = DependencyProperty.Register(
        nameof(HideAreas),
        typeof(LabelControlHideAreasType),
        typeof(LabelControlBase),
        new PropertyMetadata(LabelControlHideAreasType.None));

    public LabelControlHideAreasType HideAreas
    {
        get => (LabelControlHideAreasType)GetValue(HideAreasProperty);
        set => SetValue(HideAreasProperty, value);
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

    public static readonly DependencyProperty LabelTextProperty = DependencyProperty.Register(
        nameof(LabelText),
        typeof(string),
        typeof(LabelControlBase),
        new PropertyMetadata(Constants.DefaultLabelControlLabel));

    public string LabelText
    {
        get => (string)GetValue(LabelTextProperty);
        set => SetValue(LabelTextProperty, value);
    }

    public static readonly DependencyProperty ContentMinHeightProperty = DependencyProperty.Register(
        nameof(ContentMinHeight),
        typeof(double),
        typeof(LabelControlBase),
        new PropertyMetadata(26d));

    public double ContentMinHeight
    {
        get => (double)GetValue(ContentMinHeightProperty);
        set => SetValue(ContentMinHeightProperty, value);
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

    public static readonly DependencyProperty InformationContentProperty = DependencyProperty.Register(
        nameof(InformationContent),
        typeof(object),
        typeof(LabelControlBase),
        new FrameworkPropertyMetadata(
            defaultValue: null,
            FrameworkPropertyMetadataOptions.AffectsMeasure));

    public object? InformationContent
    {
        get => (object?)GetValue(InformationContentProperty);
        set => SetValue(InformationContentProperty, value);
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

    public string GetFullIdentifier()
        => string.IsNullOrEmpty(GroupIdentifier)
            ? Identifier
            : $"{GroupIdentifier}.{Identifier}";
}