# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

ATC.Net WPF is an enterprise-ready WPF control library framework providing MVVM infrastructure, theming, 160+ controls, and source generators. The project follows a four-tier architecture:

- **Atc.Wpf** - Core library (MVVM, layouts, value converters, helpers)
- **Atc.Wpf.Controls** - Atomic controls library (14 base controls, buttons, color controls, clocks (`AnalogClock`, the seven-segment `DigitalClock` and its `SevenSegment` digit; they follow the current time while loaded), selectors incl. the `CardSelector` card/tile picker, the `CheckListBox` multi-select list, the `MultiSelectComboBox` multi-select drop-down, the `TokenBox` tag input with suggestions, the tri-state check trees `CheckTreeView` / `TreeSelectComboBox`, the `CascaderComboBox` column-per-level cascader and the `MultiColumnComboBox` check-box grid drop-down; data display incl. the `ChipGroup` toggle-chip filter; the `DataGridFilter` attached behaviours that give the stock `DataGrid` Excel-style column filters, a search, a column chooser and layout persistence)
- **Atc.Wpf.Displays** - Hardware display controls drawn dot by dot for mirroring ESP32/Raspberry Pi displays: the `CharacterDisplay` HD44780 character LCD/VFD (1-40 x 1-4) with its `Hd44780Controller` emulator and `Pcf8574Hd44780Decoder` I2C backpack decoder, the `OledDisplay` monochrome OLED (SSD1306, SH1106, SSD1309, up to 132 x 64) with its `Ssd1306Controller` emulator and `Ssd1306I2cDecoder` I2C decoder, the `ColorGraphicDisplay` colour TFT/OLED (up to 480 x 480, RGB565 frames) with its `TftController` (ST7735, ST7789, ILI9341), `Ssd1331Controller` and `Ssd1351Controller` emulators, the `EPaperDisplay` SSD1680 e-paper (`Ssd1680Controller`), the `LedMatrixDisplay` / `SevenSegmentDisplay` MAX7219 displays (`Max7219Controller`; `SevenSegmentDisplay` uses the `SevenSegment` digit from Atc.Wpf.Controls), the `GraphicLcdDisplay` ST7920 LCD (`St7920Controller`, `St7920SerialDecoder`) and the `DisplayMirrorReceiver` that applies the display mirroring protocol (serial, TCP, UDP) from ESP32/Raspberry Pi firmware to them. Namespace `Atc.Wpf.Displays`, mapped into the `atc` XAML namespace
- **Atc.Wpf.Forms** - Form field controls (25+ labeled controls with validation, incl. `LabelCheckListBox`, `LabelMultiSelectComboBox`, `LabelChipGroup`, `LabelTokenBox`, `LabelTreeSelectComboBox`, `LabelCascaderComboBox` and `LabelMultiColumnComboBox`) plus the bare `TimeZonePicker` / `LabelTimeZonePicker` (system-locale picker, no hardware involvement)
- **Atc.Wpf.Components** - Composite components (dialogs, `InWindowDialogService` for the same dialogs as modal flyouts inside the window, viewers, settings, zoom browser, the `DualListSelector` transfer list with check-box style, one-way mode, locked items, a limit and virtualization)
- **Atc.Wpf.FontIcons** - Font-based icon rendering
- **Atc.Wpf.Theming** - Light/Dark theme infrastructure
- **Atc.Wpf.Network** - Network scanning and discovery controls (wraps external `Atc.Network` 2.x: scanner with options, mDNS/SSDP/ONVIF/NetBIOS columns, host events, presets, export and row actions; `ServiceDiscoveryView`, `OnvifCameraView`, `TracerouteView`, `WakeOnLanView`/`WakeOnLanButton`, `NtpTimeView`, `TcpClientView`/`UdpClientView`; VNC viewer with scale modes, view-only and clipboard)
- **Atc.Wpf.Hardware** - Hardware / system / inspection pickers (SerialPortPicker, UsbPortPicker, UsbCameraPicker, AudioInputPicker, AudioOutputPicker, DrivePicker, BluetoothDevicePicker, ProcessPicker, WindowPicker, NetworkAdapterPicker, PrinterPicker, DisplayPicker) with live device-state detection. TFM `net10.0-windows10.0.19041.0` (uses `Windows.Devices.Enumeration.DeviceWatcher` for the WinRT-backed pickers; polling for the rest)
- **Atc.Wpf.UndoRedo** - Undo/redo history view and keyboard behavior (wraps external `Atc.UndoRedo`)
- **Atc.Wpf.Controls.Sample** - Controls for building sample applications: the ready-made shell (`SampleApplication` application base, `SampleMainWindow`, `SampleExplorer` with `SampleCategory` tabs, filter and sample trees; see `src/Atc.Wpf.Controls.Sample/SampleExplorer_Readme.md`), the `SampleViewerView` and the side-panel editors

