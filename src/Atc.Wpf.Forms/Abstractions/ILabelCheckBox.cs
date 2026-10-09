namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Represents a labeled check box control.
/// </summary>
public interface ILabelCheckBox : ILabelControlBase
{
    /// <summary>
    /// Gets or sets a value indicating whether the check box is checked.
    /// </summary>
    bool IsChecked { get; set; }
}