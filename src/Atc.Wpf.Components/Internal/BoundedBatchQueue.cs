namespace Atc.Wpf.Components.Internal;

/// <summary>
/// A thread-safe first-in, first-out queue that any thread writes to and one consumer drains in batches. It holds at
/// most <see cref="Capacity"/> items and drops the oldest beyond that, so a paused or slow consumer cannot make it grow
/// without limit.
/// </summary>
/// <remarks>
/// <see cref="Clear"/> starts a new <see cref="Generation"/>; a batch taken before the clear carries the old one, so the
/// consumer can discard it instead of showing items that were cleared while it was on its way.
/// </remarks>
/// <typeparam name="T">The item type.</typeparam>
internal sealed class BoundedBatchQueue<T>
{
    private readonly Lock sync = new();
    private readonly Queue<T> items = new();
    private readonly Channel<bool> signal = Channel.CreateBounded<bool>(
        new BoundedChannelOptions(1)
        {
            FullMode = BoundedChannelFullMode.DropWrite,
            SingleReader = true,
            AllowSynchronousContinuations = false,
        });

    private int capacity;
    private long generation;
    private int droppedCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoundedBatchQueue{T}"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of items held; 0 or less means no limit.</param>
    public BoundedBatchQueue(int capacity)
        => this.capacity = System.Math.Max(0, capacity);

    /// <summary>
    /// Gets or sets the maximum number of items held; 0 means no limit. Lowering it drops the oldest items.
    /// </summary>
    public int Capacity
    {
        get
        {
            lock (sync)
            {
                return capacity;
            }
        }

        set
        {
            lock (sync)
            {
                capacity = System.Math.Max(0, value);
                while (capacity > 0 && items.Count > capacity)
                {
                    items.Dequeue();
                    droppedCount++;
                }
            }
        }
    }

    /// <summary>
    /// Gets the number of items waiting.
    /// </summary>
    public int Count
    {
        get
        {
            lock (sync)
            {
                return items.Count;
            }
        }
    }

    /// <summary>
    /// Gets the current generation, which <see cref="Clear"/> advances.
    /// </summary>
    public long Generation
    {
        get
        {
            lock (sync)
            {
                return generation;
            }
        }
    }

    /// <summary>
    /// Adds an item, dropping the oldest when the queue is full.
    /// </summary>
    public void Enqueue(T item)
    {
        lock (sync)
        {
            EnqueueLocked(item);
        }

        signal.Writer.TryWrite(true);
    }

    /// <summary>
    /// Adds items in order, dropping the oldest when the queue is full.
    /// </summary>
    public void EnqueueRange(IEnumerable<T> newItems)
    {
        ArgumentNullException.ThrowIfNull(newItems);

        lock (sync)
        {
            foreach (var item in newItems)
            {
                EnqueueLocked(item);
            }
        }

        signal.Writer.TryWrite(true);
    }

    /// <summary>
    /// Removes every waiting item and starts a new generation.
    /// </summary>
    /// <returns>The new generation.</returns>
    public long Clear()
    {
        lock (sync)
        {
            items.Clear();
            droppedCount = 0;
            return ++generation;
        }
    }

    /// <summary>
    /// Takes every waiting item.
    /// </summary>
    /// <returns><see langword="false"/> when nothing is waiting.</returns>
    public bool TryTakeAll(out BoundedBatch<T> batch)
    {
        lock (sync)
        {
            if (items.Count == 0)
            {
                batch = default;
                return false;
            }

            batch = new BoundedBatch<T>([.. items], generation, droppedCount);
            items.Clear();
            droppedCount = 0;
            return true;
        }
    }

    /// <summary>
    /// Waits until an item is waiting.
    /// </summary>
    /// <returns><see langword="false"/> once the queue is completed.</returns>
    public async ValueTask<bool> WaitToReadAsync(
        CancellationToken cancellationToken)
    {
        while (true)
        {
            if (Count > 0)
            {
                return true;
            }

            if (!await signal.Reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
            {
                return false;
            }

            signal.Reader.TryRead(out _);
        }
    }

    /// <summary>
    /// Completes the queue; <see cref="WaitToReadAsync"/> then returns <see langword="false"/>.
    /// </summary>
    public void Complete()
        => signal.Writer.TryComplete();

    private void EnqueueLocked(T item)
    {
        if (capacity > 0 && items.Count >= capacity)
        {
            items.Dequeue();
            droppedCount++;
        }

        items.Enqueue(item);
    }
}

/// <summary>
/// Items taken from a <see cref="BoundedBatchQueue{T}"/>.
/// </summary>
/// <param name="Items">The items, oldest first.</param>
/// <param name="Generation">The queue generation when the items were taken.</param>
/// <param name="DroppedCount">How many older items were dropped since the previous batch because the queue was full.</param>
/// <typeparam name="T">The item type.</typeparam>
internal readonly record struct BoundedBatch<T>(
    IReadOnlyList<T> Items,
    long Generation,
    int DroppedCount);