## Build Commands

```bash
# Build solution
dotnet build

# Build release (treats warnings as errors)
dotnet build -c Release

# Run all tests
dotnet test

# Run specific test project
dotnet test test/Atc.Wpf.Tests
dotnet test test/Atc.Wpf.Controls.Tests

# Run firmware (Python) tests
python -m pytest firmware/python

# Run single test by name
dotnet run --project test/Atc.Wpf.Tests -c Release --no-build -- --filter-method "*TestMethodName*"

# Run sample application
dotnet run --project sample/Atc.Wpf.Sample
```

## Architecture

### Source Generators and MVVM (from Atc.XamlToolkit)

The project uses C# source generators and MVVM infrastructure from the [Atc.XamlToolkit](https://github.com/atc-net/atc-xaml-toolkit) packages. `Atc.Wpf` itself does not reference them; `Atc.Wpf.Controls`, `Atc.Wpf.Forms`, `Atc.Wpf.Components` and the other UI packages do, and consuming apps reference `Atc.XamlToolkit` / `Atc.XamlToolkit.Wpf` directly to get the generators:

**Source Generator Attributes:**
- `[ObservableProperty]` - Generates properties with INotifyPropertyChanged from private fields
- `[RelayCommand]` - Generates ICommand properties from methods (supports async and CanExecute)
- `[DependencyProperty]` - Generates WPF dependency properties
- `[AttachedProperty]` - Generates WPF attached properties

**MVVM Base Classes:**
- `ViewModelBase`, `MainWindowViewModelBase`, `ViewModelDialogBase` - from `Atc.XamlToolkit.Mvvm`
- `RelayCommand<T>`, `RelayCommandAsync<T>` - from `Atc.XamlToolkit.Command`
- `ObservableObject` - from `Atc.XamlToolkit.Mvvm`

Source generators are provided by the `Atc.XamlToolkit` and `Atc.XamlToolkit.Wpf` NuGet packages.

### Control Architecture (Four-Tier)

**Base Controls** (`src/Atc.Wpf.Controls/Inputs/` and sibling folders): Atomic/primitive controls - single-purpose building blocks like NumericBox, IntegerBox, ToggleSwitch.

**Form Controls** (`src/Atc.Wpf.Forms/`): Labeled form field controls with validation display and mandatory indicators. Use for building data entry forms. Examples: LabelTextBox, LabelComboBox, LabelDatePicker.

**Composite Components** (`src/Atc.Wpf.Components/`): Higher-level components combining multiple controls - dialogs (InfoDialogBox, QuestionDialogBox, InputDialogBox, InputFormDialogBox, BasicApplicationSettingsDialogBox), viewers (JsonViewer, TerminalViewer), monitoring (ApplicationMonitorView), notifications (ToastNotification system, TrayIcon notification area icon), and settings panels.

### MVVM Base Classes

The MVVM base classes come from the `Atc.XamlToolkit` package (not a dependency of `Atc.Wpf`; reference it directly):
- `ViewModelBase` - Standard ViewModel base (from `Atc.XamlToolkit.Mvvm`)
- `MainWindowViewModelBase` - For main window ViewModels (from `Atc.XamlToolkit.Mvvm`)
- `ViewModelDialogBase` - For dialog ViewModels (from `Atc.XamlToolkit.Mvvm`)
- `RelayCommand<T>` / `RelayCommandAsync<T>` - Command implementations (from `Atc.XamlToolkit.Command`)

