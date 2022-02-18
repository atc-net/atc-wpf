namespace Atc.Wpf.Media.ShaderEffects;

public class FadeShaderEffect : ShaderEffectBase
{
    public static readonly DependencyProperty InputProperty =
        RegisterPixelShaderSamplerProperty(
            "Input",
            typeof(FadeShaderEffect),
            0);

    public static readonly DependencyProperty StrengthProperty =
        DependencyProperty.Register(
            nameof(Strength),
            typeof(double),
            typeof(FadeShaderEffect),
            new UIPropertyMetadata(
                default(double),
                PixelShaderConstantCallback(0)));

    public static readonly DependencyProperty ToColorProperty = DependencyProperty.Register(
        nameof(ToColor),
        typeof(Color),
        typeof(FadeShaderEffect),
        new UIPropertyMetadata(
            MakeColor(255, 0, 0, 0),
            PixelShaderConstantCallback(2)));

    public override string Name => "Fade";

    public FadeShaderEffect()
    {
        this.UpdateShaderValue(InputProperty);
        this.UpdateShaderValue(StrengthProperty);
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
        get => (Color)this.GetValue(ToColorProperty);
        set => this.SetValue(ToColorProperty, value);
    }
}