# ⚙️ AttachedProperty with SourceGeneration

In WPF, **attached properties** are a type of dependency property that allows properties to be defined in one class but used in another. They are widely used in scenarios like behaviors, layout configurations, and interactions where a property needs to be applied to multiple elements without modifying their class definitions. Traditionally, defining attached properties requires boilerplate code, but the **[Atc.XamlToolkit.Wpf](https://github.com/atc-net/atc-xaml-toolkit)** source generators (included as a dependency of Atc.Wpf) can automate this process, reducing errors and improving maintainability.

> **Note:** The `[AttachedProperty]` attribute is provided by the `Atc.XamlToolkit.Wpf` NuGet package, which is automatically included when you reference `Atc.Wpf`.

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

### 🚀 Why Use Atc.XamlToolkit.Wpf Source Generators?

- ✅ **Eliminates boilerplate** – Just declare the property, and the generator handles the rest.
- ✅ **Ensures consistency** – Less room for human error in property registration.

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
        => (bool)element.GetValue(IsDraggableProperty);

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
    IsAnimationProhibited = true,
    ValidateValueCallback = nameof(ValidateValueCallback))]
public partial class DragBehavior
{
    private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private static object CoerceValueCallback(DependencyObject d, object baseValue)
    {
        throw new NotImplementedException();
    }

    private static bool ValidateValueCallback(object value)
    {
        throw new NotImplementedException();
    }
}
```

**In this example:**

- `[AttachedProperty<bool>("IsDraggable")]`
  - Declares a **dependency property** named `IsDraggable` of type `bool` for the `DragBehavior` class.
  - Unlike a regular dependency property, an **attached property** is **not tied to a single class** but can be applied to any **UI element**.
  - The source generator will automatically create:
    - A `DependencyProperty` field for `IsDraggableProperty`.
    - **Static** `GetIsDraggable` **and** `SetIsDraggable` **methods**, allowing other controls to use this property dynamically.

- `DefaultValue = false`
  - Specifies the default value of `IsDraggable` as `false`, meaning that **elements are not draggable unless explicitly enabled**.

- `PropertyChangedCallback = nameof(PropertyChangedCallback)`
  - Assigns a **property changed callback method**, that is triggered **whenever the** `IsDraggable` **value changes**.
  - This allows dynamic behavior updates—e.g., adding or removing event handlers for drag operations.

- `CoerceValueCallback = nameof(CoerceValueCallback)`
  - Called before the property value is assigned.
  - This method can **modify the value before applying it**.
  - Example: If an element **must not be draggable** under certain conditions, the `CoerceValueCallback` can force the value back to `false`.

- `Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender`
  - Specifies that changes to `IsDraggable` affect the UI layout and rendering.
  - `AffectsMeasure`: Triggers a re-measure of the UI element if this property changes.
  - `AffectsRender`: Causes a re-render of the control when this property is modified.
  - Example: If `IsDraggable` changes, **drag indicators or visual cues might need updating**.

- `FrameworkPropertyMetadataOptions`
  - A flag-based enumeration used to specify additional **property behavior** in WPF.
  - Other possible values include:
    - `AffectsParentMeasure`: Causes a layout pass on the parent when the property changes.
    - `AffectsArrange`: Forces an arrange pass when the property changes.
    - `Inherits`: Allows the property value to propagate down the visual tree.
    - `BindsTwoWayByDefault`: Sets the default binding mode to **TwoWay**.
  - These flags **optimize performance** by ensuring layout changes only occur when necessary.

- `DefaultUpdateSourceTrigger = UpdateSourceTrigger.Default`
  - Specifies how **data binding** updates the property's source.
  - The `Default` value means that the **default behavior of the property type** is used.
  - Other possible values:
    - `PropertyChanged`: Updates the source immediately when the property changes.
    - `LostFocus`: Updates the source when the control loses focus (e.g., leaving a text box).
    - `Explicit`: Requires manual invocation of `BindingExpression.UpdateSource()`.

- `IsAnimationProhibited = true`
  - Prevents animations from affecting this property.
  - Some dependency properties allow animations to change their values smoothly over time.
  - By setting `IsAnimationProhibited = true`, you ensure that **no animations** can modify `IsDraggable`.
  - This is useful for properties where **instant updates are required**, such as boolean state changes.

- `ValidateValueCallback = nameof(ValidateValueCallback)`
  - Assigns a **validation callback method**, which ensures that only valid values are assigned to the dependency property.
  - This function **executes before** the property value is set, allowing you to **reject invalid values** before they are applied.
  - The `ValidateValueCallback` method should return a `bool`:
    - `true`: The value is accepted and applied to the property.
    - `false`: The value is considered invalid, and an exception is thrown.

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
            isAnimationProhibited: true,
            validateValueCallback = ValidateValueCallback));

    public bool IsDraggable
    {
        get => (bool)GetValue(IsDraggableProperty);
        set => SetValue(IsDraggableProperty, value);
    }
}
```
