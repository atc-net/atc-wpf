namespace Atc.Wpf.Media.ShaderEffects;

/// <summary>
/// A shader effect that inverts the colors of the input.
/// </summary>
public sealed class InvertColorsShaderEffect : ShaderEffectBase
{
    /// <summary>Identifies the <see cref="Input"/> dependency property.</summary>
    public static readonly DependencyProperty InputProperty =
        RegisterPixelShaderSamplerProperty(
            "Input",
            typeof(InvertColorsShaderEffect),
            0);

    /// <inheritdoc />
    public override string Name => "InvertColors";

    /// <summary>
    /// Initializes a new instance of the <see cref="InvertColorsShaderEffect"/> class.
    /// </summary>
    public InvertColorsShaderEffect()
    {
        UpdateShaderValue(InputProperty);
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