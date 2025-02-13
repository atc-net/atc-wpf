### AttachedProperty

### Human made Code for simple property

```csharp
[AttachedProperty<bool>("Circular"]
public partial class MyAttach
{
}
```

### Generated Code for simple property

```csharp
public static partial class MyAttach
{
    public static readonly DependencyProperty CircularProperty = DependencyProperty.RegisterAttached(
        "Circular",
        typeof(bool),
        typeof(MyAttach),
        new PropertyMetadata(
            defaultValue: BooleanBoxes.FalseBox,
            propertyChangedCallback: OnCircularChanged));

    public static bool GetCircular(UIElement element)
        => element is not null && (bool)element.GetValue(CircularProperty);

    public static void SetCircular(UIElement element, bool value)
        => element?.SetValue(CircularProperty, value);
} }
}
```