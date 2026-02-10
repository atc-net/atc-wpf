# Alert

A control for displaying prominent inline messages with severity levels, visual variants, and optional actions.

## Overview

`Alert` displays contextual feedback messages with four severity levels (Info, Success, Warning, Error) and three visual variants (Filled, Outlined, Text). It supports dismissibility via a close button, custom icons, action buttons, and dense mode for compact layouts.

## Namespace

```csharp
using Atc.Wpf.Controls.DataDisplay;
```

## Usage

### Basic Alert

```xml
<dataDisplay:Alert Severity="Info">
    This is an informational message.
</dataDisplay:Alert>
```

### Dismissible Alert

```xml
<dataDisplay:Alert Severity="Warning" IsDismissible="True" CloseClick="OnAlertClose">
    This alert can be dismissed.
</dataDisplay:Alert>
```

### Outlined Variant

```xml
<dataDisplay:Alert Severity="Error" Variant="Outlined">
    Something went wrong!
</dataDisplay:Alert>
```

### With Action Button

```xml
<dataDisplay:Alert Severity="Info">
    <dataDisplay:Alert.Actions>
        <Button Content="Undo" />
    </dataDisplay:Alert.Actions>
    Changes saved successfully.
</dataDisplay:Alert>
```

### Pulsing Alert

```xml
<dataDisplay:Alert Severity="Warning" IsPulsing="True">
    This alert pulses to draw attention.
</dataDisplay:Alert>
```

### Pulsing Alert with Custom Duration and Opacity

```xml
<dataDisplay:Alert Severity="Error" IsPulsing="True" PulseDuration="0:0:0.75" PulseOpacity="0.4">
    Fast deep-fade pulse for urgent alerts.
</dataDisplay:Alert>
```

### Custom Icon

```xml
<dataDisplay:Alert Severity="Success">
    <dataDisplay:Alert.Icon>
        <TextBlock Text="&#x2714;" FontSize="16" />
    </dataDisplay:Alert.Icon>
    Operation completed.
</dataDisplay:Alert>
```

## Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Severity` | `AlertSeverity` | `Info` | Severity level determining colors and default icon |
| `Variant` | `AlertVariant` | `Filled` | Visual variant (Filled, Outlined, Text) |
| `IsDismissible` | `bool` | `false` | Shows a close button when true |
| `IsDense` | `bool` | `false` | Uses reduced padding for compact layouts |
| `IsPulsing` | `bool` | `false` | Plays a subtle breathing animation to draw attention |
| `PulseDuration` | `TimeSpan` | `0:0:1.5` | Half-cycle duration of the pulse animation (full cycle = 2x with auto-reverse) |
| `PulseOpacity` | `double` | `0.65` | Minimum opacity during the pulse animation (0.0–1.0) |
| `ShowIcon` | `bool` | `true` | Shows or hides the severity icon |
| `Icon` | `object?` | `null` | Custom icon content (overrides default severity icon) |
| `IconTemplate` | `DataTemplate?` | `null` | Template for custom icon content |
| `Actions` | `object?` | `null` | Content for the action button area |
| `ActionsTemplate` | `DataTemplate?` | `null` | Template for action content |
| `CornerRadius` | `CornerRadius` | `4` | Corner rounding for the alert border |
| `AlertBackground` | `Brush?` | `null` | Override brush for background |
| `AlertForeground` | `Brush?` | `null` | Override brush for foreground |
| `AlertBorderBrush` | `Brush?` | `null` | Override brush for border |

## Events

| Event | Type | Description |
|-------|------|-------------|
| `CloseClick` | `RoutedEventHandler` | Fired when the close button is clicked |

## Enumerations

### AlertSeverity

| Value | Description |
|-------|-------------|
| `Info` | Informational message (blue) |
| `Success` | Success or confirmation message (green) |
| `Warning` | Warning message requiring attention (yellow) |
| `Error` | Error or critical message (red) |

### AlertVariant

| Value | Description |
|-------|-------------|
| `Filled` | Solid severity background with contrasting text |
| `Outlined` | Transparent background with severity-colored border and text |
| `Text` | No border or background, severity-colored text only |

## Notes

- The default variant uses light severity-colored backgrounds (Bootstrap 100-level) with dark text (Bootstrap 900-level)
- The `Filled` variant uses the Bootstrap 500-level color as background with white text
- `AlertBackground`, `AlertForeground`, and `AlertBorderBrush` override any severity/variant colors when set
- The close button fires the `CloseClick` event but does not automatically hide the alert; handle visibility in your code
- The pulse animation targets the border opacity (1.0 → `PulseOpacity`), configurable via `PulseDuration` and `PulseOpacity`; it composes correctly with the disabled state which sets control-level opacity

## Related Controls

- **Chip** - Compact elements for tags and filters
- **Badge** - Overlay indicators for counts and status
- **Card** - Content container for structured information

## Sample Application

See the Alert sample in the Atc.Wpf.Sample application under **Wpf.Controls > Data Display > Alert** for interactive examples.
