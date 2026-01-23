# 📋 Flyout

A sliding panel overlay control that slides in from an edge of its container.

## 🔍 Overview

`Flyout` provides a lightweight, contextual overlay panel that slides in from a specified edge (Right, Left, Top, or Bottom). It supports light dismiss behavior (click outside or press Escape), customizable animations, and optional overlay backgrounds. The default position is Right, following the Azure Portal design pattern.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Flyouts;
```

## 🚀 Usage

### Basic Flyout

```xml
<Grid>
    <!-- Main Content -->
    <Button x:Name="OpenButton" Content="Open Flyout" Click="OpenButton_Click" />

    <!-- Flyout (place at Grid root level) -->
    <flyouts:Flyout x:Name="MyFlyout" Header="Settings" FlyoutWidth="400">
        <StackPanel Margin="16">
            <TextBlock Text="This is a flyout panel." />
            <CheckBox Content="Enable notifications" />
            <CheckBox Content="Dark mode" />
        </StackPanel>
    </flyouts:Flyout>
</Grid>
```

```csharp
private void OpenButton_Click(object sender, RoutedEventArgs e)
{
    MyFlyout.IsOpen = true;
}
```

### Position Variants

```xml
<!-- Default: Right (Azure Portal style) -->
<flyouts:Flyout Position="Right" FlyoutWidth="400" Header="Right Panel">
    <TextBlock Text="Slides in from the right." />
</flyouts:Flyout>

<!-- Left Position -->
<flyouts:Flyout Position="Left" FlyoutWidth="350" Header="Left Panel">
    <TextBlock Text="Slides in from the left." />
</flyouts:Flyout>

<!-- Top Position -->
<flyouts:Flyout Position="Top" FlyoutHeight="200" Header="Top Panel">
    <TextBlock Text="Slides in from the top." />
</flyouts:Flyout>

<!-- Bottom Position -->
<flyouts:Flyout Position="Bottom" FlyoutHeight="250" Header="Bottom Panel">
    <TextBlock Text="Slides in from the bottom." />
</flyouts:Flyout>
```

### Configuring Light Dismiss

```xml
<!-- Light dismiss enabled (default) - click outside or press Escape to close -->
<flyouts:Flyout IsLightDismissEnabled="True" CloseOnEscape="True" Header="Dismissable">
    <TextBlock Text="Click outside or press Escape to close." />
</flyouts:Flyout>

<!-- Light dismiss disabled - must use close button or code -->
<flyouts:Flyout IsLightDismissEnabled="False" ShowCloseButton="True" Header="Persistent">
    <TextBlock Text="Cannot dismiss by clicking outside." />
</flyouts:Flyout>
```

### Overlay Configuration

```xml
<!-- With overlay (default) -->
<flyouts:Flyout ShowOverlay="True" OverlayOpacity="0.5" Header="With Overlay">
    <TextBlock Text="Background is dimmed." />
</flyouts:Flyout>

<!-- Without overlay -->
<flyouts:Flyout ShowOverlay="False" Header="No Overlay">
    <TextBlock Text="No background dimming." />
</flyouts:Flyout>
```

### Form Flyout Example

```xml
<flyouts:Flyout x:Name="SettingsFlyout" Header="Settings" FlyoutWidth="450">
    <Grid Margin="16">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto" />
            <RowDefinition Height="Auto" />
            <RowDefinition Height="Auto" />
            <RowDefinition Height="*" />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>

        <StackPanel Grid.Row="0" Margin="0,0,0,12">
            <TextBlock Text="Display Name" Margin="0,0,0,4" />
            <TextBox Text="{Binding DisplayName}" />
        </StackPanel>

        <StackPanel Grid.Row="1" Margin="0,0,0,12">
            <TextBlock Text="Email" Margin="0,0,0,4" />
            <TextBox Text="{Binding Email}" />
        </StackPanel>

        <StackPanel Grid.Row="2" Margin="0,0,0,12">
            <CheckBox Content="Enable notifications" IsChecked="{Binding EnableNotifications}" />
            <CheckBox Content="Auto-save" IsChecked="{Binding AutoSave}" Margin="0,8,0,0" />
        </StackPanel>

        <StackPanel Grid.Row="4" Orientation="Horizontal" HorizontalAlignment="Right">
            <Button Content="Cancel" Width="80" Margin="0,0,8,0" Click="Cancel_Click" />
            <Button Content="Save" Width="80" Click="Save_Click" />
        </StackPanel>
    </Grid>
</flyouts:Flyout>
```

### Using Async Methods

```csharp
// Open the flyout
await MyFlyout.OpenAsync();

// Close the flyout
await MyFlyout.CloseAsync();

// Close with a result value
await MyFlyout.CloseWithResultAsync(selectedItem);
```

### Handling Events

```csharp
MyFlyout.Opening += (s, e) =>
{
    // Cancel opening if needed
    if (someCondition)
    {
        e.Cancel = true;
    }
};

