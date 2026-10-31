using SharedMisc = Atc.Wpf.Resources.Miscellaneous;

namespace Atc.Wpf.UndoRedo;

/// <summary>
/// ViewModel that wraps an <see cref="IUndoRedoService"/> and exposes
/// a unified history list combining undo and redo stacks.
/// </summary>
public sealed partial class UndoRedoHistoryViewModel : ViewModelBase
{
    private IUndoRedoService? undoRedoService;
    private bool isNavigating;

    [ObservableProperty]
    private ObservableCollectionEx<UndoRedoHistoryItem> historyItems = [];

    [ObservableProperty]
    private UndoRedoHistoryItem? selectedItem;

    /// <summary>
    /// Gets or sets the undo/redo service to visualize.
    /// </summary>
    public IUndoRedoService? UndoRedoService
    {
        get => undoRedoService;
        set
        {
            if (ReferenceEquals(value, undoRedoService))
            {
                return;
            }

            if (undoRedoService is not null)
            {
                undoRedoService.StateChanged -= OnStateChanged;
            }

            undoRedoService = value;

            if (undoRedoService is not null)
            {
                undoRedoService.StateChanged += OnStateChanged;
            }

            RaisePropertyChanged();
            RefreshHistory();
        }
    }

    [RelayCommand(CanExecute = nameof(CanUndo))]
    private void Undo()
        => undoRedoService?.Undo();

    [RelayCommand(CanExecute = nameof(CanRedo))]
    private void Redo()
        => undoRedoService?.Redo();

    [RelayCommand(CanExecute = nameof(CanUndo))]
    private void UndoAll()
        => undoRedoService?.UndoAll();

    [RelayCommand(CanExecute = nameof(CanUndo))]
    private void UndoToLastUserAction()
        => undoRedoService?.UndoToLastUserAction();

    [RelayCommand(CanExecute = nameof(CanRedo))]
    private void RedoAll()
        => undoRedoService?.RedoAll();

    [RelayCommand(CanExecute = nameof(CanRedo))]
    private void RedoToLastUserAction()
        => undoRedoService?.RedoToLastUserAction();

    [RelayCommand(CanExecute = nameof(CanClear))]
    private void Clear()
        => undoRedoService?.Clear();

    [RelayCommand(CanExecute = nameof(CanMarkSaved))]
    private void MarkSaved()
        => undoRedoService?.MarkSaved();

    [RelayCommand(CanExecute = nameof(CanNavigateTo))]
    private void NavigateTo(UndoRedoHistoryItem? item)
    {
        if (item is null ||
            undoRedoService is null ||
            isNavigating)
        {
            return;
        }

        isNavigating = true;
        try
        {
            if (item.Command is null)
            {
                undoRedoService.UndoAll();
            }
            else if (item.IsRedo)
            {
                undoRedoService.RedoTo(item.Command);
            }
            else if (item.IsHighlighted)
            {
                // Already at this state, nothing to do.
            }
            else
            {
                // UndoTo undoes including the target command,
                // but clicking an undo item means "go to the state after
                // this command was executed", so redo it back.
                undoRedoService.UndoTo(item.Command);
                undoRedoService.Redo();
            }

            // The service raises StateChanged for every step; the rows are refreshed once, here,
            // still inside the guard so a selection change caused by the refresh can't navigate again.
            ApplyStateChange();
        }
        finally
        {
            isNavigating = false;
        }
    }

    private bool CanUndo()
        => undoRedoService?.CanUndo is true;

    private bool CanRedo()
        => undoRedoService?.CanRedo is true;

    private bool CanClear()
        => undoRedoService is not null &&
           (undoRedoService.CanUndo || undoRedoService.CanRedo);

    private bool CanMarkSaved()
        => undoRedoService?.HasUnsavedChanges is true;

    private bool CanNavigateTo(UndoRedoHistoryItem? item)
        => item is not null &&
           undoRedoService is not null &&
           !isNavigating;

    private void OnStateChanged(
        object? sender,
        EventArgs e)
    {
        if (isNavigating)
        {
            return;
        }

        ApplyStateChange();
    }

    private void ApplyStateChange()
    {
        RefreshHistory();
        RaiseCanExecuteChanged();
        IsDirty = undoRedoService?.HasUnsavedChanges is true;
    }

    private void RaiseCanExecuteChanged()
    {
        UndoCommand.RaiseCanExecuteChanged();
        RedoCommand.RaiseCanExecuteChanged();
        UndoAllCommand.RaiseCanExecuteChanged();
        UndoToLastUserActionCommand.RaiseCanExecuteChanged();
        RedoAllCommand.RaiseCanExecuteChanged();
        RedoToLastUserActionCommand.RaiseCanExecuteChanged();
        ClearCommand.RaiseCanExecuteChanged();
        MarkSavedCommand.RaiseCanExecuteChanged();
        NavigateToCommand.RaiseCanExecuteChanged();
    }

