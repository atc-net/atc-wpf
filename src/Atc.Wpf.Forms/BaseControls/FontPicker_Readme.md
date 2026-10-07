# 🔤 FontPicker

A compact font field that previews the current font and opens a full font dialog on demand.

## 🔍 Overview

`FontPicker` shows the selected font family and size (e.g. `Segoe UI  12pt`) rendered in the selected font, next to an edit button. Clicking the button opens the `FontPickerDialogBox` (built on `AdvancedFontPicker`), where family, size, weight, style, stretch, colors and text decorations can be edited. When the dialog is confirmed, the selected values are updated and the `FontChanged` event is raised.

Each part of the font can be hidden (`Show*`) or shown read-only (`Is*Enabled`) in the dialog, so the same control can be used for a full font editor or a simple family/size picker.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms.BaseControls;
```

## 🚀 Usage

### Basic Example

```xml
<atc:FontPicker SelectedFontSize="14" />
```

### Family and Size Only

```xml
<atc:FontPicker
    SelectedFontFamily="Arial"
    SelectedFontSize="12"
    ShowFontStretch="False"
    ShowFontStyle="False"
    ShowFontWeight="False" />
```

### Lock Some Parts

```xml
<!-- Family, size and stretch are shown but read-only in the dialog -->
<atc:FontPicker
    IsFontFamilyEnabled="False"
    IsFontSizeEnabled="False"
    IsFontStretchEnabled="False"
    SelectedFontFamily="Verdana"
    SelectedFontSize="16" />
```

### Apply the Selected Font to Other Elements

```xml
<atc:FontPicker x:Name="MyFontPicker" />

<TextBlock
    Text="Preview"
    FontFamily="{Binding ElementName=MyFontPicker, Path=SelectedFontFamily}"
    FontSize="{Binding ElementName=MyFontPicker, Path=SelectedFontSize}"
    FontWeight="{Binding ElementName=MyFontPicker, Path=SelectedFontWeight}"
    FontStyle="{Binding ElementName=MyFontPicker, Path=SelectedFontStyle}"
    Foreground="{Binding ElementName=MyFontPicker, Path=SelectedForegroundBrush}"
    Background="{Binding ElementName=MyFontPicker, Path=SelectedBackgroundBrush}"
    TextDecorations="{Binding ElementName=MyFontPicker, Path=SelectedTextDecorations}" />
```

### Work with a FontDescription in Code

```csharp
var font = myFontPicker.GetFontDescription();

myFontPicker.SetFontDescription(font);

myFontPicker.FontChanged += (sender, e) =>
{
    var oldFont = e.OldValue;
    var newFont = e.NewValue;
};
```

## ⚙️ Properties

### Selected Values

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `SelectedFontFamily` | `FontFamily?` | `Segoe UI` | Selected font family |
| `SelectedFontSize` | `double` | `12` | Selected font size (`FontDescription.DefaultSize`) |
| `SelectedFontWeight` | `FontWeight` | `Normal` | Selected font weight |
| `SelectedFontStyle` | `FontStyle` | `Normal` | Selected font style |
| `SelectedFontStretch` | `FontStretch` | `Normal` | Selected font stretch |
| `SelectedForegroundBrush` | `SolidColorBrush?` | `Black` | Selected text color (theme foreground when not set, see Notes) |
| `SelectedBackgroundBrush` | `SolidColorBrush?` | `Transparent` | Selected background color (theme background when not set, see Notes) |
| `SelectedTextDecorations` | `TextDecorationCollection?` | `null` | Selected text decorations (underline, strikethrough, ...) |
| `DisplayText` | `string?` | `""` | Text shown in the field, e.g. `Segoe UI  12pt`. Binds two-way by default |

### Dialog Visibility

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ShowFontFamily` | `bool` | `true` | Show the font family editor |
| `ShowFontSize` | `bool` | `true` | Show the font size editor |
| `ShowFontWeight` | `bool` | `true` | Show the font weight editor |
| `ShowFontStyle` | `bool` | `true` | Show the font style editor |
| `ShowFontStretch` | `bool` | `true` | Show the font stretch editor |
| `ShowForegroundColor` | `bool` | `true` | Show the foreground color editor |
| `ShowBackgroundColor` | `bool` | `true` | Show the background color editor |
| `ShowTextDecorations` | `bool` | `true` | Show the text decorations editor |
| `ShowQuickToggles` | `bool` | `true` | Show the quick toggle buttons |
| `ShowPreview` | `bool` | `true` | Show the preview area |

### Dialog Editability

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IsFontFamilyEnabled` | `bool` | `true` | Allow editing the font family |
| `IsFontSizeEnabled` | `bool` | `true` | Allow editing the font size |
| `IsFontWeightEnabled` | `bool` | `true` | Allow editing the font weight |
| `IsFontStyleEnabled` | `bool` | `true` | Allow editing the font style |
| `IsFontStretchEnabled` | `bool` | `true` | Allow editing the font stretch |
| `IsForegroundColorEnabled` | `bool` | `true` | Allow editing the foreground color |
| `IsBackgroundColorEnabled` | `bool` | `true` | Allow editing the background color |
| `IsTextDecorationsEnabled` | `bool` | `true` | Allow editing the text decorations |

### Other

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ColorEditorMode` | `FontColorEditorMode` | `WellKnownColorSelector` | Color editor used in the dialog: `WellKnownColorSelector` (inline drop-down) or `ColorPicker` (opens a color dialog) |
| `PreviewText` | `string` | `The quick brown fox jumps over the lazy dog 0123456789` | Text shown in the dialog preview |

## ⚡ Events

| Event | Type | Description |
|-------|------|-------------|
| `FontChanged` | `EventHandler<ValueChangedEventArgs<FontDescription>>` | Raised after a font is confirmed in the font dialog, with the old and new `FontDescription` |

## 🛠️ Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `GetFontDescription()` | `FontDescription` | Returns a new `FontDescription` built from the selected values |
| `SetFontDescription(FontDescription)` | `void` | Applies a `FontDescription`. Foreground, background and text decorations are only applied when not `null` |

## 📝 Notes

- `DisplayText` is updated when `SelectedFontFamily` or `SelectedFontSize` changes
- On load, if `SelectedForegroundBrush` / `SelectedBackgroundBrush` were not set, they are bound to the current theme foreground/background brushes
- The dialog owner is the window hosting the control, falling back to `Application.Current.MainWindow`
- `FontChanged` is only raised from the dialog flow, not when properties are set from code or bindings

## 🔗 Related Controls

- **LabelFontPicker** - Labeled form-field version with validation and mandatory indicator
- **AdvancedFontPicker** - Full inline font editor used inside the dialog
- **FontFamilySelector** - Drop-down selector for font families only

## 🎮 Sample Application

See the FontPicker sample in the Atc.Wpf.Sample application under **Wpf.Controls > Inputs > FontPicker** for interactive examples.
