# 📋 LabelInputFormPanel

A panel that renders a complete input form from a set of labeled controls arranged in rows and columns.

## 🔍 Overview

`LabelInputFormPanel` takes an `ILabelControlsForm` (usually a `LabelControlsForm`) describing rows and columns of `Label*` controls, and renders it as a single form. `LabelInputFormPanelSettings` controls the layout: control orientation, control width and whether each column is wrapped in a `GroupBox`.

The form can be built by hand from `LabelTextBox`, `LabelIntegerBox` and the other labeled controls, or generated from a model object with `ModelToLabelControlExtractor`. The same panel is used inside `InputFormDialogBox`.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Build a Form from Label Controls

```csharp
var labelControls = new List<ILabelControlBase>
{
    new LabelTextBox { LabelText = "FirstName", IsMandatory = true, MinLength = 2 },
    new LabelTextBox { LabelText = "LastName", IsMandatory = true, MinLength = 2 },
    new LabelIntegerBox { LabelText = "Age", Minimum = 0 },
    new LabelTextBox { LabelText = "Note" },
};

var labelControlsForm = new LabelControlsForm();
labelControlsForm.AddColumn(labelControls);

FormPanel = new LabelInputFormPanel(
    new LabelInputFormPanelSettings(),
    labelControlsForm);
```

```xml
<ContentControl Content="{Binding FormPanel}" />
```

### Multiple Columns

```csharp
var labelControlsForm = new LabelControlsForm();
labelControlsForm.AddColumn(column1Controls);
labelControlsForm.AddColumn(column2Controls);

var panel = new LabelInputFormPanel(labelControlsForm);
```

### Generate a Form from a Model

```csharp
var labelControls = ModelToLabelControlExtractor.Extract(address);

var labelControlsForm = new LabelControlsForm();
labelControlsForm.AddColumn(labelControls);

var panel = new LabelInputFormPanel(
    new LabelInputFormPanelSettings { UseGroupBox = true },
    labelControlsForm);
```

### Read the Values Back

```csharp
if (panel.Data.IsValid())
{
    Dictionary<string, object> values = panel.Data.GetKeyValues();
}
```

## ⚙️ Properties

`LabelInputFormPanel` has no dependency properties. Its CLR properties are set through the constructors or `Render`.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Settings` | `LabelInputFormPanelSettings` | `new()` | Layout settings. Private setter; set via constructor or `Render` |
| `Data` | `ILabelControlsForm` | `new LabelControlsForm()` | The form definition (rows, columns, controls). Private setter |
| `ContentControl` | `ContentControl` | `new()` | Holds the generated form panel. Private setter |

### LabelInputFormPanelSettings

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ControlOrientation` | `Orientation` | `Vertical` | Orientation applied to each label control (label above or beside the input) |
| `ControlWidth` | `int` | `320` | Width applied to each label control |
| `UseGroupBox` | `bool` | `false` | Wrap each column in a `GroupBox` |
| `MaxSize` | `Size` | `1920 x 1200` | Maximum size; used by `InputFormDialogBox` to cap the dialog size |

## 🛠️ Methods

| Method | Description |
|--------|-------------|
| `LabelInputFormPanel()` | Creates an empty panel |
| `LabelInputFormPanel(ILabelControlsForm)` | Creates and renders a panel with default settings |
| `LabelInputFormPanel(LabelInputFormPanelSettings, ILabelControlsForm)` | Creates and renders a panel with the given settings |
| `Render(LabelInputFormPanelSettings, ILabelControlsForm)` | Applies the settings to every column and generates the form panel |
| `ReRender()` | Re-applies the current settings (including `ControlOrientation`) to the already generated controls |

## 📝 Notes

- The control sets its own `DataContext` to itself and shows `ContentControl` through a binding
- `ContentControl` does not raise change notifications, so prefer the rendering constructors (or call `Render` before the panel is displayed) and host the panel itself, e.g. via `<ContentControl Content="{Binding FormPanel}" />`
- `ReRender()` walks the generated panels and group boxes and updates the `Orientation` of every label control

## 🔗 Related Controls

- **InputFormDialogBox** - Dialog that hosts a `LabelInputFormPanel` with OK/Cancel buttons
- **LabelContent** - The labeled layout used by every control in the form
- **PropertyGrid** - Reflection-based property editor for an object

## 🎮 Sample Application

See the Label-InputFormPanel sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > Label-InputFormPanel** for interactive examples.
