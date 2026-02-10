namespace Atc.Wpf.Animation;

/// <summary>
/// Configuration parameters for UI animations.
/// </summary>
public sealed class AnimationParameters
{
    /// <summary>
    /// Gets the duration of the animation.
    /// </summary>
    public TimeSpan Duration { get; init; } = TimeSpan.FromMilliseconds(300);

    /// <summary>
    /// Gets the easing function applied to the animation.
    /// </summary>
    public IEasingFunction? EasingFunction { get; init; } = new CubicEase { EasingMode = EasingMode.EaseOut };

    /// <summary>
    /// Gets the delay before the animation starts.
    /// </summary>
    public TimeSpan Delay { get; init; } = TimeSpan.Zero;

    /// <summary>
    /// Gets the distance in pixels for slide animations.
    /// </summary>
    public double SlideDistance { get; init; } = 50;

    /// <summary>
    /// Gets the default animation parameters (300ms, CubicEase EaseOut).
    /// </summary>
    public static AnimationParameters Default { get; } = new();

    /// <summary>
    /// Gets fast animation parameters (150ms).
    /// </summary>
    public static AnimationParameters Fast { get; } = new() { Duration = TimeSpan.FromMilliseconds(150) };

    /// <summary>
    /// Gets slow animation parameters (500ms).
    /// </summary>
    public static AnimationParameters Slow { get; } = new() { Duration = TimeSpan.FromMilliseconds(500) };
}