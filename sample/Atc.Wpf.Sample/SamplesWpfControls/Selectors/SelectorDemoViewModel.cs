namespace Atc.Wpf.Sample.SamplesWpfControls.Selectors;

public class SelectorDemoViewModel : ViewModelBase
{
    private bool isEnabled = true;

    [PropertyDisplay("Is Enabled", "Behavior", 1)]
    public new bool IsEnabled
    {
        get => isEnabled;
        set => Set(ref isEnabled, value);
    }
}