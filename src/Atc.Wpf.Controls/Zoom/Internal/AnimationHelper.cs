namespace Atc.Wpf.Controls.Zoom.Internal;

/// <summary>
/// A helper class to simplify animation.
/// </summary>
internal static class AnimationHelper
{
    /// <summary>
    /// Starts an animation to a particular value on the specified dependency property.
    /// </summary>
    public static void StartAnimation(
        UIElement animatableElement,
        DependencyProperty dependencyProperty,
        double toValue,
        double animationDurationSeconds,
        bool useAnimations)
    {
        StartAnimation(
            animatableElement,
            dependencyProperty,
            toValue,
            animationDurationSeconds,
            completedEvent: null,
            useAnimations);
    }

    /// <summary>
    /// Starts an animation to a particular value on the specified dependency property.
    /// You can pass in an event handler to call when the animation has completed.
    /// </summary>
    public static void StartAnimation(
        UIElement animatableElement,
        DependencyProperty dependencyProperty,
        double toValue,
        double animationDurationSeconds,
        EventHandler? completedEvent,
        bool useAnimations)
    {
        if (useAnimations)
        {
            var fromValue = (double)animatableElement.GetValue(dependencyProperty);

            var animation = new DoubleAnimation
            {
                From = fromValue,
                To = toValue,
                Duration = TimeSpan.FromSeconds(animationDurationSeconds),
            };

            animation.Completed += (sender, e) =>
            {
                animatableElement.SetValue(dependencyProperty, animatableElement.GetValue(dependencyProperty));
                CancelAnimation(animatableElement, dependencyProperty);
                completedEvent?.Invoke(sender, e);
            };

            animation.Freeze();
            animatableElement.BeginAnimation(dependencyProperty, animation);
        }
        else
        {
            animatableElement.SetValue(dependencyProperty, toValue);
            completedEvent?.Invoke(sender: null, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Cancel any animations that are running on the specified dependency property.
    /// </summary>
    public static void CancelAnimation(
        UIElement animatableElement,
        DependencyProperty dependencyProperty)
    {
        animatableElement.BeginAnimation(dependencyProperty, animation: null);
    }
}