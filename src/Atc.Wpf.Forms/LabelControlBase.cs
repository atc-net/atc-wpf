namespace Atc.Wpf.Forms;

/// <summary>
/// Base class for labeled form controls, providing the label, layout, identifier and information-text settings.
/// </summary>
public partial class LabelControlBase : UserControl, ILabelControlBase
{
    /// <summary>
    /// Gets the identifier of the control, derived from the <c>LabelText</c> in PascalCase.
    /// </summary>
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

    [DependencyProperty(DefaultValue = LabelPosition.Left)]
    private LabelPosition labelPosition;

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

    /// <summary>
    /// Gets the full identifier, which is the <see cref="Identifier"/> prefixed with the <c>GroupIdentifier</c> when one is set.
    /// </summary>
    /// <returns>The identifier, as <c>GroupIdentifier.Identifier</c> when a group identifier is set.</returns>
    public string GetFullIdentifier()
        => string.IsNullOrEmpty(GroupIdentifier)
            ? Identifier
            : $"{GroupIdentifier}.{Identifier}";
}