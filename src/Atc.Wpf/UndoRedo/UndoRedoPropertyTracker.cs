namespace Atc.Wpf.UndoRedo;

/// <summary>
/// Monitors <see cref="INotifyPropertyChanged"/> objects and automatically
/// records property changes as undo commands on an <see cref="IUndoRedoService"/>.
/// </summary>
/// <remarks>
/// <para>
/// Because <see cref="INotifyPropertyChanged.PropertyChanged"/> fires
/// <em>after</em> the property has already changed, the tracker maintains
/// snapshots of tracked property values so it can construct the correct
/// old/new pair for each <see cref="PropertyChangeCommand{T}"/>.
/// </para>
/// <para>
/// Changes that originate from the undo/redo service itself (i.e., while
/// <see cref="IUndoRedoService.IsExecuting"/> is <see langword="true"/>)
/// are silently ignored to avoid recursive recording.
/// </para>
/// </remarks>
public sealed class UndoRedoPropertyTracker : IDisposable
{
    private readonly IUndoRedoService service;
    private readonly Dictionary<INotifyPropertyChanged, HashSet<string>> trackedObjects = [];
    private readonly Dictionary<(INotifyPropertyChanged Target, string Property), object?> snapshots = [];
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="UndoRedoPropertyTracker"/> class.
    /// </summary>
    /// <param name="service">The undo/redo service to record commands on.</param>
    public UndoRedoPropertyTracker(IUndoRedoService service)
    {
        ArgumentNullException.ThrowIfNull(service);
        this.service = service;
    }

    /// <summary>
    /// Begins tracking all readable and writable properties of the specified object.
    /// </summary>
    /// <param name="target">The object to track.</param>
    public void Track(INotifyPropertyChanged target)
    {
        ArgumentNullException.ThrowIfNull(target);
        TrackCore(target, propertyFilter: null);
    }

    /// <summary>
    /// Begins tracking the specified properties of an object.
    /// Only changes to the listed properties will be recorded.
    /// </summary>
    /// <param name="target">The object to track.</param>
    /// <param name="properties">The property names to track.</param>
    public void Track(
        INotifyPropertyChanged target,
        params string[] properties)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(properties);
        TrackCore(target, new HashSet<string>(properties, StringComparer.Ordinal));
    }

    /// <summary>
    /// Stops tracking the specified object and removes its snapshots.
    /// </summary>
    /// <param name="target">The object to stop tracking.</param>
    public void Untrack(INotifyPropertyChanged target)
    {
        ArgumentNullException.ThrowIfNull(target);

        if (!trackedObjects.Remove(target))
        {
            return;
        }

        target.PropertyChanged -= OnPropertyChanged;
        RemoveSnapshotsForTarget(target);
    }

    /// <summary>
    /// Stops tracking all objects and releases all resources.
    /// </summary>
    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;

        foreach (var target in trackedObjects.Keys)
        {
            target.PropertyChanged -= OnPropertyChanged;
        }

        trackedObjects.Clear();
        snapshots.Clear();
    }

    private void TrackCore(
        INotifyPropertyChanged target,
        HashSet<string>? propertyFilter)
    {
        if (trackedObjects.ContainsKey(target))
        {
            return;
        }

        var filter = propertyFilter ?? [];
        trackedObjects[target] = filter;
        target.PropertyChanged += OnPropertyChanged;
        CaptureAllSnapshots(target, filter);
    }

    private void CaptureAllSnapshots(
        INotifyPropertyChanged target,
        HashSet<string> filter)
    {
        var properties = target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in properties)
        {
            if (!prop.CanRead || !prop.CanWrite)
            {
                continue;
            }

            if (filter.Count > 0 && !filter.Contains(prop.Name))
            {
                continue;
            }

            snapshots[(target, prop.Name)] = prop.GetValue(target);
        }
    }

    private void OnPropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (service.IsExecuting ||
            sender is not INotifyPropertyChanged target ||
            e.PropertyName is null)
        {
            return;
        }

        if (!trackedObjects.TryGetValue(target, out var filter))
        {
            return;
        }

        // If filter is non-empty, only track listed properties.
        if (filter.Count > 0 && !filter.Contains(e.PropertyName))
        {
            return;
        }

        var prop = target.GetType().GetProperty(e.PropertyName, BindingFlags.Public | BindingFlags.Instance);
        if (prop is null || !prop.CanRead || !prop.CanWrite)
        {
            return;
        }

        var key = (target, e.PropertyName);
        var newValue = prop.GetValue(target);

        // Look up old value from snapshot; default to null if not found.
        snapshots.TryGetValue(key, out var oldValue);

        // Update snapshot to current value.
        snapshots[key] = newValue;

        // Do not record a command when the value has not actually changed.
        if (Equals(oldValue, newValue))
        {
            return;
        }

        var description = $"Change {e.PropertyName}";
        var propertyInfo = prop;
        var capturedTarget = target;

        var command = new PropertyChangeCommand<object?>(
            description,
            v => propertyInfo.SetValue(capturedTarget, v),
            oldValue,
            newValue);

        service.Add(command);
    }

    private void RemoveSnapshotsForTarget(INotifyPropertyChanged target)
    {
        var keysToRemove = new List<(INotifyPropertyChanged Target, string Property)>();

        foreach (var key in snapshots.Keys)
        {
            if (ReferenceEquals(key.Target, target))
            {
                keysToRemove.Add(key);
            }
        }

        foreach (var key in keysToRemove)
        {
            snapshots.Remove(key);
        }
    }
}