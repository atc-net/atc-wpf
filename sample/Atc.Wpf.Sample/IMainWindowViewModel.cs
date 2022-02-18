namespace Atc.Wpf.Sample;

public interface IMainWindowViewModel : IMainWindowViewModelBase
{
    void UpdateSelectedView(SampleTreeViewItem? sampleTreeViewItem);
}