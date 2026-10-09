namespace Atc.Wpf.Media.ShaderEffects;

/// <summary>
/// Base class for pixel shader effects that load their compiled shader (<c>{Name}.ps</c>) from the Atc.Wpf assembly resources.
/// </summary>
public abstract class ShaderEffectBase : ShaderEffect
{
    /// <summary>
    /// Gets the name of the shader, used to locate the compiled pixel shader resource.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderEffectBase"/> class and loads the pixel shader.
    /// </summary>
    [SuppressMessage("Design", "MA0056:Do not call overridable members in constructor", Justification = "By design.")]
    protected ShaderEffectBase()
        => PixelShader = new PixelShader
        {
            UriSource = new Uri(
                $"pack://application:,,,/Atc.Wpf;component/Media/ShaderEffects/Shaders/{Name}.ps",
                UriKind.Absolute),
        };

    /// <summary>
    /// Creates an opaque <see cref="Color"/> from the specified red, green and blue components.
    /// </summary>
    protected static Color MakeColor(
        byte r,
        byte g,
        byte b)
        => MakeColor(
            0xFF,
            r,
            g,
            b);

    /// <summary>
    /// Creates a <see cref="Color"/> from the specified alpha, red, green and blue components.
    /// </summary>
    protected static Color MakeColor(
        byte alpha,
        byte r,
        byte g,
        byte b)
        => Color.FromArgb(
            alpha,
            r,
            g,
            b);
}