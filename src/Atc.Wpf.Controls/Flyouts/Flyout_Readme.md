# 📋 Flyout

A sliding panel overlay control that slides in from an edge of its container.

## 🔍 Overview

`Flyout` provides a lightweight, contextual overlay panel that slides in from a specified edge (Right, Left, Top, or Bottom). It supports light dismiss behavior (click outside or press Escape), customizable animations, and optional overlay backgrounds. The default position is Right, following the Azure Portal design pattern.

## 📊 Flyout vs Dialog - When to Use Which

| Use Case                    | Flyout          | Dialog  |
| --------------------------- | --------------- | ------- |
| Additional input collection | ✅ Yes          | No      |
| Contextual information      | ✅ Yes          | No      |
| Security confirmations      | No              | ✅ Yes  |
| Irreversible actions        | No              | ✅ Yes  |
| In-app purchases            | No              | ✅ Yes  |
| Form wizards / multi-step   | ✅ Yes (nested) | Yes     |
| Quick settings panel        | ✅ Yes          | No      |
| Navigation menu             | ✅ Yes          | No      |

**Key differences:**

- **Flyouts** are non-modal, support light dismiss, and are ideal for contextual tasks
- **Dialogs** are modal, require explicit user action, and are used for critical decisions

## 🏗️ Azure Portal Nesting Pattern

Flyouts support nesting (drill-down) similar to the Azure Portal blade pattern:

```text

+------------------+------------------+------------------+
|                  |                  |                  |
|   Main Content   |   Flyout 1       |   Flyout 2       |
|                  |   (Settings)     |   (Confirmation) |
|                  |                  |                  |
|   [Click Item]-->|   [Save] ------->|   [Yes] [No]     |
|                  |                  |                  |
+------------------+------------------+------------------+
```

Use `FlyoutHost` to manage multiple nested flyouts with the `MaxNestingDepth` property.

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

<!-- Center Position (modal-like with scale animation) -->
<flyouts:Flyout Position="Center" FlyoutWidth="500" FlyoutHeight="400" Header="Centered Dialog">
    <TextBlock Text="Appears centered with scale animation." />
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

### Pin/Unpin Feature

```xml
<!-- Flyout with pin button - users can pin to prevent light dismiss -->
<flyouts:Flyout Header="Pinnable Panel" ShowPinButton="True">
    <TextBlock Text="Click the pin icon to keep this flyout open." />
</flyouts:Flyout>

<!-- Start pinned (light dismiss disabled until unpinned) -->
<flyouts:Flyout Header="Pinned Panel" ShowPinButton="True" IsPinned="True">
    <TextBlock Text="This flyout starts pinned." />
</flyouts:Flyout>
```

### Resizable Flyout

```xml
<!-- User can drag the edge to resize -->
<flyouts:Flyout Header="Resizable" IsResizable="True"
               MinFlyoutWidth="300" MaxFlyoutWidth="800">
    <TextBlock Text="Drag the left edge to resize this panel." />
</flyouts:Flyout>
```

### Custom Animation

```xml
<!-- Custom easing function -->
<flyouts:Flyout Header="Bouncy" AnimationDuration="500">
    <flyouts:Flyout.EasingFunction>
        <BounceEase EasingMode="EaseOut" Bounces="3" Bounciness="2" />
    </flyouts:Flyout.EasingFunction>
    <TextBlock Text="Bounces when opening!" />
</flyouts:Flyout>
```

### Attached Flyout (Button.Flyout pattern)

