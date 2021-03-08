using System.Diagnostics.CodeAnalysis;
using Atc.Wpf.Messaging;
using Atc.Wpf.Mvvm;
using Atc.Wpf.SampleControls;

namespace Atc.Wpf.Sample
{
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OK.")]
    public class MainWindowViewModel : MainWindowViewModelBase
    {
        public void UpdateSelectedView(SampleTreeViewItem? sampleTreeViewItem)
        {
            var samplePath = sampleTreeViewItem?.SamplePath;
            Messenger.Default.Send(new SampleItemMessage(samplePath));
        }
    }
}