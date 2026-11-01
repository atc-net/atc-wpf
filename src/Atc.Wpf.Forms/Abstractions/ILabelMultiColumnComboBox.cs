namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Represents a labeled multi-select drop-down whose pop-up is a grid of objects with a check-box column, where the
/// ticked rows are kept as keys.
/// </summary>
public interface ILabelMultiColumnComboBox : ILabelControl
{
    /// <summary>
    /// Gets or sets the objects shown as rows.
    /// </summary>
    IEnumerable? ItemsSource { get; set; }

    /// <summary>
    /// Gets or sets the (dotted) property path of an item's key.
    /// </summary>
    string KeyMemberPath { get; set; }

    /// <summary>
    /// Gets or sets the (dotted) property path of an item's display value.
    /// </summary>
    string DisplayMemberPath { get; set; }

    /// <summary>
    /// Gets or sets the keys of the ticked items.
    /// </summary>
    ObservableCollection<string> SelectedKeys { get; set; }

    /// <summary>
    /// Gets or sets the watermark shown in the closed box when nothing is ticked.
    /// </summary>
    string WatermarkText { get; set; }

    /// <summary>
    /// Occurs when the ticked keys change through the pop-up (a row, select all, a chip removal, OK, or an items change).
    /// </summary>
    event EventHandler<ValueChangedEventArgs<IReadOnlyList<string>>>? SelectedKeysChanged;
}