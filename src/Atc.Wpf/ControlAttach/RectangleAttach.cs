namespace Atc.Wpf.ControlAttach;

public static class RectangleAttach
{
    public static readonly DependencyProperty CircularProperty = DependencyProperty.RegisterAttached(
        "Circular",
        typeof(bool),
        typeof(RectangleAttach),
        new PropertyMetadata(false, OnCircularChanged));

    public static bool GetCircular(DependencyObject obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return (bool)obj.GetValue(CircularProperty);
    }

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