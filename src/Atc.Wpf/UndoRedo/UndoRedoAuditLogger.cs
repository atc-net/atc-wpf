namespace Atc.Wpf.UndoRedo;

/// <summary>
/// Default in-memory implementation of <see cref="IUndoRedoAuditLogger"/>
/// with an optional maximum capacity that trims the oldest entries.
/// </summary>
public sealed class UndoRedoAuditLogger : IUndoRedoAuditLogger
{
    private readonly List<UndoRedoAuditEntry> entries = [];
    private readonly int maxEntries;

    public UndoRedoAuditLogger(int maxEntries = 1000)
    {
        this.maxEntries = maxEntries;
    }

    /// <inheritdoc />
    public IReadOnlyList<UndoRedoAuditEntry> Entries => entries.AsReadOnly();

    /// <inheritdoc />
    public void Log(UndoRedoAuditEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        entries.Add(entry);

        while (entries.Count > maxEntries)
        {
            entries.RemoveAt(0);
        }
    }

    /// <inheritdoc />
    public void Clear() => entries.Clear();
}