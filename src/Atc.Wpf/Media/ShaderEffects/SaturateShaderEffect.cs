namespace Atc.Wpf.Media.ShaderEffects;

public sealed class SaturateShaderEffect : ShaderEffectBase
{
    public static readonly DependencyProperty InputProperty =
        RegisterPixelShaderSamplerProperty(
            "Input",
            typeof(SaturateShaderEffect),
            0);

    public static readonly DependencyProperty SecondInputProperty =
        RegisterPixelShaderSamplerProperty(
            "SecondInput",
            typeof(SaturateShaderEffect),
            1);

    public static readonly DependencyProperty ProgressProperty =
        DependencyProperty.Register(
            nameof(Progress),
            typeof(double),
            typeof(SaturateShaderEffect),
            new UIPropertyMetadata(
                0d,
                PixelShaderConstantCallback(0)));

    public override string Name => "Saturate";

    public SaturateShaderEffect()
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