# 🏗️ DockPanelPro

An enhanced dock panel with resizable splitters, collapsible regions, tabbed pane groups, floating tool windows and layout persistence. Perfect for building IDE-style interfaces with tool windows.

## 🔍 Overview

`DockPanelPro` extends the standard WPF DockPanel with professional features commonly found in development environments:

- 📏 **Resizable splitters** between docked regions
- 📌 **Collapsible regions** with pin/unpin functionality
- 🗂️ **Tabbed pane groups** - a region can hold several `DockPane`s shown as tabs, with close buttons
- 🪟 **Floating tool windows** - a pane can float into its own themed window and dock back to where it was
- 💾 **Layout persistence** via JSON serialization, including tabs, the active tab, closed and floating panes
- ⌨️ **Keyboard and UI Automation** - Ctrl+Tab, Shift+F10 menus, selectable tabs and named Invoke buttons
- 📐 **Size constraints** (MinWidth/MaxWidth/MinHeight/MaxHeight)
- 🎨 **Full theming support** with Light/Dark modes

## 📍 Namespace

```csharp
using Atc.Wpf.Controls.Layouts;
```

## 🚀 Quick Start

```xml
<layouts:DockPanelPro LayoutId="MyLayout" SplitterThickness="5">
    <!-- Left: Solution Explorer -->
    <layouts:DockRegion
        layouts:DockPanelPro.Dock="Left"
        Width="250"
        MinWidth="150"
        MaxWidth="400"
        Header="Solution Explorer"
        IsCollapsible="True"
        RegionId="SolutionExplorer">
        <TreeView />
    </layouts:DockRegion>

    <!-- Right: Properties -->
    <layouts:DockRegion
        layouts:DockPanelPro.Dock="Right"
        Width="300"
        Header="Properties"
        IsCollapsible="True"
        RegionId="Properties">
        <StackPanel />
    </layouts:DockRegion>

    <!-- Bottom: Output -->
    <layouts:DockRegion
        layouts:DockPanelPro.Dock="Bottom"
        Height="150"
        Header="Output"
        IsCollapsible="True"
        RegionId="Output">
        <TextBox IsReadOnly="True" />
    </layouts:DockRegion>

    <!-- Center: Main content -->
    <layouts:DockRegion layouts:DockPanelPro.Dock="Center" RegionId="Editor">
        <TabControl />
    </layouts:DockRegion>
</layouts:DockPanelPro>
```

## 🗂️ Tabbed Pane Groups

Put `DockPane` elements in `DockRegion.Panes` instead of setting the region's content. Each pane is a tab; the region header shows the active pane (or the region's own `Header`) with a **float** and a **close** button for it. The tab strip hides while only one pane is docked.

```xml
<layouts:DockPanelPro AllowFloating="True" LayoutId="IDE">
    <layouts:DockRegion
        layouts:DockPanelPro.Dock="Bottom"
        Height="180"
        RegionId="Bottom"
        TabStripPlacement="Bottom">
        <layouts:DockRegion.Panes>
            <layouts:DockPane Header="Output" PaneId="output">
                <TextBox IsReadOnly="True" />
            </layouts:DockPane>
            <layouts:DockPane Header="Errors" PaneId="errors" CanClose="False">
                <DataGrid />
            </layouts:DockPane>
            <layouts:DockPane Header="Find Results" PaneId="find" CanFloat="False">
                <ListBox />
            </layouts:DockPane>
        </layouts:DockRegion.Panes>
    </layouts:DockRegion>

    <layouts:DockRegion layouts:DockPanelPro.Dock="Center" RegionId="Editor">
        <TextBox AcceptsReturn="True" />
    </layouts:DockRegion>
</layouts:DockPanelPro>
```

