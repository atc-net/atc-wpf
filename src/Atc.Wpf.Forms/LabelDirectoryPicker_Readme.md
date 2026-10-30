# 📁 LabelDirectoryPicker

A labeled directory picker form field with mandatory indicator, existence check and validation display.

## 🔍 Overview

`LabelDirectoryPicker` puts a `DirectoryPicker` (from `Atc.Wpf.Controls.Pickers`) in the standard `LabelContent` layout used by the other `Label*` controls. The user can type a path or browse with the folder dialog. The selected folder is exposed as a `DirectoryInfo`. The control validates when the value changes: a mandatory field must have a value, and with `AllowOnlyExisting` the folder must exist. It implements `ILabelDirectoryPicker` and can be created with `LabelControlFactory.CreateLabelDirectoryPicker(...)`.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Basic Example

```xml
<atc:LabelDirectoryPicker LabelText="Output folder" Value="{Binding OutputFolder, Mode=TwoWay}" />
```

### Existing Folders Only

```xml
<atc:LabelDirectoryPicker
    AllowOnlyExisting="True"
    InitialDirectory="C:\Temp"
    IsMandatory="True"
    LabelText="Folder"
    ShowClearTextButton="True"
    WatermarkText="Select a directory" />
```

### Factory

```csharp
var control = LabelControlFactory.CreateLabelDirectoryPicker(
    groupIdentifier: "Export",
    labelText: "OutputFolder",
    isReadOnly: false,
    isMandatory: true,
    watermarkText: "Select a directory",
    inputDataType: typeof(DirectoryInfo),
    value: null);
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Value` | `DirectoryInfo?` | `null` | The selected directory |
| `AllowOnlyExisting` | `bool` | `false` | Show a validation error when the selected directory does not exist |
| `ShowClearTextButton` | `bool` | `false` | Show a clear button inside the text box |
| `DefaultDirectory` | `string` | `""` | Default directory for the folder dialog |
| `InitialDirectory` | `string` | `""` | Initial directory for the folder dialog |
| `RootDirectory` | `string` | `""` | Root directory for the folder dialog |
| `WatermarkText` | `string` | `""` | Watermark shown when the text box is empty |
| `WatermarkAlignment` | `TextAlignment` | `Left` | Alignment of the watermark |
| `WatermarkTrimming` | `TextTrimming` | `None` | Trimming of the watermark |

### 🏷️ Inherited Label Properties

Inherits the common label properties from `LabelControlBase` (`LabelText`, `LabelPosition`, `Orientation`, `LabelWidthNumber`, `LabelWidthSizeDefinition`, `HideAreas`, `InformationText`, `InformationContent`, `InformationColor`, `ContentMinHeight`, `GroupIdentifier`, `InputDataType`) and `LabelControl` (`IsMandatory`, `ShowAsteriskOnMandatory`, `MandatoryColor`, `ValidationText`, `ValidationColor`).

## ⚡ Events

| Event | Type | Description |
|-------|------|-------------|
| `LostFocusValid` | `EventHandler<ValueChangedEventArgs<DirectoryInfo?>>` | Raised when the selected directory changes and passes validation |
| `LostFocusInvalid` | `EventHandler<ValueChangedEventArgs<DirectoryInfo?>>` | Raised when the selected directory changes and fails validation |

## 📝 Notes

- `Value` does not bind two-way by default; use `Mode=TwoWay` to write the selection back to the source
- Despite their names, `LostFocusValid` and `LostFocusInvalid` are raised when the inner picker's value changes
- Validation texts are localized: "Field is required" when mandatory and empty, and "Directory don't exist" when `AllowOnlyExisting` is set and the folder is missing
- `IsValid()` validates now and sets `ValidationText` without raising the events

## 🔗 Related Controls

- **DirectoryPicker** - The directory picker without the label
- **LabelFilePicker** - Labeled file picker
- **LabelTextBox** - Labeled free-text input

## 🎮 Sample Application

See the LabelDirectoryPicker sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > Pickers > LabelDirectoryPicker** for interactive examples.
