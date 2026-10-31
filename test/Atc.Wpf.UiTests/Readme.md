# Atc.Wpf.UiTests

Pilot UI / visual-regression test project, built on **[FlaUI](https://github.com/FlaUI/FlaUI)** (`FlaUI.Core` + `FlaUI.UIA3`). FlaUI drives the sample app via Windows UI Automation, captures screenshots, and lets later tests diff them against baselines.

## Why the tests are skipped by default

This project targets Windows-only and launches a real WPF process. Headless / Linux CI cannot execute it. The tests carry `[Trait("Category", "UI")]` and the smoke test is currently `[Fact(Skip = "...")]` — opt in by removing the skip locally, or wire CI to opt in via filter:

```bash
dotnet run --project test/Atc.Wpf.UiTests -c Release \
    -- --filter-trait "Category=UI"
```

## Running locally

1. Build the sample app first so the executable exists:
   ```bash
   dotnet build sample/Atc.Wpf.Sample -c Release
   ```
2. Open `SampleAppSmokeTests.cs`, remove the `Skip = "..."` from the `[Fact]`, and run:
   ```bash
   dotnet run --project test/Atc.Wpf.UiTests -c Release --no-build
   ```
3. Captured screenshots are written to `bin/<Config>/net10.0-windows/Snapshots/`.

## Tests

| Test | What it pins |
|---|---|
| `SampleAppSmokeTests.Sample_app_launches_and_shows_main_window` | App launches, main window has a non-empty title, full window screenshot can be captured. |
| `NavigationViewTests.Invoking_items_navigates_and_back_selects_the_previous_item` | On **Wpf.Controls > Navigation > NavigationView**, invoking a pane item (UI Automation Invoke) navigates and selects it, and the back button selects the previous item. |
| `CheckListBoxTests.Ticking_two_of_five_then_select_all_ticks_every_row_in_one_change` | On **Wpf.Controls > Selectors > CheckListBox**, ticking two of the five rows through UI Automation Toggle leaves **Select all** indeterminate and the count at 2 of 5; toggling **Select all** ticks every row, turns it on, shows 5 of 5 and logs exactly one more `SelectedKeysChanged`. Reads the page's event log.
| `CardSelectorTests.Toggling_two_cards_turns_them_on_and_logs_one_change_each` | On **Wpf.Controls > Selectors > CardSelector**, the selector is a UI Automation list whose cards are check boxes named from the card template; toggling **Reports** and **Maps** through Toggle turns them on (the others stay off) and toggling **Reports** again turns it off, with one `SelectedKeysChanged` line per toggle in the page's event log. |
| `ChipGroupTests.Toggling_two_chips_turns_them_on_and_logs_one_change_each` | On **Wpf.Controls > DataDisplay > ChipGroup**, the group is a UI Automation list whose chips are check boxes; toggling **Apple** and **Cherry** through Toggle turns them on (the others stay off) and toggling **Apple** again turns it off, with one `SelectedKeysChanged` line per toggle in the page's event log. |
| `SegmentedMultipleTests.Toggling_Mon_and_Wed_turns_them_on_and_logs_one_change_each` | On **Wpf.Controls > DataDisplay > Segmented**, the weekday picker (`SelectionMode=Multiple`) is a UI Automation list whose segments are toggle buttons; toggling **Mon** and **Wed** through Toggle turns them on (the other days stay off) and toggling **Mon** again turns it off, with one `SelectedKeysChanged` line per toggle in the page's event log. |
| `MultiSelectComboBoxTests.Opening_ticking_two_of_three_and_closing_shows_their_names_as_the_summary` | On **Wpf.Controls > Selectors > MultiSelectComboBox**, the box is a UI Automation combo box; expanding it through ExpandCollapse, ticking **Orders** and **Payments** through Toggle (the pop-up stays open) and collapsing it leaves "Orders, Payments" as its Value, with exactly two `SelectedKeysChanged` lines in the page's event log. |
| `MultiSelectComboBoxTests.Chips_mode_removing_a_chip_through_its_button_leaves_the_other_chips` | On the same page, the **ChipsBox** (`DisplayMode="Chips"`): ticking **Orders**, **Payments** and **Shipping** and collapsing shows three chips; invoking the "Remove Payments" button of one chip unticks it without opening the pop-up, leaving the chips **Orders** and **Shipping** (Value "Orders, Shipping") and a fourth `SelectedKeysChanged` line in the page's log. |
| `CheckTreeViewTests.Ticking_a_parent_ticks_its_children_and_unticking_one_child_makes_the_parent_indeterminate` | On **Wpf.Controls > Selectors > CheckTreeView**, the rows of the permissions tree (`CheckedKeysStrategy="Parents"`) are UI Automation tree items with ExpandCollapse and a three-state Toggle; expanding **Settings** and toggling it turns it and both its children On, toggling **Edit settings** off leaves **Settings** Indeterminate, with the page's event log showing `[admin.settings]` and then `[admin.settings.view]`. |
| `CascaderComboBoxTests.Activating_columns_ticking_a_city_and_a_filter_result_shows_both_paths_in_the_summary` | On **Wpf.Controls > Selectors > CascaderComboBox**, through UI Automation patterns: expanding the combo box, then the **Denmark** and **Jutland** rows (check boxes with ExpandCollapse) opens the next columns, toggling **Aarhus** leaves **Jutland** and **Denmark** Indeterminate, the filter `malm` lists **Malmö** in the flat results where toggling it ticks it, and after collapsing, the combo box value holds `Denmark / Jutland / Aarhus` and `Sweden / Götaland / Malmö` and the event log `[dk-jut-aar, se-got-mal]`. |
| `CascaderComboBoxTests.A_click_on_the_text_activates_a_row_a_click_on_its_check_box_ticks_it_and_a_click_outside_closes` | On the same page, with real mouse clicks: a click on the box opens the pop-up, a click on the text of **Sweden** makes it active (Expanded, not ticked, **Svealand** shown in the next column), a click on the check box of **Norway** ticks it without activating it, and a click outside (on the event log) closes the pop-up. |
| `MultiColumnComboBoxTests.Filtering_ticking_two_rows_and_sorting_keeps_the_ticks_and_shows_both_in_the_summary` | On **Wpf.Controls > Selectors > MultiColumnComboBox**, through UI Automation patterns: expanding the combo box, setting the filter box to `Berlin` leaves three grid rows, toggling the check boxes of **Bravo Foods** and **Lima Robotics** ticks them, invoking the **Name** column header twice sorts descending (**Victor Motors** first) with both ticks kept, and after collapsing the combo box value is `Bravo Foods, Lima Robotics` and the event log shows one event per tick. |
| `MultiColumnComboBoxTests.A_mouse_click_on_a_row_ticks_it_and_a_click_outside_closes_the_pop_up` | On the MultiColumnComboBox page, with real mouse clicks: a click on the box opens the pop-up, a click on the **Charlie Marine** row (on its text, not the check box) ticks it and keeps the pop-up open, and a click outside (on the event log) closes it; the value is `Charlie Marine`. |
| `MultiColumnComboBoxTests.The_label_control_page_loads_and_its_mandatory_field_opens` | On **Wpf.Forms > LabelMultiColumnComboBox**: the page loads, the interactive field's combo box expands and its grid lists **Alpha Logistics** as a check box, then it collapses. |
| `TreeSelectComboBoxTests.Clicking_rows_toggles_them_the_arrow_only_expands_and_a_click_outside_closes` | On **Wpf.Controls > Selectors > TreeSelectComboBox**, with real mouse clicks: a click on the box opens the pop-up, a click on the **Norway** row ticks it and keeps the pop-up open, a click on the arrow of **Sweden** only expands it, a click on **Malmö** ticks it, and a click outside (on the event log) closes the pop-up. |
| `DualListSelectorTests.CheckBox_style_select_all_on_a_filtered_list_moves_only_the_matches` | On **Wpf.Components > Selectors > DualListSelector**, the check-box style selector (`ItemSelectionStyle="CheckBox"`): setting the available filter to "ap" through Value makes the available **Select all** report 0/3 (its help text), toggling it turns it On with 3/3, invoking **Move to selected** puts exactly **Apple**, **Apricot** and **Grape** in the selected list (rows are check boxes), as the page's `SelectedItemsLog` confirms. |
| `TokenBoxTests.Typing_picking_pasting_and_removing_leaves_the_expected_tokens` | On **Wpf.Controls > Selectors > TokenBox**, the field is a UI Automation combo box with an edit child and a token list; typing "or" into the edit opens the suggestions (ExpandCollapse expanded, "Orders" listed), Enter picks **Orders**, setting the Value "payments, billing" is handled as a paste (one change, three tokens), and invoking "Remove Payments" leaves the tokens **Orders** and **Billing**, with one `SelectedKeysChanged` line per action in the page's event log. |
| `DockPanelProTests.Floating_a_pane_opens_a_tool_window_docking_it_closes_the_window_and_tabs_select` | On **Wpf.Controls > Layouts > DockPanelPro**, invoking the float button of the right region (`FloatPane_Right`) opens a separate tool window (`FloatingPane_properties`) and logs `Properties: Docked -> Floating`; invoking that window's **Dock** button (`RedockPane`) closes it and logs `Properties: Floating -> Docked`; selecting the **Errors** tab of the bottom region through SelectionItem selects it and logs `Active: Errors` in the page's `DockLog`. |
| `DataGridFilterTests.Unticking_a_status_and_ok_filters_the_rows_and_clear_filter_shows_them_again` | On **Wpf.Controls > DataGrids > DataGridFilter**, invoking the Status column's filter button (an Invoke button named "Filter Status") opens the filter pop-up, whose rows are children of the button in the UI Automation tree; toggling **Cancelled** off and invoking **OK** filters the 5,000 orders (the page's row count drops and its `FilterLog` shows one `FilterChanged` line with `Status in (Closed\|Open\|Pending\|Shipped)`), and **Clear filter** in the reopened pop-up brings all 5,000 rows back with a second line. |
| `NumberBoxLostFocusEventTests.Editing_the_first_input_box_raises_its_lost_focus_event_once_when_focus_leaves` | On the **DecimalXyBox**, **IntegerXyBox**, **PixelSizeBox**, **LabelDecimalXyBox**, **LabelIntegerBox**, **LabelIntegerXyBox** and **LabelPixelSizeBox** pages, typing a value raises no lost-focus event while the input box has focus, Tab raises it once with the typed value, and leaving the next input box unchanged raises nothing. Reads the page's event log. |
| `TrayIconTests.Show_icon_adds_and_removes_the_notification_area_icon` | On **Wpf.Components > Notifications > TrayIcon**, the icon is in the Windows notification area while the page is open, unchecking **Show icon** removes it and checking it adds it again. Reads the page's status instead of driving Explorer's notification area. |
| `WindowsThemeSyncTests.Following_app_mode_applies_windows_mode_and_a_manual_pick_stops_it` | On **Wpf.Theming > Theme > Windows theme sync**, following the Windows app mode applies the Windows light/dark setting, and picking a theme by hand stops following it. Reads the Windows mode from the page, so it works on any machine. |
| `DialogOwnerPlacementTests.Info_dialog_is_centred_on_the_main_window_and_focus_returns_when_it_closes` | On **Wpf.Components > Dialogs > Standard DialogBox**, with the main window moved away from the middle of the screen, **Info** opens a separate modal window owned by the main window and centred on it (not on the screen), and closing it with **OK** gives the keyboard focus back to the main window. |
| `ToastNotificationTests.Toast_appears_in_the_top_right_corner_and_closes_after_its_duration` | On **Wpf.Components > Notifications > ToastNotificationService**, **Information** shows a toast in the top-right corner of the page's `TopRight` notification area, and it closes by itself after its default 5 second duration. The toast controls have no automation peer, so the test finds the toast by its title text and close button. |
| `ToastNotificationTests.Toast_closes_at_once_from_its_close_button` | On the same page, a **10s Duration** toast closes within 2 seconds when its close button is invoked. |
| `NiceWindowTitleBarTests.Title_is_centred_between_the_icon_and_the_window_buttons_when_maximized_and_normal` | The main window's centred title sits in the middle of the space between the icon column and the minimise button, both maximized and in the normal state. |
| `NiceWindowTitleBarTests.Title_stays_centred_between_the_icon_and_the_window_buttons_with_right_window_commands` | On **Wpf.Theming > Window > NiceWindow**, **Open with RightWindowCommands** opens a NiceWindow with right-hand window commands, and its title is still centred between the icon and the window buttons, without overlapping the commands. |
| `NiceWindowTitleBarTests.Window_buttons_maximize_restore_minimize_and_close_the_window` | The main window's maximise/restore button switches between Maximized and Normal (and its name between Restore and Maximize), the minimise button minimises it, and the close button ends the app. |
| `NiceWindowSnapshotTests.NiceWindow_chrome_is_active_and_titlebar_strip_can_be_snapshotted` | Custom `NiceWindow` chrome is in play (asserts `ClassName == "NiceWindow"` via `NiceWindowAutomationPeer`). Captures full window + cropped title bar strip — the latter isolates the highest-risk theming surface (caption colors, system buttons, title text) for low-noise image diffs. |

