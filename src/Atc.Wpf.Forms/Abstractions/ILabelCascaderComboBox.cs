namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Represents a labeled drop-down with one column per hierarchy level and tri-state check boxes, where the ticked
/// nodes are kept as keys.
/// </summary>
public interface ILabelCascaderComboBox : ILabelControl
{
    /// <summary>
    /// Gets or sets the root nodes.
    /// </summary>
    ObservableCollection<CheckTreeNode> Nodes { get; set; }

    /// <summary>
    /// Gets or sets the keys of the ticked nodes (which ones depends on <c>CheckedKeysStrategy</c>).
    /// </summary>
    ObservableCollection<string> SelectedKeys { get; set; }

    /// <summary>
    /// Gets or sets the watermark shown in the closed box when nothing is ticked.
    /// </summary>
    string WatermarkText { get; set; }

    /// <summary>
    /// Occurs when the ticked keys change through the pop-up (a tick, a chip removal, or loaded children).
    /// </summary>
    event EventHandler<ValueChangedEventArgs<IReadOnlyList<string>>>? SelectedKeysChanged;
}