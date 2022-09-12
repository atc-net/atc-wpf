namespace Atc.Wpf.Media.ShaderEffects;

public class MonochromeShaderEffect : ShaderEffectBase
{
    public static readonly DependencyProperty InputProperty =
        RegisterPixelShaderSamplerProperty(
            "Input",
            typeof(MonochromeShaderEffect),
            0);

    public static readonly DependencyProperty FilterColorProperty =
        DependencyProperty.Register(
            nameof(FilterColor),
            typeof(Color),
            typeof(MonochromeShaderEffect),
            new UIPropertyMetadata(
                MakeColor(0x7F, 0x7F, 0x7F),
                PixelShaderConstantCallback(0)));

    public override string Name => "Monochrome";

    public MonochromeShaderEffect()
    {
        UpdateShaderValue(InputProperty);
        UpdateShaderValue(FilterColorProperty);
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
    /// FilterColor - default => 0x7F, 0x7F, 0x7F.
    /// </summary>
    public Color FilterColor
    {
        get => (Color)GetValue(FilterColorProperty);
        set => SetValue(FilterColorProperty, value);
    }
}