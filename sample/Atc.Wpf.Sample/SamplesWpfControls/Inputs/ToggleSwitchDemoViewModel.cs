namespace Atc.Wpf.Sample.SamplesWpfControls.Inputs;

public partial class ToggleSwitchDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Is On", "Value", 1)]
    [ObservableProperty]
    private bool isOn;

    [PropertyDisplay("On Content", "Content", 1)]
    [ObservableProperty]
    private string onContent = "On";

    [PropertyDisplay("Off Content", "Content", 2)]
    [ObservableProperty]
    private string offContent = "Off";

    [PropertyDisplay("Content Direction", "Layout", 1)]
    [ObservableProperty]
    private FlowDirection contentDirection = FlowDirection.LeftToRight;
    private bool isEnabled = true;

    [PropertyDisplay("Is Enabled", "Behavior", 1)]
    public new bool IsEnabled
    {
        get => isEnabled;
        set => Set(ref isEnabled, value);
    }
}