## Pattern for new tests

```csharp
[Trait("Category", "UI")]
[Fact(Skip = "Manual UI test")]
public void Some_view_renders_without_errors()
{
    var exePath = SampleAppPath.Resolve();
    using var app = Application.Launch(new ProcessStartInfo(exePath));
    try
    {
        using var automation = new UIA3Automation();
        var window = app.GetMainWindow(automation, TimeSpan.FromSeconds(20));

        // Bring window forward and let it settle BEFORE capturing — see Gotchas.
        window.Patterns.Window.Pattern.SetWindowVisualState(WindowVisualState.Maximized);
        window.SetForeground();
        Thread.Sleep(500);

        // 1. Drive the UI: click a TreeView item, fill a field, etc.
        // 2. Capture: var bitmap = window.Capture();
        // 3. Persist + (optionally) diff against a baseline under Snapshots/baseline/.
    }
    finally
    {
        try { app.Close(); app.WaitWhileMainHandleIsMissing(TimeSpan.FromSeconds(5)); }
        catch (Exception) { /* process already gone */ }
    }
}
```

## Gotchas (learned the hard way)

- **`mainWindow.Capture()` uses the screen as backing store.** The doc says "Captures the object as screenshot" but it actually clips to the element's bounding rectangle while pulling pixels from the screen. If another window is on top of the sample, you'll capture *that*. Always force the sample to foreground first; maximizing via the Window pattern is the most reliable way (`SetForegroundWindow` alone is blocked by Windows for non-foreground processes).
- **`Capture.Rectangle(absoluteScreenRect)` has the same problem, worse.** Use `mainWindow.Capture()` then crop the resulting `Bitmap` in pixel coordinates.
- **`application.WaitWhileMainHandleIsMissing(...)` races with `application.Close()`.** On fast machines the process exits before WaitWhile can find it by pid; FlaUI then throws a wrapped `System.Exception`. Wrap teardown in a broad try/catch — it's already best-effort and the `using` on `application` calls Dispose anyway.
- **Don't call `Bitmap.Clone(rect, format)` without an explicit cast** — overload resolution inside this project picks an unrelated generic `Clone<T>` extension first; the build error is misleading. (As of this commit the cast isn't needed because the right overload wins, but if the build breaks again on a `Clone` line, that's your suspect.)

## Why FlaUI and not Verify.Wpf or PercyIO

- **FlaUI** drives the real automation tree (UIA3), so it can interact with controls — not just snapshot static visuals. That makes it a better fit for an enterprise control library where many regressions are interaction-related.
- **Verify.Wpf** is text/markup snapshot oriented; complementary, can be added later if useful.
- **PercyIO** is cloud-hosted and adds a third-party dependency; deferred.

## Status

This is a **pilot** — two opt-in tests covering app launch and `NiceWindow` chrome capture. Future expansion: per-view screenshots driven by the existing TreeView item paths, image-diff baselines under `Snapshots/baseline/`, theme-toggle snapshots (Light + Dark), optional Verify.Wpf integration for non-visual XAML structure assertions.