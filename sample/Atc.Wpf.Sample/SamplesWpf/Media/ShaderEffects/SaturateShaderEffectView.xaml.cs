namespace Atc.Wpf.Sample.SamplesWpf.Media.ShaderEffects;

/// <summary>
/// Interaction logic for SaturateShaderEffectView.
/// </summary>
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