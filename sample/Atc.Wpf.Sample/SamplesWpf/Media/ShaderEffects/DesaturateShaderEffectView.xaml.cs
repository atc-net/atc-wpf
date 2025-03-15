namespace Atc.Wpf.Sample.SamplesWpf.Media.ShaderEffects;

public partial class DesaturateShaderEffectView
{
    public DesaturateShaderEffectView()
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