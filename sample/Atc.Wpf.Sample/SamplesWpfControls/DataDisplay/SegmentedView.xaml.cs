namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public partial class SegmentedView
{
    public SegmentedView()
    {
        InitializeComponent();
    }

    private void OnSelectionChanged(
        object? sender,
        SegmentedSelectionChangedEventArgs e)
    {
        SelectionStatus?.Text = $"Selected: {e.NewItem?.Content} (index {e.NewIndex}, was index {e.OldIndex})";
    }
}