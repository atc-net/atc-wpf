namespace Atc.Wpf.Helpers;

/// <summary>Provides attached properties that set a dependency property on an element to a resource looked up by key.</summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static partial class StaticResourceHelper
{
    /// <summary>Gets or sets the value of the Property attached property.</summary>
    [AttachedProperty(PropertyChangedCallback = nameof(OnPropertyChanged), TargetType = typeof(FrameworkElement), NullElementHandling = NullElementHandling.None)]
    private static DependencyProperty property;

    /// <summary>Gets or sets the value of the ResourceKey attached property.</summary>
    [AttachedProperty(PropertyChangedCallback = nameof(OnResourceKeyChanged), TargetType = typeof(FrameworkElement), NullElementHandling = NullElementHandling.None)]
    private static object resourceKey;

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