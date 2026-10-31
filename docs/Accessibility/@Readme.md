# ♿ Accessibility

What the Atc.Wpf libraries do for screen readers, keyboard users, Windows high contrast and right-to-left languages, and where they fall short. Everything on this page is based on the code and tests in this repository.

## 🔍 Overview

- Standard WPF controls keep their built-in UI Automation support. The custom controls listed below add their own automation peers.
- The multi-select family and the selection controls share one set of keyboard rules.
- `WindowsThemeSync` can switch the application to a high-contrast theme built from the Windows system colors.
- Controls that handle the arrow keys themselves swap `←` and `→` in a right-to-left layout; images, flags and code viewers are not mirrored.

## 🤖 UI Automation peers

These controls override `OnCreateAutomationPeer` and expose the listed patterns, so Narrator, other screen readers and test tools such as FlaUI can read and drive them.

### Atc.Wpf.Controls

| Control | Control type | Patterns |
|---|---|---|
| `NumericBox` (and `IntegerBox`, `DecimalBox`, `CurrencyBox`, which derive from it) | Spinner | `RangeValue` |
| `RangeSlider` | Slider | `RangeValue` |
| `Rating` | Slider | `RangeValue` |
| `ToggleSwitch` | Button | `Toggle` |
| `Chip` | Check box or radio button | `Toggle`, `SelectionItem` |
| `ChipGroup` | List | `Selection` (single modes) |
| `Segmented` / `SegmentedItem` | List / item | `Selection`; items `Toggle`, `SelectionItem` |
| `CardSelector` / `CardSelectorItem` | List / list item | `Selection`; items `Toggle`, `SelectionItem` |
| `NavigationView` / `NavigationViewItem` | Pane / list item | items `Invoke`, `SelectionItem` |
| `DirectoryPicker`, `FilePicker` | Custom | `Value` (read and set the path) |
| `MultiColumnComboBox` | Combo box | `ExpandCollapse`, `Value` |
| `MultiSelectComboBox` | Combo box | `ExpandCollapse`, `Value` (the summary text) |
| `TreeSelectComboBox` | Combo box | `ExpandCollapse`, `Value` (the summary text) |
| `CascaderComboBox` | Combo box | `ExpandCollapse`, `Value`; each column is a list, each row a check box with a three-state `Toggle` (rows with children also `ExpandCollapse`) |
| `CheckTreeView` | Tree | each row is a tree item with `ExpandCollapse` and a three-state `Toggle` |
| `TokenBox` | Combo box | `Value` (the typed text), `ExpandCollapse`; a token list (AutomationId `Tokens`) and a suggestion list (AutomationId `Suggestions`) |
| `DockPanelPro` / `DockRegion` / `DockPane` | Pane / group / pane | regions `ExpandCollapse` when collapsible; tabs are tab items with `SelectionItem`; the float, close and (in a floating window) dock buttons are `Invoke` buttons named "Float *pane*", "Close *pane*", "Dock *pane*". AutomationIds come from `LayoutId`, `RegionId` and `PaneId` |

| `DataGridFilter` column filter button | Button | `Invoke`; named "Filter *column*" (class name `DataGridFilterButton`). While its pop-up is open, the pop-up content (mode radio buttons, the value rows as check boxes with `Toggle`, Clear filter, OK, Cancel) is a child of the button |

`CheckListBox` and the pop-up list of `MultiSelectComboBox` have no custom peer: each row is a real WPF `CheckBox`, so it is a check box with the `Toggle` pattern.

### Atc.Wpf.Forms

| Control | Patterns |
|---|---|
| `ColorPicker` | `Value` |
| `EndpointBox` | `Value` |
| `TimeZonePicker` | `Value` |

The `Label*` wrappers host these controls (and the ones from Atc.Wpf.Controls) unchanged, so the inner control's peer is what a screen reader finds.

### Atc.Wpf.Components

