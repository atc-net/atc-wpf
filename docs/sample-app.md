# Sample App - Search, TreeView, and TabControl Behavior

## Overview

This document defines the behavior for the SearchBox, TreeView, and TabControl interaction in the sample application.

## Architecture

The left panel consists of:

1. **SearchBox** - Filters items across all categories
2. **Expand/Collapse buttons** - Expand or collapse all TreeView items
3. **TabControl** (headers only) - Styled category filter buttons with badges
4. **Stacked TreeViews** - All category TreeViews in a ScrollViewer, visibility controlled by tab selection

### Layout Structure

```
┌─────────────────────────────────────────────────────┐
│ [Filter TextBox]                                    │
│                              [Expand] [Collapse]    │
├──────────────────────┬──────────────────────────────┤
│ TabControl           │ Border (rounded corners)     │
│ (headers only)       │   └─ ScrollViewer            │
│                      │       └─ StackPanel          │
│ ► Wpf            [3] │           ├─ WpfTreeView     │
│   Wpf.Controls   [5] │           ├─ ControlsTreeView│
│   Wpf.Network...     │           ├─ NetworkTreeView │
│   Wpf.Theming        │           ├─ ThemingTreeView │
│   Wpf.SourceGen      │           ├─ SourceGenTreeView│
│   Wpf.FontIcons      │           └─ FontIconsTreeView│
└──────────────────────┴──────────────────────────────┘
```

### Key Design Decisions

- **TabControl has empty content** - The TabItems contain no content; they serve only as styled filter buttons
- **TreeViews are external** - All TreeViews are stacked in a ScrollViewer next to the TabControl
- **Visibility controlled by code** - `UpdateTreeViewVisibility()` shows/hides TreeViews based on tab selection
- **TabControl border is transparent** - Only the tab headers are visible

---

## State Definitions

| State                  | Tab Selection    | TreeView Display                                              | Badge Display            |
| ---------------------- | ---------------- | ------------------------------------------------------------- | ------------------------ |
| **Default/Startup**    | No tab selected  | All items from all tabs visible                               | No badges                |
| **Searching (no tab)** | No tab selected  | Filtered items from all tabs                                  | Show match count per tab |
| **Tab Selected**       | One tab selected | Only items from selected tab (filtered if searchbox has text) | All badges visible       |
| **Item Clicked**       | Tab auto-selected| Items from that tab only                                      | All badges visible       |

---

## Interaction Rules

### Rule 1: Application Startup

- **Trigger:** Application loads
- **Tab State:** No tab is selected (`SelectedIndex = -1`)
- **TreeView:** All TreeViews visible, showing all items
- **TreeView Selection:** No item selected
- **Badges:** Hidden (no search active)
- **SearchBox:** Empty, focused

### Rule 2: SearchBox Text Changed (No Tab Selected)

- **Trigger:** User types in searchbox while no tab is selected
- **Tab State:** Remains unselected
- **TreeView:** All TreeViews visible, items filtered by search text
- **TreeView Selection:** Cleared (all items deselected)
- **Badges:** Show match count per tab (on all tab headers)

### Rule 3: SearchBox Cleared

- **Trigger:** User clears searchbox (backspace or clear button)
- **Tab State:** Deselect any selected tab → No tab selected
- **TreeView:** All TreeViews visible, showing all items
- **TreeView Selection:** Cleared
- **Badges:** Hidden

### Rule 4: TreeView Item Clicked

- **Trigger:** User clicks on an item in the TreeView
- **Tab State:** Select the tab that contains this item
- **TreeView:** Show only the TreeView for the selected tab
- **Badges:** All badges remain visible (if search is active)
- **Content:** Load the selected sample in the viewer

### Rule 5: Tab Header Clicked

- **Trigger:** User clicks on a tab header
- **Tab State:** Select the clicked tab
- **TreeView:** Show only the TreeView for the selected tab (apply current filter)
- **Badges:** All badges remain visible (if search is active)

### Rule 6: SearchBox Text Changed (Tab Selected)

- **Trigger:** User types in searchbox while a tab is selected
- **Tab State:** Deselect the tab → No tab selected
- **TreeView:** All TreeViews visible, items filtered by search text
- **TreeView Selection:** Cleared
- **Badges:** Show match count per tab

---

## State Transition Matrix