```xml
<!-- Attach flyout to a button - opens on click -->
<Button Content="Open Settings">
    <flyouts:FlyoutBase.AttachedFlyout>
        <flyouts:Flyout Header="Quick Settings" FlyoutWidth="300">
            <StackPanel Margin="16">
                <CheckBox Content="Option 1" />
                <CheckBox Content="Option 2" />
            </StackPanel>
        </flyouts:Flyout>
    </flyouts:FlyoutBase.AttachedFlyout>
    <flyouts:FlyoutBase.OpenOnClick>True</flyouts:FlyoutBase.OpenOnClick>
</Button>

<!-- Or open/close via binding -->
<Button Content="Toggle">
    <flyouts:FlyoutBase.AttachedFlyout>
        <flyouts:Flyout Header="Bound Flyout" />
    </flyouts:FlyoutBase.AttachedFlyout>
    <flyouts:FlyoutBase.IsOpen>
        <Binding Path="IsFlyoutOpen" Mode="TwoWay" />
    </flyouts:FlyoutBase.IsOpen>
</Button>
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

| Property                | Type               | Default   | Description                                                              |
| ----------------------- | ------------------ | --------- | ------------------------------------------------------------------------ |
| `IsOpen`                | `bool`             | `false`   | Gets or sets whether the flyout is currently open                        |
| `Position`              | `FlyoutPosition`   | `Right`   | Position from which the flyout slides in                                 |
| `FlyoutWidth`           | `double`           | `400`     | Width of the flyout panel (for Left/Right/Center positions)              |
| `FlyoutHeight`          | `double`           | `300`     | Height of the flyout panel (for Top/Bottom/Center positions)             |
| `Header`                | `object?`          | `null`    | Header content displayed at the top                                      |
| `HeaderTemplate`        | `DataTemplate?`    | `null`    | Template for the header content                                          |
| `HeaderBackground`      | `Brush?`           | theme     | Background brush for the header area                                     |
| `HeaderForeground`      | `Brush?`           | theme     | Foreground brush for the header text                                     |
| `HeaderBorderBrush`     | `Brush?`           | theme     | Border brush for the header separator line                               |
| `IsLightDismissEnabled` | `bool`             | `true`    | Whether clicking outside dismisses the flyout                            |
| `ShowOverlay`           | `bool`             | `true`    | Whether to show an overlay background                                    |
| `OverlayOpacity`        | `double`           | `0.5`     | Opacity of the overlay (0.0 - 1.0)                                       |
| `OverlayBrush`          | `Brush?`           | theme     | Brush for the overlay background                                         |
| `ShowCloseButton`       | `bool`             | `true`    | Whether the close button is visible                                      |
| `ShowPinButton`         | `bool`             | `false`   | Whether the pin button is visible                                        |
| `IsPinned`              | `bool`             | `false`   | Whether the flyout is pinned (prevents light dismiss)                    |
| `IsResizable`           | `bool`             | `false`   | Whether the user can drag to resize the flyout                           |
| `MinFlyoutWidth`        | `double`           | `200`     | Minimum width when resizable                                             |
| `MaxFlyoutWidth`        | `double`           | `800`     | Maximum width when resizable                                             |
| `MinFlyoutHeight`       | `double`           | `150`     | Minimum height when resizable                                            |
| `MaxFlyoutHeight`       | `double`           | `600`     | Maximum height when resizable                                            |
| `AnimationDuration`     | `double`           | `300`     | Duration of slide animation in milliseconds                              |
| `EasingFunction`        | `IEasingFunction?` | CubicEase | Custom easing function for animations                                    |
| `CloseOnEscape`         | `bool`             | `true`    | Whether Escape key closes the flyout                                     |
| `CornerRadius`          | `CornerRadius`     | `0`       | Corner radius for the flyout panel                                       |
| `FlyoutBackground`      | `Brush?`           | theme     | Background brush for the flyout panel                                    |
| `FlyoutBorderBrush`     | `Brush?`           | theme     | Border brush for the flyout panel                                        |
| `FlyoutBorderThickness` | `Thickness`        | `1,0,0,0` | Border thickness (varies by position)                                    |
| `Result`                | `object?`          | `null`    | Result value when the flyout closes                                      |
| `IsFocusTrapEnabled`    | `bool`             | `true`    | Whether focus is trapped within the flyout (Tab cycles through elements) |

## 📊 Enumerations

### FlyoutPosition

| Value    | Description                                                 |
| -------- | ----------------------------------------------------------- |
| `Right`  | Slides in from the right edge (default, Azure Portal style) |
| `Left`   | Slides in from the left edge                                |
| `Top`    | Slides in from the top edge                                 |
| `Bottom` | Slides in from the bottom edge                              |
| `Center` | Appears centered with scale animation (modal-like)          |

## 📡 Events

| Event     | Type                                   | Description                                        |
| --------- | -------------------------------------- | -------------------------------------------------- |
| `Opening` | `EventHandler<FlyoutOpeningEventArgs>` | Occurs before the flyout opens (can be cancelled)  |
| `Opened`  | `RoutedEventHandler`                   | Occurs after the flyout has fully opened           |
| `Closing` | `EventHandler<FlyoutClosingEventArgs>` | Occurs before the flyout closes (can be cancelled) |
| `Closed`  | `RoutedEventHandler`                   | Occurs after the flyout has fully closed           |

### FlyoutOpeningEventArgs

| Property | Type   | Description                       |
| -------- | ------ | --------------------------------- |
| `Cancel` | `bool` | Set to true to cancel the opening |

### FlyoutClosingEventArgs

| Property         | Type   | Description                                    |
| ---------------- | ------ | ---------------------------------------------- |
| `Cancel`         | `bool` | Set to true to cancel the closing              |
| `IsLightDismiss` | `bool` | True if closing was triggered by light dismiss |

## 🔧 Methods

| Method                          | Returns | Description                           |
| ------------------------------- | ------- | ------------------------------------- |
| `OpenAsync()`                   | `Task`  | Opens the flyout with animation       |
| `CloseAsync()`                  | `Task`  | Closes the flyout with animation      |
| `CloseWithResultAsync(object?)` | `Task`  | Closes the flyout with a result value |

## ⌨️ Keyboard Navigation

| Key         | Action                                                                                               |
| ----------- | ---------------------------------------------------------------------------------------------------- |
| `Escape`    | Close the flyout (if `CloseOnEscape=true`)                                                           |
| `Tab`       | Navigate forward through focusable elements (trapped within flyout when `IsFocusTrapEnabled=true`)   |
| `Shift+Tab` | Navigate backwards through focusable elements (trapped within flyout when `IsFocusTrapEnabled=true`) |

### Focus Trapping

When `IsFocusTrapEnabled` is true (default), Tab/Shift+Tab navigation is confined to the flyout's content. This prevents users from accidentally tabbing out of the flyout into the underlying window.

```xml
<!-- Focus trapping enabled (default) -->
<flyouts:Flyout Header="Trapped Focus" IsFocusTrapEnabled="True">
    <StackPanel>
        <TextBox Text="First" />
        <TextBox Text="Second" />
        <Button Content="OK" />
    </StackPanel>
