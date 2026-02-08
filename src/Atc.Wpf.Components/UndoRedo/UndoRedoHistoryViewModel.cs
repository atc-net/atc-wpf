namespace Atc.Wpf.Components.UndoRedo;

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

    [RelayCommand(CanExecute = nameof(CanRedo))]
    private void RedoAll()
        => undoRedoService?.RedoAll();

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
        RefreshHistory();
        RaiseCanExecuteChanged();
        IsDirty = undoRedoService?.HasUnsavedChanges is true;
    }

    private void RaiseCanExecuteChanged()
    {
        UndoCommand.RaiseCanExecuteChanged();
        RedoCommand.RaiseCanExecuteChanged();
        UndoAllCommand.RaiseCanExecuteChanged();
        RedoAllCommand.RaiseCanExecuteChanged();
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

        var items = new List<UndoRedoHistoryItem>(undoCount + redoCommands.Count + 1)
        {
            // Root row: "Initial state"
            new()
            {
                Description = Miscellaneous.InitialState,
                IsHighlighted = undoCount == 0,
                Command = null,
            },
        };

        // Undo commands in chronological order (reversed from stack which is most-recent-first)
        for (var i = undoCount - 1; i >= 0; i--)
        {
            var cmd = undoCommands[i];
            items.Add(new UndoRedoHistoryItem
            {
                Description = cmd.Description,
                IsHighlighted = i == 0,
                Command = cmd,
            });
        }

        // Redo commands in stack order (most-recent-first = next to redo first)
        foreach (var cmd in redoCommands)
        {
            items.Add(new UndoRedoHistoryItem
            {
                Description = cmd.Description,
                IsRedo = true,
                Command = cmd,
            });
        }

        var collection = new ObservableCollectionEx<UndoRedoHistoryItem>();
        collection.AddRange(items);
        HistoryItems = collection;
    }
}