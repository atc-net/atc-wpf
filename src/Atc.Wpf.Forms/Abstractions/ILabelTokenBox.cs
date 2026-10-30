namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Represents a labeled token field where the user types to pick several values, each shown as a removable chip.
/// </summary>
public interface ILabelTokenBox : ILabelControl
{
    /// <summary>
    /// Gets or sets the known values, keyed by their selection key, with their display value.
    /// </summary>
    Dictionary<string, string> Items { get; set; }

    /// <summary>
    /// Gets or sets the keys of the tokens.
    /// </summary>
    ObservableCollection<string> SelectedKeys { get; set; }

    /// <summary>
    /// Gets or sets the watermark shown in the empty field.
    /// </summary>
    string WatermarkText { get; set; }

    /// <summary>
    /// Occurs when the tokens change through the field (a pick, removal, paste, edit, Clear, or an items change).
    /// </summary>
    event EventHandler<ValueChangedEventArgs<IReadOnlyList<string>>>? SelectedKeysChanged;
}