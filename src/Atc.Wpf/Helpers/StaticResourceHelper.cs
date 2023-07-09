namespace Atc.Wpf.Helpers;

[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class StaticResourceHelper
{
    public static readonly DependencyProperty PropertyProperty =
        DependencyProperty.RegisterAttached(
            "Property",
            typeof(DependencyProperty),
            typeof(StaticResourceHelper),
            new PropertyMetadata(OnPropertyChanged));

    public static readonly DependencyProperty ResourceKeyProperty =
        DependencyProperty.RegisterAttached(
            "ResourceKey",
            typeof(object),
            typeof(StaticResourceHelper),
            new PropertyMetadata(OnResourceKeyChanged));

    public static DependencyProperty GetProperty(
        FrameworkElement element)
        => (DependencyProperty)element.GetValue(PropertyProperty);

    public static void SetProperty(
        FrameworkElement element,
        DependencyProperty value)
        => element.SetValue(PropertyProperty, value);

    public static object GetResourceKey(
        FrameworkElement element)
        => element.GetValue(ResourceKeyProperty);

    public static void SetResourceKey(
        FrameworkElement element,
        object value)
        => element.SetValue(ResourceKeyProperty, value);

    private static void OnPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var element = (FrameworkElement)d;
        var newValue = (DependencyProperty)e.NewValue;
        Apply(element, newValue, GetResourceKey(element));
    }

    private static void OnResourceKeyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        var element = (FrameworkElement)d;
        var newValue = e.NewValue;
        Apply(element, GetProperty(element), newValue);
    }

    private static void Apply(
        FrameworkElement? element,
        DependencyProperty? property,
        object? key)
    {
        if (element is not null && property is not null && key is not null)
        {
            element.SetValue(property, element.TryFindResource(key));
        }
    }
}