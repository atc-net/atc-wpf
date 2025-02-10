namespace Atc.Wpf.Media.ShaderEffects;

public sealed class DesaturateShaderEffect : ShaderEffectBase
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
                0,
                PixelShaderConstantCallback(0)));

    public override string Name => "Desaturate";

    public DesaturateShaderEffect()
    {
        UpdateShaderValue(InputProperty);
        UpdateShaderValue(StrengthProperty);
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