    private void RefreshHistory()
    {
        if (undoRedoService is null)
        {
            HistoryItems.Clear();
            return;
        }

        var undoCommands = undoRedoService.UndoCommands;
        var redoCommands = undoRedoService.RedoCommands;
        var undoCount = undoCommands.Count;

        // Rows are reused by command, so the list keeps its selection, focus and scroll position
        // while the history moves; only the IsRedo/IsHighlighted flags change.
        UndoRedoHistoryItem? rootItem = null;
        var existingByCommand = new Dictionary<IUndoCommand, UndoRedoHistoryItem>(ReferenceEqualityComparer.Instance);
        foreach (var existing in HistoryItems)
        {
            if (existing.Command is null)
            {
                rootItem ??= existing;
            }
            else
            {
                existingByCommand.TryAdd(existing.Command, existing);
            }
        }

        if (rootItem is null)
        {
            // Root row: "Initial state"
            rootItem = new UndoRedoHistoryItem
            {
                Description = SharedMisc.InitialState,
                IsHighlighted = undoCount == 0,
                Command = null,
            };
        }
        else
        {
            rootItem.UpdateState(redo: false, highlighted: undoCount == 0);
        }

        var items = new List<UndoRedoHistoryItem>(undoCount + redoCommands.Count + 1) { rootItem };

        // Undo commands in chronological order (reversed from stack which is most-recent-first)
        for (var i = undoCount - 1; i >= 0; i--)
        {
            items.Add(GetOrBuildHistoryItem(existingByCommand, undoCommands[i], isRedo: false, isHighlighted: i == 0));
        }

        // Redo commands in stack order (most-recent-first = next to redo first)
        foreach (var cmd in redoCommands)
        {
            items.Add(GetOrBuildHistoryItem(existingByCommand, cmd, isRedo: true, isHighlighted: false));
        }

        SyncHistoryItems(items);
    }

    private void SyncHistoryItems(List<UndoRedoHistoryItem> items)
    {
        var target = HistoryItems;
        var wanted = new HashSet<UndoRedoHistoryItem>(items, ReferenceEqualityComparer.Instance);
        for (var i = target.Count - 1; i >= 0; i--)
        {
            if (!wanted.Contains(target[i]))
            {
                target.RemoveAt(i);
            }
        }

        for (var i = 0; i < items.Count; i++)
        {
            if (i < target.Count && ReferenceEquals(target[i], items[i]))
            {
                continue;
            }

            var currentIndex = target.IndexOf(items[i]);
            if (currentIndex >= 0)
            {
                target.Move(currentIndex, i);
            }
            else
            {
                target.Insert(i, items[i]);
            }
        }
    }

    private static UndoRedoHistoryItem GetOrBuildHistoryItem(
        Dictionary<IUndoCommand, UndoRedoHistoryItem> existingByCommand,
        IUndoCommand cmd,
        bool isRedo,
        bool isHighlighted)
    {
        if (existingByCommand.TryGetValue(cmd, out var existing))
        {
            existing.UpdateState(isRedo, isHighlighted);
            return existing;
        }

        return BuildHistoryItem(cmd, isRedo, isHighlighted);
    }

    private static UndoRedoHistoryItem BuildHistoryItem(
        IUndoCommand cmd,
        bool isRedo,
        bool isHighlighted)
    {
        var richCmd = cmd as IRichUndoCommand;
        IReadOnlyList<UndoRedoHistoryItem>? children = null;

        if (cmd is UndoCommandGroup group)
        {
            var childItems = new List<UndoRedoHistoryItem>(group.Commands.Count);
            foreach (var child in group.Commands)
            {
                var richChild = child as IRichUndoCommand;
                childItems.Add(new UndoRedoHistoryItem
                {
                    Description = child.Description,
                    IsRedo = isRedo,
                    IsHighlighted = isHighlighted,
                    Command = child,
                    Image = richChild?.Image as ImageSource,
                    Timestamp = richChild?.Timestamp,
                });
            }

            children = childItems;
        }

        return new UndoRedoHistoryItem
        {
            Description = cmd.Description,
            IsRedo = isRedo,
            IsHighlighted = isHighlighted,
            Command = cmd,
            Image = richCmd?.Image as ImageSource,
            Timestamp = richCmd?.Timestamp,
            Children = children,
        };
    }
}