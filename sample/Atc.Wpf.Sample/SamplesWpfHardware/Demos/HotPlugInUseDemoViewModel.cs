namespace Atc.Wpf.Sample.SamplesWpfHardware.Demos;

public partial class HotPlugInUseDemoViewModel : ViewModelBase
{
    private const int MaxEntries = 200;

    public ObservableCollection<string> Entries { get; } = [];

    public void Log(string line)
    {
        var stamped = $"{DateTime.Now:HH:mm:ss.fff}  {line}";
        Entries.Insert(0, stamped);
        while (Entries.Count > MaxEntries)
        {
            Entries.RemoveAt(Entries.Count - 1);
        }
    }

    public void Clear()
        => Entries.Clear();
}