// ReSharper disable once CheckNamespace
namespace System.Windows;

/// <summary>
/// Extension methods for Control.
/// </summary>
[SuppressMessage("Design", "CA2201:Do not raise reserved exception types", Justification = "OK.")]
[SuppressMessage("Design", "S112:General exceptions should never be thrown", Justification = "OK.")]
public static class ControlExtensions
{
    /// <summary>
    /// Start the spinning animation.
    /// </summary>
    /// <typeparam name="T">FrameworkElement and ISpinable.</typeparam>
    /// <param name="control">Control to apply the rotation.</param>
    public static void BeginSpin<T>(this T control)
        where T : FrameworkElement, ISpinable
    {
        if (control is null)
        {
            throw new ArgumentNullException(nameof(control));
        }

        var transformGroup = control.RenderTransform as TransformGroup ?? new TransformGroup();

        var rotateTransform = transformGroup.Children.OfType<RotateTransform>().FirstOrDefault();

        if (rotateTransform is not null)
        {
            rotateTransform.Angle = 0;
        }
        else
        {
            transformGroup.Children.Add(new RotateTransform(0.0));
            control.SetCurrentValue(UIElement.RenderTransformProperty, transformGroup);
            control.SetCurrentValue(UIElement.RenderTransformOriginProperty, new Point(0.5, 0.5));
        }

        var storyboard = new Storyboard();

        var animation = new DoubleAnimation
        {
            From = 0,
            To = 360,
            AutoReverse = false,
            RepeatBehavior = RepeatBehavior.Forever,
            Duration = new Duration(TimeSpan.FromSeconds(control.SpinDuration)),
        };

        storyboard.Children.Add(animation);

        Storyboard.SetTarget(animation, control);
        Storyboard.SetTargetProperty(
            animation,
            new PropertyPath(
                "(0).(1)[0].(2)",
                UIElement.RenderTransformProperty,
                TransformGroup.ChildrenProperty,
                RotateTransform.AngleProperty));

        storyboard.Begin();
        var spinnerName = GetSpinnerName(control);
        if (control.Resources[spinnerName] is Storyboard oldStoryboard)
        {
            oldStoryboard.Stop();
            control.Resources.Remove(spinnerName);
        }

        control.Resources.Add(spinnerName, storyboard);
    }

    /// <summary>
    /// Stop the spinning animation.
    /// </summary>
    /// <typeparam name="T">FrameworkElement and ISpinable.</typeparam>
    /// <param name="control">Control to stop the rotation.</param>
    public static void StopSpin<T>(this T control)
        where T : FrameworkElement?, ISpinable?
    {
        if (control is null)
        {
            throw new ArgumentNullException(nameof(control));
        }

        var spinnerName = GetSpinnerName(control);
        if (control.Resources[spinnerName] is not Storyboard storyboard)
        {
            return;
        }

        storyboard.Stop();
        control.Resources.Remove(spinnerName);
    }

    /// <summary>
    /// Sets the rotation for the control.
    /// </summary>
    /// <typeparam name="T">FrameworkElement and IRotatable.</typeparam>
    /// <param name="control">Control to apply the rotation.</param>
    public static void SetRotation<T>(this T control)
        where T : FrameworkElement, IRotatable
    {
        if (control is null)
        {
            throw new ArgumentNullException(nameof(control));
        }

        var transformGroup = control.RenderTransform as TransformGroup ?? new TransformGroup();

        var rotateTransform = transformGroup.Children.OfType<RotateTransform>().FirstOrDefault();
        if (rotateTransform is null)
        {
            transformGroup.Children.Add(new RotateTransform(control.Rotation));
            control.SetCurrentValue(UIElement.RenderTransformProperty, transformGroup);
            control.SetCurrentValue(UIElement.RenderTransformOriginProperty, new Point(0.5, 0.5));
        }
        else
        {
            rotateTransform.Angle = control.Rotation;
        }
    }

    /// <summary>
    /// Sets the flip orientation for the control.
    /// </summary>
    /// <typeparam name="T">FrameworkElement and IRotatable.</typeparam>
    /// <param name="control">Control to apply the rotation.</param>
    public static void SetFlipOrientation<T>(this T control)
        where T : FrameworkElement, IFlippable
    {
        if (control is null)
        {
            throw new ArgumentNullException(nameof(control));
        }

        var transformGroup = control.RenderTransform as TransformGroup ?? new TransformGroup();

        var scaleX = control.FlipOrientation is FlipOrientationType.Normal or FlipOrientationType.Vertical ? 1 : -1;
        var scaleY = control.FlipOrientation is FlipOrientationType.Normal or FlipOrientationType.Horizontal ? 1 : -1;

        var scaleTransform = transformGroup.Children.OfType<ScaleTransform>().FirstOrDefault();
        if (scaleTransform is null)
        {
            transformGroup.Children.Add(new ScaleTransform(scaleX, scaleY));
            control.SetCurrentValue(UIElement.RenderTransformProperty, transformGroup);
            control.SetCurrentValue(UIElement.RenderTransformOriginProperty, new Point(0.5, 0.5));
        }
        else
        {
            scaleTransform.ScaleX = scaleX;
            scaleTransform.ScaleY = scaleY;
        }
    }

    private static string GetSpinnerName(IFrameworkInputElement control)
    {
        if (string.IsNullOrEmpty(control.Name))
        {
            throw new Exception("Control don't have a x:Name");
        }

        return control.Name + "Spinner";
    }
}