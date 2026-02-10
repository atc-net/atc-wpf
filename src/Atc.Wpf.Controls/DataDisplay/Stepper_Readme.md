# 🔢 Stepper

A step-by-step progress indicator with navigation support, click-to-navigate, and cancelable step transitions.

## 🔍 Overview

`Stepper` displays a sequence of steps with visual indicators for pending, active, completed, and error states. It supports both horizontal and vertical orientations, programmatic navigation with cancelable transitions, click-to-navigate on step indicators, custom step icons, configurable colors for each state, and opacity-based visual depth. Connecting lines between steps show completion progress. All colors use theme brushes for full light/dark theme support.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.DataDisplay;
```

## 🚀 Usage

### Basic Stepper

```xml
<dataDisplay:Stepper ActiveStepIndex="1">
    <dataDisplay:StepperItem Title="Account" Subtitle="Create your account" />
    <dataDisplay:StepperItem Title="Profile" Subtitle="Set up your profile" />
    <dataDisplay:StepperItem Title="Confirm" Subtitle="Review and confirm" />
</dataDisplay:Stepper>
```

### Vertical Stepper

```xml
<dataDisplay:Stepper Orientation="Vertical" ActiveStepIndex="0">
    <dataDisplay:StepperItem Title="Step 1" Content="First step content here." />
    <dataDisplay:StepperItem Title="Step 2" Content="Second step content here." />
    <dataDisplay:StepperItem Title="Step 3" Content="Final step content here." />
</dataDisplay:Stepper>
```

### Click-to-Navigate

By default, clicking a step indicator navigates to that step. Disable with `IsClickable="False"`:

```xml
<dataDisplay:Stepper IsClickable="False" ActiveStepIndex="0">
    <dataDisplay:StepperItem Title="Step 1" />
    <dataDisplay:StepperItem Title="Step 2" />
</dataDisplay:Stepper>
```

### 🎨 Custom Colors

```xml
<dataDisplay:Stepper
    ActiveStepIndex="1"
    ActiveBrush="DodgerBlue"
    CompletedBrush="Green"
    PendingBrush="LightGray"
    ErrorBrush="Red"
    CompletedLineBrush="Green"
    LineBrush="LightGray"
    IndicatorSize="40">
    <dataDisplay:StepperItem Title="Done" Status="Completed" />
    <dataDisplay:StepperItem Title="Current" Status="Active" />
    <dataDisplay:StepperItem Title="Next" Status="Pending" />
</dataDisplay:Stepper>
```

### Programmatic Navigation

```csharp
stepper.Next();          // Go to next step
stepper.Previous();      // Go to previous step
stepper.GoToStep(2);     // Go to specific step
```

### Cancelable Step Change

```csharp
stepper.StepChanging += (s, e) =>
{
    if (!IsStepValid(e.CurrentIndex))
    {
        e.Cancel = true; // Prevent navigation
    }
};
```

## ⚙️ Properties

### Stepper Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Orientation` | `Orientation` | `Horizontal` | Layout direction |
| `ActiveStepIndex` | `int` | `0` | Currently active step index |
| `IndicatorSize` | `double` | `32` | Size of step indicator circles |
| `IsClickable` | `bool` | `True` | Whether clicking a step indicator navigates to it |
| `ActiveBrush` | `Brush?` | Theme accent | Color for active step |
| `CompletedBrush` | `Brush?` | Theme accent | Color for completed steps |
| `PendingBrush` | `Brush?` | Theme gray | Color for pending steps |
| `ErrorBrush` | `Brush?` | `#D32F2F` | Color for error steps |
| `LineBrush` | `Brush?` | Theme gray | Color for incomplete connector lines |
| `CompletedLineBrush` | `Brush?` | Theme accent | Color for completed connector lines |
| `LineThickness` | `double` | `2` | Connector line thickness |
| `StepSpacing` | `double` | `0` | Spacing between steps |
| `Items` | `ObservableCollection<StepperItem>` | empty | Step items |

### StepperItem Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Status` | `StepperStepStatus` | `Pending` | Current status of this step |
| `Title` | `string?` | `null` | Step title text |
| `Subtitle` | `string?` | `null` | Step subtitle text |
| `IconContent` | `object?` | `null` | Custom icon inside the indicator |
| `IconTemplate` | `DataTemplate?` | `null` | Template for icon content |

## 📡 Events

| Event | Type | Description |
|-------|------|-------------|
| `StepChanged` | `EventHandler<StepperStepChangedEventArgs>` | Raised after a step change |
| `StepChanging` | `EventHandler<StepperStepChangingEventArgs>` | Raised before a step change (cancelable) |

## 🔧 Methods

| Method | Description |
|--------|-------------|
| `Previous()` | Navigate to the previous step |
| `Next()` | Navigate to the next step |
| `GoToStep(int index)` | Navigate to a specific step |

## 📊 Enumerations

### StepperStepStatus

| Value | Description |
|-------|-------------|
| `Pending` | Step not yet reached |
| `Active` | Currently active step |
| `Completed` | Successfully completed |
| `Error` | Step has an error |

## 🎨 Visual States & Opacity

Step indicators use opacity to provide visual depth:

| Status | Opacity | Description |
|--------|---------|-------------|
| `Active` | 1.0 | Full opacity — current step |
| `Completed` | 0.85 | Slightly dimmed — already done |
| `Pending` | 0.5 | Half opacity — not yet reached |
| `Error` | 1.0 | Full opacity — needs attention |

## 🌗 Theme Integration

All brushes default to theme-aware `DynamicResource` values:

- **Active/Completed indicators**: `AtcApps.Brushes.Accent`
- **Pending indicators/lines**: `AtcApps.Brushes.Gray6`
- **Indicator foreground**: `AtcApps.Brushes.IdealForeground` (active/completed/error), `AtcApps.Brushes.Text` (pending)
- **Title text**: `AtcApps.Brushes.Text`
- **Subtitle text**: `AtcApps.Brushes.Gray6`

The control automatically updates when switching between light and dark themes.

## 📝 Notes

- `StepChanging` event can cancel navigation by setting `e.Cancel = true`
- Click-to-navigate respects the `StepChanging` cancelation mechanism
- Completed connector lines use `CompletedLineBrush`, pending lines use `LineBrush`
- Step indicators show the step number by default, or custom `IconContent`
- `ActiveStepIndex` change triggers both `StepChanging` (cancelable) and `StepChanged` events

## 🔗 Related Controls

- **Timeline** - Chronological event display
- **Breadcrumb** - Navigation path indicator
- **Divider** - Simple visual separator

## 🎮 Sample Application

See the Stepper sample in the Atc.Wpf.Sample application under **Wpf.Controls > Data Display > Stepper** for interactive examples including the Usage section with live property controls.
