# Navigation Framework

The **Atc.Wpf** library provides a ViewModel-based navigation framework with history support, navigation guards, and parameter passing.

## Features

| Component | Namespace | Description |
|---|---|---|
| `INavigationService` | Atc.Wpf.Navigation | Service interface for ViewModel-based navigation with back/forward history |
| `NavigationService` | Atc.Wpf.Navigation | Default implementation using a factory delegate for ViewModel creation |
| `INavigationAware` | Atc.Wpf.Navigation | Interface for ViewModels that receive `OnNavigatedTo` / `OnNavigatedFrom` callbacks |
| `INavigationGuard` | Atc.Wpf.Navigation | Interface for ViewModels that can block navigation (e.g. unsaved changes) |
| `NavigationParameters` | Atc.Wpf.Navigation | Dictionary-based parameter bag with typed `GetValue<T>` and fluent `WithParameter` |
| `NavigationHistory` | Atc.Wpf.Navigation | Back/forward stack management |

## Basic Navigation

```csharp
// Create the service with a ViewModel factory (typically from your DI container)
INavigationService navigationService = new NavigationService(
    type => serviceProvider.GetRequiredService(type));

// Navigate to a ViewModel
navigationService.NavigateTo<HomeViewModel>();

// Navigate with parameters
navigationService.NavigateTo<DetailsViewModel>(
    new NavigationParameters()
        .WithParameter("ItemId", 42)
        .WithParameter("ItemName", "Widget"));

// Back / Forward
navigationService.GoBack();
navigationService.GoForward();
```

## Navigation Awareness

Implement `INavigationAware` to respond to navigation lifecycle events:

```csharp
public class DetailsViewModel : ViewModelBase, INavigationAware
{
    public void OnNavigatedTo(NavigationParameters? parameters)
    {
        var id = parameters?.GetValue<int>("ItemId");
        // Load data...
    }

    public void OnNavigatedFrom()
    {
        // Clean up...
    }
}
```

## Navigation Guards

Implement `INavigationGuard` to conditionally block navigation away from a ViewModel:

```csharp
public class SettingsViewModel : ViewModelBase, INavigationGuard
{
    public bool HasUnsavedChanges { get; set; }

    public bool CanNavigateAway() => !HasUnsavedChanges;

    public Task<bool> CanNavigateAwayAsync(CancellationToken ct)
        => Task.FromResult(CanNavigateAway());
}
```

When `CanNavigateAway()` returns `false`, calls to `NavigateTo`, `GoBack`, and `GoForward` return `false` and the current ViewModel remains active.

## Responding to Navigation Events

Subscribe to the `Navigated` event to update the UI when the current ViewModel changes:

```csharp
navigationService.Navigated += (sender, e) =>
{
    CurrentView = CreateViewForViewModel(e.CurrentViewModel);
};
```

## Sample Application

See **Wpf > Navigation > NavigationService** in the sample application for a working demo that includes parameter passing, navigation guards with unsaved-change detection, and back/forward history.
