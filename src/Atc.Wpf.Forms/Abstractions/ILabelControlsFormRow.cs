namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Represents a row of columns in a <see cref="ILabelControlsForm"/>.
/// </summary>
public interface ILabelControlsFormRow
{
    /// <summary>
    /// Gets or sets the columns in the row.
    /// </summary>
    ICollection<ILabelControlsFormColumn>? Columns { get; set; }
}