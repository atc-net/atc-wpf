using Atc.Wpf.Mvvm;
using Atc.Wpf.SampleControls;

namespace Atc.Wpf.Sample
{
    public interface IMainWindowViewModel : IMainWindowViewModelBase
    {
        void UpdateSelectedView(SampleTreeViewItem? sampleTreeViewItem);
    }
}