</flyouts:Flyout>

<!-- Focus trapping disabled (Tab can escape flyout) -->
<flyouts:Flyout Header="Free Focus" IsFocusTrapEnabled="False">
    <StackPanel>
        <TextBox Text="First" />
        <TextBox Text="Second" />
        <Button Content="OK" />
    </StackPanel>
</flyouts:Flyout>
```

## ♿ Accessibility

### Screen Reader Support

The Flyout control includes an `AutomationPeer` that exposes:

- Control type: `Pane`
- Class name: `Flyout`
- Name: Header content (if available)

Screen readers will announce when flyouts open and close, and users can navigate flyout content using standard keyboard commands.

### Keyboard Shortcuts

| Key         | Action                                        |
| ----------- | --------------------------------------------- |
| `Escape`    | Close the flyout (if `CloseOnEscape=true`)    |
| `Tab`       | Navigate forward through focusable elements   |
| `Shift+Tab` | Navigate backwards through focusable elements |
| `Alt+Left`  | Close flyout (back navigation)                |

## 🔗 NiceWindow Integration

Flyouts integrate with `NiceWindow` using the `PART_FlyoutHost` template part at Z-Index 6:

```text

Z-Index Architecture:
├── 0: Main Content (PART_Content)
├── 3: Inactive Dialogs (PART_NiceInactiveDialogsContainer)
├── 4: Overlay Background (PART_OverlayBox)
├── 5: Active Dialog (PART_NiceActiveDialogContainer)
├── 6: Flyout Host (PART_FlyoutHost) ← Flyouts appear here
└── 7+: Nested Flyouts (stacked by FlyoutHost)
```

When using flyouts in a `NiceWindow`, the window automatically provides the hosting container.

## 🎨 Theming

The Flyout control uses theme resources for consistent styling:

- `AtcApps.Brushes.ThemeBackground` - Default flyout background
- `AtcApps.Brushes.Text` - Default text color
- `AtcApps.Brushes.Gray8` - Header border
- `AtcApps.Brushes.Gray6` - Flyout border
- `AtcApps.Brushes.Gray2` - Close button icon
- `AtcApps.Brushes.IdealForeground` - Overlay brush

### Custom Colors

You can customize flyout colors using the following properties:

```xml
<!-- Custom colored flyout -->
<flyouts:Flyout
    Header="Branded Panel"
    FlyoutWidth="400"
    FlyoutBackground="#1E3A5F"
    FlyoutBorderBrush="#4A90D9"
    FlyoutBorderThickness="2"
    HeaderBackground="#2C5282"
    HeaderForeground="White"
    HeaderBorderBrush="#4A90D9"
    OverlayBrush="#000080"
    OverlayOpacity="0.3">
    <TextBlock Text="Custom styled content" Foreground="White" Margin="16" />
