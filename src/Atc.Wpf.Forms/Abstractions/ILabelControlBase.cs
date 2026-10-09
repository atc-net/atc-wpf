namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Common interface for all labeled controls.
/// </summary>
public interface ILabelControlBase
{
    /// <summary>
    /// Gets the identifier of the control, derived from its label text.
    /// </summary>
    string Identifier { get; }

    /// <summary>
    /// Gets or sets the optional group name used to group related controls.
    /// </summary>
    string? GroupIdentifier { get; set; }

    /// <summary>
    /// Gets or sets the optional data type of the input.
    /// </summary>
    Type? InputDataType { get; set; }

    /// <summary>
    /// Gets or sets the width of the control.
    /// </summary>
    double Width { get; set; }

    /// <summary>
    /// Gets or sets the background brush of the control.
    /// </summary>
    Brush Background { get; set; }

    /// <summary>
    /// Gets or sets the areas of the label layout (asterisk, information, validation) to hide.
    /// </summary>
    LabelControlHideAreasType HideAreas { get; set; }

    /// <summary>
    /// Gets or sets the orientation of the label relative to the input.
    /// </summary>
    Orientation Orientation { get; set; }

    /// <summary>
    /// Gets or sets the label text.
    /// </summary>
    string LabelText { get; set; }

    /// <summary>
    /// Gets or sets the width of the label column, in the unit given by <see cref="LabelWidthSizeDefinition"/>.
    /// </summary>
    int LabelWidthNumber { get; set; }

    /// <summary>
    /// Gets or sets the unit used for <see cref="LabelWidthNumber"/>.
    /// </summary>
    SizeDefinitionType LabelWidthSizeDefinition { get; set; }

    /// <summary>
    /// Gets or sets the information text shown in the information icon tooltip.
    /// </summary>
    string InformationText { get; set; }

    /// <summary>
    /// Gets or sets custom content shown in the information icon tooltip.
    /// </summary>
    object? InformationContent { get; set; }

    /// <summary>
    /// Gets or sets the color of the information icon.
    /// </summary>
    Color InformationColor { get; set; }

    /// <summary>
    /// Gets the identifier prefixed with the <see cref="GroupIdentifier"/>, when one is set.
    /// </summary>
    /// <returns>The full identifier of the control.</returns>
    string GetFullIdentifier();
}