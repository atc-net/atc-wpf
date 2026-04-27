# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

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
  `SampleAppSmokeTests.Sample_app_launches_and_shows_main_window` launches the
  app, asserts the main window appears with a non-empty title, captures a
  screenshot via `mainWindow.CaptureToFile(...)` to `bin/<Config>/net10.0-windows/Snapshots/`,
  then closes the process. The test is `[Trait("Category","UI")]` + skipped by
  default so it stays opt-in for headless CI; running locally just removes the
  `Skip` argument. Pattern + future-expansion notes documented in
  `test/Atc.Wpf.UiTests/Readme.md`.
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
