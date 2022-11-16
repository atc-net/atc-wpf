namespace Atc.Wpf.Sample.SamplesWpf.Media.ShaderEffects;

/// <summary>
/// Interaction logic for ContrastAdjustShaderEffectView.
/// </summary>
public partial class ContrastAdjustShaderEffectView
{
    public ContrastAdjustShaderEffectView()
    {
        InitializeComponent();
    }

    private void ResetOnClick(
        object sender,
        RoutedEventArgs e)
    {
        SliderBrightness.Value = 0;
        SliderContrast.Value = 0;
    }
}