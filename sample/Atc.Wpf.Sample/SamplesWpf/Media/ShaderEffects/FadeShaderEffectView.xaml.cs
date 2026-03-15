namespace Atc.Wpf.Sample.SamplesWpf.Media.ShaderEffects;

public partial class FadeShaderEffectView
{
    public FadeShaderEffectView()
    {
        InitializeComponent();
        DataContext = new FadeShaderEffectDemoViewModel();
    }
}