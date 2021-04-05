using System.Windows;
using System.Windows.Media;

namespace Atc.Wpf.Media.ShaderEffects
{
    public class SaturateTransitionShaderEffect : ShaderEffectBase
    {
        public static readonly DependencyProperty InputProperty =
            RegisterPixelShaderSamplerProperty(
                "Input",
                typeof(SaturateTransitionShaderEffect),
                0);

        public static readonly DependencyProperty SecondInputProperty =
            RegisterPixelShaderSamplerProperty(
                "SecondInput",
                typeof(SaturateTransitionShaderEffect),
                1);

        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register(
                nameof(Progress),
                typeof(double),
                typeof(SaturateTransitionShaderEffect),
                new UIPropertyMetadata(
                    default(double),
                    PixelShaderConstantCallback(0)));

        public override string Name => "SaturateTransition";

        public SaturateTransitionShaderEffect()
        {
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(SecondInputProperty);
            UpdateShaderValue(ProgressProperty);
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
        /// SecondInput.
        /// </summary>
        public Brush SecondInput
        {
            get => (Brush)GetValue(SecondInputProperty);
            set => SetValue(SecondInputProperty, value);
        }

        /// <summary>
        /// Progress.
        /// </summary>
        public double Progress
        {
            get => (double)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }
    }
}