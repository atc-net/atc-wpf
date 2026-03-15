namespace Atc.Wpf.Sample.SamplesWpfControls.Progressing;

public partial class BusyOverlayDemoViewModel : ViewModelBase
{
    private bool isBusy;

    [PropertyDisplay("Display After (ms)", "Behavior", 2)]
    [PropertyRange(0, 10000, 100)]
    [ObservableProperty]
    private int displayAfterMs;

    [PropertyDisplay("Is Busy", "Behavior", 1)]
    public new bool IsBusy
    {
        get => isBusy;
        set => Set(ref isBusy, value);
    }
}