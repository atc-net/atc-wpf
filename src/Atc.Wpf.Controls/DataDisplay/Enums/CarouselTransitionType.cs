// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls;

/// <summary>
/// Specifies the transition animation type for the Carousel control.
/// </summary>
public enum CarouselTransitionType
{
    /// <summary>
    /// Instant change with no animation.
    /// </summary>
    None,

    /// <summary>
    /// Horizontal slide animation.
    /// </summary>
    Slide,

    /// <summary>
    /// Crossfade animation.
    /// </summary>
    Fade,

    /// <summary>
    /// Combined slide and fade animation.
    /// </summary>
    SlideAndFade,
}