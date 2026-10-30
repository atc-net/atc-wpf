# 🔤 LabelFontPicker

A labeled font picker form field for family, size, weight, style, stretch, colors and text decorations.

## 🔍 Overview

`LabelFontPicker` puts a `FontPicker` (from `Atc.Wpf.Forms.BaseControls`) in the standard `LabelContent` layout used by the other `Label*` controls. Every font part is a separate property that the inner picker writes back, so you can bind each part on its own. Each part of the editor can be shown or hidden (`Show*`) and enabled or disabled (`Is*Enabled`). It implements `ILabelFontPicker`.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Basic Example

```xml
<atc:LabelFontPicker
    LabelText="Font"
    SelectedFontFamily="Arial"
    SelectedFontSize="14" />
```

### Family and Size Only

```xml
<atc:LabelFontPicker
    IsMandatory="True"
    LabelText="Family + Size only"
    SelectedFontFamily="Verdana"
    SelectedFontSize="12"
    ShowFontStretch="False"
    ShowFontStyle="False"
    ShowFontWeight="False" />
```

### Locked Parts and Bound Preview

```xml
<atc:LabelFontPicker
    x:Name="MyFontPicker"
    IsFontFamilyEnabled="False"
    IsFontSizeEnabled="False"
    LabelText="Family/Size locked"
    Orientation="Vertical"
    SelectedFontFamily="Consolas"
    SelectedFontSize="13" />

<TextBlock
    FontFamily="{Binding ElementName=MyFontPicker, Path=SelectedFontFamily}"
    FontSize="{Binding ElementName=MyFontPicker, Path=SelectedFontSize}"
    Text="Preview" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `SelectedFontFamily` | `FontFamily?` | `Segoe UI` | Selected font family |
| `SelectedFontSize` | `double` | `12` | Selected font size |
| `SelectedFontWeight` | `FontWeight` | `Normal` | Selected font weight |
| `SelectedFontStyle` | `FontStyle` | `Normal` | Selected font style |
| `SelectedFontStretch` | `FontStretch` | `Normal` | Selected font stretch |
| `SelectedForegroundBrush` | `SolidColorBrush?` | `Black` | Selected text color (see Notes) |
| `SelectedBackgroundBrush` | `SolidColorBrush?` | `Transparent` | Selected background color (see Notes) |
| `SelectedTextDecorations` | `TextDecorationCollection?` | `null` | Selected text decorations |
| `PreviewText` | `string` | `The quick brown fox jumps over the lazy dog 0123456789` | Text shown in the preview |
| `ColorEditorMode` | `FontColorEditorMode` | `WellKnownColorSelector` | Editor used for the colors: `WellKnownColorSelector` or `ColorPicker` |
| `ShowFontFamily`, `ShowFontSize`, `ShowFontWeight`, `ShowFontStyle`, `ShowFontStretch` | `bool` | `true` | Show each font part |
| `ShowForegroundColor`, `ShowBackgroundColor`, `ShowTextDecorations` | `bool` | `true` | Show the color and decoration parts |
| `ShowQuickToggles`, `ShowPreview` | `bool` | `true` | Show the quick toggle buttons and the preview |
| `IsFontFamilyEnabled`, `IsFontSizeEnabled`, `IsFontWeightEnabled`, `IsFontStyleEnabled`, `IsFontStretchEnabled` | `bool` | `true` | Enable each font part |
| `IsForegroundColorEnabled`, `IsBackgroundColorEnabled`, `IsTextDecorationsEnabled` | `bool` | `true` | Enable the color and decoration parts |

### 🏷️ Inherited Label Properties

Inherits the common label properties from `LabelControlBase` (`LabelText`, `LabelPosition`, `Orientation`, `LabelWidthNumber`, `LabelWidthSizeDefinition`, `HideAreas`, `InformationText`, `InformationContent`, `InformationColor`, `ContentMinHeight`, `GroupIdentifier`, `InputDataType`) and `LabelControl` (`IsMandatory`, `ShowAsteriskOnMandatory`, `MandatoryColor`, `ValidationText`, `ValidationColor`).

## ⚡ Events

| Event | Type | Description |
|-------|------|-------------|
| `FontChanged` | `EventHandler<ValueChangedEventArgs<FontDescription>>` | Raised when a font is picked in the inner font picker, with the old and new `FontDescription` and the control's identifier |

## 📝 Notes

- The `Selected*` properties do not bind two-way by default; use `Mode=TwoWay` to write the picked values back to the source
- On load, `SelectedForegroundBrush` and `SelectedBackgroundBrush` take the theme foreground and background brushes, but only while they are still at their default value; a set or bound value always wins
- `IsValid()` is not overridden and always returns `true`; `IsMandatory` only shows the asterisk
- `FontColorEditorMode` lives in `Atc.Wpf.Forms.FontEditing`
- The editor itself is described in `FontPicker_Readme.md`

## 🔗 Related Controls

- **FontPicker** - The font picker without the label
- **LabelFontFamilySelector** - Labeled drop-down for the font family only
- **LabelColorPicker** - Labeled color picker

## 🎮 Sample Application

See the LabelFontPicker sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > Pickers > LabelFontPicker** for interactive examples.