- A region without panes works exactly as before (its `Content` is shown).
- A side region whose panes are all floating or closed collapses, so the other regions take its space; it comes back when a pane is docked again.
- A closed pane keeps its region and position: `RestorePane` puts it back there and activates it.
- Panes are logical children of their region, so they inherit its `DataContext`, also while floating.
- A floating window merges the resource dictionaries of the pane's ancestors (region, panel, page, window), so implicit styles and `DynamicResource`s keep applying, and it follows the panel's `FlowDirection`, `Language` and font (family, size, style, weight, stretch).

## 🪟 Floating Panes

With `AllowFloating="True"` a pane whose `CanFloat` is `true` can be floated into its own tool window:

- the **float** button in the region header, or **Float** in the tab / header context menu (Shift+F10),
- a double-click on its tab or on the region header,
- dragging its tab (or the region header) out of the strip - the new window follows the mouse,
- `dockPanel.FloatPane(pane)` or the `DockPanelProCommands.FloatPane` command.

The floating window is a themed `NiceWindow` owned by the panel's window, so it minimizes, restores and closes with it, follows the theme, and is hidden from the taskbar. It docks the pane back to its previous region and tab position with:

- the **Dock** button in its title bar, or **Dock** in its context menu (Shift+F10 inside the pane),
- a double-click on its title bar,
- `dockPanel.RedockPane(pane)` or the `DockPanelProCommands.RedockPane` command.

The window's close button **closes** the pane when `CanClose` is `true` (restore it with `RestorePane`), otherwise it docks the pane back. When the owner window closes, the floating windows close with it but the panes keep their floating state, so a layout saved afterwards still has them floating. When the panel is unloaded (for example its view is swapped out) its floating windows close; they open again at the same place when the panel is loaded again.

```csharp
var output = dockPanel.FindPane("output")!;
output.FloatingBounds = new Rect(100, 100, 400, 250); // optional; default is over its docked position
dockPanel.FloatPane(output);
dockPanel.RedockPane(output);
dockPanel.RedockAllPanes();
```

## ⚙️ DockPanelPro Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `LayoutId` | `string` | `null` | Identifier for layout persistence |
| `AutoSaveLayout` | `bool` | `false` | Automatically save layout changes |
| `SplitterThickness` | `double` | `5` | Thickness of resizable splitters |
| `SplitterBackground` | `Brush` | from theme | Background brush for splitters |
| `AllowFloating` | `bool` | `false` | Allow panes to float into their own tool windows |

## 🔗 DockPanelPro Attached Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DockPanelPro.Dock` | `DockPosition` | `Center` | Dock position (Left, Right, Top, Bottom, Center) |

## 🛠️ DockPanelPro Methods

| Method | Description |
|--------|-------------|
| `GetPanes()` | All panes of all regions, in region and tab order |
| `FindPane(paneId)` | The pane with the given `PaneId`, or `null` |
| `ActivatePane(pane)` | Selects a docked pane's tab, or activates a floating pane's window |
| `ClosePane(pane)` | Closes (hides) a pane that `CanClose`; returns `false` otherwise |
| `RestorePane(pane)` / `RestorePane(paneId)` | Docks a closed pane back at its position and activates it |
| `FloatPane(pane)` | Floats a docked pane (needs `AllowFloating` and `CanFloat`) |
| `RedockPane(pane)` | Docks a floating pane back at its previous region and position |
| `RedockAllPanes()` | Docks every floating pane back |
| `MovePane(pane, region, index)` | Moves a pane into another region of the panel as the active tab |
| `SaveLayout()` / `LoadLayout(json)` / `ResetLayout()` | Layout persistence (see below); reset also docks all panes |

## 📡 DockPanelPro Events

| Event | Description |
|-------|-------------|
| `PaneStateChanged` | A pane was floated, docked, closed or restored (`Pane`, `OldState`, `NewState`) |

