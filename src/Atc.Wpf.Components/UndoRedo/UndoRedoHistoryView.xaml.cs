namespace Atc.Wpf.Components.UndoRedo;

public partial class UndoRedoHistoryView
{
    [DependencyProperty(DefaultValue = true)]
    private bool showToolbar;

    [DependencyProperty(DefaultValue = true)]
    private bool showClear;

    [DependencyProperty(DefaultValue = false)]
    private bool showMarkSaved;

    public UndoRedoHistoryView()
    {
        InitializeComponent();
    }

    private void OnSelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (DataContext is UndoRedoHistoryViewModel vm &&
            LvHistory.SelectedItem is UndoRedoHistoryItem item)
        {
            vm.NavigateToCommand.Execute(item);
        }
    }
}