namespace Atc.Wpf.Sample;

public class MainWindowViewModel : MainWindowViewModelBase, IMainWindowViewModel
{
    public void UpdateSelectedView(
        SampleTreeViewItem? sampleTreeViewItem)
    {
        var samplePath = sampleTreeViewItem?.SamplePath;
        var header = sampleTreeViewItem?.Header?.ToString();
        Messenger.Default.Send(new SampleItemMessage(header, samplePath));
    }
}