## ⚙️ DockRegion Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `RegionId` | `string` | `null` | Unique identifier for persistence |
| `IsCollapsible` | `bool` | `false` | Enable collapse toggle button |
| `IsExpanded` | `bool` | `true` | Current expanded state (two-way binding) |
| `IsResizable` | `bool` | `true` | Allow resizing via splitters |
| `Panes` | `ObservableCollection<DockPane>` | empty | The panes of the region, in tab order (incl. floating and closed panes) |
| `DockedPanes` | `ReadOnlyObservableCollection<DockPane>` | empty | The panes shown as tabs |
| `ActivePane` | `DockPane` | first docked pane | The selected tab (two-way binding) |
| `HasPanes` | `bool` | `false` | Read-only; `true` when the region has panes |
| `TabStripPlacement` | `Dock` | `Top` | `Top` or `Bottom` (`Left` and `Right` render as `Top`) |
| `Header` | `object` | `null` | Header content; with panes and no header the active pane's header is shown |
| `HeaderBackground` | `Brush` | from theme | Header background color |
| `HeaderForeground` | `Brush` | from theme | Header text color |
| `HeaderPadding` | `Thickness` | `8` | Header inner padding |
| `ContentPadding` | `Thickness` | `0` | Content area padding |
| `CornerRadius` | `CornerRadius` | `0` | Corner rounding |
| `Width` | `double` | `NaN` | Initial/current width (for Left/Right regions) |
| `Height` | `double` | `NaN` | Initial/current height (for Top/Bottom regions) |
| `MinWidth` | `double` | `0` | Minimum width constraint |
| `MaxWidth` | `double` | `∞` | Maximum width constraint |
| `MinHeight` | `double` | `0` | Minimum height constraint |
| `MaxHeight` | `double` | `∞` | Maximum height constraint |

`SelectNextPane(reverse)` activates the next (or previous) tab, wrapping around.

## 📡 DockRegion Events

| Event | Description |
|-------|-------------|
| `Expanded` | Raised when the region expands |
| `Collapsed` | Raised when the region collapses |
| `RegionSizeChanged` | Raised when the region's size changes via splitter drag |
| `ActivePaneChanged` | Raised (bubbling) when the active tab changes |

## ⚙️ DockPane Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `PaneId` | `string` | `null` | Unique identifier for `FindPane` and persistence |
| `Header` | `object` | `null` | Tab header (plain text is also used for the window title and automation names) |
| `CanClose` | `bool` | `true` | Show the close button and allow `ClosePane` |
| `CanFloat` | `bool` | `true` | Allow floating (together with `AllowFloating`) |
| `State` | `DockPaneState` | `Docked` | Read-only: `Docked`, `Floating` or `Closed` |
| `IsFloating` | `bool` | `false` | Read-only shortcut for `State == Floating` |
| `Region` | `DockRegion` | - | The region the pane belongs to (kept while floating or closed) |
| `FloatingBounds` | `Rect` | `Rect.Empty` | Bounds of the floating window (DIPs); follows the window |

## ⌨️ Commands and Keyboard

`DockPanelProCommands` holds routed commands handled by the panel: `FloatPane`, `RedockPane`, `ClosePane`, `NextPane` and `PreviousPane`. The parameter is the `DockPane`; without one the command uses the pane (or the active pane of the region) it was raised from.

| Key | Action |
|-----|--------|
| `Ctrl+Tab` / `Ctrl+Shift+Tab` | Next / previous tab of the region that has the focus |
| `←` / `→` on a focused tab | Previous / next tab |
| `Shift+F10` or the context-menu key on a tab or the region header | **Float** / **Close** menu for the pane |
| `Shift+F10` inside a floating pane | **Dock** / **Close** menu |
| `Tab` | Reaches the header float and close buttons |

## ♿ UI Automation

