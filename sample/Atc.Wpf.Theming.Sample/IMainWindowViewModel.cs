namespace Atc.Wpf.Theming.Sample;

public interface IMainWindowViewModel : IMainWindowViewModelBase
{
    void UpdateSelectedView(SampleTreeViewItem? sampleTreeViewItem);
}