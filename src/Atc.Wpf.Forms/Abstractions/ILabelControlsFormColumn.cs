namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Represents a column of label controls in a <see cref="ILabelControlsForm"/>.
/// </summary>
public interface ILabelControlsFormColumn
{
    /// <summary>
    /// Gets or sets a value indicating whether the controls are wrapped in a group box per group identifier.
    /// </summary>
    public bool UseGroupBox { get; set; }

    /// <summary>
    /// Gets or sets the orientation applied to the label controls in the column.
    /// </summary>
    Orientation ControlOrientation { get; set; }

    /// <summary>
    /// Gets or sets the width of the label controls in the column.
    /// </summary>
    int ControlWidth { get; set; }

    /// <summary>
    /// Gets the label controls in the column.
    /// </summary>
    IList<ILabelControlBase> LabelControls { get; }

    /// <summary>
    /// Sets the layout settings of the column and applies the orientation to its label controls.
    /// </summary>
    /// <param name="useGroupBox">Whether to wrap the controls in group boxes.</param>
    /// <param name="controlOrientation">The orientation to apply to the label controls.</param>
    /// <param name="controlWidth">The width of the label controls.</param>
    void SetSettings(
        bool useGroupBox,
        Orientation controlOrientation,
        int controlWidth);

    /// <summary>
    /// Determines whether the column contains controls from more than one group.
    /// </summary>
    /// <returns><see langword="true"/> if there are multiple group identifiers; otherwise, <see langword="false"/>.</returns>
    bool HasMultiGroupIdentifiers();

    /// <summary>
    /// Gets the distinct group identifiers of the label controls in the column.
    /// </summary>
    /// <returns>The distinct group identifiers.</returns>
    IList<string?> GetGroupIdentifiers();

    /// <summary>
    /// Gets the label controls that belong to the given group.
    /// </summary>
    /// <param name="groupIdentifier">The group identifier to filter by.</param>
    /// <returns>The label controls in the group.</returns>
    IList<ILabelControlBase> GetLabelControlsByGroupIdentifier(
        string? groupIdentifier);

    /// <summary>
    /// Calculates the height of the column.
    /// </summary>
    /// <returns>The calculated height.</returns>
    int CalculateHeight();

    /// <summary>
    /// Generates the panel that hosts the column's label controls.
    /// </summary>
    /// <returns>The generated panel.</returns>
    Panel GeneratePanel();

    /// <summary>
    /// Determines whether all label controls in the column are valid.
    /// </summary>
    /// <returns><see langword="true"/> if all controls are valid; otherwise, <see langword="false"/>.</returns>
    bool IsValid();

    /// <summary>
    /// Gets the values of the valid label controls in the column, keyed by their full identifier.
    /// </summary>
    /// <returns>A dictionary of full identifiers and values.</returns>
    Dictionary<string, object> GetKeyValues();
}