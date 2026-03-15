namespace Atc.Wpf.Sample.SamplesWpf.Media.ShaderEffects;

public partial class MonochromeShaderEffectDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Filter Color", "Effect", 1)]
    [ObservableProperty]
    private Color filterColor = Colors.Gray;
}