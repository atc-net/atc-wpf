# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- **`DisplayPicker`** in `Atc.Wpf.Hardware` — picker for connected
  monitors/displays. Uses Win32 `EnumDisplayMonitors` + `GetMonitorInfo`
  (P/Invoked from user32.dll, with the same DllImport pattern as
  WindowPicker) to capture each monitor's `DeviceName`, full
  `Bounds`, `WorkingArea`, and the system-primary flag. Polls every
  2 s for hot-plug. The system primary monitor is rendered with a ★
  followed by its resolution (e.g. `\\.\DISPLAY1 ★ (1920×1080)`).
  Strings localised for `en-US` / `da-DK` / `de-DE`. Four new model
  tests cover constructor, primary/non-primary `ToString` formatting,
  and INPC.
- **`NetworkAdapterPicker` / `PrinterPicker`** in `Atc.Wpf.Hardware` —
  two more polling-based system pickers. `NetworkAdapterPicker`
  enumerates `NetworkInterface.GetAllNetworkInterfaces()` (with
  `IncludeLoopback=false` by default to hide loopback adapters) and
  exposes a *reactive* `OperationalStatus` so consumers can bind live
  up/down state without re-selecting. `PrinterPicker` enumerates
  `LocalPrintServer.GetPrintQueues(Local | Connections)`, including
  the system default printer (rendered with a ★ in the dropdown) and
  the queue's `IsShared` / `IsLocal` / `QueueStatus`. Both poll every
  2 s and route through the same `DeviceState` plumbing as the rest
  of the family. Strings localised for `en-US` / `da-DK` / `de-DE`.
  Eight new model tests cover both POCOs.
- **`ProcessPicker` / `WindowPicker`** in `Atc.Wpf.Hardware` —
  inspection-style pickers for hooking debuggers, automation, capture
  targets, or "attach to existing" flows. `ProcessPicker` enumerates
  `Process.GetProcesses()` (with `OnlyWithMainWindow=true` by default to
  hide background services); `WindowPicker` enumerates top-level windows
  via `EnumWindows` P/Invoked from `user32.dll` (with
  `OnlyVisibleWithTitle=true` by default). Both poll every 2 s instead of
  using DeviceWatcher (no OS hot-plug API for these). Same `DeviceState`
  surface as the hardware pickers — when a tracked process exits or
  window is destroyed, `DeviceLost` fires and the bound `Value` flips to
  `Disconnected`. `RunningProcessInfo` and `TopLevelWindowInfo` POCOs
  expose typed metadata (PID, MainWindowTitle, MainModulePath, HWND,
  ClassName, owning process). Strings localised for `en-US` / `da-DK`
  / `de-DE`. Eight new model tests cover both POCOs.
- **`BluetoothDevicePicker`** in `Atc.Wpf.Hardware` — paired-classic
  Bluetooth picker built on the same `DeviceWatcherHost` plumbing as the
  serial/USB/audio pickers, using `BluetoothDevice.GetDeviceSelectorFromPairingState(true)`
  as the AQS selector. Surfaces `IsConnected` and `IsPaired` on the
  `BluetoothDeviceInfo` model and renders connected entries with a ●
  bullet in the dropdown. Strings localised for `en-US` / `da-DK` /
  `de-DE`. BLE-only and unpaired-discovery scenarios are deferred to v2
  (require additional capability declarations).
- **`DrivePicker` / `TimeZonePicker`** in `Atc.Wpf.Hardware` — round out
  the picker family with two pure-managed pickers. `DrivePicker` enumerates
  `System.IO.DriveInfo.GetDrives()` (Fixed / Removable / Network / CDRom /
  Ram) and polls every 2 s for hot-plug — same `DeviceState` UX, same
  `ValueChanged` / `DeviceLost` / `DeviceReconnected` / `DeviceStateChanged`
  routed events, same Label* wrapper pattern. `TimeZonePicker` is the slim
  variant — `TimeZoneInfo.GetSystemTimeZones()` bound directly, no service,
  no hot-plug, no state, just a clean ComboBox with offset + display name.
  `LabelTimeZonePicker` adds mandatory validation. Strings localised for
  `en-US` / `da-DK` / `de-DE`. New `DiskDriveInfo` model (named to avoid
  the clash with `System.IO.DriveInfo`).
