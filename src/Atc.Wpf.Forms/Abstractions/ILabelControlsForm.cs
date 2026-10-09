namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Represents a form of label controls laid out in rows and columns.
/// </summary>
public interface ILabelControlsForm
{
    /// <summary>
    /// Gets or sets the rows of the form.
    /// </summary>
    IList<ILabelControlsFormRow>? Rows { get; set; }

    /// <summary>
    /// Adds a column built from the given label controls.
    /// </summary>
    /// <param name="labelControls">The label controls to place in the new column.</param>
    void AddColumn(IList<ILabelControlBase> labelControls);

    /// <summary>
    /// Adds the given column to the form.
    /// </summary>
    /// <param name="labelControlsFormColumn">The column to add.</param>
    void AddColumn(ILabelControlsFormColumn labelControlsFormColumn);

    /// <summary>
    /// Removes all rows from the form.
    /// </summary>
    void Clear();

    /// <summary>
    /// Determines whether any column in the form contains controls from more than one group.
    /// </summary>
    /// <returns><see langword="true"/> if a column has multiple group identifiers; otherwise, <see langword="false"/>.</returns>
    bool HasMultiGroupIdentifiers();

    /// <summary>
    /// Calculates the total height of the form, summing the tallest column of each row.
    /// </summary>
    /// <returns>The calculated height.</returns>
    int GetMaxHeight();

    /// <summary>
    /// Calculates the width of the widest row in the form.
    /// </summary>
    /// <returns>The calculated width.</returns>
    int GetMaxWidth();

    /// <summary>
    /// Generates the panel that hosts the form's rows and columns.
    /// </summary>
    /// <returns>The generated panel.</returns>
    Panel GeneratePanel();

    /// <summary>
    /// Determines whether all label controls in the form are valid.
    /// </summary>
    /// <returns><see langword="true"/> if all controls are valid; otherwise, <see langword="false"/>.</returns>
    bool IsValid();

    /// <summary>
    /// Gets the values of the valid label controls in the form, keyed by their full identifier.
    /// </summary>
    /// <returns>A dictionary of full identifiers and values.</returns>
    Dictionary<string, object> GetKeyValues();
}