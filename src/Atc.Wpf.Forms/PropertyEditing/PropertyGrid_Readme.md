# 🗂️ PropertyGrid

A control for displaying and editing the public properties of any object, grouped by category.

## 🔍 Overview

`PropertyGrid` reflects over the public instance properties of `SelectedObject` and builds an editor row for each one. Properties are grouped into collapsible categories, labeled with a friendly display name, and edited with a type-specific editor (text box, numeric box, check box, enum drop-down, date picker, color picker, and so on).

Standard `System.ComponentModel` attributes control what is shown and how: `[Category]`, `[DisplayName]`, `[Description]`, `[ReadOnly]` and `[Browsable(false)]`. Nested objects are expanded up to `MaxNestedDepth` levels. If the selected object implements `INotifyPropertyChanged`, rows refresh automatically when properties change.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms.PropertyEditing;
```

```xml
xmlns:propertyEditing="clr-namespace:Atc.Wpf.Forms.PropertyEditing;assembly=Atc.Wpf.Forms"
```

## 🚀 Usage

### Basic Example

```xml
<propertyEditing:PropertyGrid SelectedObject="{Binding Settings}" />
```

### Decorate the Model

```csharp
public sealed class ServerSettings : ObservableObject
{
    [Category("Connection")]
    [DisplayName("Host name")]
    [Description("The server host name or IP address.")]
    public string Host { get; set; } = "localhost";

    [Category("Connection")]
    public int Port { get; set; } = 443;

    [Category("Info")]
    [ReadOnly(true)]
    public string Version { get; set; } = "1.0";

    [Browsable(false)]
    public string InternalKey { get; set; } = string.Empty;
}
```

### Read-Only, Flat and Unsorted

```xml
<propertyEditing:PropertyGrid
    IsReadOnly="True"
    ShowCategories="False"
    ShowDescriptions="False"
    SortMode="NoSort"
    SelectedObject="{Binding Settings}" />
```

### Custom Editors

```csharp
// Register an editor for all properties of a given type
propertyGrid.RegisterEditor(new MyPointPropertyEditor());

// Or pin an editor to a single property
[PropertyGridEditor(typeof(MyPointPropertyEditor))]
public Point Location { get; set; }
```

### Refresh Without INotifyPropertyChanged

```csharp
propertyGrid.Refresh();
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `SelectedObject` | `object?` | `null` | The object whose properties are shown. Changing it rebuilds the grid |
| `ShowCategories` | `bool` | `true` | Group properties into collapsible category expanders |
| `ShowDescriptions` | `bool` | `true` | Show the description panel for the focused property |
| `IsReadOnly` | `bool` | `false` | Disable all editors |
| `SortMode` | `PropertySortMode` | `Categorized` | How properties and categories are ordered (see below) |
| `DefaultCategoryName` | `string` | `Misc` | Category name used for all properties when `ShowCategories` is `false` |
| `MaxNestedDepth` | `int` | `3` | Maximum depth for expanding nested objects |
| `Categories` | `ObservableCollection<PropertyGridCategory>` | empty | Read-only (CLR). The categories built for the current object |

### PropertySortMode

| Value | Description |
|-------|-------------|
| `Categorized` | Categories sorted by name, properties sorted by display name |
| `Alphabetical` | Properties sorted by display name, categories kept in first-seen order |
| `NoSort` | Properties and categories in declaration order |

## 🛠️ Methods

| Method | Description |
|--------|-------------|
| `Refresh()` | Re-reads all property values from `SelectedObject`. Use it when the object does not implement `INotifyPropertyChanged` |
| `RegisterEditor(IPropertyGridEditor editor)` | Adds a custom editor. The editor with the highest `Priority` whose `CanEdit` returns `true` is used |

## 🧩 Built-in Editors

| Editor | Property Types |
|--------|----------------|
| `BooleanPropertyEditor` | `bool` |
| `StringPropertyEditor` | `string` |
| `IntegerPropertyEditor` | `int`, `uint`, `long`, `ulong`, `short`, `ushort`, `byte`, `sbyte` |
| `DecimalPropertyEditor` | `decimal`, `double`, `float` |
| `EnumPropertyEditor` | Enums |
| `DateTimePropertyEditor` | `DateTime`, `DateOnly`, `DateTimeOffset` |
| `TimeOnlyPropertyEditor` | `TimeOnly` |
| `ColorPropertyEditor` | `Color` |
| `BrushPropertyEditor` | `SolidColorBrush` |
| `ThicknessPropertyEditor` | `Thickness` |
| `FileInfoPropertyEditor` | `FileInfo` |
| `DirectoryInfoPropertyEditor` | `DirectoryInfo` |
| `NestedObjectPropertyEditor` | Nested objects |

## 🏷️ Supported Attributes

| Attribute | Effect |
|-----------|--------|
| `[Category("...")]` | Category the property is grouped under (default `Misc`) |
| `[DisplayName("...")]` | Label text (default: the property name split from Pascal case) |
| `[Description("...")]` | Text shown in the description panel and as tooltip |
| `[ReadOnly(true)]` | Show the property as read-only |
| `[Browsable(false)]` | Hide the property |
| `[IgnoreDisplay]` (from Atc) | Hide the property |
| `[PropertyGridEditor(typeof(...))]` | Use a specific `IPropertyGridEditor` for the property |

## 📝 Notes

- Properties without a setter are treated as read-only
- Properties with no matching editor are shown as italic, grayed-out text (`ToString()` or `(null)`)
- Nested objects are not expanded for primitives, enums, strings, `decimal`, date/time types, `Guid`, `Color`, `Thickness`, brushes, file system types, arrays or collections
- `IsReadOnly` disables editors that derive from `Control`
- Changing `ShowCategories`, `IsReadOnly` or `SortMode` rebuilds the whole grid

## 🔗 Related Controls

- **LabelInputFormPanel** - Builds a labeled input form from a set of fields
- **LabelTextBox**, **LabelIntegerBox**, **LabelComboBox** - Individual labeled form fields

## 🎮 Sample Application

See the PropertyGrid sample in the Atc.Wpf.Sample application under **Wpf.Forms > PropertyEditing > PropertyGrid** for interactive examples.
