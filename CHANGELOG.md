# Changelog

All notable changes to this project will be documented in this file.

From 4.0.216 onwards this file is maintained by [release-please](https://github.com/googleapis/release-please) from [Conventional Commits](https://www.conventionalcommits.org/), and the project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [4.1.0](https://github.com/atc-net/atc-wpf/compare/v4.0.215...v4.1.0) (2026-10-09)


### New features

* **components:** add TrayIcon notification area icon ([fb1f7a9](https://github.com/atc-net/atc-wpf/commit/fb1f7a96d9556796ec0abd8ae0fdabde7178a078))
* **components:** address terminal output to a specific TerminalViewer ([ca2670f](https://github.com/atc-net/atc-wpf/commit/ca2670fd90a900c7b2509de7934cf34e765c1b80))
* **components:** render the xterm 256-colour palette in TerminalViewer ([d058b80](https://github.com/atc-net/atc-wpf/commit/d058b80bd00958931561fdd456f1661d4e45eedb))
* **controls:** add NavigationView shell bound to INavigationService ([78549fa](https://github.com/atc-net/atc-wpf/commit/78549fa3aba433ea8010d25839930887e6db7e23))
* **forms:** forward UpdateUiCultureOnChangeEvent from LabelCountrySelector ([547188b](https://github.com/atc-net/atc-wpf/commit/547188b4543c0530ff31ac17762fb2abf70e99c3))
* **hardware:** share picker behaviour and expose service tuning ([5187d15](https://github.com/atc-net/atc-wpf/commit/5187d15fdae772d9c52b8c2a59d662d0831fdc38))
* **hotkeys:** report global hotkeys that Windows refuses to register ([8cdd071](https://github.com/atc-net/atc-wpf/commit/8cdd071b414115972accbfc3b75955b322e0de0a))
* improve NetworkValidationRule and EndpointBox to handle hostname ([1336d80](https://github.com/atc-net/atc-wpf/commit/1336d803c8b9ce8f13db453113c69a7d11d2fc62))
* **sample:** add a right-to-left switch to the sample viewer ([a9a11a8](https://github.com/atc-net/atc-wpf/commit/a9a11a86f102b8ed95a2d3921adefbff63d4f661))
* **theming:** add Windows 11 backdrops to NiceWindow and keep it intact under ThemeMode ([a3260c5](https://github.com/atc-net/atc-wpf/commit/a3260c565bc575a9c630ef31fb11018d770c2775))
* **theming:** follow the Windows app mode and accent color ([f04c488](https://github.com/atc-net/atc-wpf/commit/f04c488324a02094004f4c6169bf1d35a4399136))
* **theming:** follow Windows high contrast ([2c8e03b](https://github.com/atc-net/atc-wpf/commit/2c8e03b337f712dcdc71b745aa20cedfced0d725))
* **zoom:** make ZoomGridOverlay styling bindable and release it on Detach ([05be68b](https://github.com/atc-net/atc-wpf/commit/05be68bdd3bbc01c0ca820ce60db8beaea6b52cf))


### Bug fixes

* **components:** animate a flyout the first time it opens ([7889a86](https://github.com/atc-net/atc-wpf/commit/7889a86519756ef84f8504c51792c7cd15be67c9))
* **components:** close the oldest toasts when MaxItems is exceeded ([6bea803](https://github.com/atc-net/atc-wpf/commit/6bea8036d9d4c258bfa1799c767f086876e479f2))
* **components:** keep JSON and terminal content left-to-right in right-to-left layouts ([9845157](https://github.com/atc-net/atc-wpf/commit/984515791746a1f5721b88f5edd3afcfa56d5ab6))
* **components:** let clicks pass through an empty FlyoutHost ([38bf8d3](https://github.com/atc-net/atc-wpf/commit/38bf8d3e31094c2034875bbd8ad3cf408945066d))
* **components:** open print preview and desktop toasts in the app's flow direction ([5e65216](https://github.com/atc-net/atc-wpf/commit/5e65216d13e6b9222a150c643a187b1bf399ccbc))
* **components:** stop CloseAllFlyouts hanging during close animations ([0560b7f](https://github.com/atc-net/atc-wpf/commit/0560b7f16d5e2d192898ca5722c6043d724b7943))
* **components:** strip private-mode ANSI sequences and truncated colours ([ae6277f](https://github.com/atc-net/atc-wpf/commit/ae6277f59a45ff4605628679c89bca8d1777a154))
* **controls:** accept lower-case percent and hex letters in NumericBox ([099a07d](https://github.com/atc-net/atc-wpf/commit/099a07daf4f51dc6067a5ac2489f59d6fe334511))
* **controls:** align UniformSpacingPanel measure with arrange ([ba63f0f](https://github.com/atc-net/atc-wpf/commit/ba63f0fc367a67cee720d6b4f1014d8a28834ee4))
* **controls:** apply ZoomMiniMap viewport border properties ([ac90a34](https://github.com/atc-net/atc-wpf/commit/ac90a34a494a498bb313d6183bba598703032aac))
* **controls:** change the UI culture from CountrySelector when enabled ([b78526c](https://github.com/atc-net/atc-wpf/commit/b78526cf3ca9265984683c605193a340f31de909))
* **controls:** keep country and language flags unmirrored in right-to-left ([2717633](https://github.com/atc-net/atc-wpf/commit/2717633fd625641687ef0386828f2c88734a6095))
* **controls:** keep ZoomBox content unmirrored when it opens in right-to-left ([0d5b05a](https://github.com/atc-net/atc-wpf/commit/0d5b05ad7955386b552d5699c80395eeb162192f))
* **controls:** keep ZoomRuler labels readable in right-to-left ([6bdd4eb](https://github.com/atc-net/atc-wpf/commit/6bdd4eb335d4b641c6f11fecaea4851dc1204496))
* **controls:** let FilePicker and DirectoryPicker inherit the DataContext ([64fac3c](https://github.com/atc-net/atc-wpf/commit/64fac3c230e527a4380ee37a33b91da4d83660af))
* **controls:** measure VirtualizingStaggeredPanel without infinite sizes ([7e21264](https://github.com/atc-net/atc-wpf/commit/7e21264a880295d56839207612026dbf0a54508d))
* **controls:** mirror the ZoomMiniMap thumbnail for right-to-left content ([b47b2d1](https://github.com/atc-net/atc-wpf/commit/b47b2d174e6c34809af4316397b4550ec9094506))
* **controls:** raise the number box LostFocus events once per edit ([fd3820d](https://github.com/atc-net/atc-wpf/commit/fd3820d437fb95c6fa0e8fa31b754a466e6aad67))
* **controls:** re-arrange ReversibleStackPanel when ReverseOrder changes ([27a81b7](https://github.com/atc-net/atc-wpf/commit/27a81b71e2fae8faf2d046972e0026ba780a45e7))
* **controls:** share column layout between StaggeredPanel measure and arrange ([8a682ba](https://github.com/atc-net/atc-wpf/commit/8a682bad31efcf27bb189a593ead8ce42bb08779))
* **controls:** stop theme-change subscriptions from leaking controls ([a3b357a](https://github.com/atc-net/atc-wpf/commit/a3b357a2d292cb2b29e414cf7911e279ff221448))
* **controls:** swap arrow keys in right-to-left layouts ([1ccbf89](https://github.com/atc-net/atc-wpf/commit/1ccbf8904649593a87c7c3e77f717eb610276173))
* **dialogs:** correct the DialogBoxSettings placeholder texts and docs ([5b95315](https://github.com/atc-net/atc-wpf/commit/5b95315d3a3d3f291f13c748ff749d2072ac0ccd))
* **dialogs:** follow the owning window's flow direction ([e326e4f](https://github.com/atc-net/atc-wpf/commit/e326e4fbe949307450ccca5a2109a0e027812f51))
* **dialogs:** make DialogService cancellation close the open dialog ([7c04c15](https://github.com/atc-net/atc-wpf/commit/7c04c15bd1b938f4139f7d31f9a8e129239eadd5))
* **forms:** declare the combo box selector events on ILabelComboBoxBase ([8ba9f64](https://github.com/atc-net/atc-wpf/commit/8ba9f6495ab6b27472750cce6e9ace1f4a58c33a))
* **forms:** make LabelSlider.IsValid validate the current value ([b5ac102](https://github.com/atc-net/atc-wpf/commit/b5ac102c56f0d883cf37916ec5ade074848311a2))
* **forms:** raise EndpointBox LostFocus events when an editor loses focus ([aec88c5](https://github.com/atc-net/atc-wpf/commit/aec88c51fa0bc10d64fdde696d328cf8a2853421))
* **forms:** raise TextChanged from LabelDatePicker and LabelTimePicker ([1c2163a](https://github.com/atc-net/atc-wpf/commit/1c2163a14ea0ae6850133c79cb3d7bf8bfcfe754))
* **forms:** report the previous color from ColorPicker.ColorChanged ([996ad7a](https://github.com/atc-net/atc-wpf/commit/996ad7a76eb99198e19732f077da8c955ebe88e3))
* **hardware:** keep unplugged devices disconnected after the just-connected delay ([71fca55](https://github.com/atc-net/atc-wpf/commit/71fca55c83705ce6d722b057b1ca983bcca21c68))
* **hardware:** refresh display and network adapter details while connected ([4de7a43](https://github.com/atc-net/atc-wpf/commit/4de7a4372a91e78022fc198332efac818cc91d31))
* **hardware:** refresh polled process, window, drive and printer entries ([c9f919c](https://github.com/atc-net/atc-wpf/commit/c9f919c0a2dbb84bba7260cc2d1b85a7fd2176bf))
* **hardware:** stop the camera and microphone running after unload ([96dcd9b](https://github.com/atc-net/atc-wpf/commit/96dcd9b2d4059fc670da135a76e1061cc614b98d))
* keep the original exception as inner exception in RtfFormatter and ColorPickerAutomationPeer ([fff08d4](https://github.com/atc-net/atc-wpf/commit/fff08d4058e7b9881a01e3b6f125d02de251bc30))
* **media:** keep SVG external file references inside the SVG folder ([6b76bc8](https://github.com/atc-net/atc-wpf/commit/6b76bc89c87a8870b2104c34d93edfa49614a1e5))
* **media:** make the AutoGrey extensions work and keep transparency ([4b75dfa](https://github.com/atc-net/atc-wpf/commit/4b75dfa699d53c2d5b8a303e19399237b18023ac))
* **media:** render an SvgImage once when it loads and keep custom brushes ([d3c1311](https://github.com/atc-net/atc-wpf/commit/d3c1311c5cce9c19b552d5ea4ae7c09938203741))
* **network:** report a dropped VNC socket as a disconnect instead of crashing ([3af02c0](https://github.com/atc-net/atc-wpf/commit/3af02c0a41505f2db13d5d00358929a4e5f2d4a1))
* **notifications:** name the toast factory title parameter title ([ad6cda9](https://github.com/atc-net/atc-wpf/commit/ad6cda98c00110ad2d3871f5d596c559853ce743))
* **sample:** match readme files from a folder boundary in the sample viewer ([04c56d2](https://github.com/atc-net/atc-wpf/commit/04c56d29b8c6b49baef278c893d63c83f122b565))
* suppress some coding rules and fix code ([71f6465](https://github.com/atc-net/atc-wpf/commit/71f646551bad7ce294223f07341960cd589cdd0c))
* **theming:** keep the DatePicker calendar icon unmirrored in right-to-left ([9666f3f](https://github.com/atc-net/atc-wpf/commit/9666f3f328dd5cd1e3039102b085f49b252132cc))
* **translation:** notify culture subscribers on their own UI thread ([1496ed9](https://github.com/atc-net/atc-wpf/commit/1496ed96701ac20c9738f2023d5c6ce3d463e6c8))
* **translation:** skip sealed style setters when updating translated values ([2074c09](https://github.com/atc-net/atc-wpf/commit/2074c09abeb46fdb904eb090d3ff1a6ca08ed735))
* **viewers:** use the JsonPropertyTemplateSelector template properties ([5cb3496](https://github.com/atc-net/atc-wpf/commit/5cb3496f8ddb7963ca2b35e30635c2da69851e5d))
* **zoom:** reject non-positive or non-finite grid spacing ([4c22453](https://github.com/atc-net/atc-wpf/commit/4c224537b2e61bb9f3818bbdaa6e387a9253d214))


### Performance improvements

* **hardware:** enumerate processes and windows off the UI thread ([dbb50a3](https://github.com/atc-net/atc-wpf/commit/dbb50a393f467861ea45ff572bd81419657d59dd))
* **hardware:** query the print spooler off the UI thread ([7bccafc](https://github.com/atc-net/atc-wpf/commit/7bccafc67a87046eaf1673b080fa61ea4d745ea4))
* **hardware:** reuse camera preview frame buffers and coalesce renders ([7f9575b](https://github.com/atc-net/atc-wpf/commit/7f9575b7bba4c47ef003332ab499eced99cc760e))
* **media:** cache AutoGreyableImage greyscale versions per source ([eedc3eb](https://github.com/atc-net/atc-wpf/commit/eedc3eb35f4fc4a9252f901f1c4f16686d837303))
* **media:** share frozen SVG drawings between images with the same source ([b702644](https://github.com/atc-net/atc-wpf/commit/b70264445d72a9a74692593294f63c81b717a718))
* **network:** stop refreshing the whole scanner view on every progress report ([c2d14ad](https://github.com/atc-net/atc-wpf/commit/c2d14adce1af70d12a4e2525fca9fe69c9492822))
* **zoom:** reuse pens and text resources in ZoomRuler and ZoomGridOverlay ([67af5d3](https://github.com/atc-net/atc-wpf/commit/67af5d36d8fc1ac8b6d404ef810fdb022b113321))

## 4.0.215 (2026-05-12)

### Added

- **`ApplicationMonitorView` upgrades** in `Atc.Wpf.Components.Monitoring`:
  - **Virtualization** — `VirtualizingPanel.IsVirtualizing="True"` with
    `VirtualizationMode="Recycling"` and pixel scroll. Tens of thousands of
    rows scroll smoothly.
  - **Channel-based batched ingestion** — entries flow through an unbounded
    `Channel<ApplicationEventEntry>`; a background drain loop dispatches
    batches to the UI thread under a single `InvokeAsync(Background)` and
    sends one `ApplicationMonitorScrollEvent` per batch. The TerminalViewer
    pattern, applied to the monitor.
  - **`MaxEntries`** DP (default `10000`, `0` = unbounded) — ring-buffer
    cap; oldest entries (by insertion order) drop when exceeded. Two-way
    bindable, bridged to the VM.
  - **`IsPaused`** DP + `ShowPauseInToolbar` DP — Pause/Resume toolbar
    toggle freezes display while the channel keeps capturing; the button
    shows a badge with the buffered-but-not-shown count
    (`BufferedCount` on the VM, exposed via `ChannelReader.Count`).
  - **Smart auto-scroll ("tail mode")** — `ScrollChanged` hook detects
    when the user moves away from the tail; auto-scroll suppresses and a
    floating **"↓ Jump to live (N)"** overlay appears with a count of new
    entries since detachment. Click (or call `JumpToLive()` from code) to
    re-attach. New `IsDetachedFromTail` and `NewSinceDetached` DPs back
    the overlay binding. Direction-aware (works for ascending and
    descending sort).
  - **`ExportCommand`** + `ShowExportInToolbar` DP — toolbar Export button
    opens a `SaveFileDialog` (CSV / JSON / TXT) and writes the currently
    *visible* (filtered) entries via the new internal
    `ApplicationMonitorExportService`. Format inferred from the chosen
    extension; error path surfaces a `MessageBox`.
  - **`ApplicationMonitorLoggerProvider`** in
    `Atc.Wpf.Components.Monitoring.Logging` — drop-in
    `Microsoft.Extensions.Logging` provider. `builder.Logging.AddAtcWpfApplicationMonitor()`
    routes every `ILogger<T>` call into the live picker via `Messenger.Default`
    (`LogLevel` → `LogCategoryType` mapping, `categoryName` → `Area`).
    Optional level filter overload. `ProviderAlias("AtcWpfApplicationMonitor")`
    so it can be addressed by name from `appsettings.json` filters.

- **`AudioInputPicker.ShowLivePreview`** + **`AudioOutputPicker.ShowLivePreview`**
  with a shared `PreviewHeight` DP. When enabled and a device is selected,
  the picker shows a small live pane below the dropdown — a scrolling
  waveform + side peak-level bar. Both backed by WinRT `AudioGraph` (no
  NAudio dependency).

  `AudioInputPicker` opens the selected microphone via
  `AudioDeviceInputNode` + `AudioFrameOutputNode`; per-quantum peaks
  are pushed into a 200-slot ring buffer rendered at ~30 fps as
  mirrored top/bottom polylines. Permission denial and "device held
  by another app" surface as localized inline messages
  (`MicrophonePermissionDenied` / `AudioPreviewUnavailable`).

  `AudioOutputPicker` adds a `Test` / `Stop` button next to the
  waveform: clicking `Test` opens the selected speakers via
  `AudioGraph.PrimaryRenderDevice` + `AudioFrameInputNode` and feeds
  a 1 kHz sine for ~3 s — Left → Right → Both stereo, with a 30 ms
  linear fade in/out per segment to suppress click artefacts. The
  same buffer drives the visualisation, so the picker shows exactly
  what's being rendered. The fade-envelope math lives in
  `SineToneEnvelope` so it stays unit-tested independently of the
  `AudioGraph` runtime.

  Both DPs are forwarded through `LabelAudioInputPicker` and
  `LabelAudioOutputPicker`. `Test`, `Stop`, `AudioPreviewUnavailable`,
  and `AudioPermissionDenied` localised across en-US / da-DK / de-DE.

  Buffer access uses the WinRT `IMemoryBufferByteAccess` COM interface
  (the only safe path to the raw audio bytes — `AudioBuffer` has no
  `CopyToBuffer(IBuffer)` overload like `SoftwareBitmap` does), so
  `<AllowUnsafeBlocks>` is now enabled at the project level. `unsafe`
  is confined to `IMemoryBufferByteAccess.cs` and `AudioBufferAccess.cs`;
  the rest of the assembly stays in safe code.

  Closes the §9 audio live preview roadmap. 8 new unit tests for the
  sine-tone envelope (segment boundaries, fade-in/out linearity,
  past-end silence, edge cases).
- **`UsbCameraPicker.PreferredFormat`** + lazy supported-format
  enumeration. `UsbCameraFormat` is a new record (Width / Height /
  FrameRate / Subtype) and `UsbCameraInfo.SupportedFormats` is an
  `[ObservableProperty]` populated lazily once `LiveCameraPreview`
  opens the camera and reads `MediaFrameSource.SupportedFormats`. The
  picker subscribes to a new `LiveCameraPreview.FormatsAvailable`
  event and forwards the list (sorted descending by resolution then
  FPS, deduplicated) onto the bound `UsbCameraInfo`. The
  `PreferredFormat` DP is a culture-invariant hint: when set, the
  preview calls `MediaFrameSource.SetFormatAsync` for the matching
  format before starting the reader; if no exact match, the preview
  falls back silently to the device default. Forwarded through
  `LabelUsbCameraPicker`. Sample app shows a `ComboBox` bound to
  `Value.SupportedFormats` ↔ `PreferredFormat` to demonstrate the
  end-to-end flow. Closes the §3.1 / §3.2 v2 items in the picker
  roadmap.
- **`UsbCameraPicker.ShowLivePreview`** + companion `PreviewHeight` DP.
  When `ShowLivePreview="True"` and a camera is selected, the picker
  renders a live preview pane below the dropdown driven by an internal
  `LiveCameraPreview` control. The preview opens the camera via WinRT
  `MediaCapture` + `MediaFrameReader`, normalises each frame to BGRA8
  premultiplied, and copies pixel bytes into a recycled `WriteableBitmap`
  on the UI thread (no `AllowUnsafeBlocks`, no per-frame BMP encode).
  The first activation triggers Windows' webcam-permission prompt; if
  the user denies access or another process holds the device, the pane
  shows a localized inline message (`PreviewPermissionDenied` /
  `PreviewUnavailable`) rather than crashing the picker. Preview
  lifecycle restarts on `Value` changes and stops cleanly on `Unloaded`
  / `ShowLivePreview="False"` / `Dispose`. Forwarded through
  `LabelUsbCameraPicker`. Strings localised for `en-US` / `da-DK` /
  `de-DE`. The separate `PreferredFormat` DP for picking a specific
  resolution/FPS remains parked v2.
- **Runtime culture smoke tests** for `Atc.Wpf.Hardware` resources.
  Flips `Miscellaneous.Culture` and `Validations.Culture` between
  invariant / `da-DK` / `de-DE` and asserts representative keys come
  back in the right language; 34 cases across 11 keys (8 picker /
  state strings in `Miscellaneous`, 3 messages in `Validations`).
  Tests use xUnit `[Collection("Localization")]` so the static
  resource-culture flips don't race in parallel runs. Retires §5 last
  ⬜ deferral on the picker roadmap.
- **WinRT-free `IDeviceWatcherHost` abstraction + `FakeDeviceWatcherHost`
  test harness** for `Atc.Wpf.Hardware`. The concrete `DeviceWatcherHost`
  now implements `IDeviceWatcherHost` and maps WinRT `DeviceInformation`
  to a `DeviceSnapshot` POCO before raising `Added` / `Updated` /
  `Removed` / `EnumerationCompleted`. Each WinRT-backed service
  (Serial / USB / UsbCamera / Audio / Bluetooth) accepts the host (or a
  `Func<string, IDeviceWatcherHost>` factory for `UsbDeviceService` to
  support its filter-rebuild flow) via an internal constructor. The
  test project ships a `FakeDeviceWatcherHost` that drives event
  scenarios deterministically — retires the four "deferred — needs
  WinRT mock harness" items in the roadmap (§1.5, §2.5, §3.5, §4.8).
  Adds 34 service tests (95 total in `Atc.Wpf.Hardware.Tests`, up from
  61) covering Added / Removed / RefreshAsync / Dispose / idempotent
  Start/Stop / Available↔Disconnected rebinding / IsEnabled→InUse
  mapping. No public API change.
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
- **`DrivePicker`** in `Atc.Wpf.Hardware` and **`TimeZonePicker`** in
  `Atc.Wpf.Forms` — two pure-managed pickers. `DrivePicker` enumerates
  `System.IO.DriveInfo.GetDrives()` (Fixed / Removable / Network / CDRom /
  Ram) and polls every 2 s for hot-plug — same `DeviceState` UX, same
  `ValueChanged` / `DeviceLost` / `DeviceReconnected` / `DeviceStateChanged`
  routed events, same Label* wrapper pattern. `TimeZonePicker` is the slim
  variant — `TimeZoneInfo.GetSystemTimeZones()` bound directly, no service,
  no hot-plug, no state, just a clean ComboBox with offset + display name.
  Lives in `Atc.Wpf.Forms` (alongside the other labeled pickers) since it
  has no hardware involvement. `LabelTimeZonePicker` adds mandatory
  validation. `TimeZone` / `SelectTimeZone` strings live in the shared
  `Atc.Wpf.Controls.Resources.Miscellaneous` (`en-US` / `da-DK` / `de-DE`).
  New `DiskDriveInfo` model (named to avoid the clash with `System.IO.DriveInfo`).
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
