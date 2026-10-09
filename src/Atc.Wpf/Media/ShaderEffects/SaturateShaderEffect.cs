namespace Atc.Wpf.Media.ShaderEffects;

/// <summary>
/// A shader effect that applies a saturation transition between two inputs controlled by a progress value.
/// </summary>
public sealed class SaturateShaderEffect : ShaderEffectBase
{
    /// <summary>Identifies the <see cref="Input"/> dependency property.</summary>
    public static readonly DependencyProperty InputProperty =
        RegisterPixelShaderSamplerProperty(
            "Input",
            typeof(SaturateShaderEffect),
            0);

    /// <summary>Identifies the <see cref="SecondInput"/> dependency property.</summary>
    public static readonly DependencyProperty SecondInputProperty =
        RegisterPixelShaderSamplerProperty(
            "SecondInput",
            typeof(SaturateShaderEffect),
            1);

    /// <summary>Identifies the <see cref="Progress"/> dependency property.</summary>
    public static readonly DependencyProperty ProgressProperty =
        DependencyProperty.Register(
            nameof(Progress),
            typeof(double),
            typeof(SaturateShaderEffect),
            new UIPropertyMetadata(
                0d,
                PixelShaderConstantCallback(0)));

    /// <inheritdoc />
    public override string Name => "Saturate";

    /// <summary>
    /// Initializes a new instance of the <see cref="SaturateShaderEffect"/> class.
    /// </summary>
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