### Project Structure

```
src/
├── Atc.Wpf/                      # Core: MVVM, layouts, converters
├── Atc.Wpf.Controls/             # Atomic controls (base, buttons, colors)
├── Atc.Wpf.Displays/             # Hardware display controls, chip emulators, display mirroring
├── Atc.Wpf.Forms/                # Form field controls (Label*)
├── Atc.Wpf.Components/           # Composite components (dialogs, viewers, zoom)
├── Atc.Wpf.Controls.Sample/      # Sample app shell (SampleApplication, SampleMainWindow, SampleExplorer) and viewer
├── Atc.Wpf.FontIcons/            # Font icon support
├── Atc.Wpf.Theming/              # Theme infrastructure
├── Atc.Wpf.Network/              # Network scanning controls
├── Atc.Wpf.Hardware/             # Hardware / system pickers (12 pickers + Label* wrappers)
└── Atc.Wpf.UndoRedo/             # Undo/redo UI (HistoryView, keyboard behavior)
test/                             # XUnit test projects
firmware/                         # Display mirroring: protocol test vectors, Python module, Arduino library
sample/Atc.Wpf.Sample/            # Demo application
```

## Code Style

- Target: .NET 10.0, C# 14.0
- Nullable reference types enabled
- StyleCop, Meziantou, SonarAnalyzer enforced
- Release builds treat warnings as errors
- Every public member in `src/` has an XML doc comment: each library project turns CS1591 (missing XML comment) back on, so an undocumented public member fails the Release build. `[assembly: Atc.XamlToolkit.Mvvm.GenerateDocumentationDefault]` makes the source generators document generated members from the field/method docs; document a `[RoutedEvent]` field yourself
- Resource (translation) keys in `*.resx` end with the number of format parameters they take: no placeholders, no suffix (`SelectAll`, `CountOfMax`); `"{0} of max {1}"` is `CountOfMax2`; `"+{0}"` is `MoreCount1`. Add every key to the neutral, `.da-DK` and `.de-DE` files
- Package validation compares the public API with the latest release: CI passes the newest `v*` tag as `PackageValidationBaselineVersion` (nothing to bump in `src/Directory.Build.props`); locally run `dotnet pack -c Release -p:PackageValidationBaselineVersion=<last release>`

## Testing

- Framework: XUnit 3 with Microsoft Testing Platform
- WPF-specific testing: Xunit.StaFact (for STA thread tests)
- Test utilities: Atc.XUnit, AutoFixture, AwesomeAssertions, NSubstitute
- Global usings for test projects include: AutoFixture, AwesomeAssertions, NSubstitute, Xunit

### Test Projects Structure

