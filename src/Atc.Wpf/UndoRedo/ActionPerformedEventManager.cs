namespace Atc.Wpf.UndoRedo;

/// <summary>
/// Provides a <see cref="WeakEventManager"/> for the
/// <see cref="IUndoRedoService.ActionPerformed"/> event, preventing
/// subscribers from preventing the service from being garbage-collected.
/// </summary>
public sealed class ActionPerformedEventManager : WeakEventManager
{
    private static ActionPerformedEventManager CurrentManager
    {
        get
        {
            var managerType = typeof(ActionPerformedEventManager);
            var manager = (ActionPerformedEventManager?)GetCurrentManager(managerType);
            if (manager is null)
            {
                manager = new ActionPerformedEventManager();
                SetCurrentManager(managerType, manager);
            }

            return manager;
        }
    }

    public static void AddHandler(
        IUndoRedoService source,
        EventHandler<UndoRedoEventArgs> handler)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(handler);
        CurrentManager.ProtectedAddHandler(source, handler);
    }

    public static void RemoveHandler(
        IUndoRedoService source,
        EventHandler<UndoRedoEventArgs> handler)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(handler);
        CurrentManager.ProtectedRemoveHandler(source, handler);
    }

    protected override ListenerList NewListenerList()
        => new ListenerList<UndoRedoEventArgs>();

    protected override void StartListening(object source)
    {
        if (source is IUndoRedoService service)
        {
            service.ActionPerformed += OnActionPerformed;
        }
    }

    protected override void StopListening(object source)
    {
        if (source is IUndoRedoService service)
        {
            service.ActionPerformed -= OnActionPerformed;
        }
    }

    private void OnActionPerformed(
        object? sender,
        UndoRedoEventArgs e)
        => DeliverEvent(sender, e);
}