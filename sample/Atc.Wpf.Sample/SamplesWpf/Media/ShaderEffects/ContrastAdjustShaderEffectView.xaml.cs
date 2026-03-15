namespace Atc.Wpf.Sample.SamplesWpf.Media.ShaderEffects;

public partial class ContrastAdjustShaderEffectView
{
    public ContrastAdjustShaderEffectView()
    {
        InitializeComponent();
        DataContext = new ContrastAdjustShaderEffectDemoViewModel();
    }
}