- **`AudioInputPicker` / `AudioOutputPicker`** in `Atc.Wpf.Hardware` —
  microphone and speaker pickers built on the same `DeviceWatcherHost`
  plumbing as the existing pickers, using `DeviceClass.AudioCapture` and
  `DeviceClass.AudioRender`. Both surface live state (Available / InUse /
  Disconnected / JustConnected), raise `ValueChanged` / `DeviceLost` /
  `DeviceReconnected` / `DeviceStateChanged`, and ship labeled wrappers
  (`LabelAudioInputPicker`, `LabelAudioOutputPicker`) with mandatory +
  disconnected-device validation. The system default endpoint is rendered
  with a ★ in the dropdown. New `AudioDeviceInfo` model and `AudioDeviceKind`
  enum. Strings localised for `en-US` / `da-DK` / `de-DE`.
- **USB `ClassFilter` → AQS translation** in `Atc.Wpf.Hardware` —
  `UsbDeviceClassFilter` flags (Hid / Imaging / Audio / Printer / MassStorage
  / Communication) now translate to a real `System.Devices.InterfaceClassGuid`
  AQS query OR-joining the relevant device interface GUIDs, and the
  `DeviceWatcher` rebuilds when the filter changes. Previously the filter
  was stored but never applied — narrowing happens at the OS level now.
- **`Atc.Wpf.Hardware` package** — new assembly providing three hardware
  picker controls (`SerialPortPicker`, `UsbPortPicker`, `UsbCameraPicker`)
  and their labeled wrappers (`LabelSerialPortPicker`, `LabelUsbPortPicker`,
  `LabelUsbCameraPicker`) with **live device-state detection**. Connect /
  disconnect / in-use are reflected in the dropdown via colour-coded status
  dots (green / amber / red) — the user never has to click *Refresh* and
  never picks a device that's silently unusable. When a *bound* `Value`
  device disconnects, the picker raises `DeviceLost`, shows an inline
  warning, and preserves the selection so reconnect rebinds silently
  (`AutoRebindOnReconnect` default-on); auto-clear is opt-in via
  `ClearValueOnDisconnect`. Hot-plug detection is shared infrastructure
  via `Windows.Devices.Enumeration.DeviceWatcher` (no `WM_DEVICECHANGE`
  plumbing). User-visible strings localised for `en-US` / `da-DK` /
  `de-DE`. Targets `net10.0-windows10.0.19041.0` (Windows 10 May 2020 / 2004
  and later, all Windows 11). See `docs/Hardware/@Readme.md` and
  `docs/roadmap-pickers.md`.
- **`ColorEditorMode` on the FontPicker family** — new `FontColorEditorMode`
  enum (`WellKnownColorSelector` / `ColorPicker`) lets consumers pick between
  an inline named-color dropdown and the dialog-based color picker for the
  Foreground / Background fields inside `AdvancedFontPicker`. `LabelFontPicker`,
  `FontPicker`, and `FontPickerDialogBox` default to `WellKnownColorSelector`
  so opening a font picker no longer cascades into a *second* modal dialog when
  the user edits a colour; standalone `AdvancedFontPicker` keeps the dialog
  variant by default. Each level exposes the property as a DP so consumers can
  override (e.g. set `ColorEditorMode="ColorPicker"` if nested dialogs are
  desired). Backed by a new two-way `BrushToColorNameValueConverter` in
  `Atc.Wpf.ValueConverters` that maps between `SolidColorBrush` and the
  English-invariant well-known colour key. The hidden well-known selector uses
  `DropDownFirstItemType="Blank"` so theme-default brushes whose colour does
  not match any named entry don't get silently rewritten on load.
