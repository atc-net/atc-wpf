namespace Atc.Wpf.Helpers;

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

    public static DependencyProperty GetProperty(FrameworkElement element)
    {
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        return (DependencyProperty)element.GetValue(PropertyProperty);
    }

    public static void SetProperty(FrameworkElement element, DependencyProperty value)
    {
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        element.SetValue(PropertyProperty, value);
    }

    public static object GetResourceKey(FrameworkElement element)
    {
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        return element.GetValue(ResourceKeyProperty);
    }

    public static void SetResourceKey(FrameworkElement element, object value)
    {
        if (element is null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        element.SetValue(ResourceKeyProperty, value);
    }

    private static void OnPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        var element = (FrameworkElement)obj;
        var newValue = (DependencyProperty)args.NewValue;
        Apply(element, newValue, GetResourceKey(element));
    }

    private static void OnResourceKeyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        var element = (FrameworkElement)obj;
        var newValue = args.NewValue;
        Apply(element, GetProperty(element), newValue);
    }

    private static void Apply(FrameworkElement? element, DependencyProperty? property, object? key)
    {
        if (element != null && property != null && key is not null)
        {
            element.SetValue(property, element.TryFindResource(key));
        }
    }
}