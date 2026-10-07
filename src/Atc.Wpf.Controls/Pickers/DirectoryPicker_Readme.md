# 📁 DirectoryPicker

A text box with a browse button for selecting a single folder.

## 🔍 Overview

`DirectoryPicker` combines an editable `TextBox` that shows the folder path with a browse button that opens the standard Windows `OpenFolderDialog`. The selected folder is exposed as a `DirectoryInfo` through `Value`, and the path text through `DisplayValue`; the two are kept in sync in both directions. The dialog title and start folders are configurable through properties.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Pickers;
```

## 🚀 Usage

### Basic Example

```xml
<atc:DirectoryPicker Value="{Binding DataContext.OutputFolder, RelativeSource={RelativeSource AncestorType=UserControl}}" />
```

### Title and Watermark

```xml
<atc:DirectoryPicker
    Title="- Select Directory - "
    WatermarkText="Select a directory"
    ShowClearTextButton="True" />
```

### Start Directory

```xml
<atc:DirectoryPicker
    InitialDirectory="C:\Projects"
    RootDirectory="C:\" />
```

### Handling Changes

```xml
<atc:DirectoryPicker ValueChanged="OnDirectoryChanged" />
```

```csharp
private void OnDirectoryChanged(object sender, RoutedPropertyChangedEventArgs<DirectoryInfo?> e)
{
    var newDirectory = e.NewValue;
}
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Value` | `DirectoryInfo?` | `null` | The selected folder. Two-way by default |
| `DisplayValue` | `string?` | `null` | The path text shown in the text box. Two-way by default; typing a path updates `Value` |
| `Title` | `string` | `null` | Dialog title. When empty, a localized "Select directory" text is used |
| `DefaultDirectory` | `string` | `""` | Dialog `DefaultDirectory` |
| `InitialDirectory` | `string` | `""` | Dialog `InitialDirectory`. When empty, the current `Value` is used if it exists |
| `RootDirectory` | `string` | `""` | Dialog `RootDirectory` |
| `ShowClearTextButton` | `bool` | `false` | Shows a clear button inside the text box |
| `WatermarkText` | `string` | `""` | Watermark shown when the text box is empty |
| `WatermarkAlignment` | `TextAlignment` | `Left` | Alignment of the watermark |
| `WatermarkTrimming` | `TextTrimming` | `None` | Trimming of the watermark |

## 📡 Events

| Event | Type | Description |
|-------|------|-------------|
| `ValueChanged` | Routed (bubble), `RoutedPropertyChangedEventHandler<DirectoryInfo?>` | Raised when `Value` changes |

## 📝 Notes

- The dialog is opened with `Multiselect = false`; only one folder can be selected
- Clearing the text sets `Value` to `null`
- The constructor sets `DataContext = this` so the internal XAML can bind to the control's own properties. A plain `{Binding X}` set on the picker therefore resolves against the picker itself, not your view model - use `RelativeSource`, `ElementName` or `Source` (as `LabelDirectoryPicker` does) when binding from outside
- Includes a dedicated automation peer (`DirectoryPickerAutomationPeer`) for UI automation

## 🔗 Related Controls

- **FilePicker** - Select a file instead of a folder
- **LabelDirectoryPicker** - Labeled form-control wrapper (`Atc.Wpf.Forms`)

## 🎮 Sample Application

See the DirectoryPicker sample in the Atc.Wpf.Sample application under **Wpf.Controls > Pickers > DirectoryPicker** for interactive examples.
