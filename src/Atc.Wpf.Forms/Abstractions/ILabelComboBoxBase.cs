namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Common interface for labeled combo box controls that select an item by key.
/// </summary>
public interface ILabelComboBoxBase : ILabelControlBase
{
    /// <summary>
    /// Gets or sets the key of the selected item.
    /// </summary>
    string SelectedKey { get; set; }

    /// <summary>
    /// Occurs when the selected key changes.
    /// </summary>
    static event EventHandler<ValueChangedEventArgs<string?>>? SelectedKeyChanged;
}