</flyouts:Flyout>
```

### Color Properties Reference

| Property            | Description                              | Default                 |
| ------------------- | ---------------------------------------- | ----------------------- |
| `FlyoutBackground`  | Main panel background                    | Theme background        |
| `FlyoutBorderBrush` | Border around the flyout panel           | Gray6                   |
| `HeaderBackground`  | Header area background                   | Theme background        |
| `HeaderForeground`  | Header text color                        | Theme text              |
| `HeaderBorderBrush` | Separator line under the header          | Gray8                   |
| `OverlayBrush`      | Semi-transparent overlay behind flyout   | Black                   |
| `OverlayOpacity`    | Opacity of the overlay (0.0 - 1.0)       | 0.5                     |

### Themed Flyout Examples

```xml
<!-- Success/Green theme -->
<flyouts:Flyout
    Header="Success"
    FlyoutBackground="#E6F4EA"
    FlyoutBorderBrush="#34A853"
    HeaderBackground="#34A853"
    HeaderForeground="White"
    HeaderBorderBrush="#34A853">
    <TextBlock Text="Operation completed successfully!" Margin="16" />
</flyouts:Flyout>

<!-- Warning/Orange theme -->
<flyouts:Flyout
    Header="Warning"
    FlyoutBackground="#FEF7E0"
    FlyoutBorderBrush="#F9AB00"
    HeaderBackground="#F9AB00"
    HeaderForeground="White"
    HeaderBorderBrush="#F9AB00">
    <TextBlock Text="Please review before continuing." Margin="16" />
</flyouts:Flyout>

<!-- Error/Red theme -->
<flyouts:Flyout
    Header="Error"
    FlyoutBackground="#FCE8E6"
    FlyoutBorderBrush="#EA4335"
    HeaderBackground="#EA4335"
    HeaderForeground="White"
    HeaderBorderBrush="#EA4335">
    <TextBlock Text="An error occurred." Margin="16" />
</flyouts:Flyout>

<!-- Info/Blue theme -->
<flyouts:Flyout
    Header="Information"
    FlyoutBackground="#E8F0FE"
    FlyoutBorderBrush="#4285F4"
    HeaderBackground="#4285F4"
    HeaderForeground="White"
    HeaderBorderBrush="#4285F4">
    <TextBlock Text="Here's some helpful information." Margin="16" />
</flyouts:Flyout>
```

### Using Resource Brushes

For consistent theming across your application, define brushes in resources:

```xml
<Application.Resources>
    <SolidColorBrush x:Key="MyApp.Flyout.Background" Color="#1E3A5F" />
    <SolidColorBrush x:Key="MyApp.Flyout.Border" Color="#4A90D9" />
    <SolidColorBrush x:Key="MyApp.Flyout.HeaderBackground" Color="#2C5282" />
    <SolidColorBrush x:Key="MyApp.Flyout.HeaderForeground" Color="White" />
</Application.Resources>