| Control | Notes |
|---|---|
| `DualListSelector` | A group peer for the control; the two lists, the transfer and move buttons and each item's remove button have `AutomationProperties.Name` set in XAML |

### Atc.Wpf.Hardware

`AudioInputPicker`, `AudioOutputPicker`, `BluetoothDevicePicker`, `DisplayPicker`, `DrivePicker`, `NetworkAdapterPicker`, `PrinterPicker`, `ProcessPicker`, `SerialPortPicker`, `UsbCameraPicker`, `UsbPortPicker` and `WindowPicker` each have a peer with the `Value` pattern. Reading it returns the selected item (for example the port name); setting it selects the matching item, and an empty value clears the selection.

### Atc.Wpf.Theming

`NiceWindow`, its `WindowCommands` and the `NiceThumbContentControl` title-bar area have their own peers.

### 🧪 How this is tested

- Unit tests in `Atc.Wpf.Controls.Tests`, `Atc.Wpf.Forms.Tests` and `Atc.Wpf.Components.Tests` call the peers of the selection controls, `ChipGroup`, `Segmented`, `NavigationView`, the `DataGridFilter` button and `DualListSelector` directly.
- `Atc.Wpf.UiTests` drives the sample application with FlaUI (UI Automation) for `CardSelector`, `CascaderComboBox`, `CheckListBox`, `CheckTreeView`, `ChipGroup`, `DataGridFilter`, `DualListSelector`, `MultiColumnComboBox`, `MultiSelectComboBox`, `NavigationView`, `Segmented`, `TokenBox`, `TreeSelectComboBox` and the `NiceWindow` title bar. These tests are tagged `Category=UI` and are not part of the default CI run.

## ⌨️ Keyboard

Every control in the multi-select family documents its keys in its own readme (`CheckListBox`, `MultiSelectComboBox`, `CheckTreeView`, `TreeSelectComboBox`, `CascaderComboBox`, `TokenBox`, `ChipGroup`, `Segmented`, `DualListSelector`). They follow the same rules:

| Key | Rule |
|---|---|
| `Alt+Down` / `Alt+Up` / `F4` | Open or close a drop-down |
| `Esc` / `Enter` | Close a drop-down; focus returns to the box. In deferred mode `Esc` discards the ticks and `Enter` is OK |
| `Space` | Toggle the focused row, chip or segment |
| `Ctrl+A` | Select all shown items (outside a filter box) |
| Arrow keys, `Home`, `End` | Move between rows |
| `Down` in a filter box | Move to the first row |
| `Backspace` | In `Chips` display mode, on the closed box: remove the last chip |

When a drop-down opens, focus goes to its filter box (if shown) or the first row.

Exceptions, as documented by each control:

- `CascaderComboBox`: `Enter` on a row acts on the row (into the next column, or toggle on a leaf) and does not close the pop-up; `Esc` first clears the filter text.
- `CheckTreeView` and `TreeSelectComboBox`: `→` / `←` expand and collapse, `*` on the numeric keypad expands everything under the row.
- `TokenBox`: `↓` / `↑` move in the suggestions, `Enter`, `Tab` or a separator picks one, `Backspace` in an empty text selects and then removes the last token, `Esc` closes the suggestions and then clears the text.
- `ChipGroup`: `Tab` enters and leaves the group as one stop; in the single modes the group is a radio group and the arrow keys move and select.
- `Segmented`: in the single modes the arrow keys and `Home` / `End` select; in `Multiple` they only move the focus.
- `DualListSelector`: `Enter` transfers, `Ctrl+Up` / `Ctrl+Down` reorder, `Delete` removes from the selected list.
- `DataGridFilter`: stock column headers cannot take the focus, so `Alt+Down` opens the filter of the focused filter button or of the current cell's column; in the pop-up `Enter` is OK and `Esc` is Cancel, and closing it gives the focus back to the filter button.

## 🌗 High contrast

Set `WindowsThemeSync.Mode` to `WindowsThemeSyncMode.HighContrast` (or `All`) to follow Windows high contrast:

```csharp
using Atc.Wpf.Theming;
using Atc.Wpf.Theming.Helpers;

WindowsThemeSync.Mode = WindowsThemeSyncMode.All;
```

While a Windows contrast theme is on, the theme provider builds a high-contrast theme from the Windows system colors (`Window`, `WindowText`, `Highlight`, `HighlightText`, `GrayText`):

- The accent colors become the system highlight color, and text on the accent becomes the highlight text color.
- Every other theme color becomes the window color or the window text color, depending on whether it is closer to the theme's own background or foreground. This works for both the Light and Dark base themes.
- Colors whose key mentions "Disabled" become the gray text color.
- Transparency is kept, so overlays stay overlays.
- Border, stroke and separator brushes that would end up in the window color get the window text color, so they stay visible.

`WindowsThemeSync.IsWindowsHighContrast` tells whether Windows high contrast is on. `Atc.Wpf.Theming.Tests` covers the color mapping (`HighContrastColorSchemeTests`), the generated theme (`AtcAppsLibraryThemeProviderTests`) and the sync modes (`WindowsThemeSyncTests`). See `WindowsThemeSync_Readme.md` and the [Theming](../Theming/@Readme.md) page for details.

## ↔️ Right-to-left

WPF mirrors a layout when `FlowDirection="RightToLeft"` is set. On top of that:

- **Arrow keys follow the screen.** `Rating`, `Segmented`, `Carousel`, `RangeSlider`, `ChipGroup`, `CardSelector` and the columns of `CascaderComboBox` swap `←` and `→` in a right-to-left layout, so `←` still moves left on screen (`RightToLeftKeyboardTests`).
- **Pictures are not mirrored.** `Avatar` images and the flags in `CountrySelector` and `LanguageSelector` keep their orientation (`RightToLeftImageTests`), and so does the `DatePicker` calendar icon (`DatePickerStyleTests`).
- **Code stays left-to-right.** The `JsonViewer` tree and the `TerminalViewer` stay left-to-right while the toolbar around them mirrors (`RightToLeftViewerTests`).
- **Zoom.** The `ZoomRuler` labels are drawn readable, and the zoom content and mini map are not mirrored twice against their parent (`ZoomRightToLeftTests`).

## ⚠️ Known gaps

- **Row ticks in `CascaderComboBox` may not be announced.** Its row peers report a three-state `Toggle`, but they don't raise a property-changed event when the state changes, so a screen reader may not announce a tick until it reads the row again. The `CheckTreeView` / `TreeSelectComboBox` node peers do raise it.
- **The `Value` peers of the pickers don't raise value-changed events.** This applies to `DirectoryPicker`, `FilePicker`, `ColorPicker`, `EndpointBox`, `TimeZonePicker` and the hardware pickers. The value is correct when it is read, but a change is not announced on its own.
- **`Label*` fields don't link their label to the input.** No `AutomationProperties.LabeledBy` or `AutomationProperties.Name` is set from `LabelText`, so a screen reader announces the inner control without the label text. Set `AutomationProperties.Name` on the field yourself where it matters.
- **Most other custom controls have no dedicated peer** and rely on the default WPF peer of their base class.
- **High contrast must be turned on in code.** Without `WindowsThemeSyncMode.HighContrast` the application keeps its own Light or Dark colors when Windows switches to a contrast theme.
- **No automated screen-reader testing.** The UI Automation tests check the automation tree and patterns, not what Narrator or another screen reader actually says.

## 🔗 Related

- [Theming](../Theming/@Readme.md) - Light, Dark and accent themes
- `CheckListBox_Readme.md`, `MultiSelectComboBox_Readme.md`, `TokenBox_Readme.md`, `CheckTreeView_Readme.md`, `TreeSelectComboBox_Readme.md`, `CascaderComboBox_Readme.md`, `DataGridFilter_Readme.md` - keyboard and UI Automation per control