- **FontPicker control family** in `Atc.Wpf.Forms` — a four-tier font selection
  stack mirroring the existing ColorPicker trinity:
  - `FontPicker` (compact): `"Aa"` sample rendered in the selected font + family
    name + size in the host UI font + an edit button that opens the dialog.
  - `FontPickerDialogBox`: `NiceDialogBox` wrapper around `AdvancedFontPicker`
    with OK / Cancel and full TwoWay round-trip of the selection.
  - `AdvancedFontPicker`: full editor with a responsive star-column layout —
    family list with type-ahead search, recently-used items, virtualised
    all-fonts view; Weight / Style / Stretch lists derived dynamically from
    `family.FamilyTypefaces` (only valid combinations) with snap-to-closest
    when switching families; size as `IntegerBox` + preset list with custom
    sizes auto-inserted; Foreground / Background `LabelColorPicker`s with
    inline swatch; Bold / Italic / Underline / Strikethrough quick toggles
    bound bidirectionally; editable preview text + multi-sample preview area
    with theme-aware default colours and a WCAG contrast-warning glyph.
  - `LabelFontPicker`: form-tier wrapper mirroring `LabelColorPicker`.
  - `FontDescription` value type bundles all appearance properties (Family,
    Size, Weight, Style, Stretch, Foreground, Background, TextDecorations)
    with `ApplyTo` / `FromControl` helpers so a picker round-trips cleanly
    against any `Control`.
  - Granular `Show*` / `IsEnabled*` DPs per section, plus a 🔒 indicator on
    disabled `GroupBox`es. Full `AutomationProperties.Name` + ToolTips +
    keyboard navigation (Tab / arrow / type-ahead).
  - Recents persisted via the new `IFontPickerStorage` abstraction (default:
    in-memory, capped at 8); apps wanting cross-restart persistence assign
    their own implementation to `FontPickerStorage.Current`.
  - Translation keys added in `en` / `da` / `de` for `FontPicker`, `FontFamily`,
    `FontSize`, `FontWeight`, `FontStyle`, `FontStretch`, `Preview`,
    `Foreground`, `Background`, `Bold`, `Italic`, `Underline`, `Strikethrough`,
    `Locked`, `ContrastWarning`.
- **`ToggleButton` theme styles** in `Atc.Wpf.Theming`, mirroring the existing
  `Button.xaml` style hierarchy. `AtcApps.Styles.ToggleButton` (+ `.Small` /
  `.Large`) carries an `IsChecked` trigger that swaps Background / BorderBrush
  to the existing Pressed brushes; chromeless variants apply a subtle background
  on checked. Seven Bootstrap colours (Default / Primary / Secondary / Success /
  Danger / Warning / Info) ship in solid + outline flavours and three sizes —
  outline variants additionally swap Foreground to white when checked for
  legibility. The implicit `TargetType="ToggleButton"` style is registered in
  `Controls.xaml`, so any unstyled `ToggleButton` in a consuming app picks it up
  automatically. New "Theming → Input - Button → ToggleButton" sample page
  replaces a previously-disabled placeholder.
- **`ApplicationMonitorView` context menu** with Copy / Copy Selected / Copy All
  commands. New `EnableContextMenu` DP gates the menu; `[RelayCommand]` handlers
  carry `CanExecute` predicates so redundant items hide rather than grey out.
  The `ListView` switches to `SelectionMode="Extended"` with a `SelectionChanged`
  shim that bridges multi-selection into the VM. `CopyAllToClipboard` /
  `CopySelectedToClipboard` resource strings added in `en` / `da` / `de`.

### Fixed

- **`CultureManager.UiCultureChanged` is now a weak event.** Long-lived static
  subscriptions no longer root WPF controls, removing a systemic memory leak.
  Existing subscribers (`+= OnUiCultureChanged`) continue to work unchanged.
  Lambda subscribers with closures must keep the delegate referenced themselves,
  as documented on the event.
