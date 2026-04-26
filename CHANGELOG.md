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
- **`CLAUDE.md`** test counts and Microsoft Testing Platform invocation refreshed
  (`dotnet run --project ...` instead of `dotnet test <dir>`); the `Components`
  and `FontIcons` test rows were added.

### Tests

- **`Atc.Wpf.FontIcons.Tests`** project added (referenced from `Atc.Wpf.slnx`).
  Adds an `IAssemblyMarkerAtcWpfFontIcons` to the source assembly and a small
  enum smoke-test suite that asserts every icon set defines `None = 0` and
  exposes more than just `None` — catches generator regressions across all
  ten icon sets (FontAwesome 5/7 solid/regular/brand, Bootstrap, Material,
  Weather, IcoFont). 21 tests pass.

[Unreleased]: https://github.com/atc-net/atc-wpf/compare/HEAD
