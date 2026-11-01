namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Represents a labeled group of toggle chips whose items are key/value pairs and where one or several keys can be selected.
/// </summary>
public interface ILabelChipGroup : ILabelControl
{
    /// <summary>
    /// Gets or sets the items, keyed by their selection key, with their display value.
    /// </summary>
    Dictionary<string, string> Items { get; set; }

    /// <summary>
    /// Gets or sets the keys of the selected chips.
    /// </summary>
    ObservableCollection<string> SelectedKeys { get; set; }

    /// <summary>
    /// Gets or sets how many chips can be selected.
    /// </summary>
    ChipSelectionMode SelectionMode { get; set; }

    /// <summary>
    /// Occurs when the selected keys change through the chips (a toggle, the "All" chip, an arrow key in a single-select group, or an items change).
    /// </summary>
    event EventHandler<ValueChangedEventArgs<IReadOnlyList<string>>>? SelectedKeysChanged;
}