| Test Project | Focus Area | Test Files |
|--------------|------------|-----------:|
| `Atc.Wpf.Tests` | Core library (Helpers, Extensions, Collections, ValueConverters, Serialization, JSON tree, Hotkeys, SVG loader, AutoGreyableImage and AutoGrey extensions, SvgImage brushes and drawing cache, managed markup extensions, culture change across UI threads), SVG rendering, drag and drop, animation and helper attached properties, navigation guards, clipboard, JSON formatting, observable collections with collection views, hotkey chords, SVG transform order, text-box monitoring, SVG render snapshots, auto-scroll list view coalescing, string validation rule, animation helpers (Animate attach, animation extensions), LRU cache, JSON formatter output, SVG robustness limits, SVG image elements, exception report, best-effort clipboard helper, culture-change subscription lifetime (lambdas tied to their owner), no network access from http(s) SVG sources, resource-manager cache under concurrent use, drag state per UI thread, hotkey bindings merged on load (rebind by description), configuration errors traced once (missing resx keys, invalid cultures, failing converters), slider mouse-wheel stepping | 178 |
| `Atc.Wpf.Controls.Tests` | Inputs (NumericBox incl. currency/unit format editing, UI-culture following, template and wheel, XY boxes, their lost-focus events and value metadata), CountrySelector, layouts, zoom (grid overlay, ruler, minimap), navigation (NavigationView), value converters, event args, theme-subscription leak tests, right-to-left keyboard, images and zoom, picker DataContext, sample readme lookup, zoom box content and animations, busy overlay culture, grid lines, toggle switch, Row panel, sample viewer view model, syntax colour adjuster and markdown code-block theming, multi-select controls (CheckListBox, MultiSelectComboBox, TokenBox, ChipGroup, Segmented multi-select, CheckTreeView and its check engine, TreeSelectComboBox, CascaderComboBox, MultiColumnComboBox, CardSelector), multi-select DataContext swaps, multi-select limit validation, collection leaks and selection-change storms, DockPanelPro tab groups, floating windows, layout persistence, LayoutChanged/auto-save and dock guides, DataGridFilter (distinct values, column filters, search, column chooser, layout round-trip, one refresh per apply, leaks, 100k rows, themed rows without binding errors during refresh, DataTable and ExpandoObject sources), selector/color picker DataContext bindings, Popover keyboard and unload, Timeline item inheritance, zoom ruler label layouts reused across frames, missing-style check for every control with a default style, sample file index, RangeSlider (bounds in any order, no recursion, bound range written back, thumbs, dragging, keyboard), Breadcrumb (overflow menu, keyboard activation, cleared items), Alert (severity colours, overrides, close, pulse stops when unloaded), clocks (AnalogClock hand angles and AM/PM, DigitalClock digits, SevenSegment digit-to-segment mapping, timer running only while loaded, config data round-trip), sample shell (SampleExplorer tabs, trees, filter and badges, SampleTreeFilter rules, SampleMainWindow, SampleApplication start-up rules, sample viewer class names from the type) | 114 |
| `Atc.Wpf.Displays.Tests` | CharacterDisplay (buffer DDRAM/CGRAM and 20x4/40x4 addressing, A00/A02 ROM tables, HD44780 instruction set, PCF8574 decoder (4-bit init, enable edges, backlight bit, read cycles, return to 8-bit, pin maps), dot-matrix rendering and measure, colour presets, cursor blink timer lifecycle, cross-thread redraws, automation text), graphic displays (monochrome and colour frame buffers, OledDisplay and ColorGraphicDisplay rendering, SSD1306/SH1106/SSD1309 emulator and mapping, SSD1306 I2C decoder, ST7735/ST7789/ILI9341, SSD1331 and SSD1351 emulators, trace-once warnings, inert after dispose), display mirroring (protocol framing and golden vectors, DisplayMirrorReceiver, serial/TCP/UDP transports, statistics), e-paper (SSD1680), MAX7219 (LED matrix, seven-segment) and ST7920 (graphic LCD, serial decoder), assembly compliance | 31 |
| `Atc.Wpf.Forms.Tests` | Extractors, Factories, Helpers, FontPicker storage, settings POCOs, Label* controls (incl. lost-focus events and culture selector forwarding), EndpointBox, ColorPicker, decimal box and toggle switch events, factory range bounds, control-data extractor, date text culture, multi-select Label controls (LabelCheckListBox, LabelMultiSelectComboBox, LabelChipGroup, LabelTokenBox, LabelTreeSelectComboBox, LabelCascaderComboBox, LabelMultiColumnComboBox), Label multi-select DataContext swaps and validation, model-generated multi-select fields, PropertyGrid, colour/font picker dialog owner and work-area fit, dialog owner helper, dialog button bar (Yes/No/Cancel, default button, Escape), dialog button brushes from the current theme, password box values in input forms, mandatory colour/font pickers, selector lost-focus valid event, AdvancedFontPicker (typeface lists, weight snapping, sizes, family filter, quick toggles, recent families) | 49 |
| `Atc.Wpf.Components.Tests` | Flyouts, DualListSelector pieces, dialogs, Terminal events/routing, ANSI parser, terminal term matching, line highlight and trimming, flyout service close-all, DualListSelector reorder, monitor dispose, Zoom browser, tray icon, value converters, right-to-left viewers, JSON property template selector, flyout first open and host hit testing, DualListSelector item counts, dialog owner, toast area registry, topmost flyout Escape, print page size, DualListSelector check-box style, one-way mode, locked items, limit, grouping and virtualization, printing and input dialogs without a main window, DualListSelector bulk transfers and SelectedKeys, busy indicator service, flyouts added through FlyoutService, print preview re-pagination, toast close-on-click and hover pause, dialog owner resolution, dialog text wrapping and auto height, per-dialog icon copies, dialog details area, "Don't ask again" and Ctrl+C text, exception dialog, settings dialog save failures, restartable background loop (TerminalViewer runs its queue only while loaded), InputFormDialogBox (form, header, group boxes, sizing, OK only when valid), centred flyouts sized to their content, flyout views shown as passed, in-window dialogs (every IDialogService method, input form, font picker, cancellation, stacking, right-to-left, windowed fallback), flyout default styles without app resources, views passed to FlyoutService keep their DataContext | 70 |
| `Atc.Wpf.Hardware.Tests` | Device/polling services, picker controller, pickers, frame slot, models, localization, RefreshAsync UI-thread marshalling, watcher update snapshots, polling service UI-thread marshalling, live preview dispose, process main-window lookup, default-constructed picker lifetime, watcher start gate, waveform buffers, camera preview sizing, device session resync failures observed, process/window lists polled only while the drop-down is open, DeviceError event, contract tests run against all twelve pickers and all twelve Label* wrappers (lifecycle, auto-select, DeviceLost, validation incl. in-use, settings forwarding), serial in-use probe | 48 |
| `Atc.Wpf.Network.Tests` | Network ViewModels, scanner progress, scan cancellation on dispose, scanner options and factory (fake `IIPScanner`), host/scan events and `HostFilter`, discovery fields (device name, model, services, ONVIF), port presets, CIDR/local-network/IPv6 ranges, live sorting, settings round-trip, CSV/JSON export, row actions, offline hosts, ranges and virtual adapters, service discovery, ONVIF camera reading, traceroute, Wake-on-LAN (view and button), NTP, TCP/UDP clients, VNC viewer (retry, view-only, clipboard, scale mapping), VNC connection service, VNC pointer masks and layout-independent keyboard, value converters, CSV formula neutralisation and failed export writes, VNC connect dialog (password box, culture-independent keys, non-blocking callbacks), Wake-on-LAN button ignores a second send while sending | 34 |
| `Atc.Wpf.Theming.Tests` | Compliance, value-converter functional tests, Windows theme sync, high-contrast theme generation, DatePicker style, NiceWindow backdrop and ThemeMode, window placement save and upgrade, WindowButtonCommands parent, ThemeManagerHelper fallbacks, NiceWindow culture and title margin, validation error template, watermarks, window placement restore and per-monitor title-bar check, StatusBar in a plain Window, TabItem focus visual, ModeChanged does not keep subscribers alive, TryGet theme color/brush lookups, CheckBox/RadioButton/Slider helper templates (every state trigger reads its own helper properties), ClipBorder layout, clip and rendering, TransitioningContentControl | 29 |
| `Atc.Wpf.UndoRedo.Tests` | UndoRedo UI (HistoryViewModel + compliance) | 2 |
| `Atc.Wpf.FontIcons.Tests` | FontIcons assembly marker + per-icon-set enum smoke tests, converter parameter parsing, Image* icon rotation and shared vector sources | 6 |
| `Atc.Wpf.UiTests` | FlaUI desktop tests against the sample app incl. drag-and-drop auto-scroll, dialog placement, long-text dialogs (wrapping, light/dark, right-to-left, auto height, Yes/No/Cancel, default button, details, "Don't ask again", Ctrl+C, input form sizing) the owned colour/font picker dialogs and the exception dialog sample (incl. unhandled exceptions that end the app), toasts, NiceWindow title bar, the multi-select controls, DockPanelPro floating the DataGrid column filter and a smoke test that opens every sample without binding errors (`Category=UI`, excluded from the default CI run), in-window dialogs (no new window, answer, Escape) | 31 |