- `DockPanelPro` is a pane (automation id = `LayoutId` unless one is set), each `DockRegion` a group named after its header or active pane (automation id = `RegionId`, `ExpandCollapse` when collapsible), and each `DockPane` a pane (automation id = `PaneId`).
- The tabs are tab items with `SelectionItem` (automation id = `PaneId`).
- The region header buttons are Invoke buttons named "Float *pane*" and "Close *pane*" (automation ids `FloatPane_<RegionId>` and `ClosePane_<RegionId>`); each tab has a "Close *pane*" button (`CloseTab_<PaneId>`).
- A floating window has the automation id `FloatingPane_<PaneId>` and a "Dock *pane*" Invoke button (`RedockPane`).

The texts are translated (English, Danish, German) and follow `CultureManager.UiCulture`.

## 📋 DockPosition Enumeration

| Value | Description |
|-------|-------------|
| `Left` | Dock to the left side |
| `Right` | Dock to the right side |
| `Top` | Dock to the top |
| `Bottom` | Dock to the bottom |
| `Center` | Fill the remaining space |

## 💾 Layout Persistence

### Saving Layout

```csharp
// Save current layout to JSON
string layoutJson = dockPanel.SaveLayout();

// Store in settings or file
File.WriteAllText("layout.json", layoutJson);
```

### Loading Layout

```csharp
// Load layout from JSON
string layoutJson = File.ReadAllText("layout.json");
dockPanel.LoadLayout(layoutJson);
```

### Resetting Layout

```csharp
// Reset all regions to default sizes and dock all panes
dockPanel.ResetLayout();
```

### 📄 Layout JSON Format

Layouts are written as version 2. Besides the region sizes it holds, for each region, its panes in tab order and the active tab, and for each pane with a `PaneId` its state (`0` docked, `1` floating, `2` closed) and the floating window bounds. On load the bounds are moved (and shrunk if needed) onto the screen they overlap most, so a window saved on a monitor that is gone opens on a visible one.

```json
{
  "version": 2,
  "layoutId": "IDE",
  "regions": [
    {
      "regionId": "Bottom",
      "dock": 3,
      "width": 0,
      "height": 180,
      "isExpanded": true,
      "paneIds": [ "output", "errors", "find" ],
      "activePaneId": "errors"
    }
  ],
  "panes": [
    { "paneId": "output", "state": 1, "floatingLeft": 120, "floatingTop": 140, "floatingWidth": 400, "floatingHeight": 250 },
    { "paneId": "errors", "state": 0, "floatingLeft": 0, "floatingTop": 0, "floatingWidth": 0, "floatingHeight": 0 },
    { "paneId": "find", "state": 2, "floatingLeft": 0, "floatingTop": 0, "floatingWidth": 0, "floatingHeight": 0 }
  ]
}
```

Older layouts without a `version` (version 1, region sizes only) still load: they restore the regions and leave the panes as they are. Regions and panes the layout does not know are ignored, and a floating pane in a layout docks when the panel does not allow floating.

## 🖥️ Examples

### Basic IDE Layout

```xml
<layouts:DockPanelPro LayoutId="IDE" SplitterThickness="5">
    <layouts:DockRegion
        layouts:DockPanelPro.Dock="Left"
        Width="250"
        Header="Explorer"
        IsCollapsible="True">
        <TreeView />
    </layouts:DockRegion>

    <layouts:DockRegion
        layouts:DockPanelPro.Dock="Bottom"
        Height="150"
        Header="Output"
        IsCollapsible="True">
        <TabControl>
            <TabItem Header="Output"><TextBox /></TabItem>
            <TabItem Header="Errors"><DataGrid /></TabItem>
        </TabControl>
    </layouts:DockRegion>

    <layouts:DockRegion layouts:DockPanelPro.Dock="Center">
        <TabControl />
    </layouts:DockRegion>
</layouts:DockPanelPro>
```

### 📊 Dashboard with Navigation

```xml
<layouts:DockPanelPro>
    <layouts:DockRegion
        layouts:DockPanelPro.Dock="Left"
        Width="200"
        MinWidth="150"
        Header="Navigation"
        IsResizable="True">
        <StackPanel>
            <Button Content="Dashboard" />
            <Button Content="Reports" />
            <Button Content="Settings" />
        </StackPanel>
    </layouts:DockRegion>

    <layouts:DockRegion layouts:DockPanelPro.Dock="Center" Header="Dashboard">
        <Grid />
    </layouts:DockRegion>
</layouts:DockPanelPro>
```

