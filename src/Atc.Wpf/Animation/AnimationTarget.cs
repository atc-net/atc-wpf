namespace Atc.Wpf.Animation;

/// <summary>
/// One property animated by an <see cref="AnimationRun"/>.
/// </summary>
/// <param name="Target">The animated object.</param>
/// <param name="Property">The animated property.</param>
/// <param name="Animation">The animation to apply.</param>
internal readonly record struct AnimationTarget(
    IAnimatable Target,
    DependencyProperty Property,
    AnimationTimeline Animation);