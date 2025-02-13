# DependencyProperties with SourceGeneration

---

## 🔎 Behind the scenes

### 📝 Human made code - for simple example

```csharp
[DependencyProperty<bool>("IsRunning"]
public partial class MyControl : UserControl
{
}
```

### ⚙️ Auto-Generated code - for simple example

```csharp
public partial class MyControl
{
    public static readonly DependencyProperty IsRunningProperty = DependencyProperty.Register(
        nameof(IsRunning),
        typeof(bool),
        typeof(MyControl),
        new FrameworkPropertyMetadata(defaultValue: BooleanBoxes.TrueBox);

    public bool IsRunning
    {
        get => (bool)GetValue(IsRunningProperty);
        set => SetValue(IsRunningProperty, value);
    }
}
```

### 📝 Human made code - for complex example

```csharp

[DependencyProperty<bool>(
    "IsRunning",
    DefaultValue = false,
    PropertyChangedCallback = nameof(PropertyChangedCallback),
    CoerceValueCallback = nameof(CoerceValueCallback),
    Flags = FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
    DefaultUpdateSourceTrigger = UpdateSourceTrigger.Default,
    IsAnimationProhibited = true)]
public partial class MyControl : UserControl
{
    private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
    }

    private static void CoerceValueCallback(DependencyObject d, object baseValue)
    {
    }
}
```

### ⚙️ Auto-Generated code - for complex example

```csharp
public partial class MyControl
{
    public static readonly DependencyProperty IsRunningProperty = DependencyProperty.Register(
        nameof(IsRunning),
        typeof(bool),
        typeof(MyControl),
        new FrameworkPropertyMetadata(
            defaultValue: BooleanBoxes.FalseBox,
            propertyChangedCallback: PropertyChangedCallback,
            coerceValueCallback: CoerceValueCallback,
            flags: FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
            defaultUpdateSourceTrigger: UpdateSourceTrigger.Default,
            isAnimationProhibited: true));

    public bool IsRunning
    {
        get => (bool)GetValue(IsRunningProperty);
        set => SetValue(IsRunningProperty, value);
    }
}
```
