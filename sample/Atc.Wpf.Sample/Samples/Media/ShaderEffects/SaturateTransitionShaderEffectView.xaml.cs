using System.Windows;

namespace Atc.Wpf.Sample.Samples.Media.ShaderEffects
{
    /// <summary>
    /// Interaction logic for SaturateTransitionShaderEffectView.
    /// </summary>
    public partial class SaturateTransitionShaderEffectView
    {
        public SaturateTransitionShaderEffectView()
        {
            this.InitializeComponent();
        }

        private void ResetOnClick(object sender, RoutedEventArgs e)
        {
            this.SliderProgress.Value = 0;
        }
    }
}