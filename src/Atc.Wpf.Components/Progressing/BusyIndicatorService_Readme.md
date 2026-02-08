# BusyIndicatorService

An MVVM-friendly service for managing busy overlays from ViewModels, with support for scoped regions, progress reporting, and cancellation.

## Overview

`IBusyIndicatorService` provides a DI-friendly way to show and hide `BusyOverlay` controls from ViewModels. It wraps the `BusyIndicatorManager` which manages the registration and activation of `BusyOverlay` instances via an attached property. The service supports manual show/hide, automatic lifecycle via `RunAsync`, progress reporting, and cancellation.

## Namespace

```csharp
using Atc.Wpf.Progressing;           // IBusyIndicatorService, BusyInfo, BusyToken
using Atc.Wpf.Components.Progressing; // BusyIndicatorService, BusyIndicatorManager
```

## Usage

### Register a BusyOverlay Region

```xml
<atc:BusyOverlay atc:BusyIndicatorManager.RegionName="MainRegion">
    <ContentControl Content="{Binding CurrentView}" />
</atc:BusyOverlay>
```

### Inject and Use from ViewModel

```csharp
public class MyViewModel : ViewModelBase
{
    private readonly IBusyIndicatorService busyService = new BusyIndicatorService();

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        await busyService.RunAsync(
            async (progress, ct) =>
            {
                progress.Report(BusyInfo.FromMessage("Loading..."));
                await LoadAsync(ct);
            },
            message: "Loading data...",
            regionName: "MainRegion");
    }
}
```

### Manual Show/Hide

```csharp
var token = busyService.Show("Processing...", "MainRegion");
try
{
    await DoWorkAsync();
}
finally
{
    busyService.Hide(token);
}
```

### Progress Reporting

```csharp
await busyService.RunAsync(
    async (progress, ct) =>
    {
        for (var i = 0; i <= 100; i += 10)
        {
            progress.Report(BusyInfo.FromProgress("Importing", i));
            await Task.Delay(200, ct);
        }
    },
    message: "Importing...",
    regionName: "MainRegion");
```

### Cancellable Operation

```csharp
await busyService.RunAsync(
    async (progress, ct) =>
    {
        while (!ct.IsCancellationRequested)
        {
            await Task.Delay(500, ct);
        }
    },
    message: "Working...",
    regionName: "MainRegion",
    allowCancellation: true);
```

## API Reference

### IBusyIndicatorService Methods

| Method | Description |
|--------|-------------|
| `Show(message, regionName, allowCancellation)` | Shows the busy overlay and returns a `BusyToken` |
| `Hide(token)` | Hides the busy overlay for the given token |
| `HideAll(regionName)` | Hides all busy overlays in the specified region |
| `Report(token, info)` | Updates the busy message/progress for a token |
| `RunAsync(operation, message, regionName, allowCancellation)` | Runs an async operation with automatic show/hide |
| `RunAsync<T>(operation, message, regionName, allowCancellation)` | Same as above with a return value |

### BusyInfo Factory Methods

| Method | Description |
|--------|-------------|
| `BusyInfo.FromMessage(message)` | Creates info with just a message |
| `BusyInfo.FromProgress(message, percentage)` | Creates info with message and progress (0-100) |

## Notes

- All UI operations are dispatched to the UI thread automatically
- If a named region is not found, the service falls back to the default (empty name) region
- Progress display format: `"Message 50%"` when percentage is set, `"Message"` otherwise
- Cancellation creates a `CancellationTokenSource` per session, disposed on hide
- Multiple overlays can share the same region name

## Related Controls

- **BusyOverlay** - The UI control driven by this service
- **Overlay** - Simpler content dimming overlay
- **Skeleton** - Placeholder content during loading

## Sample Application

See the BusyIndicatorService sample in the Atc.Wpf.Sample application under **Wpf.Components > Progressing > BusyIndicatorService** for interactive examples.
