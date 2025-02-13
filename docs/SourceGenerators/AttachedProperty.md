# AttachedProperty with SourceGeneration

---

## 🔎 Behind the scenes

### 📝 Human made code - for simple example

```csharp
[AttachedProperty<bool>("IsDraggable"]
public partial class DragBehavior
{
}
```

### ⚙️ Auto-Generated code - for simple example

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

### 📝 Human made code - for complex example

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

### ⚙️ Auto-Generated code - for complex example

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
