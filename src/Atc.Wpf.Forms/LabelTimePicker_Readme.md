# ⏰ LabelTimePicker

A labeled time form field with a text box, a clock drop-down, culture-aware parsing and validation.

## 🔍 Overview

`LabelTimePicker` combines a label area (label text, mandatory asterisk, information popup and validation message) with a time text box and a clock button. The user can type a time or pick hour and minute in the clock popup (`ClockPanelPicker`).

Typed text is parsed as a short time with `CustomCulture` (or the current UI culture). Valid text updates `SelectedTime`; invalid text shows a validation message. Parsing happens when the text box loses focus or when **Enter** is pressed.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Basic Example

```xml
<atc:LabelTimePicker
    LabelText="Start time"
    SelectedTime="{Binding StartTime}" />
```

### Initial Text and Watermark

```xml
<atc:LabelTimePicker
    LabelText="Alarm"
    Text="14:47"
    WatermarkText="Select a time..." />
```

### Mandatory

```xml
<atc:LabelTimePicker
    IsMandatory="True"
    LabelText="Pickup time" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `SelectedTime` | `DateTime?` | `null` | The selected time (as a `DateTime`). Binds two-way by default |
| `Text` | `string` | `""` | The time text. Binds two-way by default and updates the source on lost focus |
| `CustomCulture` | `CultureInfo?` | `null` | Culture used for formatting and parsing (current UI culture when `null`) |
| `OpenClock` | `bool` | `false` | Whether the clock popup is open |
| `WatermarkText` | `string` | `""` | Watermark for the empty text box (see Notes) |
| `WatermarkAlignment` | `TextAlignment` | `Left` | Watermark alignment |
| `WatermarkTrimming` | `TextTrimming` | `None` | Watermark trimming |

### 🏷️ Inherited Label Properties

Inherits the common label properties from `LabelControlBase` (`LabelText`, `LabelPosition`, `Orientation`, `LabelWidthNumber`, `LabelWidthSizeDefinition`, `HideAreas`, `InformationText`, `InformationContent`, `InformationColor`, `ContentMinHeight`, `GroupIdentifier`, `InputDataType`) and `LabelControl` (`IsMandatory`, `ShowAsteriskOnMandatory`, `MandatoryColor`, `ValidationText`, `ValidationColor`).

## ⚡ Events

| Event | Type | Description |
|-------|------|-------------|
| `LostFocusValid` | `EventHandler<ValueChangedEventArgs<DateTime?>>` | Raised when the text is committed and valid |
| `LostFocusInvalid` | `EventHandler<ValueChangedEventArgs<DateTime?>>` | Raised when the text is committed and invalid |
| `TextChanged` | Routed event (`Bubble`), `RoutedPropertyChangedEventHandler<string>` | Routed event identifier declared on the control |

## 📝 Notes

- When `WatermarkText` is empty or looks like a time pattern (contains `:` or `.`), it is replaced with the culture's short time pattern on load and when the UI culture changes
- When the UI culture changes, the text of the selected time is reformatted
- With `IsMandatory="True"`, empty text gives a "field is required" validation message; otherwise empty text clears `SelectedTime`
- Picking a time in the clock popup updates `Text` with the culture's short time format
- The `TextChanged` routed event is declared but is not raised by the control's own code

## 🔗 Related Controls

- **LabelDateTimePicker** - Labeled date and time picker
- **LabelDatePicker** - Labeled date picker
- **ClockPanelPicker** - Hour/minute panel used in the clock popup

## 🎮 Sample Application

See the LabelTimePicker sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > Pickers > LabelTimePicker** for interactive examples.
