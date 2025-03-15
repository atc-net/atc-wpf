namespace Atc.Wpf.Sample.SamplesWpf.Media.ShaderEffects;

public partial class FadeShaderEffectView
{
    public FadeShaderEffectView()
    {
        InitializeComponent();
    }

    private void ResetOnClick(
        object sender,
        RoutedEventArgs e)
    {
        SliderStrength.Value = 0;
    }
}