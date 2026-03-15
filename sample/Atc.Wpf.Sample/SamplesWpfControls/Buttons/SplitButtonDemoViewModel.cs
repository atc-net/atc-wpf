namespace Atc.Wpf.Sample.SamplesWpfControls.Buttons;

public partial class SplitButtonDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Content", "Content", 1)]
    [ObservableProperty]
    private string content = "Save";
    private bool isEnabled = true;

    [PropertyDisplay("Is Enabled", "Behavior", 1)]
    public new bool IsEnabled
    {
        get => isEnabled;
        set => Set(ref isEnabled, value);
    }
}