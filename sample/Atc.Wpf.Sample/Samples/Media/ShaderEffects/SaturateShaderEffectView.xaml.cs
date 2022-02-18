namespace Atc.Wpf.Sample.Samples.Media.ShaderEffects;

/// <summary>
/// Interaction logic for SaturateShaderEffectView.
/// </summary>
public partial class SaturateShaderEffectView
{
    public SaturateShaderEffectView()
    {
        this.InitializeComponent();
    }

    private void ResetOnClick(object sender, RoutedEventArgs e)
    {
        this.SliderProgress.Value = 0;
    }
}