| Current State            | Action         | New State                | Tab         | TreeView Shows          |
| ------------------------ | -------------- | ------------------------ | ----------- | ----------------------- |
| Any                      | App Start      | No Tab + No Filter       | None        | All items, all tabs     |
| No Tab + No Filter       | Type in search | No Tab + Filtering       | None        | Filtered, all tabs      |
| No Tab + Filtering       | Clear search   | No Tab + No Filter       | None        | All items, all tabs     |
| No Tab + Filtering       | Click item     | Tab Selected + Filtering | Item's tab  | Filtered, selected tab  |
| No Tab + Filtering       | Click tab      | Tab Selected + Filtering | Clicked tab | Filtered, selected tab  |
| No Tab + No Filter       | Click item     | Tab Selected + No Filter | Item's tab  | All items, selected tab |
| No Tab + No Filter       | Click tab      | Tab Selected + No Filter | Clicked tab | All items, selected tab |
| Tab Selected + Filtering | Type in search | No Tab + Filtering       | None        | Filtered, all tabs      |
| Tab Selected + Filtering | Clear search   | No Tab + No Filter       | None        | All items, all tabs     |
| Tab Selected + Filtering | Click item     | Tab Selected + Filtering | Item's tab  | Filtered, selected tab  |
| Tab Selected + Filtering | Click tab      | Tab Selected + Filtering | Clicked tab | Filtered, selected tab  |
| Tab Selected + No Filter | Type in search | No Tab + Filtering       | None        | Filtered, all tabs      |
| Tab Selected + No Filter | Click item     | Tab Selected + No Filter | Item's tab  | All items, selected tab |
| Tab Selected + No Filter | Click tab      | Tab Selected + No Filter | Clicked tab | All items, selected tab |

---

## Search Behavior

### Match Rules

1. **Normal text** (e.g., "range"): Case-insensitive contains match
2. **PascalCase abbreviation** (e.g., "RS"): Match against uppercase letters in item name
   - "RS" matches "RangeSlider" (R+S) and "ReversibleStackPanel" (R+S+P contains RS)
   - Only triggers when filter is 2+ characters and ALL UPPERCASE

### Badge Display Rules

1. Badges show match count only when search is active
2. Badges use `HideWhenZero` - automatically hidden when count is 0
3. Badges are cleared (set to null) when SearchBox is empty

### TreeView Selection Rules

1. When user types in SearchBox, all TreeView item selections are cleared
2. This prevents stale selections from persisting during search
3. Selection is cleared recursively through all nested TreeViewItems

---

## Implementation Details

### Files Modified

- `sample/Atc.Wpf.Sample/MainWindow.xaml` - Layout with TabControl and stacked TreeViews
- `sample/Atc.Wpf.Sample/MainWindow.xaml.cs` - Filtering and selection logic

### Key Methods

| Method                      | Purpose                                                |
| --------------------------- | ------------------------------------------------------ |
| `InitializeTabMappings()`   | Maps TreeViews to TabItems and Badges                  |
| `UpdateTreeViewVisibility()`| Shows/hides TreeViews based on selected tab            |
| `FilterOnTextChanged()`     | Handles search text changes, filters items             |
| `ClearTreeViewSelections()` | Deselects all TreeView items when searching            |
| `SelectTabForTreeView()`    | Selects the tab containing a clicked TreeView item     |
| `ApplyCurrentFilter()`      | Reapplies filter after tab selection change            |
| `UpdateTabHeaders()`        | Updates badge counts on all tab headers                |

### XAML Structure

```xml
<Grid>
    <!-- TabControl for styled headers (no content) -->
    <TabControl
        Background="Transparent"
        BorderBrush="Transparent"
        TabStripPlacement="Left">
        <TabItem Tag="Wpf">
            <TabItem.Header>
                <!-- Header with Badge -->
            </TabItem.Header>
            <!-- No content -->
        </TabItem>
        <!-- ... more TabItems -->
    </TabControl>

    <!-- TreeViews in ScrollViewer -->
    <Border CornerRadius="5" BorderThickness="1">
        <ScrollViewer>
            <StackPanel>
                <sample:SamplesWpfTreeView />
                <sample:SamplesWpfControlsTreeView />
                <!-- ... more TreeViews -->
            </StackPanel>
        </ScrollViewer>
    </Border>
</Grid>
```

---

## Verification Checklist

- [x] App starts with no tab selected
- [x] All items from all categories are visible on startup
- [x] Typing deselects any selected TreeView item
- [x] Typing "RS" shows both RangeSlider AND ReversibleStackPanel
- [x] Badges show correct counts per category
- [x] Clicking an item selects its category tab
- [x] Clicking a tab filters to show only that category's TreeView
- [x] Clearing searchbox deselects tab and shows all TreeViews
- [x] TabControl border is transparent (only headers visible)
- [x] TreeViews area has rounded border
