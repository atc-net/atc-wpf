# Pickers

Picker controls provide file system selection capabilities for directories and files.

## Table of Contents

- [Pickers](#pickers)
  - [Table of Contents](#table-of-contents)
  - [Overview](#overview)
  - [Quick Reference](#quick-reference)
  - [DirectoryPicker](#directorypicker)
    - [Features](#features)
    - [Properties](#properties)
    - [Usage Example](#usage-example)
  - [FilePicker](#filepicker)
    - [Features](#features-1)
    - [Properties](#properties-1)
    - [Usage Example](#usage-example-1)
  - [Usage in Label Controls](#usage-in-label-controls)
  - [Theming](#theming)

## Overview

Picker controls provide specialized file system selection functionality:

- Browse dialogs for selecting directories and files
- Path validation and display
- Filter support for file types
- Integration with Label Controls (LabelDirectoryPicker, LabelFilePicker)

All picker controls support:
- Data binding with `INotifyPropertyChanged`
- WPF commanding patterns
- Light and Dark theme support
- Validation and error handling

## Quick Reference

| Control | Description |
|---------|-------------|
| DirectoryPicker | Select directories/folders from the file system |
| FilePicker | Select files from the file system with filter support |

## DirectoryPicker

A control for selecting directories/folders from the file system.

### Features

- Browse folder dialog
- Path display and validation
- Recent folders support
- Create new folder option

### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Value` | `DirectoryInfo?` | `null` | The selected directory |
| `InitialDirectory` | `string` | `""` | Starting directory for browsing |
| `RootDirectory` | `string` | `""` | Root directory constraint |
| `DefaultDirectory` | `string` | `""` | Default directory when cleared |
| `ShowClearTextButton` | `bool` | `true` | Show/hide clear button |
| `WatermarkText` | `string` | `""` | Placeholder text |
| `WatermarkAlignment` | `TextAlignment` | `Left` | Watermark text alignment |
| `WatermarkTrimming` | `TextTrimming` | `None` | Watermark text trimming |

### Usage Example

```xml
<!-- Basic directory picker -->
<atc:DirectoryPicker
    Value="{Binding OutputDirectory, Mode=TwoWay}" />

<!-- With initial directory -->
<atc:DirectoryPicker
    Value="{Binding ProjectPath, Mode=TwoWay}"
    InitialDirectory="C:\Projects" />

<!-- With watermark -->
<atc:DirectoryPicker
    Value="{Binding BackupFolder, Mode=TwoWay}"
    WatermarkText="Select backup folder..."
    ShowClearTextButton="True" />
```

## FilePicker

A control for selecting files from the file system.

### Features

- Browse file dialog
- File filter support
- Path validation
- Preview pane option

### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Value` | `FileInfo?` | `null` | The selected file |
| `Filter` | `string` | `"All Files (*.*)\|*.*"` | File type filter |
| `InitialDirectory` | `string` | `""` | Starting directory for browsing |
| `RootDirectory` | `string` | `""` | Root directory constraint |
| `DefaultDirectory` | `string` | `""` | Default directory when cleared |
| `AllowOnlyExisting` | `bool` | `true` | Only allow existing files |
| `UsePreviewPane` | `bool` | `false` | Show preview pane in dialog |
| `ShowClearTextButton` | `bool` | `true` | Show/hide clear button |
| `WatermarkText` | `string` | `""` | Placeholder text |
| `WatermarkAlignment` | `TextAlignment` | `Left` | Watermark text alignment |
| `WatermarkTrimming` | `TextTrimming` | `None` | Watermark text trimming |

### Usage Example

```xml
<!-- Single file selection -->
<atc:FilePicker
    Value="{Binding ConfigFile, Mode=TwoWay}"
    Filter="JSON Files (*.json)|*.json|All Files (*.*)|*.*" />

<!-- With preview pane -->
<atc:FilePicker
    Value="{Binding ImageFile, Mode=TwoWay}"
    Filter="Images (*.jpg;*.png)|*.jpg;*.png"
    UsePreviewPane="True" />

<!-- With initial directory -->
<atc:FilePicker
    Value="{Binding TemplateFile, Mode=TwoWay}"
    Filter="XML Files (*.xml)|*.xml"
    InitialDirectory="C:\Templates" />

<!-- Allow new files -->
<atc:FilePicker
    Value="{Binding OutputFile, Mode=TwoWay}"
    Filter="Text Files (*.txt)|*.txt"
    AllowOnlyExisting="False" />
```

## Usage in Label Controls

Picker controls are used as the core input mechanism for Label Controls:

```xml
<!-- As a standalone control -->
<atc:DirectoryPicker Value="{Binding OutputPath}" />

<!-- Within a Label Control -->
<atc:LabelDirectoryPicker
    LabelText="Output Directory"
    Value="{Binding OutputPath}" />

<!-- File picker standalone -->
<atc:FilePicker Value="{Binding ConfigFile}" />

<!-- Within a Label Control -->
<atc:LabelFilePicker
    LabelText="Configuration File"
    Value="{Binding ConfigFile}"
    Filter="JSON Files (*.json)|*.json" />
```

## Theming

All picker controls support both Light and Dark themes through the Atc.Wpf.Theming package.

- Automatic theme switching based on application theme
- Consistent styling across all picker controls
- Custom theme colors for focused/hover states
- High contrast mode support
