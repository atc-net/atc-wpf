# 🔁 Migration: hand-rolled MVVM → source generators

This guide is for projects already using `Atc.Wpf` (or any other MVVM stack) with **hand-rolled** `INotifyPropertyChanged` / `RelayCommand` boilerplate, and are now adopting the **source-generator attributes** that ship with [`Atc.XamlToolkit`](https://github.com/atc-net/atc-xaml-toolkit) (a transitive dependency of `Atc.Wpf`).

The generators are additive: the legacy patterns continue to compile. Migrate at your own pace, file by file.

## TL;DR

| Old (manual) | New (generated) |
|---|---|
| `private string _name; public string Name { get => _name; set { ... RaisePropertyChanged(); } }` | `[ObservableProperty] private string name;` |
| `public IRelayCommand SaveCommand => new RelayCommand(Save, CanSave);` | `[RelayCommand(CanExecute = nameof(CanSave))] public void Save() { ... }` |
| `public static readonly DependencyProperty FooProperty = DependencyProperty.Register(...);` | `[DependencyProperty] private int foo;` |
| `public static readonly DependencyProperty FooProperty = DependencyProperty.RegisterAttached(...);` | `[AttachedProperty] private int foo;` |

All four attributes require the host class to be `partial`.

## 1 · `INotifyPropertyChanged` properties → `[ObservableProperty]`

### Before

```csharp
public sealed class PersonViewModel : ViewModelBase
{
    private string firstName = string.Empty;

    public string FirstName
    {
        get => firstName;
        set
        {
            if (firstName == value)
            {
                return;
            }

            firstName = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(FullName));
        }
    }

    public string FullName => $"{FirstName} {LastName}";
}
```

### After

```csharp
public sealed partial class PersonViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FullName))]
    private string firstName = string.Empty;

    public string FullName => $"{FirstName} {LastName}";
}
```

### Notes

- The class becomes `partial`. The generator emits the public CLR property + `RaisePropertyChanged` plumbing.
- `[NotifyPropertyChangedFor(...)]` triggers a change notification for the named computed property too.
- `[Required]`, `[MinLength]`, `[Range]`, etc. on the field flow through to the generated property — useful with `ObservableValidator`.
- Validation: `[NotifyDataErrorInfo]` (added in newer toolkit versions) wires `INotifyDataErrorInfo` automatically.

## 2 · `RelayCommand` properties → `[RelayCommand]`

### Before

```csharp
public sealed class EditorViewModel : ViewModelBase
{
    private IRelayCommand? saveCommand;

    public IRelayCommand SaveCommand
        => saveCommand ??= new RelayCommand(Save, CanSave);

    private void Save() { /* ... */ }
    private bool CanSave() => !string.IsNullOrWhiteSpace(Name);
}
```

### After

```csharp
public sealed partial class EditorViewModel : ViewModelBase
{
    [RelayCommand(CanExecute = nameof(CanSave))]
    private void Save() { /* ... */ }

    private bool CanSave() => !string.IsNullOrWhiteSpace(Name);
}
```

### Notes

- The generated property is named `SaveCommand` (verb suffix added).
- `async` methods → `IRelayCommandAsync`. Add a `CancellationToken` parameter to expose cancellation.
- `InvertCanExecute = true` flips the `CanSave` result — handy when the predicate is more naturally written as "is busy".
- `[NotifyCanExecuteChangedFor(nameof(SaveCommand))]` on a property triggers `SaveCommand.RaiseCanExecuteChanged()` whenever that property changes.

## 3 · Dependency properties → `[DependencyProperty]`

### Before

```csharp
public sealed class FancyControl : Control
{
    public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register(
        nameof(Caption),
        typeof(string),
        typeof(FancyControl),
        new FrameworkPropertyMetadata(string.Empty, OnCaptionChanged));

    public string Caption
    {
        get => (string)GetValue(CaptionProperty);
        set => SetValue(CaptionProperty, value);
    }

    private static void OnCaptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((FancyControl)d).OnCaptionChanged((string)e.NewValue);
    }

    private void OnCaptionChanged(string newValue) { /* ... */ }
}
```

### After

```csharp
public sealed partial class FancyControl : Control
{
    [DependencyProperty(
        DefaultValue = "",
        PropertyChangedCallback = nameof(OnCaptionChanged))]
    private string caption;

    private static void OnCaptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((FancyControl)d).OnCaptionChanged((string)e.NewValue);
    }

    private void OnCaptionChanged(string newValue) { /* ... */ }
}
```

### Notes

- `[DependencyProperty]` accepts `DefaultValue`, `PropertyChangedCallback`, `CoerceValueCallback`, `ValidateValueCallback`, and `Flags` (for `FrameworkPropertyMetadataOptions` like `BindsTwoWayByDefault`).
- For `RoutedEvent` declarations, use `[RoutedEvent]`.
- **Known limitation:** the `[DependencyProperty]` generator does **not** currently forward XML doc comments from the field to the generated property — see [toolkit issue `dp-generator-xml-doc-forwarding`](https://github.com/atc-net/atc-xaml-toolkit/issues). Until that lands, hand-write the DP + property pair when you need IntelliSense docs on the public surface, or document the control in a co-located `[Control]_Readme.md` file.

## 4 · Attached properties → `[AttachedProperty]`

### Before

```csharp
public static class TextBoxBehavior
{
    public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached(
        "Watermark",
        typeof(string),
        typeof(TextBoxBehavior),
        new PropertyMetadata(string.Empty));

    public static string GetWatermark(DependencyObject obj) => (string)obj.GetValue(WatermarkProperty);
    public static void SetWatermark(DependencyObject obj, string value) => obj.SetValue(WatermarkProperty, value);
}
```

### After

```csharp
public static partial class TextBoxBehavior
{
    [AttachedProperty(DefaultValue = "")]
    private static string watermark;
}
```

### Notes

- The generator emits `WatermarkProperty`, `GetWatermark(DependencyObject)`, and `SetWatermark(DependencyObject, value)`.
- For attached **events** use the generic Atc.XamlToolkit attribute (mirrors the DP-level `[RoutedEvent]`).

## 5 · Migration cookbook

| Pain point in legacy code | Replace with |
|---|---|
| `if (field == value) return; field = value; RaisePropertyChanged();` | `[ObservableProperty]` |
| Hand-coded `RaisePropertyChanged(nameof(Other))` from a setter | `[NotifyPropertyChangedFor(nameof(Other))]` on the source field |
| `RelayCommandAsync<int>` field + lambda factory | `[RelayCommand]` on an `async Task DoWork(int x)` method |
| `_command?.RaiseCanExecuteChanged()` after a property changes | `[NotifyCanExecuteChangedFor(nameof(SomeCommand))]` on the source property |
| Manual `INotifyDataErrorInfo` plumbing | Inherit `ObservableValidator` + `[NotifyDataErrorInfo]` on a validated `[ObservableProperty]` |
| Manual `DependencyProperty.Register` + getter/setter pair | `[DependencyProperty]` on a private field |
| Manual `DependencyProperty.RegisterAttached` + `Get`/`Set` static methods | `[AttachedProperty]` on a private static field |

## 6 · Pre-flight checklist before merging the conversion

1. **Class is marked `partial`.** Compile error otherwise.
2. **Field is `private` and lower-camel-case.** The generator emits a warning otherwise (`Atc.XamlToolkit` 3.x onward).
3. **Inheritance.** ViewModels inherit from `ViewModelBase` / `ObservableObject` / `ObservableValidator`. Controls inherit from a `DependencyObject` descendant.
4. **No leftover hand-coded `field`/property pair** with the same name as the generated one — that's a duplicate-member error.
5. **XAML bindings unchanged.** Property names are identical to the manual version, so existing `Binding`s keep working.

## 7 · Reference

- **Generators reference**: [`docs/SourceGenerators/ViewModel.md`](../SourceGenerators/ViewModel.md), [`DependencyProperty.md`](../SourceGenerators/DependencyProperty.md), [`AttachedProperty.md`](../SourceGenerators/AttachedProperty.md).
- **MVVM overview**: [`docs/Mvvm/@Readme.md`](./@Readme.md).
- **Toolkit repo**: [`atc-net/atc-xaml-toolkit`](https://github.com/atc-net/atc-xaml-toolkit).
