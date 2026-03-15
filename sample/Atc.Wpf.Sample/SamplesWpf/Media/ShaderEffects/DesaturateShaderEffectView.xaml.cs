namespace Atc.Wpf.Sample.SamplesWpf.Media.ShaderEffects;

public partial class DesaturateShaderEffectView
{
    public DesaturateShaderEffectView()
    {
        InitializeComponent();
        DataContext = new DesaturateShaderEffectDemoViewModel();
    }
}