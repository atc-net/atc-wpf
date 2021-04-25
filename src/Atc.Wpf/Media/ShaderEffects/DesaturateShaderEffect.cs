using System.Windows;
using System.Windows.Media;

namespace Atc.Wpf.Media.ShaderEffects
{
    public class DesaturateShaderEffect : ShaderEffectBase
    {
        public static readonly DependencyProperty InputProperty =
            RegisterPixelShaderSamplerProperty(
                "Input",
                typeof(DesaturateShaderEffect),
                0);

        public static readonly DependencyProperty StrengthProperty =
            DependencyProperty.Register(
                nameof(Strength),
                typeof(double),
                typeof(DesaturateShaderEffect),
                new UIPropertyMetadata(
                    default(double),
                    PixelShaderConstantCallback(0)));

        public override string Name => "Desaturate";

        public DesaturateShaderEffect()
        {
            this.UpdateShaderValue(InputProperty);
            this.UpdateShaderValue(StrengthProperty);
        }

        /// <summary>
        /// Input.
        /// </summary>
        public Brush Input
        {
            get => (Brush)GetValue(InputProperty);
            set => SetValue(InputProperty, value);
        }

        /// <summary>
        /// Strength.
        /// </summary>
        public double Strength
        {
            get => (double)GetValue(StrengthProperty);
            set => SetValue(StrengthProperty, value);
        }
    }
}