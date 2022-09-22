namespace Atc.Wpf.Sample.Samples.Media.ShaderEffects;

/// <summary>
/// Interaction logic for DesaturateShaderEffectView.
/// </summary>
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