namespace Atc.Wpf.Media.ShaderEffects;

/// <summary>
/// A shader effect that renders the input in a single filter color.
/// </summary>
public sealed class MonochromeShaderEffect : ShaderEffectBase
{
    /// <summary>Identifies the <see cref="Input"/> dependency property.</summary>
    public static readonly DependencyProperty InputProperty =
        RegisterPixelShaderSamplerProperty(
            "Input",
            typeof(MonochromeShaderEffect),
            0);

    /// <summary>Identifies the <see cref="FilterColor"/> dependency property.</summary>
    public static readonly DependencyProperty FilterColorProperty =
        DependencyProperty.Register(
            nameof(FilterColor),
            typeof(Color),
            typeof(MonochromeShaderEffect),
            new UIPropertyMetadata(
                MakeColor(0x7F, 0x7F, 0x7F),
                PixelShaderConstantCallback(0)));

    /// <inheritdoc />
    public override string Name => "Monochrome";

    /// <summary>
    /// Initializes a new instance of the <see cref="MonochromeShaderEffect"/> class.
    /// </summary>
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