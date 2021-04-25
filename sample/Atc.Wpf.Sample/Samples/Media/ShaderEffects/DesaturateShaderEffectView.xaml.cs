using System.Windows;

namespace Atc.Wpf.Sample.Samples.Media.ShaderEffects
{
    /// <summary>
    /// Interaction logic for DesaturateShaderEffectView.
    /// </summary>
    public partial class DesaturateShaderEffectView
    {
        public DesaturateShaderEffectView()
        {
            this.InitializeComponent();
        }

        private void ResetOnClick(object sender, RoutedEventArgs e)
        {
            this.SliderStrength.Value = 0;
        }
    }
}