MyFlyout.Opened += (s, e) =>
{
    // Flyout is now fully visible
};

MyFlyout.Closing += (s, e) =>
{
    // Check if it's a light dismiss
    if (e.IsLightDismiss)
    {
        // Optionally cancel light dismiss
        e.Cancel = hasUnsavedChanges;
    }
};

MyFlyout.Closed += (s, e) =>
{
    // Flyout is now hidden
    var result = MyFlyout.Result;
};
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IsOpen` | `bool` | `false` | Gets or sets whether the flyout is currently open |
| `Position` | `FlyoutPosition` | `Right` | Position from which the flyout slides in |
| `FlyoutWidth` | `double` | `400` | Width of the flyout panel (for Left/Right positions) |
| `FlyoutHeight` | `double` | `300` | Height of the flyout panel (for Top/Bottom positions) |
| `Header` | `object?` | `null` | Header content displayed at the top |
| `HeaderTemplate` | `DataTemplate?` | `null` | Template for the header content |
| `HeaderBackground` | `Brush?` | theme | Background brush for the header area |
| `HeaderForeground` | `Brush?` | theme | Foreground brush for the header text |
| `IsLightDismissEnabled` | `bool` | `true` | Whether clicking outside dismisses the flyout |
| `ShowOverlay` | `bool` | `true` | Whether to show an overlay background |
| `OverlayOpacity` | `double` | `0.5` | Opacity of the overlay (0.0 - 1.0) |
| `OverlayBrush` | `Brush?` | theme | Brush for the overlay background |
| `ShowCloseButton` | `bool` | `true` | Whether the close button is visible |
| `AnimationDuration` | `double` | `300` | Duration of slide animation in milliseconds |
| `CloseOnEscape` | `bool` | `true` | Whether Escape key closes the flyout |
| `CornerRadius` | `CornerRadius` | `0` | Corner radius for the flyout panel |
| `FlyoutBackground` | `Brush?` | theme | Background brush for the flyout panel |
| `FlyoutBorderBrush` | `Brush?` | theme | Border brush for the flyout panel |
| `FlyoutBorderThickness` | `Thickness` | `1,0,0,0` | Border thickness (varies by position) |
| `Result` | `object?` | `null` | Result value when the flyout closes |

## 📊 Enumerations

### FlyoutPosition

| Value | Description |
|-------|-------------|
| `Right` | Slides in from the right edge (default, Azure Portal style) |
| `Left` | Slides in from the left edge |
| `Top` | Slides in from the top edge |
| `Bottom` | Slides in from the bottom edge |

## 📡 Events

| Event | Type | Description |
|-------|------|-------------|
| `Opening` | `EventHandler<FlyoutOpeningEventArgs>` | Occurs before the flyout opens (can be cancelled) |
| `Opened` | `RoutedEventHandler` | Occurs after the flyout has fully opened |
| `Closing` | `EventHandler<FlyoutClosingEventArgs>` | Occurs before the flyout closes (can be cancelled) |
| `Closed` | `RoutedEventHandler` | Occurs after the flyout has fully closed |

### FlyoutOpeningEventArgs

| Property | Type | Description |
|----------|------|-------------|
| `Cancel` | `bool` | Set to true to cancel the opening |

### FlyoutClosingEventArgs

| Property | Type | Description |
|----------|------|-------------|
| `Cancel` | `bool` | Set to true to cancel the closing |
| `IsLightDismiss` | `bool` | True if closing was triggered by light dismiss |

## 🔧 Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `OpenAsync()` | `Task` | Opens the flyout with animation |
| `CloseAsync()` | `Task` | Closes the flyout with animation |
| `CloseWithResultAsync(object?)` | `Task` | Closes the flyout with a result value |

## ⌨️ Keyboard Navigation

| Key | Action |
|-----|--------|
| `Escape` | Close the flyout (if `CloseOnEscape=true`) |
| `Tab` | Navigate between focusable elements |
| `Shift+Tab` | Navigate backwards |

## 🎨 Theming

The Flyout control uses theme resources for consistent styling:

- `AtcApps.Brushes.ThemeBackground` - Default flyout background
- `AtcApps.Brushes.Text` - Default text color
- `AtcApps.Brushes.Gray8` - Header border
- `AtcApps.Brushes.Gray6` - Flyout border
- `AtcApps.Brushes.Gray2` - Close button icon
- `AtcApps.Brushes.IdealForeground` - Overlay brush

## 📝 Notes

- Place the Flyout at the root level of a Grid to ensure it overlays other content
- The Flyout stores and restores focus when opening/closing
- Animations use CubicEase for smooth transitions
- The header area is hidden when `Header` is null
- Border thickness automatically adjusts based on position
- Drop shadow direction changes based on slide-in position

## 🗃️ FlyoutHost

`FlyoutHost` is a container that manages multiple flyouts with support for nesting (like Azure Portal drill-down experiences).

### Basic Usage

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
                <Button Content="View Metrics" Click="OpenMetrics_Click" />
            </StackPanel>
        </flyouts:Flyout>

        <flyouts:Flyout x:Name="MetricsFlyout" Header="Metrics" FlyoutWidth="400">
            <StackPanel Margin="16">
                <TextBlock Text="CPU: 23%" />
                <TextBlock Text="Memory: 4.2 GB / 8 GB" />
            </StackPanel>
        </flyouts:Flyout>
    </flyouts:FlyoutHost>
</Grid>
```

