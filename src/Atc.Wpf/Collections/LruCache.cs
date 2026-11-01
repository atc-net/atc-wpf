namespace Atc.Wpf.Collections;

/// <summary>
/// A thread-safe cache with a fixed capacity that evicts the least recently used entry when full.
/// </summary>
/// <remarks>
/// Both reads and writes mark an entry as recently used, so both take the lock.
/// </remarks>
/// <typeparam name="TKey">The key type.</typeparam>
/// <typeparam name="TValue">The value type.</typeparam>
internal sealed class LruCache<TKey, TValue>
    where TKey : notnull
{
    private readonly Lock syncRoot = new();
    private readonly int capacity;
    private readonly Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> nodes;

    // Most recently used first.
    private readonly LinkedList<KeyValuePair<TKey, TValue>> recency = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="LruCache{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="capacity">The most entries the cache holds.</param>
    /// <param name="comparer">The key comparer, or <see langword="null"/> for the default.</param>
    public LruCache(
        int capacity,
        IEqualityComparer<TKey>? comparer = null)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 1);

        this.capacity = capacity;
        nodes = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>(capacity, comparer);
    }

    /// <summary>
    /// Gets the number of entries.
    /// </summary>
    public int Count
    {
        get
        {
            lock (syncRoot)
            {
                return nodes.Count;
            }
        }
    }

    /// <summary>
    /// Gets the value for <paramref name="key"/> and marks the entry as most recently used.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value, when found.</param>
    /// <returns><see langword="true"/> when the key was found.</returns>
    public bool TryGet(
        TKey key,
        [MaybeNullWhen(false)] out TValue value)
    {
        lock (syncRoot)
        {
            if (!nodes.TryGetValue(key, out var node))
            {
                value = default;
                return false;
            }

            MoveToFront(node);
            value = node.Value.Value;
            return true;
        }
    }

    /// <summary>
    /// Determines whether the cache holds <paramref name="key"/>, without marking it as used.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns><see langword="true"/> when the key is cached.</returns>
    public bool ContainsKey(TKey key)
    {
        lock (syncRoot)
        {
            return nodes.ContainsKey(key);
        }
    }

    /// <summary>
    /// Adds or replaces the value for <paramref name="key"/> and marks it as most recently used;
    /// evicts the least recently used entry when a new key would exceed the capacity.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public void Set(
        TKey key,
        TValue value)
    {
        lock (syncRoot)
        {
            if (nodes.TryGetValue(key, out var existing))
            {
                existing.Value = new KeyValuePair<TKey, TValue>(key, value);
                MoveToFront(existing);
                return;
            }

            if (nodes.Count >= capacity)
            {
                var leastRecentlyUsed = recency.Last!;
                recency.RemoveLast();
                nodes.Remove(leastRecentlyUsed.Value.Key);
            }

            nodes[key] = recency.AddFirst(new KeyValuePair<TKey, TValue>(key, value));
        }
    }

    /// <summary>
    /// Removes every entry.
    /// </summary>
    public void Clear()
    {
        lock (syncRoot)
        {
            nodes.Clear();
            recency.Clear();
        }
    }

    private void MoveToFront(LinkedListNode<KeyValuePair<TKey, TValue>> node)
    {
        if (node == recency.First)
        {
            return;
        }

        recency.Remove(node);
        recency.AddFirst(node);
    }
}