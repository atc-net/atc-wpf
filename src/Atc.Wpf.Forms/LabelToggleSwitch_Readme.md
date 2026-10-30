# 🔘 LabelToggleSwitch

A labeled on/off switch form field with localized On/Off texts.

## 🔍 Overview

`LabelToggleSwitch` combines a label area (label text and information popup) with a `ToggleSwitch` from `Atc.Wpf.Controls`. It derives from `LabelControlBase`, not `LabelControl`, so it has no mandatory marker and no validation message. The switch shows "Off" / "On" texts from the current UI culture, and they follow culture changes unless you set your own. It implements `ILabelToggleSwitch`.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Basic Example

```xml
<atc:LabelToggleSwitch LabelText="Notifications" IsOn="{Binding IsNotificationsOn}" />
```

### Label to the Right

```xml
<atc:LabelToggleSwitch
    LabelPosition="Right"
    LabelText="Dark mode"
    Orientation="Horizontal" />
```

### Content Next to the Switch

```xml
<atc:LabelToggleSwitch
    LabelText="Enable feature"
    OffOnContent="Enable feature"
    Orientation="Horizontal" />
```

### Custom On/Off Texts

```xml
<atc:LabelToggleSwitch
    LabelText="Mode"
    OffText="Manual"
    OnText="Automatic" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IsOn` | `bool` | `false` | Whether the switch is on. Binds two-way by default |
| `OffText` | `string?` | localized "Off" | Text shown when off. Binds two-way by default |
| `OnText` | `string?` | localized "On" | Text shown when on. Binds two-way by default |
| `OffOnContent` | `string?` | `null` | Content shown next to the switch. Binds two-way by default |
| `ContentDirection` | `FlowDirection` | `RightToLeft` | Which side of the switch the content is placed on |

### 🏷️ Inherited Label Properties

Inherits the common label properties from `LabelControlBase` (`LabelText`, `LabelPosition`, `Orientation`, `LabelWidthNumber`, `LabelWidthSizeDefinition`, `HideAreas`, `InformationText`, `InformationContent`, `InformationColor`, `ContentMinHeight`, `GroupIdentifier`, `InputDataType`).

## ⚡ Events

| Event | Type | Description |
|-------|------|-------------|
| `IsOnChanged` | `EventHandler<ValueChangedEventArgs<bool>>` | Raised when `IsOn` changes, with the old and new value and the control's `Identifier` |

## 📝 Notes

- Setting `LabelPosition` to `Right` sets the label width to `50` pixels and clears `OffText` and `OnText`; any other position sets the width to `120` pixels and restores the localized texts
- When the UI culture changes, `OffText` and `OnText` are updated only if they still hold the localized text of the old culture, so custom texts are kept

## 🔗 Related Controls

- **ToggleSwitch** - The switch without the label
- **LabelCheckBox** - Labeled check box

## 🎮 Sample Application

See the LabelToggleSwitch sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > LabelToggleSwitch** for interactive examples.
