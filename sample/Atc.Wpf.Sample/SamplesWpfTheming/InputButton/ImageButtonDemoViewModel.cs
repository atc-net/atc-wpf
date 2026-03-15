namespace Atc.Wpf.Sample.SamplesWpfTheming.InputButton;

public partial class ImageButtonDemoViewModel : ViewModelBase
{
    private bool isBusy;

    [PropertyDisplay("Is Busy", "Behavior", 1)]
    public new bool IsBusy
    {
        get => isBusy;
        set => Set(ref isBusy, value);
    }
}
