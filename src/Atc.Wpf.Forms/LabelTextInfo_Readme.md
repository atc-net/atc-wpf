# ℹ️ LabelTextInfo

A labeled, read-only text field for showing information in a form, with optional copy-to-clipboard.

## 🔍 Overview

`LabelTextInfo` shows a label and a piece of read-only text using the same layout as the other `Label*` form controls. It is meant for values the user should see but not edit, such as IDs, computed values or status text.

When `EnableCopyToClipboard` is `true`, the text gets a hand cursor, an accent color on hover, a "right-click to copy" tooltip and a context menu with a **Copy to clipboard** item.

`LabelTextInfo` derives from `LabelControlBase`, so it has no mandatory indicator or validation message.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Basic Example

```xml
<atc:LabelTextInfo LabelText="Customer ID" Text="{Binding CustomerId}" />
```

### Copy to Clipboard

```xml
<atc:LabelTextInfo
    EnableCopyToClipboard="True"
    LabelText="API key"
    Text="{Binding ApiKey}" />
```

### Vertical Layout with Help Text

```xml
<atc:LabelTextInfo
    InformationText="Calculated from the order lines."
    LabelText="Total"
    Orientation="Vertical"
    Text="{Binding Total}" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Text` | `string` | `""` | The text to display |
| `EnableCopyToClipboard` | `bool` | `false` | Adds a context menu to copy `Text` to the clipboard, plus hover styling and a tooltip |

### 🏷️ Inherited Label Properties

Inherits the common label properties from `LabelControlBase`: `LabelText`, `LabelPosition`, `Orientation`, `LabelWidthNumber`, `LabelWidthSizeDefinition`, `HideAreas`, `InformationText`, `InformationContent`, `InformationColor`, `ContentMinHeight`, `GroupIdentifier` and `InputDataType`.

## 📝 Notes

- The **Copy to clipboard** menu item is disabled while `Text` is empty
- The context menu is created once and reused when `EnableCopyToClipboard` is toggled
- Setting `EnableCopyToClipboard` to `false` removes the control's `ContextMenu`

## 🔗 Related Controls

- **LabelTextBox** - Labeled editable text input
- **LabelContent** - Labeled container for any content

## 🎮 Sample Application

See the LabelTextInfo sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > LabelTextInfo** for interactive examples.
