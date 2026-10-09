namespace Atc.Wpf.Media.ShaderEffects;

/// <summary>
/// A shader effect that adjusts the brightness and contrast of the input.
/// </summary>
public sealed class ContrastAdjustShaderEffect : ShaderEffectBase
{
    /// <summary>Identifies the <see cref="Input"/> dependency property.</summary>
    public static readonly DependencyProperty InputProperty =
        RegisterPixelShaderSamplerProperty(
            "Input",
            typeof(ContrastAdjustShaderEffect),
            0);

    /// <summary>Identifies the <see cref="Brightness"/> dependency property.</summary>
    public static readonly DependencyProperty BrightnessProperty =
        DependencyProperty.Register(
            nameof(Brightness),
            typeof(double),
            typeof(ContrastAdjustShaderEffect),
            new UIPropertyMetadata(
                defaultValue: 0d,
                PixelShaderConstantCallback(0)));

    /// <summary>Identifies the <see cref="Contrast"/> dependency property.</summary>
    public static readonly DependencyProperty ContrastProperty =
        DependencyProperty.Register(
            nameof(Contrast),
            typeof(double),
            typeof(ContrastAdjustShaderEffect),
            new UIPropertyMetadata(
                defaultValue: 0d,
                PixelShaderConstantCallback(1)));

    /// <inheritdoc />
    public override string Name => "ContrastAdjust";

    /// <summary>
    /// Initializes a new instance of the <see cref="ContrastAdjustShaderEffect"/> class.
    /// </summary>
    public ContrastAdjustShaderEffect()
    {
        UpdateShaderValue(InputProperty);
        UpdateShaderValue(BrightnessProperty);
        UpdateShaderValue(ContrastProperty);
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
    /// Brightness.
    /// </summary>
    public double Brightness
    {
        get => (double)GetValue(BrightnessProperty);
        set => SetValue(BrightnessProperty, value);
    }

    /// <summary>
    /// Contrast.
    /// </summary>
    public double Contrast
    {
        get => (double)GetValue(ContrastProperty);
        set => SetValue(ContrastProperty, value);
    }
}