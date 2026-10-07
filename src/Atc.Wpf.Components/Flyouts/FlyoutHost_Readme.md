# 🗃️ FlyoutHost

A container control that manages multiple flyouts with support for nested, drill-down experiences.

## 🔍 Overview

`FlyoutHost` is an `ItemsControl` whose items are `Flyout` controls. It keeps a stack of the currently open flyouts, enforces a maximum nesting depth, and exposes the open count so the UI can react (for example to disable background content). It enables Azure Portal-style "blade" navigation where each flyout can open another on top of it. Place it at the root level of a window or page so the flyouts overlay the rest of the content.

## 📍 Namespace

```csharp
using Atc.Wpf.Components.Flyouts;
```

## 🚀 Usage

### Basic Example

```xml
<Grid>
    <!-- Main content -->
    <Button Content="Open Resource Group" Click="OpenResourceGroup_Click" />

    <!-- FlyoutHost manages nested flyouts -->
    <flyouts:FlyoutHost x:Name="FlyoutsHost" MaxNestingDepth="5">
        <flyouts:Flyout x:Name="ResourceGroupFlyout" Header="Resource Group" FlyoutWidth="500">
            <StackPanel Margin="16">
                <TextBlock Text="Click a resource to see details" />
                <Button Content="Virtual Machine" Click="OpenVmDetails_Click" />
            </StackPanel>
        </flyouts:Flyout>

        <flyouts:Flyout x:Name="VmDetailsFlyout" Header="VM Details" FlyoutWidth="450">
            <StackPanel Margin="16">
                <TextBlock Text="Virtual Machine Details" />
            </StackPanel>
        </flyouts:Flyout>
    </flyouts:FlyoutHost>
</Grid>
```

### Opening and Closing from Code

```csharp
private void OpenResourceGroup_Click(object sender, RoutedEventArgs e)
    => FlyoutsHost.OpenFlyout(ResourceGroupFlyout);

private void OpenVmDetails_Click(object sender, RoutedEventArgs e)
{
    // Returns false when MaxNestingDepth is reached or the flyout is already open
    if (!FlyoutsHost.OpenFlyout(VmDetailsFlyout))
    {
        // Handle nesting limit
    }
}

// Close only the topmost flyout
FlyoutsHost.CloseTopFlyout();

// Close a flyout and every flyout opened after it
FlyoutsHost.CloseFlyoutAndDescendants(ResourceGroupFlyout);
```

### Using with IFlyoutService

```csharp
var flyoutHost = new FlyoutHost { MaxNestingDepth = 5 };
var flyoutService = new FlyoutService(flyoutHost);

// or on an existing service
flyoutService.SetFlyoutHost(flyoutHost);
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `MaxNestingDepth` | `int` | `5` | Maximum number of flyouts that can be open at the same time |
| `OpenFlyoutCount` | `int` | `0` | Number of currently open flyouts (maintained by the host) |
| `IsAnyFlyoutOpen` | `bool` | `false` | Whether any flyout is currently open (maintained by the host) |
| `OpenFlyouts` | `IReadOnlyCollection<Flyout>` | empty | The stack of currently open flyouts (read-only, CLR property) |
| `TopFlyout` | `Flyout?` | `null` | The topmost open flyout, if any (read-only, CLR property) |

## 🔧 Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `OpenFlyout(Flyout)` | `bool` | Opens the flyout; returns `false` if `MaxNestingDepth` is reached or it is already open |
| `CloseTopFlyout()` | `bool` | Closes the topmost flyout; returns `false` if none are open |
| `CloseAllFlyouts()` | `void` | Closes all open flyouts |
| `CloseFlyoutAndDescendants(Flyout)` | `void` | Closes the given flyout and all flyouts opened after it |

## 📝 Notes

- `Items` is the XAML content property; only `Flyout` items are treated as their own containers (other items are wrapped in a new `Flyout`)
- Flyouts declared as items are tracked automatically: opening one directly (`IsOpen="True"`) pushes it on the stack, and an attempt beyond `MaxNestingDepth` is cancelled via the flyout's `Opening` event
- A flyout is removed from the stack when its `Closed` event is raised
- `OpenFlyoutCount` and `IsAnyFlyoutOpen` are dependency properties updated by the host; treat them as read-only and bind to them (e.g. `OneWay`)
- When the host is unloaded, the open-flyout stack is cleared
- The default style uses a `Grid` as items panel so all flyouts overlay each other, and cycles keyboard tab navigation inside the host
- `NiceWindow` has its own `PART_FlyoutHost` template part (a `Grid`) where window-level flyouts are placed; a `FlyoutHost` can be added as window content for nested scenarios

## 🔗 Related Controls

- **Flyout** - The slide-in panel managed by the host (see `Flyout_Readme.md`)
- **FlyoutPresenter** - Standardized header/content/footer layout for flyout content
- **IFlyoutService / FlyoutService** - MVVM-friendly way to show flyouts from ViewModels
- **NiceWindow** - Window that integrates a flyout host

## 🎮 Sample Application

See the NiceWindow with Flyout sample in the Atc.Wpf.Sample application under **Wpf.Theming > Window > NiceWindow with Flyout** ("Open NiceWindow with FlyoutHost") for interactive examples.
