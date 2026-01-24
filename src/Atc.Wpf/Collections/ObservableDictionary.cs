namespace Atc.Wpf.Collections;

public sealed class ObservableDictionary<TKey, TValue>
    : ObservableCollection<ObservableKeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>
{
    public void Add(
        TKey key,
        TValue value)
    {
        if (ContainsKey(key))
        {
            throw new ArgumentException(
                "The dictionary already contains the key",
                nameof(key));
        }

        var pair = new ObservableKeyValuePair<TKey, TValue> { Key = key, Value = value };
        Add(pair);
        OnCollectionChanged(
            new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add,
                pair));
    }

    public bool ContainsKey(TKey key)
    {
        var r = ThisAsCollection()
            .FirstOrDefault(i => Equals(
                key,
                i.Key));

        return !Equals(
            default(ObservableKeyValuePair<TKey, TValue>),
            r);
    }

#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
    public bool Equals<TKey>(
        TKey a,
        TKey b)
#pragma warning restore CS0693 // Type parameter has the same name as the type parameter from outer type
        => EqualityComparer<TKey>.Default.Equals(
            a,
            b);

    private ObservableCollection<ObservableKeyValuePair<TKey, TValue>> ThisAsCollection()
        => this;

    public ICollection<TKey> Keys
        => GetKeys();

    public bool Remove(TKey key)
    {
        var pair = ThisAsCollection().FirstOrDefault(p => Equals(key, p.Key));

        if (pair is null || (pair.Key is null && !typeof(TKey).IsValueType))
        {
            return false;
        }

        if (!ThisAsCollection().Remove(pair))
        {
            return false;
        }

        OnCollectionChanged(
            new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Remove,
                pair));

        return true;
    }

    public bool TryGetValue(
        TKey key,
        out TValue value)
    {
        value = default!;
        var r = GetKvpByTheKey(key);
        if (Equals(
            r,
            default))
        {
            return false;
        }

        value = r!.Value;
        return true;
    }

    private ObservableKeyValuePair<TKey, TValue>? GetKvpByTheKey(TKey key)
        => ThisAsCollection()
            .FirstOrDefault(i => i.Key is not null && i.Key.Equals(key));

    public ICollection<TValue> Values
        => GetValues();

    [SuppressMessage("Usage", "MA0015:Specify the parameter name in ArgumentException", Justification = "OK")]
    public TValue this[TKey key]
    {
        get
        {
            if (!TryGetValue(
                key,
                out var result))
            {
                throw new ArgumentException("Key not found");
            }

            return result;
        }

        set
        {
            if (ContainsKey(key))
            {
                GetKvpByTheKey(key)!.Value = value;
            }
            else
            {
                Add(
                    key,
                    value);
            }
        }
    }

    public void Add(KeyValuePair<TKey, TValue> item)
        => Add(
            item.Key,
            item.Value);

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        var r = GetKvpByTheKey(item.Key);
        if (Equals(
            r,
            default))
        {
            return false;
        }

        return Equals(
            r!.Value,
            item.Value);
    }

    public void CopyTo(
        KeyValuePair<TKey, TValue>[] array,
        int arrayIndex)
        => throw new NotImplementedException();

    public bool IsReadOnly => false;

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        var pair = GetKvpByTheKey(item.Key);
        if (Equals(
            pair,
            default))
        {
            return false;
        }

        if (!Equals(
            pair!.Value,
            item.Value))
        {
            return false;
        }

        if (!ThisAsCollection().Remove(pair))
        {
            return false;
        }

        OnCollectionChanged(
            new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Remove,
                pair));
        return true;
    }

    public void Update(KeyValuePair<TKey, TValue> item)
    {
        var index = 0;
        for (var i = 0; i < ThisAsCollection().Count; i++)
        {
            if (!object.Equals(
                this[i].Key,
                item.Key))
            {
                continue;
            }

            index = i;
            break;
        }

        Remove(item.Key);
        Insert(
            index,
            new ObservableKeyValuePair<TKey, TValue>()
            {
                Key = item.Key,
                Value = item.Value,
            });
    }

    public new IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        => ThisAsCollection()
            .Select(i => new KeyValuePair<TKey, TValue>(i.Key, i.Value))
            .GetEnumerator();

    private ICollection<TKey> GetKeys()
        => ThisAsCollection()
            .Select(i => i.Key)
            .ToArray();

    private ICollection<TValue> GetValues()
        => ThisAsCollection()
            .Select(i => i.Value)
            .ToArray();
}