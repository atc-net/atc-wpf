// ReSharper disable CheckNamespace
namespace Atc.Wpf.ValueConverters;

/// <summary>
/// Library-wide fallback values used by color/brush converters when their bound value is
/// <see langword="null"/> or otherwise unbindable.
/// </summary>
/// <remarks>
/// Many converters render <see cref="Colors.DeepPink"/> as a visible "you wired the binding wrong"
/// sentinel. Centralising the value here lets consumers retheme this single source — useful for
/// apps that prefer a neutral fallback like transparent or grey, or that want to flag binding
/// errors in a different colour.
/// <para>
/// Override at application startup:
/// <code>
/// BindingFallbacks.Color = Colors.Transparent;
/// </code>
/// Use <see cref="Reset"/> to restore the built-in default.
/// </para>
/// <para>
/// <see cref="Brush"/> is a frozen <see cref="SolidColorBrush"/> derived from <see cref="Color"/>;
/// it rebuilds automatically when <see cref="Color"/> changes.
/// </para>
/// </remarks>
public static class BindingFallbacks
{
    /// <summary>The built-in default fallback color (<see cref="Colors.DeepPink"/>).</summary>
    public static readonly Color DefaultColor = Colors.DeepPink;

    /// <summary>
    /// Gets or sets the library-wide fallback <see cref="Color"/>.
    /// Defaults to <see cref="DefaultColor"/>.
    /// </summary>
    public static Color Color { get; set; } = DefaultColor;

    private static SolidColorBrush? cachedBrush;
    private static Color cachedBrushColor;

    /// <summary>
    /// Gets a frozen <see cref="SolidColorBrush"/> derived from the current <see cref="Color"/>.
    /// The brush is cached and rebuilt automatically when <see cref="Color"/> changes.
    /// </summary>
    public static SolidColorBrush Brush
    {
        get
        {
            if (cachedBrush is null ||
                cachedBrushColor != Color)
            {
                var brush = new SolidColorBrush(Color);
                brush.Freeze();
                cachedBrush = brush;
                cachedBrushColor = Color;
            }

            return cachedBrush;
        }
    }

    /// <summary>Restores <see cref="Color"/> to <see cref="DefaultColor"/>.</summary>
    public static void Reset()
        => Color = DefaultColor;
}