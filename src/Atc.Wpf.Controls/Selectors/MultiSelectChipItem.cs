namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// The data context of a chip in a <see cref="MultiSelectComboBox"/> in <see cref="MultiSelectDisplayMode.Chips"/> mode,
/// for a custom <see cref="MultiSelectComboBox.ChipTemplate"/>.
/// </summary>
public sealed class MultiSelectChipItem
{
    internal MultiSelectChipItem(
        string key,
        string displayValue,
        bool canRemove,
        Action<string> remove)
    {
        Key = key;
        DisplayValue = displayValue;
        CanRemove = canRemove;
        RemoveCommand = new RelayCommand(() => remove(key), () => CanRemove);
    }

    /// <summary>
    /// Gets the selection key of the item.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets the display value of the item.
    /// </summary>
    public string DisplayValue { get; }

    /// <summary>
    /// Gets a value indicating whether the item can be unticked (it is not one of the disabled keys).
    /// </summary>
    public bool CanRemove { get; }

    /// <summary>
    /// Gets the command that unticks the item without opening the pop-up.
    /// </summary>
    public ICommand RemoveCommand { get; }

    /// <inheritdoc />
    public override string ToString()
        => DisplayValue;
}