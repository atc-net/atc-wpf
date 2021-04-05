using Atc.Wpf.Messaging;
using Atc.Wpf.Mvvm;
using Atc.Wpf.SampleControls;

namespace Atc.Wpf.Sample
{
    public class MainWindowViewModel : MainWindowViewModelBase
    {
        public static void UpdateSelectedView(SampleTreeViewItem? sampleTreeViewItem)
        {
            var samplePath = sampleTreeViewItem?.SamplePath;
            var header = sampleTreeViewItem?.Header?.ToString();
            Messenger.Default.Send(new SampleItemMessage(header, samplePath));
        }
    }
}