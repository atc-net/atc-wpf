# ⚙️ AttachedProperty with SourceGeneration

In WPF, **attached properties** are a type of dependency property that allows properties to be defined in one class but used in another. They are widely used in scenarios like behaviors, layout configurations, and interactions where a property needs to be applied to multiple elements without modifying their class definitions. Traditionally, defining attached properties requires boilerplate code, but source generators can automate this process, reducing errors and improving maintainability.

---

## 🚀 Defining an Attached Property

### ✨ Creating a Simple Attached Property

Let's define an attached property using source generators.

```csharp
[AttachedProperty<bool>("IsDraggable")]
public static partial class DragBehavior
{
}
```

### 🔍 What's Happening Here?

- The `AttachedPropertyAttribute` automatically generates the `IsDraggable` property with `Get` and `Set` methods.

### 🖥️ XAML Example

```xml
<UserControl xmlns:local="clr-namespace:MyApp.MyUserControl"
    x:Name="UcMyUserControl">
    <Grid local:DragBehavior.IsDraggable="True">
        <TextBlock Text="Drag Me!" />
    </Grid>
</UserControl>
```

This allows the `IsDraggable` property to be applied to any UI element dynamically.

---

## 📌 Summary

This example showcases `advanced metadata` for attached properties, allowing:

- ✔️ **Automatic property registration**
- ✔️ **Flexible application to various UI elements**
- ✔️ **Custom property value coercion and validation**
- ✔️ **Efficient UI updates**
- ✔️ **Simplified code structure**

### 🚀 Why Use Atc.Wpf Source Generators?

- ✅ **Eliminates boilerplate** – Just declare the property, and the generator handles the rest.
- ✅ **Ensures consistency** – Less room for human error.

---

## 🔎 Behind the scenes

### 📝 Human-Written Code - for simple example

```csharp
[AttachedProperty<bool>("IsDraggable"]
public static partial class DragBehavior
{
}
```

In this example:

- The `[AttachedProperty<bool>("IsDraggable")]` attribute declares an attached property named `IsDraggable` of type `bool` for the `DragBehavior` class.
- The source generator will automatically create the necessary methods and property registration.

### ⚙️ Auto-Generated Code - for simple example

The source generator will produce code equivalent to:

```csharp
public static partial class DragBehavior
{
    public static readonly DependencyProperty IsDraggableProperty = DependencyProperty.RegisterAttached(
        "IsDraggable",
        typeof(bool),
        typeof(DragBehavior),
        new PropertyMetadata(defaultValue: BooleanBoxes.FalseBox));

    public static bool GetIsDraggable(UIElement element)
        => element is not null && (bool)element.GetValue(IsDraggableProperty);

    public static void SetIsDraggable(UIElement element, bool value)
        => element?.SetValue(IsDraggableProperty, value);
}
```

### 📝 Human-Written Code - for complex example

```csharp
[AttachedProperty<bool>(
    "IsDraggable",
    DefaultValue = false,
    PropertyChangedCallback = nameof(PropertyChangedCallback),
    CoerceValueCallback = nameof(CoerceValueCallback),
    Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
    DefaultUpdateSourceTrigger = UpdateSourceTrigger.Default,
    IsAnimationProhibited = true)]
public partial class DragBehavior
{
}
```

**In this example:**

- **`[AttachedProperty<bool>("IsDraggable")]`**
  - Declares a `dependency property` named `IsDraggable` of type `bool` in `DragBehavior`.
  - The source generator will handle property registration and accessors.
- **`DefaultValue = false`**
  - Sets the default value of IsDraggable to false.
- **`PropertyChangedCallback = nameof(PropertyChangedCallback)`**
  - Calls a method whenever the property value changes.
- **`CoerceValueCallback = nameof(CoerceValueCallback)`**
  - Restricts or modifies values before setting the property.
- **`Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender`**
  - Ensures layout recalculation when IsDraggable changes.
- **`DefaultUpdateSourceTrigger = UpdateSourceTrigger.Default`**
  - Defines how bindings update their source.
- **`IsAnimationProhibited = true`**
  - Prevents animations from modifying this property.

### ⚙️ Auto-Generated Code - for complex example

The source generator will produce equivalent code:

```csharp
public partial class DragBehavior
{
    public static readonly DependencyProperty IsDraggableProperty = DependencyProperty.Register(
        nameof(IsDraggable),
        typeof(bool),
        typeof(DragBehavior),
        new FrameworkPropertyMetadata(
            defaultValue: BooleanBoxes.FalseBox,
            propertyChangedCallback: PropertyChangedCallback,
            coerceValueCallback: CoerceValueCallback,
            flags: FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
            defaultUpdateSourceTrigger: UpdateSourceTrigger.Default,
            isAnimationProhibited: true));

    public bool IsDraggable
    {
        get => (bool)GetValue(IsDraggableProperty);
        set => SetValue(IsDraggableProperty, value);
    }
}
```
