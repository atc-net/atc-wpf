namespace Atc.Wpf.Controls.Zoom;

[SuppressMessage("Design", "MA0048:File name must match type name", Justification = "OK - partial class")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1601:Partial elements should be documented", Justification = "OK - partial class")]
public partial class ZoomBox
{
    private readonly Stack<UndoRedoStackItem> undoStack = new();
    private readonly Stack<UndoRedoStackItem> redoStack = new();
    private KeepAliveTimer? timer750MilliSeconds;
    private KeepAliveTimer? timer1500MilliSeconds;
    private UndoRedoStackItem? viewportZoomCache;
    private UndoRedoStackItem? serviceZoomSnapshot;
    private RelayCommand? redoZoomCommand;
    private RelayCommand? undoZoomCommand;

    /// <summary>
    /// Gets or sets an optional external undo/redo service.
    /// When set, zoom state changes are recorded through the service
    /// instead of the internal undo/redo stacks.
    /// </summary>
    [DependencyProperty]
    private IUndoRedoService? undoRedoService;

    private bool CanUndoZoom
        => UndoRedoService is not null
            ? UndoRedoService.CanUndo
            : undoStack.Count != 0;

    private bool CanRedoZoom
        => UndoRedoService is not null
            ? UndoRedoService.CanRedo
            : redoStack.Count != 0;

    /// <summary>
    /// Record the previous zoom level, so that we can return to it.
    /// </summary>
    public void SaveZoom()
    {
        if (UndoRedoService is not null)
        {
            SaveZoomToService();
            return;
        }

        viewportZoomCache = CreateUndoRedoStackItem();
        if (undoStack.Count != 0 &&
            viewportZoomCache.Equals(undoStack.Peek()))
        {
            return;
        }

        undoStack.Push(viewportZoomCache);
        redoStack.Clear();
        undoZoomCommand?.RaiseCanExecuteChanged();
        redoZoomCommand?.RaiseCanExecuteChanged();
    }

    /// <summary>
    /// Record the last saved zoom level, so that we can return to it if no activity for 750 milliseconds.
    /// </summary>
    [SuppressMessage("Major Code Smell", "S1121:Assignments should not be made from within sub-expressions", Justification = "OK.")]
    public void DelayedSaveZoom750MilliSeconds()
    {
        if (timer750MilliSeconds?.Running != true)
        {
            viewportZoomCache = CreateUndoRedoStackItem();
        }

        if (viewportZoomCache is null)
        {
            return;
        }

        if (UndoRedoService is not null)
        {
            (timer750MilliSeconds ??= new KeepAliveTimer(
                TimeSpan.FromMilliseconds(740),
                () => SaveZoomToService())).Nudge();
            return;
        }

        (timer750MilliSeconds ??= new KeepAliveTimer(TimeSpan.FromMilliseconds(740), () =>
        {
            if (undoStack.Count != 0 &&
                viewportZoomCache.Equals(undoStack.Peek()))
            {
                return;
            }

            undoStack.Push(viewportZoomCache);
            redoStack.Clear();
            undoZoomCommand?.RaiseCanExecuteChanged();
            redoZoomCommand?.RaiseCanExecuteChanged();
        })).Nudge();
    }

    /// <summary>
    /// Record the last saved zoom level, so that we can return to it if no activity for 1500 milliseconds.
    /// </summary>
    [SuppressMessage("Major Code Smell", "S1121:Assignments should not be made from within sub-expressions", Justification = "OK.")]
    public void DelayedSaveZoom1500MilliSeconds()
    {
        if (timer1500MilliSeconds?.Running != true)
        {
            viewportZoomCache = CreateUndoRedoStackItem();
        }

        if (viewportZoomCache is null)
        {
            return;
        }

        if (UndoRedoService is not null)
        {
            (timer1500MilliSeconds ??= new KeepAliveTimer(
                TimeSpan.FromMilliseconds(1500),
                () => SaveZoomToService())).Nudge();
            return;
        }

        (timer1500MilliSeconds ??= new KeepAliveTimer(TimeSpan.FromMilliseconds(1500), () =>
        {
            if (undoStack.Count != 0 &&
                viewportZoomCache.Equals(undoStack.Peek()))
            {
                return;
            }

            undoStack.Push(viewportZoomCache);
            redoStack.Clear();
            undoZoomCommand?.RaiseCanExecuteChanged();
            redoZoomCommand?.RaiseCanExecuteChanged();
        })).Nudge();
    }

    private UndoRedoStackItem CreateUndoRedoStackItem()
        => new(
            ContentOffsetX,
            ContentOffsetY,
            ContentViewportWidth,
            ContentViewportHeight,
            InternalViewportZoom);

    private void SaveZoomToService()
    {
        if (UndoRedoService is null)
        {
            return;
        }

        if (UndoRedoService.IsExecuting)
        {
            return;
        }

        var currentState = CreateUndoRedoStackItem();
        var fromState = serviceZoomSnapshot ?? currentState;
        serviceZoomSnapshot = currentState;

        if (fromState.Equals(currentState))
        {
            return;
        }

        var command = new ZoomUndoCommand(
            this,
            fromState.Rect,
            fromState.Zoom,
            currentState.Rect,
            currentState.Zoom);

        UndoRedoService.Add(command);
        undoZoomCommand?.RaiseCanExecuteChanged();
        redoZoomCommand?.RaiseCanExecuteChanged();
    }

    private void UndoZoom()
    {
        if (UndoRedoService is not null)
        {
            serviceZoomSnapshot = null;
            UndoRedoService.Undo();
            SetScrollViewerFocus();
            undoZoomCommand?.RaiseCanExecuteChanged();
            redoZoomCommand?.RaiseCanExecuteChanged();
            return;
        }

        viewportZoomCache = CreateUndoRedoStackItem();
        if (undoStack.Count == 0 ||
            !viewportZoomCache.Equals(undoStack.Peek()))
        {
            redoStack.Push(viewportZoomCache);
        }

        viewportZoomCache = undoStack.Pop();
        AnimatedZoomTo(viewportZoomCache.Zoom, viewportZoomCache.Rect);
        SetScrollViewerFocus();
        undoZoomCommand?.RaiseCanExecuteChanged();
        redoZoomCommand?.RaiseCanExecuteChanged();
    }

    private void RedoZoom()
    {
        if (UndoRedoService is not null)
        {
            serviceZoomSnapshot = null;
            UndoRedoService.Redo();
            SetScrollViewerFocus();
            undoZoomCommand?.RaiseCanExecuteChanged();
            redoZoomCommand?.RaiseCanExecuteChanged();
            return;
        }

        viewportZoomCache = CreateUndoRedoStackItem();
        if (redoStack.Count == 0 ||
            !viewportZoomCache.Equals(redoStack.Peek()))
        {
            undoStack.Push(viewportZoomCache);
        }

        viewportZoomCache = redoStack.Pop();
        AnimatedZoomTo(viewportZoomCache.Zoom, viewportZoomCache.Rect);
        SetScrollViewerFocus();
        undoZoomCommand?.RaiseCanExecuteChanged();
        redoZoomCommand?.RaiseCanExecuteChanged();
    }

    /// <summary>
    /// Command to undo the last zoom/pan change.
    /// </summary>
    public ICommand UndoZoomCommand
        => undoZoomCommand ??= new RelayCommand(UndoZoom, () => CanUndoZoom);

    /// <summary>
    /// Command to redo the last undone zoom/pan change.
    /// </summary>
    public ICommand RedoZoomCommand
        => redoZoomCommand ??= new RelayCommand(RedoZoom, () => CanRedoZoom);

    /// <summary>
    /// Navigate to the previous viewport state (alias for <see cref="UndoZoomCommand"/>).
    /// </summary>
    public ICommand ZoomPreviousCommand => UndoZoomCommand;

    /// <summary>
    /// Navigate to the next viewport state (alias for <see cref="RedoZoomCommand"/>).
    /// </summary>
    public ICommand ZoomNextCommand => RedoZoomCommand;

    private void SetScrollViewerFocus()
    {
        var scrollViewer = content?.TryFindParent<ScrollViewer>();
        if (scrollViewer is null)
        {
            return;
        }

        Keyboard.Focus(scrollViewer);
        scrollViewer.Focus();
    }
}