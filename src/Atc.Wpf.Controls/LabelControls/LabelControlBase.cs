namespace Atc.Wpf.Controls.LabelControls;

public partial class LabelControlBase : UserControl, ILabelControlBase
{
    public string Identifier
        => LabelText == Constants.DefaultLabelControlLabel
            ? LabelText
            : ControlHelper.GetIdentifier(this, LabelText.PascalCase(removeSeparators: true));

    [DependencyProperty]
    private string? groupIdentifier;

    [DependencyProperty]
    private Type? inputDataType;

    [DependencyProperty(DefaultValue = LabelControlHideAreasType.None)]
    private LabelControlHideAreasType hideAreas;

    [DependencyProperty(DefaultValue = Orientation.Horizontal)]
    private Orientation orientation;

    [DependencyProperty(DefaultValue = 120)]
    private int labelWidthNumber;

    [DependencyProperty(DefaultValue = SizeDefinitionType.Pixel)]
    private SizeDefinitionType labelWidthSizeDefinition;

    [DependencyProperty(DefaultValue = "")]
    private string labelText;

    [DependencyProperty(DefaultValue = 26d)]
    private double contentMinHeight;

    [DependencyProperty(DefaultValue = "")]
    private string informationText;

    [DependencyProperty(Flags = FrameworkPropertyMetadataOptions.AffectsMeasure)]
    private object? informationContent;

    [DependencyProperty(DefaultValue = nameof(Colors.DodgerBlue))]
    private Color informationColor;

    public string GetFullIdentifier()
        => string.IsNullOrEmpty(GroupIdentifier)
            ? Identifier
            : $"{GroupIdentifier}.{Identifier}";
}