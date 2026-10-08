# 📄 FilePicker

A text box with a browse button for selecting a single file.

## 🔍 Overview

`FilePicker` combines an editable `TextBox` that shows the file path with a browse button that opens the standard Windows `OpenFileDialog`. The selected file is exposed as a `FileInfo` through `Value`, and the path text through `DisplayValue`; the two are kept in sync in both directions. Dialog behaviour (title, filter, start folder, existence checks, preview pane) is configurable through properties.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Pickers;
```

## 🚀 Usage

### Basic Example

```xml
<atc:FilePicker Value="{Binding SelectedFile}" />
```

### Title and Watermark

```xml
<atc:FilePicker
    Title="- Select File - "
    WatermarkText="Select a file"
    ShowClearTextButton="True" />
```

### Filter and Start Directory

```xml
<atc:FilePicker
    Value="{Binding ConfigFile}"
    Filter="JSON files (*.json)|*.json|All files (*.*)|*.*"
    InitialDirectory="C:\Config"
    AllowOnlyExisting="True" />
```

### Handling Changes

```xml
<atc:FilePicker ValueChanged="OnFileChanged" />
```

```csharp
private void OnFileChanged(object sender, RoutedPropertyChangedEventArgs<FileInfo?> e)
{
    var newFile = e.NewValue;
}
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Value` | `FileInfo?` | `null` | The selected file. Two-way by default |
| `DisplayValue` | `string?` | `null` | The path text shown in the text box. Two-way by default; typing a path updates `Value` |
| `Title` | `string` | `null` | Dialog title. When empty, a localized "Select file" text is used |
| `Filter` | `string` | `""` | File type filter passed to the dialog (e.g. `"Text files (*.txt)\|*.txt"`) |
| `AllowOnlyExisting` | `bool` | `false` | Sets the dialog's `CheckFileExists` and `CheckPathExists` |
| `UsePreviewPane` | `bool` | `false` | Sets the dialog's `ForcePreviewPane` |
| `DefaultDirectory` | `string` | `""` | Dialog `DefaultDirectory` |
| `InitialDirectory` | `string` | `""` | Dialog `InitialDirectory`. When empty, the directory of the current `Value` is used if it exists |
| `RootDirectory` | `string` | `""` | Dialog `RootDirectory` |
| `ShowClearTextButton` | `bool` | `false` | Shows a clear button inside the text box |
| `WatermarkText` | `string` | `""` | Watermark shown when the text box is empty |
| `WatermarkAlignment` | `TextAlignment` | `Left` | Alignment of the watermark |
| `WatermarkTrimming` | `TextTrimming` | `None` | Trimming of the watermark |

## 📡 Events

| Event | Type | Description |
|-------|------|-------------|
| `ValueChanged` | Routed (bubble), `RoutedPropertyChangedEventHandler<FileInfo?>` | Raised when `Value` changes |

## 📝 Notes

- The dialog is opened with `Multiselect = false`; only one file can be selected
- Clearing the text sets `Value` to `null`
- The picker keeps the `DataContext` it inherits, so a plain `{Binding X}` set on it resolves against your view model; its internal layout binds to the picker's own properties
- Includes a dedicated automation peer (`FilePickerAutomationPeer`) for UI automation

## 🔗 Related Controls

- **DirectoryPicker** - Select a folder instead of a file
- **LabelFilePicker** - Labeled form-control wrapper (`Atc.Wpf.Forms`)

## 🎮 Sample Application

See the FilePicker sample in the Atc.Wpf.Sample application under **Wpf.Controls > Pickers > FilePicker** for interactive examples.
