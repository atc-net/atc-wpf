# ⏳ BusyOverlay

A content overlay that dims the UI and displays a loading message during async operations.

## 🔍 Overview

`BusyOverlay` wraps content and displays a dimming overlay with customizable busy content when `IsBusy` is true. It supports a configurable display delay to prevent flash-of-overlay for fast operations, custom content before/during/after the busy state, and automatic focus restoration when the operation completes. The default busy message is localized ("Please Wait...").

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Progressing;
```

## 🚀 Usage

### Basic Busy Overlay

```xml
<progressing:BusyOverlay IsBusy="{Binding IsProcessing}">
    <DataGrid ItemsSource="{Binding Items}" />
</progressing:BusyOverlay>
```

### Custom Busy Content

```xml
<progressing:BusyOverlay IsBusy="{Binding IsLoading}">
    <progressing:BusyOverlay.BusyContent>
        <StackPanel HorizontalAlignment="Center">
            <progressing:LoadingIndicator Mode="Ring" />
            <TextBlock Text="Loading data..." Margin="0,8,0,0" />
        </StackPanel>
    </progressing:BusyOverlay.BusyContent>

    <ListView ItemsSource="{Binding Results}" />
</progressing:BusyOverlay>
```

### Display Delay

```xml
<!-- Only show overlay if operation takes longer than 500ms -->
<progressing:BusyOverlay
    IsBusy="{Binding IsBusy}"
    DisplayAfter="0:0:0.5">
    <ContentControl Content="{Binding CurrentView}" />
</progressing:BusyOverlay>
```

### Focus Restoration

```xml
<!-- Return focus to search box after busy state ends -->
<progressing:BusyOverlay
    IsBusy="{Binding IsSearching}"
    FocusAfterBusy="{Binding ElementName=SearchBox}">
    <TextBox x:Name="SearchBox" />
</progressing:BusyOverlay>
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IsBusy` | `bool` | `false` | Whether the overlay is active |
| `BusyContent` | `object?` | `null` | Content shown during busy state |
| `BusyContentTemplate` | `DataTemplate?` | `null` | Template for busy content |
| `BusyContentBefore` | `object?` | `null` | Content shown above busy content |
| `BusyContentTemplateBefore` | `DataTemplate?` | `null` | Template for before content |
| `BusyContentAfter` | `object?` | `null` | Content shown below busy content |
| `BusyContentTemplateAfter` | `DataTemplate?` | `null` | Template for after content |
| `DisplayAfter` | `TimeSpan` | `0.1s` | Delay before showing overlay (prevents flash) |
| `FocusAfterBusy` | `Control?` | `null` | Control to focus when busy ends |
| `OverlayStyle` | `Style?` | `null` | Style for the dimming rectangle |
| `ProgressBarStyle` | `Style?` | `null` | Style for the progress bar |

## 📝 Notes

- The `DisplayAfter` delay prevents overlay from flashing during fast operations
- Default busy text is localized and updates on culture change
- Focus is automatically restored to `FocusAfterBusy` control when `IsBusy` becomes false
- Visual states: `StateBusy`/`StateIdle` and `StateVisible`/`StateHidden`

## 🔗 Related Controls

- **LoadingIndicator** - Animated loading indicators (can be used inside BusyOverlay)
- **Skeleton** - Placeholder content during loading
- **Overlay** - Simpler content dimming overlay

## 🎮 Sample Application

See the BusyOverlay sample in the Atc.Wpf.Sample application under **Wpf.Controls > Progressing > BusyOverlay** for interactive examples.
