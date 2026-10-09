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
    /// Occurs when the selected key changes and passes validation.
    /// </summary>
    event EventHandler<ValueChangedEventArgs<string?>>? SelectorChanged;

    /// <summary>
    /// Occurs when the selected key changes and fails validation (a mandatory field with no selection).
    /// </summary>
    event EventHandler<ValueChangedEventArgs<string?>>? SelectorLostFocusInvalid;
}