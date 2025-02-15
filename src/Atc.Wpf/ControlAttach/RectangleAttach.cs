namespace Atc.Wpf.ControlAttach;

[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class RectangleAttach
{
    public static readonly DependencyProperty CircularProperty = DependencyProperty.RegisterAttached(
        "Circular",
        typeof(bool),
        typeof(RectangleAttach),
        new PropertyMetadata(
            defaultValue: BooleanBoxes.FalseBox,
            propertyChangedCallback: OnCircularChanged));

    public static void SetCircular(
        DependencyObject element,
        bool value)
        => element.SetValue(CircularProperty, BooleanBoxes.Box(value));

    public static bool GetCircular(
        DependencyObject element)
        => (bool)element.GetValue(CircularProperty);

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