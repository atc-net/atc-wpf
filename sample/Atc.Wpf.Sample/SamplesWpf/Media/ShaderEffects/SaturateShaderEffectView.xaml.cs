namespace Atc.Wpf.Sample.SamplesWpf.Media.ShaderEffects;

public partial class SaturateShaderEffectView
{
    public SaturateShaderEffectView()
    {
        InitializeComponent();
    }

    private void ResetOnClick(
        object sender,
        RoutedEventArgs e)
    {
        SliderProgress.Value = 0;
    }
}