### FlyoutHost Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `MaxNestingDepth` | `int` | `5` | Maximum number of flyouts that can be open simultaneously |
| `OpenFlyoutCount` | `int` | `0` | Number of currently open flyouts (read-only) |
| `IsAnyFlyoutOpen` | `bool` | `false` | Whether any flyout is currently open (read-only) |

### FlyoutHost Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `OpenFlyout(Flyout)` | `bool` | Opens a flyout if nesting depth allows |
| `CloseTopFlyout()` | `bool` | Closes the topmost flyout |
| `CloseAllFlyouts()` | `void` | Closes all open flyouts |
| `CloseFlyoutAndDescendants(Flyout)` | `void` | Closes a flyout and all flyouts opened after it |

## 🎯 IFlyoutService (MVVM)

`IFlyoutService` provides an MVVM-friendly way to show flyouts from ViewModels.

### Setup

```csharp
// In your application startup or DI registration
var flyoutService = new FlyoutService();

// Option 1: Set host panel (typically window's root Grid)
flyoutService.SetHostPanel(myRootGrid);

// Option 2: Use a FlyoutHost
flyoutService.SetFlyoutHost(myFlyoutHost);

// Register View-ViewModel mappings
flyoutService.RegisterView<SettingsViewModel, SettingsView>();
flyoutService.RegisterView<UserDetailsViewModel, UserDetailsView>();
```

### Usage in ViewModel

```csharp
public class MainViewModel
{
    private readonly IFlyoutService flyoutService;

    public MainViewModel(IFlyoutService flyoutService)
    {
        this.flyoutService = flyoutService;
    }

    public async Task ShowSettingsAsync()
    {
        var viewModel = new SettingsViewModel();
        var result = await flyoutService.ShowAsync("Settings", viewModel);

        if (result is true)
        {
            // User saved settings
        }
    }

    public async Task ShowUserDetailsAsync(User user)
    {
        var viewModel = new UserDetailsViewModel(user);
        var result = await flyoutService.ShowAsync<UserDetailsViewModel, User>(
            "User Details",
            viewModel,
            FlyoutOptions.Wide);

        if (result is not null)
        {
            // User was updated
            UpdateUser(result);
        }
    }
}
```

### FlyoutViewModelBase

Inherit from `FlyoutViewModelBase` for built-in close functionality:

```csharp
public class SettingsViewModel : FlyoutViewModelBase
{
    public SettingsViewModel(IFlyoutService flyoutService)
        : base(flyoutService)
    {
    }

    public ICommand SaveCommand => new RelayCommand(() =>
    {
        // Save settings
        CloseWithResult(true);
    });

    public ICommand CancelCommand => new RelayCommand(() =>
    {
        Close();
    });
}
```

### FlyoutOptions Presets

```csharp
FlyoutOptions.Default    // Right, 400px width
FlyoutOptions.Wide       // Right, 600px width
FlyoutOptions.Narrow     // Right, 300px width
FlyoutOptions.Left       // Left position
FlyoutOptions.Top        // Top position
FlyoutOptions.Bottom     // Bottom position
FlyoutOptions.Modal      // No light dismiss
```

### IFlyoutService Interface

| Method | Description |
|--------|-------------|
| `ShowAsync(header, content, options)` | Show flyout with content |
| `ShowAsync<TViewModel>(header, vm, options)` | Show flyout with ViewModel |
| `ShowAsync<TViewModel, TResult>(...)` | Show flyout and get typed result |
| `CloseTopFlyout()` | Close the topmost flyout |
| `CloseAllFlyouts()` | Close all open flyouts |
| `CloseTopFlyoutWithResult(result)` | Close with a result value |
| `RegisterView<TViewModel, TView>()` | Register View-ViewModel mapping |
| `RegisterViewFactory<TViewModel>(factory)` | Register view factory |

## 🔗 Related Controls

- **Card** - Container for grouping related content
- **GroupBoxExpander** - Collapsible grouping control

## 🎮 Sample Application

See the Flyout sample in the Atc.Wpf.Sample application under **Wpf.Controls > Flyouts > Flyout** for interactive examples including:
- Basic flyout positions (Right, Left, Top, Bottom)
- Configuration options (overlay, light dismiss, close button)
- Form content
- Nested flyouts (Azure Portal style drill-down)
