namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public partial class SegmentedView
{
    public SegmentedView()
    {
        InitializeComponent();
        DataContext = new SegmentedDemoViewModel();

        InteractiveBox.SelectedKeysChanged += (_, e) =>
        {
            var line = string.Create(
                CultureInfo.InvariantCulture,
                $"SelectedKeysChanged: [{string.Join(", ", e.OldValue)}] -> [{string.Join(", ", e.NewValue)}]");
            SelectedKeysLog.Text = SelectedKeysLog.Text.Length == 0
                ? line
                : SelectedKeysLog.Text + Environment.NewLine + line;
        };
    }

    private void OnSelectionChanged(
        object? sender,
        SegmentedSelectionChangedEventArgs e)
    {
        SelectionStatus?.Text = $"Selected: {e.NewItem?.Content} (index {e.NewIndex}, was index {e.OldIndex})";
    }
}