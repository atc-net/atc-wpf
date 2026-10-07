# 🕒 ClockPanelPicker

A small panel with hour and minute drop-downs for picking a time of day.

## 🔍 Overview

`ClockPanelPicker` shows two labeled combo boxes, one for the hour (0-23) and one for the minute (0-59). When either value changes, the control updates `SelectedDateTime` with the chosen time and raises the `SelectedClockChanged` event.

It is used inside the drop-downs of `LabelTimePicker` and `LabelDateTimePicker`, and can also be used on its own.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms.BaseControls;
```

## 🚀 Usage

### Basic Example

```xml
<atc:ClockPanelPicker
    SelectedClockChanged="OnSelectedClockChanged"
    SelectedDateTime="{Binding SelectedTime, Mode=TwoWay}" />
```

```csharp
private void OnSelectedClockChanged(
    object? sender,
    RoutedEventArgs e)
{
    var picker = (ClockPanelPicker)sender!;
    if (picker.SelectedDateTime.HasValue)
    {
        var time = picker.SelectedDateTime.Value.TimeOfDay;
    }
}
```

### Set the Time from Code

```csharp
clockPanelPicker.SelectedKeyHour = "14";
clockPanelPicker.SelectedKeyMinute = "30";
```

## ⚙️ Properties

### Dependency Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `SelectedDateTime` | `DateTime?` | `null` | The selected time. The date part is kept if a value is already set, otherwise today's date is used |

### CLR Properties (INotifyPropertyChanged)

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `SelectedKeyHour` | `string?` | `"0"` | Selected hour key (`"0"`-`"23"`). Setting it updates `SelectedDateTime` |
| `SelectedKeyMinute` | `string?` | `"0"` | Selected minute key (`"0"`-`"59"`). Setting it updates `SelectedDateTime` |
| `Hours` | `IDictionary<string, string>` | `0`-`23` | Items for the hour drop-down |
| `Minutes` | `IDictionary<string, string>` | `0`-`59` | Items for the minute drop-down |

## ⚡ Events

| Event | Type | Description |
|-------|------|-------------|
| `SelectedClockChanged` | Routed event (`Direct`), `EventHandler<RoutedEventArgs>` | Raised when the hour or minute is changed |
| `PropertyChanged` | `PropertyChangedEventHandler` | Raised when one of the CLR properties changes |

## 📝 Notes

- `SelectedDateTime` is created with `DateTimeKind.Local` and seconds set to `0`
- The constructor sets both keys to `"0"`, so `SelectedDateTime` starts at midnight today and `SelectedClockChanged` is raised during construction
- Setting `SelectedDateTime` from the outside does not move the hour and minute drop-downs; use `SelectedKeyHour` / `SelectedKeyMinute` for that
- The control sets its own `DataContext` to itself
- The labels "Hour" and "Minute" are localized via the `Atc.Wpf.Resources.Word` resources

## 🔗 Related Controls

- **LabelTimePicker** - Labeled time picker that hosts this panel in its drop-down
- **LabelDateTimePicker** - Labeled date and time picker that hosts this panel in its drop-down
- **LabelComboBox** - Used for the hour and minute drop-downs

## 🎮 Sample Application

No dedicated sample yet. The panel can be seen in the drop-downs of the **Wpf.Forms > Label Controls > Pickers > LabelTimePicker** and **LabelDateTimePicker** samples.
