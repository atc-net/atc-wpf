namespace Atc.Wpf.Controls.Selectors;

/// <summary>
/// A row in a check list: either an item (a key with its display value) or a group header.
/// It is the data context of the row templates, so a custom <c>ItemTemplate</c> can bind to its properties.
/// </summary>
public sealed class CheckListItem : INotifyPropertyChanged
{
    private readonly Action<CheckListItem, bool?>? toggleRequested;
    private string displayValue;
    private bool? isChecked = false;
    private bool isEnabled = true;

    internal CheckListItem(
        string key,
        string displayValue,
        string? groupName,
        bool isGroupHeader,
        Action<CheckListItem, bool?>? toggleRequested)
    {
        Key = key;
        this.displayValue = displayValue;
        GroupName = groupName;
        IsGroupHeader = isGroupHeader;
        this.toggleRequested = toggleRequested;
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the key of the item (for a group header: the group name).
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Gets the display value of the item (for a group header: the group name).
    /// </summary>
    public string DisplayValue
    {
        get => displayValue;
        internal set
        {
            if (string.Equals(displayValue, value, StringComparison.Ordinal))
            {
                return;
            }

            displayValue = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets the name of the group the item belongs to, or <see langword="null"/> when it has no group.
    /// </summary>
    public string? GroupName { get; }

    /// <summary>
    /// Gets a value indicating whether this row is a group header rather than an item.
    /// </summary>
    public bool IsGroupHeader { get; }

    /// <summary>
    /// Gets or sets whether the row is ticked. Items are <see langword="true"/> or <see langword="false"/>;
    /// a group header is <see langword="null"/> when only some of its shown items are ticked.
    /// Setting it asks the owning list to tick or untick the row (a group header ticks or unticks its shown items).
    /// </summary>
    public bool? IsChecked
    {
        get => isChecked;
        set
        {
            if (isChecked == value)
            {
                return;
            }

            if (toggleRequested is null)
            {
                isChecked = value;
                OnPropertyChanged();
                return;
            }

            toggleRequested(this, value);
        }
    }

    /// <summary>
    /// Gets a value indicating whether the row can be toggled (it is not a disabled key and the selection limit allows it).
    /// </summary>
    public bool IsEnabled
    {
        get => isEnabled;
        internal set
        {
            if (isEnabled == value)
            {
                return;
            }

            isEnabled = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc />
    public override string ToString()
        => DisplayValue;

    /// <summary>
    /// Sets the ticked state without asking the owner; raises <see cref="PropertyChanged"/> when it changes.
    /// </summary>
    internal void SetIsChecked(bool? value)
    {
        if (isChecked == value)
        {
            return;
        }

        isChecked = value;
        OnPropertyChanged(nameof(IsChecked));
    }

    /// <summary>
    /// Raises <see cref="PropertyChanged"/> for <see cref="IsChecked"/> so a binding that pushed a refused value reads the real one.
    /// </summary>
    internal void RefreshIsChecked()
        => OnPropertyChanged(nameof(IsChecked));

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}