- **`TerminalViewer.Dispose` no longer risks a UI-thread deadlock.** The previous
  `Wait()` on a task that posts back to the dispatcher was replaced with a
  cancel + dispose-on-continuation pattern. The `Channel<>` writer is now
  completed during `Dispose`, and `Messenger.Default.UnRegister(this)` is called
  to release subscriptions promptly.
- **`Dispatcher.Invoke` paths in `TerminalViewer` short-circuit via `CheckAccess()`**,
  eliminating an unnecessary marshaling hop when already on the UI thread.
- **SVG `TextShape` parse failures are now logged** via `Trace.TraceError`
  instead of being silently swallowed, matching `ImageShape`.
- **`Messenger.Default.Register` calls are paired with `UnRegister`** in `ZoomBox`
  and `ApplicationMonitorView` (registration moved to `Loaded`/`Unloaded`),
  preventing dead controls from continuing to receive messages until the next
  `Cleanup()`.
- **`ObservableDictionary<TKey, TValue>.CopyTo` is now implemented** with proper
  argument validation; the type can now be passed to APIs expecting
  `ICollection<KeyValuePair<,>>`.
- **`RenderFlagIndicatorTypeToVisibilityValueConverter.ConvertBack` and
  `ZoomMiniMapClampMultiValueConverter.ConvertBack`** now throw
  `NotSupportedException` (matching the WPF idiom for one-way converters)
  instead of `NotImplementedException`.

### Performance

- **`SolidColorBrushHelper.GetBrushFromString`** is now O(1) via a reverse-lookup
  dictionary instead of an O(n) LINQ scan over the localized brush dictionary.
- **`VirtualizingStaggeredPanel`** reuses a `HashSet<int>` scratch buffer for
  visible-index tracking and replaces `columnHeights.Max()` with a manual loop,
  removing per-measure-pass allocations.
- **`GridEx.OnRowsChanged` / `OnColumnsChanged`** now share a single
  `GridLengthConverter` instead of allocating one per parse.
- **`BitmapImageFactory`** freezes absolute-URI `BitmapImage` results
  (`CacheOption.OnLoad` + `Freeze()`), making them safe to share across threads
  and avoiding repeated decode work.
- **`DebounceDispatcher`** reuses its `DispatcherTimer` across calls when the
  priority + dispatcher match, removing per-call timer + closure allocations
  from `Debounce`/`Throttle`.
- **`KeepAliveTimer.Nudge`** dropped a redundant lock — both `Nudge` and the
  timer tick run on the UI thread already, so the lock only serialised zoom
  interactions.
- **`LabelTextInfo.EnableCopyToClipboard`** caches the copy `ContextMenu` and
  swaps it in/out, instead of allocating a fresh menu + bindings on every flip.
- **`ClipBorder`** caches the combined "border ring" geometry across renders.
  The two `StreamGeometry`s were already cached in `ArrangeOverride`, but the
  `Geometry.Combine(...)` call in `OnRender` (3 code paths) was rebuilding the
  ring on every render. The new `borderRingGeometryCache` field is invalidated
  alongside the inputs and rebuilt lazily; brush-only changes (which trigger
  `OnRender` but not `ArrangeOverride`) now reuse the cached ring.

### Documentation

- **Project READMEs** added for the previously-bare projects: `Atc.Wpf`,
  `Atc.Wpf.Controls`, `Atc.Wpf.Components`, `Atc.Wpf.Theming`, `Atc.Wpf.FontIcons`.
  Each gives a short orientation, a "what lives here" map, an install snippet,
  and links into `docs/`.
- **Top-level doc indexes** added: `docs/Forms/@Readme.md` and
  `docs/Components/@Readme.md` — conceptual overviews that cover the deferred
  validation pattern, dispose / Loaded / Unloaded conventions, and pointers to
  per-control readmes.
- **Sample app README** added at `sample/Atc.Wpf.Sample/Readme.md` — quick-start,
  TreeView category map, search syntax, theme-switching tips, and a how-to for
  contributing a new sample.
- **`docs/SourceGenerators/ViewModel.md` TODOs** resolved — `ShowData` opens an
  `InfoDialogBox` with formatted person info; `CanSaveHandler` validates
  first/last/age non-empty.
