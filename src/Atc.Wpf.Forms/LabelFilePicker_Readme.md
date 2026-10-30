# 📄 LabelFilePicker

A labeled file picker form field with file filter, mandatory indicator, existence check and validation display.

## 🔍 Overview

`LabelFilePicker` puts a `FilePicker` (from `Atc.Wpf.Controls.Pickers`) in the standard `LabelContent` layout used by the other `Label*` controls. The user can type a path or browse with the file dialog. The selected file is exposed as a `FileInfo`. The control validates when the value changes: a mandatory field must have a value, and with `AllowOnlyExisting` the file must exist. It implements `ILabelFilePicker` and can be created with `LabelControlFactory.CreateLabelFilePicker(...)`.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Basic Example

```xml
<atc:LabelFilePicker LabelText="Input file" Value="{Binding InputFile, Mode=TwoWay}" />
```

### Filter, Preview Pane and Existing Files Only

```xml
<atc:LabelFilePicker
    AllowOnlyExisting="True"
    Filter="Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*"
    InitialDirectory="C:\Temp"
    IsMandatory="True"
    LabelText="Image"
    ShowClearTextButton="True"
    UsePreviewPane="True"
    WatermarkText="Select a file" />
```

### Factory

```csharp
var control = LabelControlFactory.CreateLabelFilePicker(
    groupIdentifier: "Import",
    labelText: "InputFile",
    isReadOnly: false,
    isMandatory: true,
    watermarkText: "Select a file",
    inputDataType: typeof(FileInfo),
    value: null);
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Value` | `FileInfo?` | `null` | The selected file |
| `AllowOnlyExisting` | `bool` | `false` | Show a validation error when the selected file does not exist |
| `ShowClearTextButton` | `bool` | `false` | Show a clear button inside the text box |
| `Filter` | `string` | `""` | File type filter for the file dialog, e.g. `Text files (*.txt)\|*.txt` |
| `UsePreviewPane` | `bool` | `false` | Show a preview pane in the file dialog |
| `DefaultDirectory` | `string` | `""` | Default directory for the file dialog |
| `InitialDirectory` | `string` | `""` | Initial directory for the file dialog |
| `RootDirectory` | `string` | `""` | Root directory for the file dialog |
| `WatermarkText` | `string` | `""` | Watermark shown when the text box is empty |
| `WatermarkAlignment` | `TextAlignment` | `Left` | Alignment of the watermark |
| `WatermarkTrimming` | `TextTrimming` | `None` | Trimming of the watermark |

### 🏷️ Inherited Label Properties

Inherits the common label properties from `LabelControlBase` (`LabelText`, `LabelPosition`, `Orientation`, `LabelWidthNumber`, `LabelWidthSizeDefinition`, `HideAreas`, `InformationText`, `InformationContent`, `InformationColor`, `ContentMinHeight`, `GroupIdentifier`, `InputDataType`) and `LabelControl` (`IsMandatory`, `ShowAsteriskOnMandatory`, `MandatoryColor`, `ValidationText`, `ValidationColor`).

## ⚡ Events

| Event | Type | Description |
|-------|------|-------------|
| `LostFocusValid` | `EventHandler<ValueChangedEventArgs<FileInfo?>>` | Raised when the selected file changes and passes validation |
| `LostFocusInvalid` | `EventHandler<ValueChangedEventArgs<FileInfo?>>` | Raised when the selected file changes and fails validation |

## 📝 Notes

- `Value` does not bind two-way by default; use `Mode=TwoWay` to write the selection back to the source
- Despite their names, `LostFocusValid` and `LostFocusInvalid` are raised when the inner picker's value changes
- `AllowOnlyExisting` is checked by the labeled control; the inner `FilePicker` always accepts any path
- Validation texts are localized: "Field is required" when mandatory and empty, and "File don't exist" when `AllowOnlyExisting` is set and the file is missing
- `IsValid()` validates now and sets `ValidationText` without raising the events

## 🔗 Related Controls

- **FilePicker** - The file picker without the label
- **LabelDirectoryPicker** - Labeled directory picker
- **LabelTextBox** - Labeled free-text input

## 🎮 Sample Application

See the LabelFilePicker sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > Pickers > LabelFilePicker** for interactive examples.
