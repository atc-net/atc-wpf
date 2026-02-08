namespace Atc.Wpf.Sample.SamplesWpfComponents.UndoRedo;

public sealed partial class UndoRedoHistoryViewSampleViewModel : ViewModelBase
{
    private readonly IUndoRedoService undoRedoService = new UndoRedoService();

    [ObservableProperty]
    private string textInput = string.Empty;

    [ObservableProperty]
    private string currentText = "(empty)";

    public UndoRedoHistoryViewSampleViewModel()
    {
        HistoryViewModel = new UndoRedoHistoryViewModel
        {
            UndoRedoService = undoRedoService,
        };
    }

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
}