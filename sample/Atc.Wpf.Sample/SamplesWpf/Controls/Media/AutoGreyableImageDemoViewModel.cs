// ReSharper disable IdentifierTypo
namespace Atc.Wpf.Sample.SamplesWpf.Controls.Media;

public class AutoGreyableImageDemoViewModel : ViewModelBase
{
    private bool isEnabled = true;

    [PropertyDisplay("Is Enabled", "Behavior", 1)]
    public new bool IsEnabled
    {
        get => isEnabled;
        set => Set(ref isEnabled, value);
    }
}