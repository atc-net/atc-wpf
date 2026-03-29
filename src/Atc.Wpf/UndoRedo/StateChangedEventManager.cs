namespace Atc.Wpf.UndoRedo;

/// <summary>
/// Provides a <see cref="WeakEventManager"/> for the
/// <see cref="IUndoRedoService.StateChanged"/> event, preventing
/// subscribers from preventing the service from being garbage-collected.
/// </summary>
public sealed class StateChangedEventManager : WeakEventManager
{
    private static StateChangedEventManager CurrentManager
    {
        get
        {
            var managerType = typeof(StateChangedEventManager);
            var manager = (StateChangedEventManager?)GetCurrentManager(managerType);
            if (manager is null)
            {
                manager = new StateChangedEventManager();
                SetCurrentManager(managerType, manager);
            }

            return manager;
        }
    }

    public static void AddHandler(
        IUndoRedoService source,
        EventHandler handler)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(handler);
        CurrentManager.ProtectedAddHandler(source, handler);
    }

    public static void RemoveHandler(
        IUndoRedoService source,
        EventHandler handler)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(handler);
        CurrentManager.ProtectedRemoveHandler(source, handler);
    }

    protected override void StartListening(object source)
    {
        if (source is IUndoRedoService service)
        {
            service.StateChanged += OnStateChanged;
        }
    }

    protected override void StopListening(object source)
    {
        if (source is IUndoRedoService service)
        {
            service.StateChanged -= OnStateChanged;
        }
    }

    private void OnStateChanged(
        object? sender,
        EventArgs e)
        => DeliverEvent(sender, e);
}