# 📅 LabelDatePicker

A labeled date form field with a text box, a calendar drop-down, culture-aware parsing and validation.

## 🔍 Overview

`LabelDatePicker` combines a label area (label text, mandatory asterisk, information popup and validation message) with a date text box and a calendar button. The user can type a date or pick one from the calendar popup.

Typed text is parsed with `CustomCulture` (or the current UI culture) using the short or long date pattern given by `SelectedDateFormat`. Valid text updates `SelectedDate`; invalid text shows a validation message with the expected pattern. Parsing happens when the text box loses focus or when **Enter** is pressed.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Basic Example

```xml
<atc:LabelDatePicker
    LabelText="Birth date"
    SelectedDate="{Binding BirthDate}" />
```

### Mandatory with Watermark

```xml
<atc:LabelDatePicker
    IsMandatory="True"
    LabelText="Start date"
    WatermarkText="Select a date" />
```

### Long Date Format and Limited Range

```xml
<atc:LabelDatePicker
    LabelText="Delivery"
    SelectedDateFormat="Long"
    DisplayDateStart="{Binding MinDate}"
    DisplayDateEnd="{Binding MaxDate}"
    FirstDayOfWeek="Sunday" />
```

### Handle Validation in Code

```csharp
labelDatePicker.LostFocusValid += (sender, e) =>
{
    var date = e.NewValue;
};

labelDatePicker.LostFocusInvalid += (sender, e) =>
{
    // ValidationText describes the problem
};
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `SelectedDate` | `DateTime?` | `null` | The selected date. Binds two-way by default |
| `Text` | `string` | `""` | The date text. Binds two-way by default and updates the source on lost focus |
| `SelectedDateFormat` | `DatePickerFormat` | `Short` | Use the culture's short or long date pattern for text and parsing |
| `CustomCulture` | `CultureInfo?` | `null` | Culture used for formatting and parsing (current UI culture when `null`) |
| `DisplayDate` | `DateTime` | `DateTime.Now` | Date the calendar shows when opened |
| `DisplayDateStart` | `DateTime?` | `null` | First date shown in the calendar |
| `DisplayDateEnd` | `DateTime?` | `null` | Last date shown in the calendar |
| `FirstDayOfWeek` | `DayOfWeek` | `Monday` | First day of the week in the calendar |
| `IsTodayHighlighted` | `bool` | `true` | Highlight today's date in the calendar |
| `OpenCalender` | `bool` | `false` | Whether the calendar popup is open |
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

- When `WatermarkText` is empty, or looks like a date pattern (contains `/`, `.` or `,`), it is replaced with the culture's short or long date pattern on load and when the UI culture changes
- When the UI culture changes, the text of the selected date is reformatted for the new culture
- With `IsMandatory="True"`, empty text gives a "field is required" validation message; otherwise empty text clears `SelectedDate`
- Invalid text sets `SelectedDate` to `null`
- The calendar icon is re-colored with the accent color when the theme changes
- The `TextChanged` routed event is declared but is not raised by the control's own code

## 🔗 Related Controls

- **LabelDateTimePicker** - Labeled date and time picker
- **LabelTimePicker** - Labeled time picker
- **LabelTextBox** - Labeled text input

## 🎮 Sample Application

See the LabelDatePicker sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > Pickers > LabelDatePicker** for interactive examples.