### 🔒 Non-Resizable Navigation

```xml
<layouts:DockPanelPro>
    <layouts:DockRegion
        layouts:DockPanelPro.Dock="Left"
        Width="60"
        IsResizable="False">
        <!-- Icon-only navigation -->
    </layouts:DockRegion>

    <layouts:DockRegion layouts:DockPanelPro.Dock="Center">
        <ContentControl />
    </layouts:DockRegion>
</layouts:DockPanelPro>
```

## 🎨 Theming

DockPanelPro integrates with the Atc.Wpf theming system; the tabs, header buttons and floating windows use theme brushes and follow Light/Dark switches. Customize appearance using the helper attached properties:

```xml
<layouts:DockRegion
    helpers:DockPanelProHelper.ToggleCircleFill="LightGreen"
    helpers:DockPanelProHelper.ToggleCircleStroke="Green"
    helpers:DockPanelProHelper.ToggleArrowStroke="DarkGreen"
    Header="Custom Styled"
    IsCollapsible="True">
    <!-- Content -->
</layouts:DockRegion>
```

The styles are `AtcApps.Styles.DockPanelPro`, `AtcApps.Styles.DockRegion`, `AtcApps.Styles.DockPane`, `AtcApps.Styles.DockRegion.TabControl` and `AtcApps.Styles.DockRegion.TabItem`.

## 🔀 Comparison with Grid + GridSplitter

| Feature | Grid + GridSplitter | DockPanelPro |
|---------|---------------------|--------------|
| Resizable regions | ⚠️ Manual setup | ✅ Built-in |
| Collapsible panes | ❌ Not available | ✅ Built-in |
| Tabbed pane groups | ❌ Not available | ✅ Built-in |
| Floating tool windows | ❌ Not available | ✅ Built-in |
| Layout persistence | ⚠️ Manual implementation | ✅ Built-in |
| Min/Max constraints | ✅ Supported | ✅ Supported |
| XAML complexity | 🔴 High | 🟢 Low |
| IDE-style layouts | 🔴 Complex | 🟢 Simple |

## ✅ Best Practices

1. **Always set RegionId and PaneId** for regions and panes you want to persist
2. **Set reasonable Min/Max constraints** to prevent unusable layouts
3. **Use IsCollapsible sparingly** - typically for tool windows, not main content
4. **Center region should be last** - it fills remaining space
5. **Use plain text pane headers** - they also name the floating window and the automation buttons
6. **Save the layout in the window's `Closing` event** - floating panes are still recorded as floating

## 📝 Notes

- A floating window hosts exactly one pane; panes cannot be tabbed together inside a floating window.
- Window bounds are in device-independent units of the panel's window; check mixed-DPI multi-monitor setups by hand.

## ⚠️ Limitations

Current limitations (may be addressed in future versions):

- Only one region per dock position (Left, Right, Top, Bottom)
- Dragging a floating window back onto the panel does not dock it (use the Dock button or a title-bar double-click)
- No snap points during resize
- No drag-and-drop reordering of tabs or regions

## 🔗 Related Controls

- **[FlexPanel](FlexPanel_Readme.md)** - CSS Flexbox-inspired layout
- **[Card](../DataDisplay/Card_Readme.md)** - Content container with elevation
- **[GroupBoxExpander](GroupBoxExpander_Readme.md)** - Collapsible grouped content

## 🎮 Sample Application

See the DockPanelPro sample in the Atc.Wpf.Sample application under **Wpf.Controls > Layouts > DockPanelPro** for interactive examples: tab groups, floating panes, a pane that cannot float, one that cannot close, save/load/reset of the layout and an event log.
