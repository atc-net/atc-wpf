namespace Atc.Wpf.Sample.SamplesWpfComponents.UndoRedo;

public sealed partial class UndoRedoServiceViewModel : ViewModelBase
{
    private readonly IUndoRedoService undoRedoService = new UndoRedoService();

    [ObservableProperty]
    private string textInput = string.Empty;

    [ObservableProperty]
    private string currentText = "(empty)";

    [ObservableProperty]
    private ObservableCollection<IUndoCommand> undoItems = [];

    [ObservableProperty]
    private ObservableCollection<IUndoCommand> redoItems = [];

    [ObservableProperty]
    private string undoHeaderText = "Undo Stack (0)";

    [ObservableProperty]
    private string redoHeaderText = "Redo Stack (0)";

    public UndoRedoServiceViewModel()
    {
        undoRedoService.StateChanged += (_, _) => RefreshStacks();
    }

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
    private void Undo()
        => undoRedoService.Undo();

    [RelayCommand]
    private void Redo()
        => undoRedoService.Redo();

    [RelayCommand]
    private void UndoAll()
        => undoRedoService.UndoAll();

    [RelayCommand]
    private void RedoAll()
        => undoRedoService.RedoAll();

    [RelayCommand]
    private void ClearHistory()
        => undoRedoService.Clear();

    private void RefreshStacks()
    {
        UndoItems = new ObservableCollection<IUndoCommand>(undoRedoService.UndoCommands);
        RedoItems = new ObservableCollection<IUndoCommand>(undoRedoService.RedoCommands);
        UndoHeaderText = $"Undo Stack ({UndoItems.Count})";
        RedoHeaderText = $"Redo Stack ({RedoItems.Count})";
    }
}