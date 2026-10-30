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
    /// <returns>
    /// A task that completes when the animation finishes, or when another animation started by these helpers
    /// takes over the same property.
    /// </returns>
    public static Task FadeInAsync(
        this UIElement element,
        AnimationParameters? parameters = null)
        => element.FadeInAsync(parameters, CancellationToken.None);

    /// <summary>
    /// Fades the element in from transparent to opaque.
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="parameters">Optional animation parameters.</param>
    /// <param name="cancellationToken">Stops the animation where it is and cancels the task.</param>
    /// <returns>
    /// A task that completes when the animation finishes, or when another animation started by these helpers
    /// takes over the same property.
    /// </returns>
    public static Task FadeInAsync(
        this UIElement element,
        AnimationParameters? parameters,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(element);
        parameters ??= AnimationParameters.Default;

        return RunAsync(
            element,
            parameters,
            () =>
            {
                element.Visibility = Visibility.Visible;
                element.Opacity = 0;

                return new AnimationRun(
                    [new AnimationTarget(element, UIElement.OpacityProperty, CreateAnimation(0, 1, parameters))],
                    onFinished: null);
            },
            cancellationToken);
    }

    /// <summary>
    /// Fades the element out from opaque to transparent, then collapses it.
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="parameters">Optional animation parameters.</param>
    /// <returns>
    /// A task that completes when the animation finishes, or when another animation started by these helpers
    /// takes over the same property (the element is then not collapsed).
    /// </returns>
    public static Task FadeOutAsync(
        this UIElement element,
        AnimationParameters? parameters = null)
        => element.FadeOutAsync(parameters, CancellationToken.None);

    /// <summary>
    /// Fades the element out from opaque to transparent, then collapses it.
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="parameters">Optional animation parameters.</param>
    /// <param name="cancellationToken">Stops the animation where it is and cancels the task; the element is not collapsed.</param>
    /// <returns>
    /// A task that completes when the animation finishes, or when another animation started by these helpers
    /// takes over the same property (the element is then not collapsed).
    /// </returns>
    public static Task FadeOutAsync(
        this UIElement element,
        AnimationParameters? parameters,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(element);
        parameters ??= AnimationParameters.Default;

        return RunAsync(
            element,
            parameters,
            () => new AnimationRun(
                [new AnimationTarget(element, UIElement.OpacityProperty, CreateAnimation(1, 0, parameters))],
                onFinished: () => element.Visibility = Visibility.Collapsed),
            cancellationToken);
    }

    /// <summary>
    /// Slides the element in from the specified direction.
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="direction">The direction to slide from.</param>
    /// <param name="parameters">Optional animation parameters.</param>
    /// <returns>
    /// A task that completes when the animation finishes, or when another animation started by these helpers
    /// takes over the same property.
    /// </returns>
    public static Task SlideInFromAsync(
        this UIElement element,
        SlideDirection direction,
        AnimationParameters? parameters = null)
        => element.SlideInFromAsync(direction, parameters, CancellationToken.None);

    /// <summary>
    /// Slides the element in from the specified direction.
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="direction">The direction to slide from.</param>
    /// <param name="parameters">Optional animation parameters.</param>
    /// <param name="cancellationToken">Stops the animation where it is and cancels the task.</param>
    /// <returns>
    /// A task that completes when the animation finishes, or when another animation started by these helpers
    /// takes over the same property.
    /// </returns>
    public static Task SlideInFromAsync(
        this UIElement element,
        SlideDirection direction,
        AnimationParameters? parameters,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(element);
        parameters ??= AnimationParameters.Default;

        return RunAsync(
            element,
            parameters,
            () =>
            {
                element.Visibility = Visibility.Visible;

                var translateTransform = EnsureTranslateTransform(element);
                var (property, startValue) = GetSlidePropertyAndStartValue(direction, parameters.SlideDistance);

                translateTransform.SetValue(property, startValue);

                return new AnimationRun(
                    [new AnimationTarget(translateTransform, property, CreateAnimation(startValue, 0, parameters))],
                    onFinished: null);
            },
            cancellationToken);
    }

    /// <summary>
    /// Slides the element out to the specified direction, then collapses it.
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="direction">The direction to slide to.</param>
    /// <param name="parameters">Optional animation parameters.</param>
    /// <returns>
    /// A task that completes when the animation finishes, or when another animation started by these helpers
    /// takes over the same property (the element is then not collapsed).
    /// </returns>
    public static Task SlideOutToAsync(
        this UIElement element,
        SlideDirection direction,
        AnimationParameters? parameters = null)
        => element.SlideOutToAsync(direction, parameters, CancellationToken.None);

    /// <summary>
    /// Slides the element out to the specified direction, then collapses it.
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="direction">The direction to slide to.</param>
    /// <param name="parameters">Optional animation parameters.</param>
    /// <param name="cancellationToken">Stops the animation where it is and cancels the task; the element is not collapsed.</param>
    /// <returns>
    /// A task that completes when the animation finishes, or when another animation started by these helpers
    /// takes over the same property (the element is then not collapsed).
    /// </returns>
    public static Task SlideOutToAsync(
        this UIElement element,
        SlideDirection direction,
        AnimationParameters? parameters,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(element);
        parameters ??= AnimationParameters.Default;

        return RunAsync(
            element,
            parameters,
            () =>
            {
                var translateTransform = EnsureTranslateTransform(element);
                var (property, endValue) = GetSlidePropertyAndStartValue(direction, parameters.SlideDistance);

                return new AnimationRun(
                    [new AnimationTarget(translateTransform, property, CreateAnimation(0, endValue, parameters))],
                    onFinished: () =>
                    {
                        element.Visibility = Visibility.Collapsed;
                        translateTransform.SetValue(property, 0d);
                    });
            },
            cancellationToken);
    }

    /// <summary>
    /// Scales the element in from zero to full size.
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="parameters">Optional animation parameters.</param>
    /// <returns>
    /// A task that completes when the animation finishes, or when another animation started by these helpers
    /// takes over the same property.
    /// </returns>
    public static Task ScaleInAsync(
        this UIElement element,
        AnimationParameters? parameters = null)
        => element.ScaleInAsync(parameters, CancellationToken.None);

    /// <summary>
    /// Scales the element in from zero to full size.
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="parameters">Optional animation parameters.</param>
    /// <param name="cancellationToken">Stops the animation where it is and cancels the task.</param>
    /// <returns>
    /// A task that completes when the animation finishes, or when another animation started by these helpers
    /// takes over the same property.
    /// </returns>
    public static Task ScaleInAsync(
        this UIElement element,
        AnimationParameters? parameters,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(element);
        parameters ??= AnimationParameters.Default;

        return RunAsync(
            element,
            parameters,
            () =>
            {
                element.Visibility = Visibility.Visible;

                var scaleTransform = EnsureScaleTransform(element);
                scaleTransform.ScaleX = 0;
                scaleTransform.ScaleY = 0;

                return new AnimationRun(
                    [
                        new AnimationTarget(scaleTransform, ScaleTransform.ScaleXProperty, CreateAnimation(0, 1, parameters)),
                        new AnimationTarget(scaleTransform, ScaleTransform.ScaleYProperty, CreateAnimation(0, 1, parameters)),
                    ],
                    onFinished: null);
            },
            cancellationToken);
    }

    /// <summary>
    /// Scales the element out from full size to zero, then collapses it.
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="parameters">Optional animation parameters.</param>
    /// <returns>
    /// A task that completes when the animation finishes, or when another animation started by these helpers
    /// takes over the same property (the element is then not collapsed).
    /// </returns>
    public static Task ScaleOutAsync(
        this UIElement element,
        AnimationParameters? parameters = null)
        => element.ScaleOutAsync(parameters, CancellationToken.None);

    /// <summary>
    /// Scales the element out from full size to zero, then collapses it.
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="parameters">Optional animation parameters.</param>
    /// <param name="cancellationToken">Stops the animation where it is and cancels the task; the element is not collapsed.</param>
    /// <returns>
    /// A task that completes when the animation finishes, or when another animation started by these helpers
    /// takes over the same property (the element is then not collapsed).
    /// </returns>
    public static Task ScaleOutAsync(
        this UIElement element,
        AnimationParameters? parameters,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(element);
        parameters ??= AnimationParameters.Default;

        return RunAsync(
            element,
            parameters,
            () =>
            {
                var scaleTransform = EnsureScaleTransform(element);

                return new AnimationRun(
                    [
                        new AnimationTarget(scaleTransform, ScaleTransform.ScaleXProperty, CreateAnimation(1, 0, parameters)),
                        new AnimationTarget(scaleTransform, ScaleTransform.ScaleYProperty, CreateAnimation(1, 0, parameters)),
                    ],
                    onFinished: () => element.Visibility = Visibility.Collapsed);
            },
            cancellationToken);
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
        => element.AnimateAsync(kind, parameters, CancellationToken.None);

    /// <summary>
    /// Animates the element using the specified animation kind.
    /// </summary>
    /// <param name="element">The element to animate.</param>
    /// <param name="kind">The kind of animation to apply.</param>
    /// <param name="parameters">Optional animation parameters.</param>
    /// <param name="cancellationToken">Stops the animation where it is and cancels the task.</param>
    /// <returns>A task that completes when the animation finishes.</returns>
    public static Task AnimateAsync(
        this UIElement element,
        AnimationKind kind,
        AnimationParameters? parameters,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(element);

        return kind switch
        {
            AnimationKind.None => Task.CompletedTask,
            AnimationKind.FadeIn => element.FadeInAsync(parameters, cancellationToken),
            AnimationKind.FadeOut => element.FadeOutAsync(parameters, cancellationToken),
            AnimationKind.SlideInFromLeft => element.SlideInFromAsync(SlideDirection.Left, parameters, cancellationToken),
            AnimationKind.SlideInFromRight => element.SlideInFromAsync(SlideDirection.Right, parameters, cancellationToken),
            AnimationKind.SlideInFromTop => element.SlideInFromAsync(SlideDirection.Top, parameters, cancellationToken),
            AnimationKind.SlideInFromBottom => element.SlideInFromAsync(SlideDirection.Bottom, parameters, cancellationToken),
            AnimationKind.SlideOutToLeft => element.SlideOutToAsync(SlideDirection.Left, parameters, cancellationToken),
            AnimationKind.SlideOutToRight => element.SlideOutToAsync(SlideDirection.Right, parameters, cancellationToken),
            AnimationKind.SlideOutToTop => element.SlideOutToAsync(SlideDirection.Top, parameters, cancellationToken),
            AnimationKind.SlideOutToBottom => element.SlideOutToAsync(SlideDirection.Bottom, parameters, cancellationToken),
            AnimationKind.ScaleIn => element.ScaleInAsync(parameters, cancellationToken),
            AnimationKind.ScaleOut => element.ScaleOutAsync(parameters, cancellationToken),
            _ => Task.CompletedTask,
        };
    }

    private static DoubleAnimation CreateAnimation(
        double from,
        double to,
        AnimationParameters parameters)
        => new(from, to, new Duration(parameters.Duration))
        {
            EasingFunction = parameters.EasingFunction,
        };

    private static async Task RunAsync(
        UIElement element,
        AnimationParameters parameters,
        Func<AnimationRun> createRun,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (parameters.Delay > TimeSpan.Zero)
        {
            await Task.Delay(parameters.Delay, cancellationToken).ConfigureAwait(true);
        }

        var dispatcher = element.Dispatcher;
        var completion = dispatcher.CheckAccess()
            ? StartRun(createRun, dispatcher, cancellationToken)
            : await dispatcher.InvokeAsync(() => StartRun(createRun, dispatcher, cancellationToken));

        await completion.ConfigureAwait(false);
    }

    private static Task StartRun(
        Func<AnimationRun> createRun,
        Dispatcher dispatcher,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var run = createRun();
        run.Start(dispatcher, cancellationToken);
        return run.Completion;
    }

    private static TranslateTransform EnsureTranslateTransform(
        UIElement element)
    {
        ThawRenderTransform(element);

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
        ThawRenderTransform(element);

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

    /// <summary>
    /// Replaces a frozen render transform (for example one from a shared resource) with an unfrozen copy,
    /// because a frozen transform cannot be animated.
    /// </summary>
    private static void ThawRenderTransform(UIElement element)
    {
        if (element.RenderTransform is { IsFrozen: true } frozen)
        {
            element.RenderTransform = frozen.Clone();
        }
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
}