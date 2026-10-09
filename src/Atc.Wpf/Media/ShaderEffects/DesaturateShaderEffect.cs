namespace Atc.Wpf.Media.ShaderEffects;

/// <summary>
/// A shader effect that desaturates the colors of the input.
/// </summary>
public sealed class DesaturateShaderEffect : ShaderEffectBase
{
    /// <summary>Identifies the <see cref="Input"/> dependency property.</summary>
    public static readonly DependencyProperty InputProperty =
        RegisterPixelShaderSamplerProperty(
            "Input",
            typeof(DesaturateShaderEffect),
            0);

    /// <summary>Identifies the <see cref="Strength"/> dependency property.</summary>
    public static readonly DependencyProperty StrengthProperty =
        DependencyProperty.Register(
            nameof(Strength),
            typeof(double),
            typeof(DesaturateShaderEffect),
            new UIPropertyMetadata(
                0d,
                PixelShaderConstantCallback(0)));

    /// <inheritdoc />
    public override string Name => "Desaturate";

    /// <summary>
    /// Initializes a new instance of the <see cref="DesaturateShaderEffect"/> class.
    /// </summary>
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