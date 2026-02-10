// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Animation;

/// <summary>
/// Defines the kind of animation to apply to a UIElement.
/// </summary>
public enum AnimationKind
{
    /// <summary>
    /// No animation.
    /// </summary>
    None,

    /// <summary>
    /// Fade in from transparent to opaque.
    /// </summary>
    FadeIn,

    /// <summary>
    /// Fade out from opaque to transparent.
    /// </summary>
    FadeOut,

    /// <summary>
    /// Slide in from the left.
    /// </summary>
    SlideInFromLeft,

    /// <summary>
    /// Slide in from the right.
    /// </summary>
    SlideInFromRight,

    /// <summary>
    /// Slide in from the top.
    /// </summary>
    SlideInFromTop,

    /// <summary>
    /// Slide in from the bottom.
    /// </summary>
    SlideInFromBottom,

    /// <summary>
    /// Slide out to the left.
    /// </summary>
    SlideOutToLeft,

    /// <summary>
    /// Slide out to the right.
    /// </summary>
    SlideOutToRight,

    /// <summary>
    /// Slide out to the top.
    /// </summary>
    SlideOutToTop,

    /// <summary>
    /// Slide out to the bottom.
    /// </summary>
    SlideOutToBottom,

    /// <summary>
    /// Scale in from zero to full size.
    /// </summary>
    ScaleIn,

    /// <summary>
    /// Scale out from full size to zero.
    /// </summary>
    ScaleOut,
}