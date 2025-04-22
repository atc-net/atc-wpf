namespace Atc.Wpf.ControlAttach;

public static partial class RectangleAttach
{
    [AttachedProperty(PropertyChangedCallback = nameof(OnCircularChanged))]
    private static bool circular;

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