> Counts are files containing tests; total executed tests are higher (e.g. `Atc.Wpf.Tests` runs about 1,500 tests).
> Source generators are consumed via the [`Atc.XamlToolkit`](https://github.com/atc-net/atc-xaml-toolkit) NuGet packages; this repo does not ship its own generators.

### Running Tests

```bash
# Run all tests
dotnet test

# Run a specific test project (Microsoft Testing Platform — use `dotnet run`)
dotnet run --project test/Atc.Wpf.Tests -c Release --no-build
dotnet run --project test/Atc.Wpf.Forms.Tests -c Release --no-build

# Run single test by name (filter via xUnit)
dotnet run --project test/Atc.Wpf.Tests -c Release --no-build -- --filter-method "*MethodName*"
```

## Sample Application

The sample app (`sample/Atc.Wpf.Sample/`) serves as a control explorer with 11 main categories. It is built on the shell in `Atc.Wpf.Controls.Sample`: `App` is a `SampleApplication` and `MainWindow.xaml` is a `SampleMainWindow` that lists one `SampleCategory` per category (`Key` names the automation IDs `Tab{Key}` and `StvSamples{Key}`; the UI tests read the categories from that file). Add a new category there, in tab order:

| Category | TreeView | Samples |
|----------|----------|---------|
| Wpf | `SamplesWpfTreeView.xaml` | Commands, Layouts, Media, Markup, etc. |
| Wpf.Controls | `SamplesWpfControlsTreeView.xaml` | Base controls, Buttons, Clocks (AnalogClock, DigitalClock, SevenSegment), Colors, Layouts, Selectors (incl. CardSelector, CascaderComboBox, CheckListBox, CheckTreeView, MultiColumnComboBox, MultiSelectComboBox, TokenBox, TreeSelectComboBox), DataDisplay (incl. Chip, ChipGroup), DataGrids (DataGridFilter) |
| Wpf.Displays | `SamplesWpfDisplaysTreeView.xaml` | CharacterDisplay, ColorGraphicDisplay, DisplayMirror, EPaperDisplay, GraphicLcdDisplay, LedMatrixDisplay, OledDisplay, SevenSegmentDisplay |
| Wpf.Forms | `SamplesWpfFormsTreeView.xaml` | Label controls (incl. LabelCascaderComboBox, LabelCheckListBox, LabelMultiSelectComboBox, LabelChipGroup, LabelTokenBox, LabelTreeSelectComboBox), Selectors, Pickers (incl. TimeZonePicker / LabelTimeZonePicker) |
| Wpf.Components | `SamplesWpfComponentsTreeView.xaml` | Dialogs, Viewers, Monitoring, Notifications (incl. TrayIcon), Capture, Printing, Progressing, Selectors, Flyouts, Zoom |
| Wpf.Network | `SamplesWpfNetworkTreeView.xaml` | NetworkScanner, OnvifCamera, ServiceDiscovery, VncViewer, Clients (TcpClient, UdpClient), Diagnostics (NtpTime, Traceroute, WakeOnLan), value converters |
| Wpf.Hardware | `SamplesWpfHardwareTreeView.xaml` | SerialPortPicker, UsbPortPicker, UsbCameraPicker, AudioInputPicker, AudioOutputPicker, DrivePicker, BluetoothDevicePicker, ProcessPicker, WindowPicker, NetworkAdapterPicker, PrinterPicker, DisplayPicker |
| Wpf.Theming | `SamplesWpfThemingTreeView.xaml` | Themed standard WPF controls |
| Wpf.SourceGenerators | `SamplesWpfSourceGeneratorsTreeView.xaml` | Generator demos |
| Wpf.FontIcons | `SamplesWpfFontIconsTreeView.xaml` | Icon rendering |
| Wpf.UndoRedo | `SamplesWpfUndoRedoTreeView.xaml` | UndoRedoService, UndoRedoHistoryView |

## Control Readme Conventions

Each control can have a `_Readme.md` file that is displayed in the sample app's **Readme** tab. Three naming conventions exist:

| Convention | Placement | Scope | Example |
|---|---|---|---|
| `[Control]_Readme.md` | Next to the control `.cs` file | Single control | `Badge_Readme.md` |
| `@Readme.md` | In a category folder under `docs/` or `src/` | All controls in that namespace | `docs/Theming/@Readme.md` |
| `Readme.md` | In a library/folder root | Multiple controls in that folder | `src/Atc.Wpf.Forms/Readme.md` |

### Sample App Readme Discovery

The sample app auto-discovers readme files via `SampleViewerViewModel.LoadAndRenderMarkdownDocumentIfPossible()` in `src/Atc.Wpf.Controls.Sample/SampleViewerViewModel.cs`. It pre-caches all `*.md` files from the solution root and matches the end of the path case-insensitively, starting at a folder boundary (`MarkdownFileLookup`, so `ScrollViewer_Readme.md` never matches `ZoomScrollViewer_Readme.md`), with this fallback chain:

1. `docs/{section}/{className}@Readme.md` — section-based lookup
2. `{className}_Readme.md` — control-specific readme next to source
3. `docs/{namespace_folder}/@Readme.md` — namespace-based category fallback (also tries singular form)
4. `{classViewName}_Readme.md` — view class name fallback

### When Creating a New Control

Place a `[ControlName]_Readme.md` file next to the control's `.cs` file. Follow the template pattern from `GridEx_Readme.md`: emoji title, Overview, Namespace, Usage examples, Properties table (Property/Type/Default/Description), Notes, Related Controls, Sample Application path.

### Important

- The `@Readme.md` and `_Readme.md` naming patterns are **intentional** for sample-app readme pickup — do NOT rename them
- `@` prefix files serve as category-level documentation shared across a namespace
- Controls without their own `_Readme.md` fall back to category or folder-level readmes

## Key Documentation

- MVVM guide: `docs/Mvvm/@Readme.md`
- Source generators: `docs/SourceGenerators/ViewModel.md`
- Theming controls: `docs/Theming/@Readme.md`
- Font icons: `docs/FontIcons/@Readme.md`
- Layout controls: `docs/Layouts/@Readme.md`
- Data display controls: `docs/DataDisplay/@Readme.md`
- DataGrid filters, search, column chooser and layout: `src/Atc.Wpf.Controls/DataGrids/DataGridFilter_Readme.md`
- Display controls: `docs/Displays/@Readme.md`, `src/Atc.Wpf.Displays/Readme.md`
- Display mirroring: `docs/Displays/DisplayMirrorProtocol.md`, `src/Atc.Wpf.Displays/DisplayMirror_Readme.md`, `firmware/python/README.md`, `firmware/arduino/AtcDisplayMirror/README.md`
- Hardware pickers: `docs/Hardware/@Readme.md`, `src/Atc.Wpf.Hardware/Readme.md`
- Form controls: `src/Atc.Wpf.Forms/` (LabelTextBox, LabelComboBox, etc.)
- Base controls: `src/Atc.Wpf.Controls/Inputs/` (NumericBox, IntegerBox, etc.)
- Composite components: `src/Atc.Wpf.Components/` (Dialogs, Viewers, Monitoring, Notifications, Zoom)
- Unhandled exceptions in apps: `src/Atc.Wpf.Components/Dialogs/ExceptionDialog_Readme.md` (`ExceptionDialog`, `ExceptionReport`)
- Undo/redo UI: `src/Atc.Wpf.UndoRedo/` (history view, keyboard behavior) — wraps external `Atc.UndoRedo` NuGet

## Learned rules

Rules saved with atc-reflect from corrections made while working.

- After finishing the core parts, continue directly to the next phase (e.g. 02-older-controls) without waiting for confirmation, and keep going through all remaining phases.
- Do not run e2e tests after making fixes; the developer will verify the changes manually.