- **`NumericBox_Readme.md`** added — full reference for the base numeric input
  class (value/range/interval, formatting/culture, input behaviour, spin-button
  layout, routed events).
- **`ThicknessBox_Readme.md`** added — short reference for the four-up
  `Thickness` editor.
- **`CONTRIBUTING.md`** added at the repo root — covers build / test / PR
  checklist, project structure, and links to the ATC umbrella guidelines.
- **`CODE_OF_CONDUCT.md`** added — adopts Contributor Covenant 2.1 by reference.
- **`SECURITY.md`** added — documents private vulnerability reporting via the
  GitHub Security tab and the supported-version policy.
- **`CLAUDE.md`** test counts and Microsoft Testing Platform invocation refreshed
  (`dotnet run --project ...` instead of `dotnet test <dir>`); the `Components`
  and `FontIcons` test rows were added.
- **DocFX docs site scaffold** added — `docfx.json` at the repo root pulls API
  metadata from all 8 source projects, drops the generated reference under
  `docs/api/`, and builds the existing `docs/` markdown plus a new `index.md`
  landing page with a curated section index. `toc.yml` (root) wires Home /
  Documentation / API Reference / GitHub; `docs/toc.yml` orders the left nav
  by category. `_site/` and `docs/api/` are gitignored. Local preview:
  `dotnet tool install -g docfx && docfx docfx.json --serve`.
- **Architecture diagram** added to `README.md` under the "🎯 Four-Tier Architecture"
  section. Mermaid graph (renders natively on GitHub, no PNG/SVG asset to maintain)
  showing `Atc.Wpf` (core) → `Controls` → `Forms` → `Components` spine plus
  `Theming` / `FontIcons` / `Network` / `UndoRedo` side packages and the upstream
  `Atc` + `Atc.XamlToolkit` deps. Color-coded by role.
