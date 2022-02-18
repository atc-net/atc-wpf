namespace Atc.Wpf.Media.ShaderEffects;

public abstract class ShaderEffectBase : ShaderEffect
{
    public abstract string Name { get; }

    [SuppressMessage("Design", "MA0056:Do not call overridable members in constructor", Justification = "By design.")]
    protected ShaderEffectBase() => this.PixelShader = new PixelShader
    {
        UriSource = new Uri(
            $"pack://application:,,,/Atc.Wpf;component/Media/ShaderEffects/Shaders/{this.Name}.ps",
            UriKind.Absolute),
    };

    protected static Color MakeColor(byte r, byte g, byte b)
    {
        return MakeColor(0xFF, r, g, b);
    }

    protected static Color MakeColor(byte alpha, byte r, byte g, byte b)
    {
        return Color.FromArgb(alpha, r, g, b);
    }
}