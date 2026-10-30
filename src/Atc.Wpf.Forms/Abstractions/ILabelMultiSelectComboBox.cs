namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Represents a labeled multi-select drop-down whose items are key/value pairs and where several keys can be ticked.
/// </summary>
public interface ILabelMultiSelectComboBox : ILabelControl
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
    /// Gets or sets the watermark shown in the closed box when nothing is ticked.
    /// </summary>
    string WatermarkText { get; set; }

    /// <summary>
    /// Occurs when the ticked keys change through the pop-up (a tick, Select all, Clear, a group, or an items change).
    /// </summary>
    event EventHandler<ValueChangedEventArgs<IReadOnlyList<string>>>? SelectedKeysChanged;
}