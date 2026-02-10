// ReSharper disable once CheckNamespace
namespace System.Windows;

/// <summary>
/// Extension methods for animating UIElements with fade, slide, and scale effects.
/// </summary>
public static class AnimationExtensions
{
    /// <summary>
    /// Fades the element in from transparent to opaque.
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="parameters">Optional animation parameters.</param>
    /// <returns>A task that completes when the animation finishes.</returns>
    public static async Task FadeInAsync(
        this UIElement element,
        AnimationParameters? parameters = null)
    {
        ArgumentNullException.ThrowIfNull(element);
        parameters ??= AnimationParameters.Default;

        if (parameters.Delay > TimeSpan.Zero)
        {
            await Task.Delay(parameters.Delay).ConfigureAwait(true);
        }

        element.Visibility = Visibility.Visible;
        element.Opacity = 0;

        var animation = new DoubleAnimation(0, 1, new Duration(parameters.Duration))
        {
            EasingFunction = parameters.EasingFunction,
        };

        await RunAnimationAsync(element, UIElement.OpacityProperty, animation).ConfigureAwait(false);
    }

    /// <summary>
    /// Fades the element out from opaque to transparent, then collapses it.
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="parameters">Optional animation parameters.</param>
    /// <returns>A task that completes when the animation finishes.</returns>
    public static async Task FadeOutAsync(
        this UIElement element,
        AnimationParameters? parameters = null)
    {
        ArgumentNullException.ThrowIfNull(element);
        parameters ??= AnimationParameters.Default;

        if (parameters.Delay > TimeSpan.Zero)
        {
            await Task.Delay(parameters.Delay).ConfigureAwait(true);
        }

        var animation = new DoubleAnimation(1, 0, new Duration(parameters.Duration))
        {
            EasingFunction = parameters.EasingFunction,
        };

        await RunAnimationAsync(element, UIElement.OpacityProperty, animation).ConfigureAwait(true);
        element.Visibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Slides the element in from the specified direction.
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="direction">The direction to slide from.</param>
    /// <param name="parameters">Optional animation parameters.</param>
    /// <returns>A task that completes when the animation finishes.</returns>
    public static async Task SlideInFromAsync(
        this UIElement element,
        SlideDirection direction,
        AnimationParameters? parameters = null)
    {
        ArgumentNullException.ThrowIfNull(element);
        parameters ??= AnimationParameters.Default;

        if (parameters.Delay > TimeSpan.Zero)
        {
            await Task.Delay(parameters.Delay).ConfigureAwait(true);
        }

        element.Visibility = Visibility.Visible;

        var translateTransform = EnsureTranslateTransform(element);
        var (property, startValue) = GetSlidePropertyAndStartValue(direction, parameters.SlideDistance);

        translateTransform.SetValue(property, startValue);

        var animation = new DoubleAnimation(startValue, 0, new Duration(parameters.Duration))
        {
            EasingFunction = parameters.EasingFunction,
        };

        await RunAnimationAsync(translateTransform, property, animation).ConfigureAwait(false);
    }

    /// <summary>
    /// Slides the element out to the specified direction, then collapses it.
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="direction">The direction to slide to.</param>
    /// <param name="parameters">Optional animation parameters.</param>
    /// <returns>A task that completes when the animation finishes.</returns>
    public static async Task SlideOutToAsync(
        this UIElement element,
        SlideDirection direction,
        AnimationParameters? parameters = null)
    {
        ArgumentNullException.ThrowIfNull(element);
        parameters ??= AnimationParameters.Default;

        if (parameters.Delay > TimeSpan.Zero)
        {
            await Task.Delay(parameters.Delay).ConfigureAwait(true);
        }

        var translateTransform = EnsureTranslateTransform(element);
        var (property, endValue) = GetSlidePropertyAndStartValue(direction, parameters.SlideDistance);

        var animation = new DoubleAnimation(0, endValue, new Duration(parameters.Duration))
        {
            EasingFunction = parameters.EasingFunction,
        };

        await RunAnimationAsync(translateTransform, property, animation).ConfigureAwait(true);
        element.Visibility = Visibility.Collapsed;
        translateTransform.SetValue(property, 0d);
    }

    /// <summary>
    /// Scales the element in from zero to full size.
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="parameters">Optional animation parameters.</param>
    /// <returns>A task that completes when the animation finishes.</returns>
    public static async Task ScaleInAsync(
        this UIElement element,
        AnimationParameters? parameters = null)
    {
        ArgumentNullException.ThrowIfNull(element);
        parameters ??= AnimationParameters.Default;

        if (parameters.Delay > TimeSpan.Zero)
        {
            await Task.Delay(parameters.Delay).ConfigureAwait(true);
        }

        element.Visibility = Visibility.Visible;

        var scaleTransform = EnsureScaleTransform(element);
        scaleTransform.ScaleX = 0;
        scaleTransform.ScaleY = 0;

        var animationX = new DoubleAnimation(0, 1, new Duration(parameters.Duration))
        {
            EasingFunction = parameters.EasingFunction,
        };

        var animationY = new DoubleAnimation(0, 1, new Duration(parameters.Duration))
        {
            EasingFunction = parameters.EasingFunction,
        };

        await RunScaleStoryboardAsync(element, animationX, animationY).ConfigureAwait(false);
    }

    /// <summary>
    /// Scales the element out from full size to zero, then collapses it.
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="parameters">Optional animation parameters.</param>
    /// <returns>A task that completes when the animation finishes.</returns>
    public static async Task ScaleOutAsync(
        this UIElement element,
        AnimationParameters? parameters = null)
    {
        ArgumentNullException.ThrowIfNull(element);
        parameters ??= AnimationParameters.Default;

        if (parameters.Delay > TimeSpan.Zero)
        {
            await Task.Delay(parameters.Delay).ConfigureAwait(true);
        }

        EnsureScaleTransform(element);

        var animationX = new DoubleAnimation(1, 0, new Duration(parameters.Duration))
        {
            EasingFunction = parameters.EasingFunction,
        };

        var animationY = new DoubleAnimation(1, 0, new Duration(parameters.Duration))
        {
            EasingFunction = parameters.EasingFunction,
        };

        await RunScaleStoryboardAsync(element, animationX, animationY).ConfigureAwait(true);
        element.Visibility = Visibility.Collapsed;
    }

    /// <summary>
    /// Animates the element using the specified animation kind.
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="kind">The kind of animation to apply.</param>
    /// <param name="parameters">Optional animation parameters.</param>
    /// <returns>A task that completes when the animation finishes.</returns>
    public static Task AnimateAsync(
        this UIElement element,
        AnimationKind kind,
        AnimationParameters? parameters = null)
    {
        ArgumentNullException.ThrowIfNull(element);

        return kind switch
        {
            AnimationKind.None => Task.CompletedTask,
            AnimationKind.FadeIn => element.FadeInAsync(parameters),
            AnimationKind.FadeOut => element.FadeOutAsync(parameters),
            AnimationKind.SlideInFromLeft => element.SlideInFromAsync(SlideDirection.Left, parameters),
            AnimationKind.SlideInFromRight => element.SlideInFromAsync(SlideDirection.Right, parameters),
            AnimationKind.SlideInFromTop => element.SlideInFromAsync(SlideDirection.Top, parameters),
            AnimationKind.SlideInFromBottom => element.SlideInFromAsync(SlideDirection.Bottom, parameters),
            AnimationKind.SlideOutToLeft => element.SlideOutToAsync(SlideDirection.Left, parameters),
            AnimationKind.SlideOutToRight => element.SlideOutToAsync(SlideDirection.Right, parameters),
            AnimationKind.SlideOutToTop => element.SlideOutToAsync(SlideDirection.Top, parameters),
            AnimationKind.SlideOutToBottom => element.SlideOutToAsync(SlideDirection.Bottom, parameters),
            AnimationKind.ScaleIn => element.ScaleInAsync(parameters),
            AnimationKind.ScaleOut => element.ScaleOutAsync(parameters),
            _ => Task.CompletedTask,
        };
    }

    private static Task RunAnimationAsync(
        IAnimatable target,
        DependencyProperty property,
        AnimationTimeline animation)
    {
        var tcs = new TaskCompletionSource<bool>();
        animation.Completed += (_, _) => tcs.TrySetResult(true);
        target.BeginAnimation(property, animation);
        return tcs.Task;
    }

    private static Task RunScaleStoryboardAsync(
        UIElement element,
        DoubleAnimation animationX,
        DoubleAnimation animationY)
    {
        var tcs = new TaskCompletionSource<bool>();

        var storyboard = new Storyboard();
        storyboard.Children.Add(animationX);
        storyboard.Children.Add(animationY);

        Storyboard.SetTarget(animationX, element);
        Storyboard.SetTarget(animationY, element);
        Storyboard.SetTargetProperty(animationX, GetScaleTransformPropertyPath(element, "ScaleX"));
        Storyboard.SetTargetProperty(animationY, GetScaleTransformPropertyPath(element, "ScaleY"));

        storyboard.Completed += (_, _) => tcs.TrySetResult(true);
        storyboard.Begin();

        return tcs.Task;
    }

    private static TranslateTransform EnsureTranslateTransform(
        UIElement element)
    {
        if (element.RenderTransform is TranslateTransform existing)
        {
            return existing;
        }

        if (element.RenderTransform is TransformGroup group)
        {
            foreach (var transform in group.Children)
            {
                if (transform is TranslateTransform translate)
                {
                    return translate;
                }
            }

            var newTranslate = new TranslateTransform();
            group.Children.Add(newTranslate);
            return newTranslate;
        }

        var translateTransform = new TranslateTransform();
        if (element.RenderTransform is null or MatrixTransform { Matrix.IsIdentity: true })
        {
            element.RenderTransform = translateTransform;
        }
        else
        {
            var newGroup = new TransformGroup();
            newGroup.Children.Add(element.RenderTransform);
            newGroup.Children.Add(translateTransform);
            element.RenderTransform = newGroup;
        }

        return translateTransform;
    }

    private static ScaleTransform EnsureScaleTransform(UIElement element)
    {
        element.RenderTransformOrigin = new Point(0.5, 0.5);

        if (element.RenderTransform is ScaleTransform existing)
        {
            return existing;
        }

        if (element.RenderTransform is TransformGroup group)
        {
            foreach (var transform in group.Children)
            {
                if (transform is ScaleTransform scale)
                {
                    return scale;
                }
            }

            var newScale = new ScaleTransform(1, 1);
            group.Children.Add(newScale);
            return newScale;
        }

        var scaleTransform = new ScaleTransform(1, 1);
        if (element.RenderTransform is null or MatrixTransform { Matrix.IsIdentity: true })
        {
            element.RenderTransform = scaleTransform;
        }
        else
        {
            var newGroup = new TransformGroup();
            newGroup.Children.Add(element.RenderTransform);
            newGroup.Children.Add(scaleTransform);
            element.RenderTransform = newGroup;
        }

        return scaleTransform;
    }

    private static (DependencyProperty Property, double Value) GetSlidePropertyAndStartValue(
        SlideDirection direction,
        double distance) =>
        direction switch
        {
            SlideDirection.Left => (TranslateTransform.XProperty, -distance),
            SlideDirection.Right => (TranslateTransform.XProperty, distance),
            SlideDirection.Top => (TranslateTransform.YProperty, -distance),
            SlideDirection.Bottom => (TranslateTransform.YProperty, distance),
            _ => (TranslateTransform.XProperty, 0),
        };

    [SuppressMessage("Blocker Code Smell", "S3220:Method calls should not resolve ambiguously to overloads with \"params\"", Justification = "Must use string path constructor, not object accessor constructor")]
    private static PropertyPath GetScaleTransformPropertyPath(
        UIElement element,
        string scaleProperty)
    {
        if (element.RenderTransform is ScaleTransform)
        {
            return new PropertyPath("RenderTransform." + scaleProperty);
        }

        if (element.RenderTransform is TransformGroup group)
        {
            for (var i = 0; i < group.Children.Count; i++)
            {
                if (group.Children[i] is ScaleTransform)
                {
                    return new PropertyPath(
                        "(UIElement.RenderTransform).(TransformGroup.Children)[" + i + "].(" + scaleProperty + ")");
                }
            }
        }

        return new PropertyPath("RenderTransform." + scaleProperty);
    }
}