<!-- Then use in flyouts -->
<flyouts:Flyout
    Header="Branded Panel"
    FlyoutBackground="{StaticResource MyApp.Flyout.Background}"
    FlyoutBorderBrush="{StaticResource MyApp.Flyout.Border}"
    HeaderBackground="{StaticResource MyApp.Flyout.HeaderBackground}"
    HeaderForeground="{StaticResource MyApp.Flyout.HeaderForeground}">
    <TextBlock Text="Consistently themed content" />
</flyouts:Flyout>
```

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

| Property          | Type   | Default | Description                                               |
| ----------------- | ------ | ------- | --------------------------------------------------------- |
| `MaxNestingDepth` | `int`  | `5`     | Maximum number of flyouts that can be open simultaneously |
| `OpenFlyoutCount` | `int`  | `0`     | Number of currently open flyouts (read-only)              |
| `IsAnyFlyoutOpen` | `bool` | `false` | Whether any flyout is currently open (read-only)          |

### FlyoutHost Methods

| Method                              | Returns | Description                                     |
| ----------------------------------- | ------- | ----------------------------------------------- |
| `OpenFlyout(Flyout)`                | `bool`  | Opens a flyout if nesting depth allows          |
| `CloseTopFlyout()`                  | `bool`  | Closes the topmost flyout                       |
| `CloseAllFlyouts()`                 | `void`  | Closes all open flyouts                         |
| `CloseFlyoutAndDescendants(Flyout)` | `void`  | Closes a flyout and all flyouts opened after it |

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
FlyoutOptions.Center     // Centered modal-like with scale animation
```

### Extension Methods (Fluent API)

```csharp
// Configure service with fluent API
flyoutService
    .WithView<SettingsViewModel, SettingsView>()
    .WithView<UserDetailsViewModel, UserDetailsView>()
    .WithViewFactory<DynamicViewModel>(() => new DynamicView());

// Convenience methods
await flyoutService.ShowWideAsync("Wide Panel", viewModel);
await flyoutService.ShowNarrowAsync("Narrow Panel", viewModel);
await flyoutService.ShowModalAsync("Modal Panel", viewModel);
await flyoutService.ShowCenteredAsync("Centered Dialog", viewModel);
await flyoutService.ShowFromLeftAsync("Left Panel", viewModel);
```

### Form Flyout with Validation

```csharp
// Show a form flyout that returns typed result
var result = await flyoutService.ShowFormAsync(
    "Edit User",
    userViewModel,
    vm => vm.User,  // Extract model from ViewModel
    FlyoutOptions.Wide);

if (result.IsSuccess)
{
    // Form was submitted and validation passed
    await SaveUser(result.Model);
}
else if (result.IsCancelled)
{
    // User cancelled (clicked outside or close button)
}
else if (!result.IsValid)
{
    // Validation failed
    foreach (var error in result.ValidationErrors)
    {
        Console.WriteLine(error);
    }
}
```

### IFlyoutService Interface

| Method                                       | Description                      |
| -------------------------------------------- | -------------------------------- |
| `ShowAsync(header, content, options)`        | Show flyout with content         |
| `ShowAsync<TViewModel>(header, vm, options)` | Show flyout with ViewModel       |
| `ShowAsync<TViewModel, TResult>(...)`        | Show flyout and get typed result |
| `CloseTopFlyout()`                           | Close the topmost flyout         |
| `CloseAllFlyouts()`                          | Close all open flyouts           |
| `CloseTopFlyoutWithResult(result)`           | Close with a result value        |
| `RegisterView<TViewModel, TView>()`          | Register View-ViewModel mapping  |
| `RegisterViewFactory<TViewModel>(factory)`   | Register view factory            |

## 📎 FlyoutBase (Attached Properties)

`FlyoutBase` provides attached properties for associating flyouts with UI elements.

### Attached Properties

| Property         | Type     | Description                                               |
| ---------------- | -------- | --------------------------------------------------------- |
| `AttachedFlyout` | `Flyout` | The flyout associated with the element                    |
| `IsOpen`         | `bool`   | Gets/sets whether the attached flyout is open             |
| `OpenOnClick`    | `bool`   | When true, clicking the element opens the attached flyout |

### Static Methods

