# 🕒 TimeZonePicker

A control for selecting a system-defined time zone.

## Overview

Lightweight picker that enumerates `TimeZoneInfo.GetSystemTimeZones()` once at construction and binds directly to the dropdown — no service, no hot-plug, no `DeviceWatcher`. Items are rendered with their UTC offset followed by the system display name.

## Namespace

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## Usage

```xml
<atc:TimeZonePicker
    Value="{Binding SelectedTimeZone, Mode=TwoWay}" />

<atc:TimeZonePicker WatermarkText="Select time zone…" />
```

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Value` | `TimeZoneInfo?` | `null` | Selected time zone (TwoWay) |
| `WatermarkText` | `string` | `""` | Empty-state placeholder |
| `ItemTemplate` | `DataTemplate?` | default | Override default item visuals |
| `TimeZones` | `IReadOnlyCollection<TimeZoneInfo>` | system list | Read-only, populated once |

## Routed Events

| Event | Args | Fires when |
|-------|------|------------|
| `ValueChanged` | `RoutedPropertyChangedEventArgs<TimeZoneInfo?>` | Selected time zone changed |

## Notes

- Stores the standard `System.TimeZoneInfo` directly; `Value.Id` round-trips cleanly to `TimeZoneInfo.FindSystemTimeZoneById`.
- The list is the OS view as of construction. Recreate the control to pick up time-zone definition changes (rare).
- Unlike the hardware pickers, this control has no concept of "Available / InUse / Disconnected" — time zones don't transition.

## Related Controls

- `LabelTimeZonePicker` — labeled wrapper with mandatory validation
- `DrivePicker` — sibling system-metadata picker

## Sample Application Path

`SamplesWpfHardware → Pickers → TimeZonePicker`