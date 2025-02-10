namespace Atc.Wpf.Collections;

public sealed class ObservableKeyValuePair<TKey, TValue> : INotifyPropertyChanged
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private TKey key;
    private TValue value;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public TKey Key
    {
        get => key;
        set
        {
            key = value;
            OnPropertyChanged(nameof(Key));
        }
    }

    public TValue Value
    {
        get => value;
        set
        {
            this.value = value;
            OnPropertyChanged(nameof(Value));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void OnPropertyChanged(string name)
    {
        var handler = PropertyChanged;
        handler?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}