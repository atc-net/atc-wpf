namespace Atc.Wpf.Controls.Sample;

public interface IMainWindowViewModel : IMainWindowViewModelBase
{
    void UpdateSelectedView(SampleTreeViewItem? sampleTreeViewItem);
}