namespace Atc.Wpf.Media.ShaderEffects;

/// <summary>
/// A shader effect that fades the input toward a target color.
/// </summary>
public sealed class FadeShaderEffect : ShaderEffectBase
{
    /// <summary>Identifies the <see cref="Input"/> dependency property.</summary>
    public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty(
            "Input",
            typeof(FadeShaderEffect),
            0);

    /// <summary>Identifies the <see cref="Strength"/> dependency property.</summary>
    public static readonly DependencyProperty StrengthProperty = DependencyProperty.Register(
            nameof(Strength),
            typeof(double),
            typeof(FadeShaderEffect),
            new UIPropertyMetadata(
                0d,
                PixelShaderConstantCallback(0)));

    /// <summary>Identifies the <see cref="ToColor"/> dependency property.</summary>
    public static readonly DependencyProperty ToColorProperty = DependencyProperty.Register(
        nameof(ToColor),
        typeof(Color),
        typeof(FadeShaderEffect),
        new UIPropertyMetadata(
            MakeColor(255, 0, 0, 0),
            PixelShaderConstantCallback(2)));

    /// <inheritdoc />
    public override string Name => "Fade";

    /// <summary>
    /// Initializes a new instance of the <see cref="FadeShaderEffect"/> class.
    /// </summary>
    public FadeShaderEffect()
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

    /// <summary>
    /// ToColor - default => 0x00, 0x00, 0x00.
    /// </summary>
    public Color ToColor
    {
        get => (Color)GetValue(ToColorProperty);
        set => SetValue(ToColorProperty, value);
    }
}