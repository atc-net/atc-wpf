namespace Atc.Wpf.Media.ShaderEffects;

public sealed class ContrastAdjustShaderEffect : ShaderEffectBase
{
    public static readonly DependencyProperty InputProperty =
        RegisterPixelShaderSamplerProperty(
            "Input",
            typeof(ContrastAdjustShaderEffect),
            0);

    public static readonly DependencyProperty BrightnessProperty =
        DependencyProperty.Register(
            nameof(Brightness),
            typeof(double),
            typeof(ContrastAdjustShaderEffect),
            new UIPropertyMetadata(
                defaultValue: 0d,
                PixelShaderConstantCallback(0)));

    public static readonly DependencyProperty ContrastProperty =
        DependencyProperty.Register(
            nameof(Contrast),
            typeof(double),
            typeof(ContrastAdjustShaderEffect),
            new UIPropertyMetadata(
                defaultValue: 0d,
                PixelShaderConstantCallback(1)));

    public override string Name => "ContrastAdjust";

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