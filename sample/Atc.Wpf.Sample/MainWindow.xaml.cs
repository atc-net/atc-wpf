namespace Atc.Wpf.Sample;

public partial class MainWindow
{
    public MainWindow()
        => InitializeComponent();

    internal IReadOnlyList<TreeView> SampleTreeViews
        => Explorer.SampleTreeViews;
}