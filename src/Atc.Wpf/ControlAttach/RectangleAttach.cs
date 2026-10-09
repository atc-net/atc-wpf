namespace Atc.Wpf.ControlAttach;

/// <summary>
/// Provides attached properties for <see cref="System.Windows.Shapes.Rectangle"/>.
/// </summary>
public static class RectangleAttach
{
    /// <summary>
    /// Identifies the Circular attached property, which binds the rectangle's corner radii so it renders as a circle or pill shape.
    /// </summary>
    public static readonly DependencyProperty CircularProperty = DependencyProperty.RegisterAttached(
        "Circular",
        typeof(bool),
        typeof(RectangleAttach),
        new PropertyMetadata(false, OnCircularChanged));

    /// <summary>
    /// Gets the value of the Circular attached property.
    /// </summary>
    public static bool GetCircular(DependencyObject obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return (bool)obj.GetValue(CircularProperty);
    }

    /// <summary>
    /// Sets the value of the Circular attached property.
    /// </summary>
    public static void SetCircular(
        DependencyObject obj,
        bool value)
    {
        ArgumentNullException.ThrowIfNull(obj);
        obj.SetValue(CircularProperty, value);
    }

    private static void OnCircularChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not System.Windows.Shapes.Rectangle rectangle)
        {
            return;
        }

        if ((bool)e.NewValue)
        {
            var binding = new MultiBinding
            {
                Converter = new RectangleCircularValueConverter(),
            };

            binding.Bindings.Add(new Binding(FrameworkElement.ActualWidthProperty.Name) { Source = rectangle });
            binding.Bindings.Add(new Binding(FrameworkElement.ActualHeightProperty.Name) { Source = rectangle });
            rectangle.SetBinding(System.Windows.Shapes.Rectangle.RadiusXProperty, binding);
            rectangle.SetBinding(System.Windows.Shapes.Rectangle.RadiusYProperty, binding);
        }
        else
        {
            BindingOperations.ClearBinding(rectangle, FrameworkElement.ActualWidthProperty);
            BindingOperations.ClearBinding(rectangle, FrameworkElement.ActualHeightProperty);
            BindingOperations.ClearBinding(rectangle, System.Windows.Shapes.Rectangle.RadiusXProperty);
            BindingOperations.ClearBinding(rectangle, System.Windows.Shapes.Rectangle.RadiusYProperty);
        }
    }
}