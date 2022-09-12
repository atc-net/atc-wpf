namespace Atc.Wpf.Sample.Samples.Media.ShaderEffects;

/// <summary>
/// Interaction logic for FadeShaderEffectView.
/// </summary>
public partial class FadeShaderEffectView
{
    public FadeShaderEffectView()
    {
        InitializeComponent();
    }

    private void ResetOnClick(object sender, RoutedEventArgs e)
    {
        SliderStrength.Value = 0;
    }
}