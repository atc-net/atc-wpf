namespace Atc.Wpf.Controls.Displays;

/// <summary>
/// Batches the change notifications of a frame buffer: one notification per outermost batch, raised outside the buffer lock.
/// </summary>
internal sealed class DisplayChangeNotifier
{
    private readonly Lock sync;
    private readonly Action raise;
    private int batchDepth;
    private bool changedPending;

    public DisplayChangeNotifier(
        Lock sync,
        Action raise)
    {
        this.sync = sync;
        this.raise = raise;
    }

    // Raises the notification, or remembers it while a batch is open. Must not be called with the lock held.
    public void Notify()
    {
        lock (sync)
        {
            if (batchDepth > 0)
            {
                changedPending = true;
                return;
            }
        }

        raise();
    }

    public IDisposable BeginBatch()
    {
        lock (sync)
        {
            batchDepth++;
        }

        return new BatchScope(this);
    }

    private void EndBatch()
    {
        lock (sync)
        {
            batchDepth--;
            if (batchDepth > 0 || !changedPending)
            {
                return;
            }

            changedPending = false;
        }

        raise();
    }

    private sealed class BatchScope : IDisposable
    {
        private DisplayChangeNotifier? notifier;

        public BatchScope(DisplayChangeNotifier notifier)
            => this.notifier = notifier;

        public void Dispose()
            => Interlocked.Exchange(ref notifier, null)?.EndBatch();
    }
}