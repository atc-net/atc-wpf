namespace Atc.Wpf.Sample.SamplesWpfComponents.UndoRedo;

public sealed partial class UndoRedoHistoryViewSampleViewModel : ViewModelBase
{
    private readonly IUndoRedoService undoRedoService = new UndoRedoService();

    [ObservableProperty]
    private string textInput = string.Empty;

    [ObservableProperty]
    private string currentText = "(empty)";

    [PropertyDisplay("Show Toolbar", "Behavior", 1)]
    [ObservableProperty]
    private bool showToolbar = true;

    [PropertyDisplay("Show Clear", "Behavior", 2)]
    [ObservableProperty]
    private bool showClear = true;

    [PropertyDisplay("Show Mark Saved", "Behavior", 3)]
    [ObservableProperty]
    private bool showMarkSaved;

    public UndoRedoHistoryViewSampleViewModel()
    {
        HistoryViewModel = new UndoRedoHistoryViewModel
        {
            UndoRedoService = undoRedoService,
        };
    }

    public IUndoRedoService UndoRedoService => undoRedoService;

    public UndoRedoHistoryViewModel HistoryViewModel { get; }

    [RelayCommand]
    private void SetText()
    {
        if (string.IsNullOrEmpty(TextInput))
        {
            return;
        }

        var oldValue = CurrentText;
        var newValue = TextInput;

        undoRedoService.Execute(new PropertyChangeCommand<string>(
            $"Set text to \"{newValue}\"",
            v => CurrentText = v,
            oldValue,
            newValue));

        TextInput = string.Empty;
    }

    [RelayCommand]
    private void GroupedChange()
    {
        using (undoRedoService.BeginGroup("Grouped: set A + B"))
        {
            var oldValue = CurrentText;

            undoRedoService.Execute(new PropertyChangeCommand<string>(
                "Set text to \"A\"",
                v => CurrentText = v,
                oldValue,
                "A"));

            undoRedoService.Execute(new PropertyChangeCommand<string>(
                "Set text to \"B\"",
                v => CurrentText = v,
                "A",
                "B"));
        }
    }

    [RelayCommand]
    private void InternalChange()
    {
        var oldValue = CurrentText;
        var newValue = $"[auto-{DateTime.Now:ss}]";

        undoRedoService.Execute(new RichUndoCommand(
            $"Internal: \"{newValue}\"",
            () => CurrentText = newValue,
            () => CurrentText = oldValue,
            allowUserAction: false));
    }

    [RelayCommand]
    private void UndoToUserAction()
        => undoRedoService.UndoToLastUserAction();

    [RelayCommand]
    private void RedoToUserAction()
        => undoRedoService.RedoToLastUserAction();

    [RelayCommand]
    private void CreateSnapshot()
    {
        var name = $"Snapshot {undoRedoService.Snapshots.Count + 1}";
        undoRedoService.CreateSnapshot(name);
    }

    [RelayCommand]
    private void BatchImport()
    {
        using (undoRedoService.SuspendRecording())
        {
            for (var i = 0; i < 10; i++)
            {
                CurrentText = $"batch-{i}";
            }
        }

        // Record a single summary command after the batch
        var finalValue = CurrentText;
        undoRedoService.Add(new RichUndoCommand(
            "Batch import (10 items)",
            () => CurrentText = finalValue,
            () => CurrentText = "(empty)"));
    }
}