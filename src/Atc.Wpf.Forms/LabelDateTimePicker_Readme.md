# 🗓️ LabelDateTimePicker

A labeled date and time form field with separate date and time text boxes, a calendar drop-down and a clock drop-down.

## 🔍 Overview

`LabelDateTimePicker` combines a label area (label text, mandatory asterisk, information popup and validation message) with two inputs: a date text box with a calendar popup, and a time text box with a clock popup (`ClockPanelPicker`). The user can type the values or pick them from the popups.

The date and time text are parsed together with `CustomCulture` (or the current UI culture). Valid input updates `SelectedDate` with both the date and the time; invalid input shows a validation message with the expected date and time patterns. Parsing happens when a text box loses focus or when **Enter** is pressed.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Basic Example

```xml
<atc:LabelDateTimePicker
    LabelText="Appointment"
    SelectedDate="{Binding AppointmentTime}" />
```

### Initial Text and Watermark

```xml
<atc:LabelDateTimePicker
    LabelText="Meeting"
    TextDate="8/25/2023"
    TextTime="14:47"
    WatermarkDateText="Select a date" />
```

### Mandatory, Long Date Format

```xml
<atc:LabelDateTimePicker
    IsMandatory="True"
    LabelText="Deadline"
    SelectedDateFormat="Long"
    Orientation="Vertical" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `SelectedDate` | `DateTime?` | `null` | The selected date and time. Binds two-way by default |
| `TextDate` | `string` | `""` | The date text. Binds two-way by default and updates the source on lost focus |
| `TextTime` | `string` | `""` | The time text. Binds two-way by default and updates the source on lost focus |
| `SelectedDateFormat` | `DatePickerFormat` | `Short` | Use the culture's short or long date pattern |
| `CustomCulture` | `CultureInfo?` | `null` | Culture used for formatting and parsing (current UI culture when `null`) |
| `DisplayDate` | `DateTime` | `DateTime.Now` | Date the calendar shows when opened |
| `DisplayDateStart` | `DateTime?` | `null` | First date shown in the calendar |
| `DisplayDateEnd` | `DateTime?` | `null` | Last date shown in the calendar |
| `FirstDayOfWeek` | `DayOfWeek` | `Monday` | First day of the week in the calendar |
| `IsTodayHighlighted` | `bool` | `true` | Highlight today's date in the calendar |
| `OpenCalender` | `bool` | `false` | Whether the calendar popup is open |
| `OpenClock` | `bool` | `false` | Whether the clock popup is open |
| `WatermarkDateText` | `string` | `""` | Watermark for the empty date box (see Notes) |
| `WatermarkTimeText` | `string` | `""` | Watermark for the empty time box (see Notes) |
| `WatermarkAlignment` | `TextAlignment` | `Left` | Watermark alignment |
| `WatermarkTrimming` | `TextTrimming` | `None` | Watermark trimming |

### 🏷️ Inherited Label Properties

Inherits the common label properties from `LabelControlBase` (`LabelText`, `LabelPosition`, `Orientation`, `LabelWidthNumber`, `LabelWidthSizeDefinition`, `HideAreas`, `InformationText`, `InformationContent`, `InformationColor`, `ContentMinHeight`, `GroupIdentifier`, `InputDataType`) and `LabelControl` (`IsMandatory`, `ShowAsteriskOnMandatory`, `MandatoryColor`, `ValidationText`, `ValidationColor`).

## ⚡ Events

| Event | Type | Description |
|-------|------|-------------|
| `LostFocusValid` | `EventHandler<ValueChangedEventArgs<DateTime?>>` | Raised when the text is committed and valid |
| `LostFocusInvalid` | `EventHandler<ValueChangedEventArgs<DateTime?>>` | Raised when the text is committed and invalid |

## 📝 Notes

- When `WatermarkDateText` is empty or looks like a date pattern (contains `/`, `.` or `,`), it is replaced with the culture's short or long date pattern; the same applies to `WatermarkTimeText` (contains `:` or `.`) with the short time pattern. This happens on load and when the UI culture changes
- When the UI culture changes, the date and time text of the selected value are reformatted
- With `IsMandatory="True"`, both date and time must be filled in; otherwise empty date and time clears `SelectedDate`
- Picking a time in the clock popup updates `TextTime` with the culture's short time format
- The calendar and clock icons are re-colored with the accent color when the theme changes

## 🔗 Related Controls

- **LabelDatePicker** - Labeled date-only picker
- **LabelTimePicker** - Labeled time-only picker
- **ClockPanelPicker** - Hour/minute panel used in the clock popup

## 🎮 Sample Application

See the LabelDateTimePicker sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > Pickers > LabelDateTimePicker** for interactive examples.
