using System.Windows;
using System.Windows.Media;

namespace Atc.Wpf.Media.ShaderEffects
{
    public class InvertColorsShaderEffect : ShaderEffectBase
    {
        public static readonly DependencyProperty InputProperty =
            RegisterPixelShaderSamplerProperty(
                "Input",
                typeof(InvertColorsShaderEffect),
                0);

        public override string Name => "InvertColors";

        public InvertColorsShaderEffect()
        {
            this.UpdateShaderValue(InputProperty);
        }

        /// <summary>
        /// Input.
        /// </summary>
        public Brush Input
        {
            get => (Brush)GetValue(InputProperty);
            set => SetValue(InputProperty, value);
        }
    }
}