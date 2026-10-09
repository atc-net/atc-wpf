namespace Atc.Wpf.Collections;

/// <summary>
/// A mutable key/value pair that raises <see cref="PropertyChanged"/> when its key or value changes.
/// </summary>
public sealed class ObservableKeyValuePair<TKey, TValue> : INotifyPropertyChanged
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private TKey key;
    private TValue value;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <summary>
    /// Gets or sets the key.
    /// </summary>
    public TKey Key
    {
        get => key;
        set
        {
            key = value;
            OnPropertyChanged(nameof(Key));
        }
    }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public TValue Value
    {
        get => value;
        set
        {
            this.value = value;
            OnPropertyChanged(nameof(Value));
        }
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event for the specified property name.
    /// </summary>
    public void OnPropertyChanged(string name)
    {
        var handler = PropertyChanged;
        handler?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}