| Method                        | Returns | Description                               |
| ----------------------------- | ------- | ----------------------------------------- |
| `ShowAttachedFlyout(element)` | `void`  | Opens the flyout attached to the element  |
| `HideAttachedFlyout(element)` | `void`  | Closes the flyout attached to the element |

## 🎨 FlyoutPresenter

`FlyoutPresenter` is a content container that provides a standardized layout for flyout content with header, content, and footer sections.

### Basic Usage

```xml
<flyouts:Flyout Header="My Flyout" ShowCloseButton="False">
    <flyouts:FlyoutPresenter Header="Section Title" Footer="{Binding FooterButtons}">
        <StackPanel>
            <TextBlock Text="Main content goes here." />
            <TextBox Text="{Binding SomeValue}" />
        </StackPanel>
    </flyouts:FlyoutPresenter>
</flyouts:Flyout>
```

### Properties

| Property                        | Type                  | Default       | Description                                     |
| ------------------------------- | --------------------- | ------------- | ----------------------------------------------- |
| `Header`                        | `object?`             | `null`        | Header content                                  |
| `HeaderTemplate`                | `DataTemplate?`       | `null`        | Template for header content                     |
| `HeaderPadding`                 | `Thickness`           | `16,12,16,12` | Padding around the header                       |
| `HeaderBackground`              | `Brush?`              | theme         | Background brush for header area                |
| `HeaderForeground`              | `Brush?`              | theme         | Foreground brush for header text                |
| `Footer`                        | `object?`             | `null`        | Footer content (typically buttons)              |
| `FooterTemplate`                | `DataTemplate?`       | `null`        | Template for footer content                     |
| `FooterPadding`                 | `Thickness`           | `16,12,16,12` | Padding around the footer                       |
| `FooterBackground`              | `Brush?`              | theme         | Background brush for footer area                |
| `ContentPadding`                | `Thickness`           | `16`          | Padding around the main content                 |
| `ShowHeader`                    | `bool`                | `true`        | Whether to show the header section              |
| `ShowFooter`                    | `bool`                | `true`        | Whether to show the footer section              |
| `ShowHeaderSeparator`           | `bool`                | `true`        | Whether to show line between header and content |
| `ShowFooterSeparator`           | `bool`                | `true`        | Whether to show line between content and footer |
| `SeparatorBrush`                | `Brush?`              | theme         | Brush for separator lines                       |
| `SeparatorThickness`            | `double`              | `1.0`         | Thickness of separator lines                    |
| `IsContentScrollable`           | `bool`                | `true`        | Whether content area scrolls                    |
| `VerticalScrollBarVisibility`   | `ScrollBarVisibility` | `Auto`        | Vertical scrollbar visibility                   |
| `HorizontalScrollBarVisibility` | `ScrollBarVisibility` | `Disabled`    | Horizontal scrollbar visibility                 |
| `CornerRadius`                  | `CornerRadius`        | `0`           | Corner radius for the presenter                 |

### Example with Custom Footer

```xml
<flyouts:FlyoutPresenter Header="Edit Settings">
    <flyouts:FlyoutPresenter.Footer>
        <StackPanel Orientation="Horizontal" HorizontalAlignment="Right">
            <Button Content="Cancel" Margin="0,0,8,0" Click="Cancel_Click" />
            <Button Content="Save" Style="{StaticResource AccentButton}" Click="Save_Click" />
        </StackPanel>
    </flyouts:FlyoutPresenter.Footer>

    <StackPanel>
        <TextBlock Text="Name" />
        <TextBox Text="{Binding Name}" Margin="0,4,0,16" />
        <TextBlock Text="Description" />
        <TextBox Text="{Binding Description}" Margin="0,4,0,0" />
    </StackPanel>
</flyouts:FlyoutPresenter>
```

## 🔗 Related Controls

- **Card** - Container for grouping related content
- **GroupBoxExpander** - Collapsible grouping control

## 🎮 Sample Application

See the Flyout sample in the Atc.Wpf.Sample application under **Wpf.Controls > Flyouts > Flyout** for interactive examples including:

- Basic flyout positions (Right, Left, Top, Bottom)
- Configuration options (overlay, light dismiss, close button)
- Form content
- Nested flyouts (Azure Portal style drill-down)
