namespace Atc.Wpf.Sample.Samples.Media.ShaderEffects;

/// <summary>
/// Interaction logic for ContrastAdjustShaderEffectView.
/// </summary>
public partial class ContrastAdjustShaderEffectView
{
    public ContrastAdjustShaderEffectView()
    {
        this.InitializeComponent();
    }

    private void ResetOnClick(object sender, RoutedEventArgs e)
    {
        this.SliderBrightness.Value = 0;
        this.SliderContrast.Value = 0;
    }
}