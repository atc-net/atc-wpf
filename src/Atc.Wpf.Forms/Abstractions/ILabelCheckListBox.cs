namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Represents a labeled, always-visible check list whose items are key/value pairs and where several keys can be ticked.
/// </summary>
public interface ILabelCheckListBox : ILabelControl
{
    /// <summary>
    /// Gets or sets the items, keyed by their selection key, with their display value.
    /// </summary>
    Dictionary<string, string> Items { get; set; }

    /// <summary>
    /// Gets or sets the keys of the ticked items.
    /// </summary>
    ObservableCollection<string> SelectedKeys { get; set; }

    /// <summary>
    /// Occurs when the ticked keys change through the list (a tick, Select all, Clear, a group or a range).
    /// </summary>
    event EventHandler<ValueChangedEventArgs<IReadOnlyList<string>>>? SelectedKeysChanged;
}