- **MVVM migration guide** added at `docs/Mvvm/Migration.md`. Covers all four
  generator attributes (`[ObservableProperty]`, `[RelayCommand]`,
  `[DependencyProperty]`, `[AttachedProperty]`) with before/after snippets, a
  quick-reference cookbook, a pre-flight checklist, and notes on known
  limitations (e.g. the DP generator's missing XML-doc forwarding). Linked from
  `docs/Mvvm/@Readme.md` and added to `docs/toc.yml`.

### Removed

- **Empty `src/Atc.Wpf.SourceGenerators/` and `test/Atc.Wpf.SourceGenerators.Tests/`
  directories** — both contained zero files, were not referenced from
  `Atc.Wpf.slnx`, and would have implied a generator project that doesn't exist
  here (the actual generators ship via `Atc.XamlToolkit` / `Atc.XamlToolkit.Wpf`
  NuGet packages). `CLAUDE.md` caveat updated accordingly.

### Tests

- **`Atc.Wpf.Benchmarks`** project added (referenced from `Atc.Wpf.slnx` under
  a new `/benchmark/` solution folder) — BenchmarkDotNet 0.13.4 pilot. Lives
  outside `test/` so `dotnet test` never picks it up, but is part of the
  solution so a plain `dotnet build` keeps it from rotting. First benchmark
  (`SolidColorBrushHelperBenchmarks`, `[MemoryDiagnoser]`, parameterised on
  four colour names) pins the post-fix O(1) reverse lookup in
  `SolidColorBrushHelper.GetBrushFromString` so future regressions surface
  immediately. Verified locally: ~10–12 ns/op with **0 allocations** per call
  on a 12th-gen i9. `BenchmarkConfig.Create()` overrides the BDN toolchain to
  `net10.0-windows` (the auto-generated boilerplate project otherwise defaults
  to plain `net10.0` and fails to restore against our WPF-targeted assembly
  with NU1201). `benchmark/Directory.Build.props` relaxes the strict analyser
  set (CA1707/CA1304/CA1822/SA1623/CA1812/MA0051) because BenchmarkDotNet
  conventions clash with them. `benchmark/Atc.Wpf.Benchmarks/Readme.md`
  documents how to run (`dotnet run -c Release --project benchmark/Atc.Wpf.Benchmarks -- --filter '*'`),
  filtering, the WPF/toolchain gotcha (CLI `--job short` adds a *second* job
  with the default toolchain that breaks; use job attributes or
  `--iterationCount`/`--launchCount`/`--warmupCount` instead), and the
  convention to add a benchmark whenever a measurable hot-path fix lands.
- **`Atc.Wpf.UiTests`** project added (referenced from `Atc.Wpf.slnx`) — pilot
  visual / UI regression harness on **FlaUI 5.0.0** (`FlaUI.Core` + `FlaUI.UIA3`).
  `SampleAppPath.Resolve()` walks up to the repo root to find the sample exe.
  Two opt-in tests (`[Trait("Category","UI")]` + `[Fact(Skip="...")]` so they
  stay out of headless CI by default):
  - `SampleAppSmokeTests.Sample_app_launches_and_shows_main_window` — launches
    the sample, asserts a non-empty title, captures the main window to
    `bin/<Config>/net10.0-windows/Snapshots/sample-app-main-window.png`.
  - `NiceWindowSnapshotTests.NiceWindow_chrome_is_active_and_titlebar_strip_can_be_snapshotted` —
    asserts `mainWindow.ClassName == "NiceWindow"` (proves the custom themed
    chrome is in play, not a fallback `Window`), maximizes the window so it's
    deterministically on top, captures the full window plus a cropped 40-px
    title-bar strip — the highest-risk theming surface (accent colors, system
    buttons, title text), isolated for low-noise future image diffs.
  Teardown was hardened in both tests: `WaitWhileMainHandleIsMissing` races
  with `Close()` on fast machines and FlaUI throws a wrapped `System.Exception`,
  so cleanup is now wrapped in a broad try/catch. The Readme documents the
  Z-order trap (`Capture()` clips to element bounds but reads from the screen
  as backing store, so the window must be foregrounded; `SetForeground()`
  alone is blocked by Windows for non-foreground processes — maximize via the
  Window pattern is the workaround), the `Bitmap.Clone` overload trap, and the
  pattern for new tests.
- **`Atc.Wpf.FontIcons.Tests`** project added (referenced from `Atc.Wpf.slnx`).
  Adds an `IAssemblyMarkerAtcWpfFontIcons` to the source assembly and a small
  enum smoke-test suite that asserts every icon set defines `None = 0` and
  exposes more than just `None` — catches generator regressions across all
  ten icon sets (FontAwesome 5/7 solid/regular/brand, Bootstrap, Material,
  Weather, IcoFont). 21 tests pass.
- **Removed dead test-fixture duplicates from `Atc.Wpf.Controls.Tests`.** Five
  files under `XUnitTestTypes/` (`Account` / `Address` / `DriveItem` / `Person` /
  `PrimitiveTypesModel`) were byte-identical copies of the same files in
  `Atc.Wpf.Forms.Tests` but had no consumers in `Controls.Tests`. Deleted them
  and the now-empty folder; also dropped two `System.ComponentModel*` global
  usings that became unused.
- **`Atc.Wpf.Theming.Tests` upgraded from compliance-only to functional**
  (1 → 43 tests). New value-converter test files: `CornerRadiusBindingValueConverterTests`,
  `CornerRadiusFilterValueConverterTests`, `LeftRightCornerRadiusValueConverterTests`,
  `ColorToNameValueConverterTests`, `RenderColorIndicatorTypeToVisibilityValueConverterTests`.
  Together they cover the per-corner / per-side / single-corner switch logic,
  `IgnoreRadius` / `Filter` property fallbacks, invalid-input fallbacks
  (`Binding.DoNothing`, `default(CornerRadius)`, `Colors.Pink`),
  `ConvertBack` semantics (`DependencyProperty.UnsetValue` for one-ways,
  `NotSupportedException` for multi-binding back-conversion), and
  `Visibility` mapping for `RenderColorIndicatorType`.
- **`Atc.Wpf.Components.Tests` extended with pure-logic coverage** (164 → 177
  tests). New test files: `FlyoutFormResultFactoryTests` (Success / Cancelled /
  ValidationFailed semantics on the non-generic factory), `DualListSelectorItemTests`
  (POCO defaults + `ToString`), `DualListSelectorItemsReorderedEventArgsTests`,
  `DualListSelectorItemsTransferredEventArgsTests` (both directions + empty-items
  case), `TerminalReceivedDataEventArgsTests` (line-array exposure + `ToString`).
- **`Atc.Wpf.Tests` extended with core value-converter coverage** (818 → 869
  tests, +51). New test files: `ThicknessBindingValueConverterTests` (per-side
  zero-out + `IgnoreThicknessSide` property fallback + invalid-input default),
  `ThicknessFilterValueConverterTests` (per-side keep-only + `Filter` property
  fallback + `Binding.DoNothing`), `MathValueConverterTests` (single-binding
  `value`/`parameter` operands across +/-/×/÷, multi-binding first-two-values
  operands, divide-by-non-positive guard, null-operand fallback, non-numeric
  fallback, `ConvertBack` semantics for both the single and multi shapes),
  `RectangleCircularValueConverterTests` (half-of-min-dimension + zero-dimension
  + wrong-length + non-double + `NotSupportedException` on `ConvertBack`),
  `ColorHexToColorValueConverterTests` (Color → `AARRGGBB`, hex → Color
  including without `#` prefix and 7-char form, null + invalid-length →
  `Binding.DoNothing`, round-trip). Follow-up batch (+38 tests, 869 → 907):
  `ColorToSolidColorValueConverterTests` (alpha-to-255 normalisation + DeepPink
  null fallback + wrong-type guard + `NotSupportedException` on `ConvertBack`),
  `BackgroundToForegroundValueConverterTests` (ideal text colour against dark
  vs light background, frozen brush invariant, multi-binding explicit-title
  passthrough + multi-binding `ConvertBack` returns one `UnsetValue` per
  `targetType`), `WindowResizeModeMinMaxButtonVisibilityMultiValueConverterTests`
  (full Min/Max/Close × NoResize/CanMinimize/CanResize/CanResizeWithGrip
  matrix, `useNoneWindowStyle` and `showButton` precedence, null-values and
  wrong-parameter safe fallbacks), `MethodToValueConverterTests` (parameterless
  method invocation via reflection cache, instance-state preservation across
  calls, missing-method / wrong-parameter / null-value safe fallbacks,
  `NotSupportedException` on `ConvertBack`),
  `ObservableDictionaryToDictionaryOfStringsValueConverterTests` (string-key,
  int-key supported variants + null → empty + unsupported-type throws +
  `NotSupportedException` on `ConvertBack`). JSON-tree batch (+20 tests, 907
  → 927): `JsonArrayLengthConverterTests` (`[N]` for array nodes vs `[ N ]`
  padded form for properties holding an array, empty string for non-array
  property + unsupported input), `JsonNodeChildrenConverterTests` (object
  property children, array item children, empty for value nodes, null for
  unsupported input), `JsonValueDisplayConverterTests` (returns `DisplayValue`
  for string + numeric value nodes, passes non-`JsonValueNode` input through
  unchanged). With this batch, every previously-untested **public** value
  converter in `Atc.Wpf` now has functional tests. Extensions / Collections
  batch (+44 tests, 927 → 971): `CornerRadiusExtensionsTests` (`IsValid` matrix
  across negative / NaN / ±∞ flags, `IsZero`, `IsUniform`),
  `ThicknessExtensionsTests` (same shape + `CollapseThickness` width/height
  sum), `RectExtensionsTests` (`Deflate` shrink + clamp-to-zero when thickness
  exceeds size, `Inflate` expansion), `ColorExtensionsTests` (`Lerp` at 0 / 0.5
  / 1, HSB `GetHue` for pure red/green/blue, `GetBrightness` for black / white),
  `ObservableDictionaryTests` (Add / `ContainsKey` / Remove / `TryGetValue` /
  indexer / `Keys` / `Values` / duplicate-key throw, plus the `CopyTo`
  implementation added earlier this session — null array, negative `arrayIndex`,
  insufficient destination size guards). Follow-up batch (+18 tests, 971 →
  989): `GradientStopCollectionExtensionsTests` (`GetColorAtOffset` clamps
  below 0 and above 1, returns first/last stop at the boundaries, interpolates
  to the truncated mid-grey at 0.5 — note that `(255 * 0.5) + 0 = 127.5` casts
  to byte 127, not 128 — interpolates within a single segment of a three-stop
  gradient, and sorts unsorted stops internally), `ObservableKeyValuePairTests`
  (`INotifyPropertyChanged` for both `Key` and `Value`, multiple-event capture,
  direct `OnPropertyChanged` invocation), `ObservableDictionaryExtensionsTests`
  (all three `ToDictionaryOfStrings` overloads — string keys passed through,
  int keys via invariant `ToString`, Guid keys via default Guid format —
  plus null-input guards on each). Follow-up batch (+23 tests, 989 → 1012):
  `ObservableCollectionExTests` (`AddRange` raises a single `Add` event for the
  whole batch, `Refresh` raises a single `Reset`, toggling
  `SuppressOnChangedNotification` to the same value is a no-op, toggling
  off after suppressed mutations raises a `Reset`, individual `Add` outside
  `AddRange` still raises one event per call, null-input throws),
  `BrushExtensionsTests` (`IsOpaqueSolidColorBrush` true/false matrix on
  `SolidColorBrush` alpha + non-solid types, `IsEqualTo` symmetric / different-
  type / matching solid / different-color / different-opacity / matching linear-
  gradient / different-stop-count linear / different-radius radial / null-
  guard), `DrawingGroupExtensionsTests` (`ApplyTransform` assigns directly on
  no-existing-transform, appends to existing `TransformGroup`, wraps existing
  non-group transform into a new `TransformGroup`, throws on null drawing).
- **`Atc.Wpf.Forms.Tests` extended with pure-logic coverage** (148 → 166
  tests, +18). New test files: `InMemoryFontPickerStorageTests` (LRU
  promote-on-rerecord, blank-input ignore, `MaxRecentItems` cap, snapshot
  isolation between reads and later writes), `FontPickerStorageTests` (default
  `Current` is `InMemoryFontPickerStorage`, null-guard on setter, custom
  storage replaces the singleton), `LabelControlDataTests` (POCO defaults +
  round-trip + `ToString`), `LabelInputFormPanelSettingsTests` (defaults +
  round-trip + `ToString`).
- **`Atc.Wpf.Controls.Tests` extended with value-converter and event-arg
  coverage** (387 → 438 tests, +51). New test files:
  `IntegerToDoubleValueConverterTests` (pinning the per-arm boxing semantics —
  `int i => i` keeps `int` because there's no common numeric type across
  `int` / `decimal` / `double` arms; `ConvertBack`'s `_ => 0` is implicitly
  promoted to `0d` because the sibling arm is `double`),
  `NetworkProtocolToStringValueConverterTests` (all 8 protocols + 7 schemes
  round-trip, whitespace trimming, blank/null fallbacks, `Binding.DoNothing`
  for unsupported input), `RenderFlagIndicatorTypeToVisibilityValueConverterTests`
  (Visible/Collapsed mapping + `ArgumentNullException` + wrong-type/wrong-parameter
  guards + `NotSupportedException` on `ConvertBack`),
  `StepperStepChangedEventArgsTests`, `StepperStepChangingEventArgsTests`
  (`Cancel` defaults to false + setter), `SegmentedSelectionChangedEventArgsTests`
  (null items allowed + index pair).

[Unreleased]: https://github.com/atc-net/atc-wpf/compare/HEAD
