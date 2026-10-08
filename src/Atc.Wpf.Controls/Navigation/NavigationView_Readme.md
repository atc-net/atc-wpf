# 🧭 NavigationView

An application shell with a collapsible navigation pane, a back button and a content area, wired to `INavigationService`.

## 🔍 Overview

`NavigationView` shows `MenuItems` at the top of the pane and `FooterMenuItems` (for example Settings) at the bottom. The hamburger button switches between the open pane (icons and labels) and the compact pane (icons only).

Set `NavigationService` and the view drives and follows it:

- Invoking an item with a `TargetViewModelType` navigates to that ViewModel, passing the item's `NavigationParameters`
- The service's current ViewModel is shown as `Content`; map each ViewModel to a view with a `DataTemplate`
- Back, forward and navigation from code select the item that targets the new ViewModel (or clear the selection when no item does)
- A navigation blocked by an `INavigationGuard` keeps the previous selection, without raising `SelectionChanged`
- The back button is enabled while the service can go back

Without a service, `NavigationView` is a selection control: handle `SelectionChanged` or `ItemInvoked` and set `Content` yourself.

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Navigation;
using Atc.Wpf.Navigation; // INavigationService, NavigationService
```

## 🚀 Usage

### Shell bound to a navigation service

```xml
<UserControl.Resources>
    <DataTemplate DataType="{x:Type vm:HomeViewModel}">
        <views:HomeView />
    </DataTemplate>
    <DataTemplate DataType="{x:Type vm:SettingsViewModel}">
        <views:SettingsView />
    </DataTemplate>
</UserControl.Resources>

<atc:NavigationView NavigationService="{Binding NavigationService}" PaneTitle="My application">
    <atc:NavigationViewItem Content="Home" TargetViewModelType="{x:Type vm:HomeViewModel}">
        <atc:NavigationViewItem.Icon>
            <TextBlock FontFamily="Segoe MDL2 Assets" Text="&#xE80F;" />
        </atc:NavigationViewItem.Icon>
    </atc:NavigationViewItem>

    <atc:NavigationView.FooterMenuItems>
        <atc:NavigationViewItem Content="Settings" TargetViewModelType="{x:Type vm:SettingsViewModel}" />
    </atc:NavigationView.FooterMenuItems>
</atc:NavigationView>
```

```csharp
public MainWindowViewModel()
{
    NavigationService = new NavigationService(type => serviceProvider.GetRequiredService(type));
    NavigationService.NavigateTo<HomeViewModel>();
}

public INavigationService NavigationService { get; }
```

### Item that runs an action instead of navigating

```xml
<atc:NavigationViewItem
    Command="{Binding ShowAboutCommand}"
    Content="About"
    SelectsOnInvoked="False" />
```

## ⚙️ Properties

### NavigationView

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `MenuItems` | `ObservableCollection<NavigationViewItem>` | empty | Items at the top of the pane (content property) |
| `FooterMenuItems` | `ObservableCollection<NavigationViewItem>` | empty | Items at the bottom of the pane |
| `SelectedItem` | `NavigationViewItem?` | `null` | The selected item from either collection (two-way) |
| `NavigationService` | `INavigationService?` | `null` | The service the view drives and follows |
| `Content` | `object?` | `null` | The content area; with a service, the current ViewModel |
| `ContentTemplate` | `DataTemplate?` | `null` | Template for `Content` |
| `ContentTemplateSelector` | `DataTemplateSelector?` | `null` | Template selector for `Content` |
| `PaneTitle` | `object?` | `null` | Title next to the hamburger button, hidden when the pane is compact |
| `IsPaneOpen` | `bool` | `true` | Open (icons and labels) or compact (icons only) pane (two-way) |
| `OpenPaneLength` | `double` | `240` | Pane width when open |
| `CompactPaneLength` | `double` | `48` | Pane width when compact; also sizes the icon column |
| `IsBackButtonVisible` | `bool` | `true` | Shows the back button |
| `IsBackEnabled` | `bool` | `false` | Whether the service can go back; set by the view |
| `PaneBackground` | `Brush?` | theme `Gray10` | Pane background |

### NavigationViewItem

`NavigationViewItem` is a `ButtonBase`, so `Command`, `CommandParameter` and `Click` are available too.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Content` | `object?` | `null` | The label |
| `Icon` | `object?` | `null` | Shown in the icon column, also when the pane is compact |
| `TargetViewModelType` | `Type?` | `null` | ViewModel to navigate to when invoked |
| `NavigationParameters` | `NavigationParameters?` | `null` | Parameters passed with that navigation |
| `SelectsOnInvoked` | `bool` | `true` | Set to `false` for items that only run an action |
| `IsSelected` | `bool` | `false` | Set by the view |

## 🔔 Events and methods

| Member | Description |
|--------|-------------|
| `SelectionChanged` | `SelectedItem` changed (`OldItem`, `NewItem`) |
| `ItemInvoked` | An item was invoked, also when it does not select or navigate; raised before the selection changes |
| `GoBack()` | Navigates back through the service; returns `false` when it cannot |

## 📝 Notes

- Two items may target the same ViewModel type; navigation from code then selects the first one, unless the selected item already targets it
- The view subscribes to the service's `Navigated` event while `NavigationService` is set; clear it if the service outlives the view
- Template parts: `PART_MenuItemsHost` and `PART_FooterMenuItemsHost` (panels), `PART_BackButton` (`ButtonBase`), plus `PART_Pane` and `PART_PaneToggleButton` in the default template

## 🔗 Related Controls

- **INavigationService / NavigationService** - ViewModel navigation with history and guards (`docs/Navigation/@Readme.md`)
- **Breadcrumb** - Shows a navigation path
- **Segmented** - Switches between a few views inline

## 🎮 Sample Application

See **Wpf.Controls > Navigation > NavigationView** in the sample application.