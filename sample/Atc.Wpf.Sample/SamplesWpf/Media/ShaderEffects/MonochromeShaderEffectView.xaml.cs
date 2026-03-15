namespace Atc.Wpf.Sample.SamplesWpf.Media.ShaderEffects;

public partial class MonochromeShaderEffectView
{
    public MonochromeShaderEffectView()
    {
        InitializeComponent();
        DataContext = new MonochromeShaderEffectDemoViewModel();
    }
}