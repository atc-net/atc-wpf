namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Represents a labeled combo box control whose items are key/value pairs.
/// </summary>
public interface ILabelComboBox : ILabelComboBoxBase
{
    /// <summary>
    /// Gets or sets the items shown in the combo box, keyed by their selection key.
    /// </summary>
    Dictionary<string, string> Items { get; set; }
}