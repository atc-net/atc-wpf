# Roadmap: Hardware Pickers (and friends)

Tracking work to introduce hardware-aware picker controls modeled after the
existing `FilePicker` / `DirectoryPicker` / `LabelFilePicker` family.

> Status legend
> - ✅ Done
> - 🟡 In progress
> - ⬜ Not started
> - 🔵 Design / under discussion
> - ⏸️ Parked / awaiting decision

---

## 0. Foundation — naming & location

> **Awaiting validation before any code lands.**

### 0.1 New project: `Atc.Wpf.Hardware`

| Item | Decision | Status |
|------|----------|--------|
| Project path | `src/Atc.Wpf.Hardware/` | ✅ |
| Assembly / root namespace | `Atc.Wpf.Hardware` | ✅ |
| **TargetFramework** | **`net10.0-windows10.0.19041.0`** (Win10 2004+ / all Win11) — required for `Windows.Devices.Enumeration` (`DeviceWatcher`) and `MediaFoundation` WinRT projection | ✅ |
| Test project | `test/Atc.Wpf.Hardware.Tests/` | ✅ |
| Sample tree | `sample/Atc.Wpf.Sample/SamplesWpfHardwareTreeView.xaml` | 🔵 |
| Sample folder | `sample/Atc.Wpf.Sample/SamplesWpfHardware/Pickers/` | 🔵 |
| Docs folder | `docs/Hardware/@Readme.md` | 🔵 |
| Precedent | Mirrors `Atc.Wpf.Network` (separate assembly, OS-specific deps) | — |

### 0.2 Folder layout inside `Atc.Wpf.Hardware`

```
Atc.Wpf.Hardware/
├── Abstractions/
│   ├── ILabelSerialPortPicker.cs
│   ├── ILabelUsbPortPicker.cs
│   └── ILabelUsbCameraPicker.cs
├── Pickers/
│   ├── SerialPortPicker.xaml(.cs)
│   ├── UsbPortPicker.xaml(.cs)
│   ├── UsbCameraPicker.xaml(.cs)
│   └── Internal/
│       ├── SerialPortPickerAutomationPeer.cs
│       ├── UsbPortPickerAutomationPeer.cs
│       └── UsbCameraPickerAutomationPeer.cs
├── Services/
│   ├── Internal/
│   │   └── DeviceWatcherHost.cs    (shared DeviceWatcher wrapper)
│   ├── ISerialPortService.cs
│   ├── SerialPortService.cs
│   ├── IUsbDeviceService.cs
│   ├── UsbDeviceService.cs
│   ├── IUsbCameraService.cs
│   └── UsbCameraService.cs
├── Models/
│   ├── SerialPortInfo.cs
│   ├── UsbDeviceInfo.cs
│   └── UsbCameraInfo.cs
├── Resources/
│   ├── Miscellaneous.resx        (invariant / en-US strings)
│   ├── Miscellaneous.da-DK.resx
│   ├── Miscellaneous.de-DE.resx
│   ├── Validations.resx          (validation messages)
│   ├── Validations.da-DK.resx
│   └── Validations.de-DE.resx
├── LabelSerialPortPicker.xaml(.cs)
├── LabelUsbPortPicker.xaml(.cs)
└── LabelUsbCameraPicker.xaml(.cs)
```

### 0.3 Conventions

| Concern | Convention | Status |
|---------|------------|--------|
| `Value` type for `SerialPortPicker` | `SerialPortInfo?` (POCO with `PortName`, `FriendlyName`, `VendorId`, `ProductId`) — bindable string fallback via `DisplayValue` | 🔵 |
| `Value` type for `UsbPortPicker` | `UsbDeviceInfo?` (DeviceId, FriendlyName, VendorId/ProductId, BusReportedDeviceDesc) | 🔵 |
| `Value` type for `UsbCameraPicker` | `UsbCameraInfo?` (DeviceId/Moniker, FriendlyName, supported MediaTypes) | 🔵 |
| Refresh button | Built-in icon button next to dropdown — same pattern as `FilePicker`'s browse button | 🔵 |
| Hot-plug detection | **`Windows.Devices.Enumeration.DeviceWatcher`** — shared across all three pickers (Serial / USB / Camera). Fires `Added` / `Removed` / `Updated` on a background thread; service marshals to UI thread. Full UX in §4 | 🔵 |
| In-use detection | Passive always-on, active probing opt-in via `DetectInUseState`. Per-class strategy in §4.2 | 🔵 |
| Service abstraction | All hardware enumeration behind `I*Service` to keep XAML Designer working without real devices | 🔵 |
| Shared base | `Services/Internal/DeviceWatcherHost.cs` — wraps a `DeviceWatcher` + `IObservable<DeviceChange>` for reuse by all three services | 🔵 |
| Empty-state | Watermark text + disabled `Value` when list is empty (mirrors `WatermarkText` on `FilePicker`) | 🔵 |
| **Localization** | All user-visible strings via `.resx` — invariant + **`en-US`** (default), **`da-DK`**, **`de-DE`** — same triplet as `Atc.Wpf.Controls/Resources/Miscellaneous*.resx` and `Atc.Wpf.Network/Resources/Resources*.resx`. No literal strings in XAML or C# | 🔵 |

---

## 1. SerialPortPicker

### 1.1 Service layer

| Task | Status |
|------|--------|
| `Models/SerialPortInfo.cs` (DeviceId, PortName, FriendlyName, VendorId, ProductId, State) | ✅ |
| `Services/ISerialPortService.cs` (`Ports`, `StartWatching`, `StopWatching`, `RefreshAsync`, `ProbeInUseAsync`) | ✅ |
| `Services/SerialPortService.cs` — `DeviceWatcher` with `SerialDevice.GetDeviceSelector()` AQS filter, requests `System.DeviceInterface.Serial.PortName` property | ✅ |
| `Services/Internal/DeviceWatcherHost.cs` — shared wrapper used by all three services | ✅ |
| `Services/Internal/UsbIdParser.cs` — VID/PID extraction (shared with USB/camera pickers) | ✅ |
| **In-use detection** — passive via `DeviceWatcher`; active probe `SerialDevice.FromIdAsync(id) is null` exposed via `ProbeInUseAsync` | ✅ |
| Auto-trigger active probe when `DetectInUseState=true` | ✅ |
| Unit tests with stubbed `ISerialPortService` | 🟡 *POCO + UsbIdParser tested; full service stub deferred* |

### 1.2 Control: `SerialPortPicker`

| Task | Status |
|------|--------|
| `Pickers/SerialPortPicker.xaml` (ComboBox + Refresh button + status dot DataTemplate + inline state-warning row) | ✅ |
| `Pickers/SerialPortPicker.xaml.cs` with DPs: `Value`, `WatermarkText`, `ShowRefreshButton`, `AutoRefreshOnDeviceChange`, `DetectInUseState`, `ClearValueOnDisconnect`, `AutoRebindOnReconnect`, `AutoSelectFirstAvailable`, `ItemTemplate`, `ResolvedItemTemplate`, `SelectedStateMessage`, `HasSelectedStateMessage` | ✅ |
| `RoutedEvent` `ValueChanged` + `DeviceLost` + `DeviceReconnected` | ✅ |
| `Internal/SerialPortPickerAutomationPeer.cs` (IValueProvider) | ✅ |
| `ValueConverters/DeviceStateToBrushConverter.cs` + `DeviceStateToTextConverter.cs` | ✅ |
| `Pickers/SerialPortPicker_Readme.md` | ✅ |

### 1.3 Label wrapper: `LabelSerialPortPicker`

| Task | Status |
|------|--------|
| `Abstractions/ILabelSerialPortPicker.cs` | ✅ |
| `LabelSerialPortPicker.xaml` (LabelContent → SerialPortPicker — mirror `LabelFilePicker.xaml`) | ✅ |
| `LabelSerialPortPicker.xaml.cs` with validation (`IsMandatory` + disconnected-device check) | ✅ |
| `LostFocusValid` / `LostFocusInvalid` events | ✅ |
| `LabelSerialPortPicker_Readme.md` | ✅ |

### 1.4 Sample app

| Task | Status |
|------|--------|
| `SamplesWpfHardware/Pickers/SerialPortPickerView.xaml(.cs)` | ✅ |
| `SamplesWpfHardware/Pickers/SerialPortPickerDemoViewModel.cs` | ✅ |
| `SamplesWpfHardware/LabelControls/LabelSerialPortPickerView.xaml(.cs)` | ✅ |
| TreeView entries in `SamplesWpfHardwareTreeView.xaml` | ✅ |
| Wire `SamplesWpfHardwareTreeView` into `MainWindow` left nav (TabItem, Badge, mappings) | ✅ |

### 1.5 Tests

| Task | Status |
|------|--------|
| `SerialPortServiceTests` (with stubbed `DeviceWatcher` — WinRT-shaped) | ⬜ *deferred — needs WinRT mock harness* |
| `SerialPortInfoTests` (POCO, INPC) | ✅ |
| `UsbIdParserTests` (VID/PID extraction) | ✅ |

---

## 2. UsbPortPicker

> *USB device picker — enumerates connected USB devices regardless of class.
> Targets the use case "let the user pick which connected USB widget to talk to."*

### 2.1 Service layer

| Task | Status |
|------|--------|
| `Models/UsbDeviceInfo.cs` (DeviceId, FriendlyName, VendorId, ProductId, PnpClass, InterfaceEnabled, State) | ✅ |
| `Models/UsbDeviceClassFilter.cs` flags enum (HID, Imaging, Audio, Printer, MassStorage, Communication) | ✅ |
| `Services/IUsbDeviceService.cs` (`Devices`, `ClassFilter`, `StartWatching`, `StopWatching`, `RefreshAsync`) | ✅ |
| `Services/UsbDeviceService.cs` — `DeviceWatcher` with USB device-interface AQS (`InterfaceClassGuid={a5dcbf10-...}`); requests `System.Devices.ClassGuid` and `System.Devices.InterfaceEnabled` | ✅ |
| Hot-plug via shared `DeviceWatcherHost` (no `WM_DEVICECHANGE` plumbing) | ✅ |
| **In-use detection** — passive: maps `InterfaceEnabled=false` → `DeviceState.InUse`; active probe via `UsbDevice.FromIdAsync` deferred (different driver classes need different selectors) | 🟡 |
| Filter DSL → AQS translation (`UsbDeviceClassFilter` → `System.Devices.InterfaceClassGuid:="{guid}" OR …`; rebuilds the watcher when filter changes) | ✅ |

### 2.2 Control: `UsbPortPicker`

| Task | Status |
|------|--------|
| `Pickers/UsbPortPicker.xaml` (ComboBox + Refresh button + status dot DataTemplate + inline state-warning row) | ✅ |
| `Pickers/UsbPortPicker.xaml.cs` with DPs: `Value`, `WatermarkText`, `ClassFilter`, `ShowRefreshButton`, `AutoRefreshOnDeviceChange`, `DetectInUseState`, `ClearValueOnDisconnect`, `AutoRebindOnReconnect`, `AutoSelectFirstAvailable`, `ItemTemplate` | ✅ |
| `RoutedEvent` `ValueChanged` + `DeviceLost` + `DeviceReconnected` | ✅ |
| `Internal/UsbPortPickerAutomationPeer.cs` | ✅ |
| `Pickers/UsbPortPicker_Readme.md` | ✅ |

### 2.3 Label wrapper: `LabelUsbPortPicker`

| Task | Status |
|------|--------|
| `Abstractions/ILabelUsbPortPicker.cs` | ✅ |
| `LabelUsbPortPicker.xaml(.cs)` with validation | ✅ |
| `LabelUsbPortPicker_Readme.md` | ✅ |

### 2.4 Sample app

| Task | Status |
|------|--------|
| `SamplesWpfHardware/Pickers/UsbPortPickerView.xaml(.cs)` + ViewModel | ✅ |
| `SamplesWpfHardware/LabelControls/LabelUsbPortPickerView.xaml(.cs)` | ✅ |
| TreeView entries | ✅ |

### 2.5 Tests

| Task | Status |
|------|--------|
| `UsbDeviceServiceTests` (filter logic with mocked DeviceInformation) | ⬜ *deferred — needs WinRT mock harness* |
| `UsbDeviceInfoTests` (POCO, INPC, ToString) | ✅ |

---

## 3. UsbCameraPicker

> *Specialised picker for video-capture devices (webcams, capture cards, etc.).
> Distinct from `UsbPortPicker` because it surfaces media capabilities (resolutions, FPS).*

### 3.1 Service layer

| Task | Status |
|------|--------|
| `Models/UsbCameraInfo.cs` (DeviceId, FriendlyName, Panel, IsEnabled, State) | ✅ |
| `Models/CameraPanel.cs` enum (Unknown / Front / Back / Top / Bottom / Left / Right / External) | ✅ |
| `Services/IUsbCameraService.cs` (`Cameras`, `StartWatching`, `StopWatching`, `RefreshAsync`) | ✅ |
| `Services/UsbCameraService.cs` — **MediaFoundation via WinRT** `DeviceInformation.FindAllAsync(DeviceClass.VideoCapture)` + shared `DeviceWatcherHost`; reads `EnclosureLocation.Panel` for orientation | ✅ |
| Round-trips cleanly: persisted `DeviceId` can be passed straight to `MediaCaptureInitializationSettings.VideoDeviceId` by consumers | ✅ |
| **In-use detection** — passive via `InterfaceEnabled=false` → `DeviceState.InUse`; active "Test camera" probe deferred to v2 (opening MediaCapture is heavyweight) | 🟡 |
| Format enumeration (`MediaCapture.VideoDeviceController`) — lazy, deferred to v2 | ⬜ |

### 3.2 Control: `UsbCameraPicker`

| Task | Status |
|------|--------|
| `Pickers/UsbCameraPicker.xaml` (ComboBox + Refresh button + status dot DataTemplate + inline state-warning row) | ✅ |
| `Pickers/UsbCameraPicker.xaml.cs` with DPs: `Value`, `WatermarkText`, `ShowRefreshButton`, `AutoRefreshOnDeviceChange`, `ClearValueOnDisconnect`, `AutoRebindOnReconnect`, `AutoSelectFirstAvailable`, `ItemTemplate` | ✅ |
| `RoutedEvent` `ValueChanged` + `DeviceLost` + `DeviceReconnected` | ✅ |
| Live-preview popout / `ShowLivePreview` / `PreferredFormat` DPs — **deferred to v2** per §7.6 | ⏸️ |
| `Internal/UsbCameraPickerAutomationPeer.cs` | ✅ |
| `Pickers/UsbCameraPicker_Readme.md` | ✅ |

### 3.3 Label wrapper: `LabelUsbCameraPicker`

| Task | Status |
|------|--------|
| `Abstractions/ILabelUsbCameraPicker.cs` | ✅ |
| `LabelUsbCameraPicker.xaml(.cs)` with validation | ✅ |
| `LabelUsbCameraPicker_Readme.md` | ✅ |

### 3.4 Sample app

| Task | Status |
|------|--------|
| `SamplesWpfHardware/Pickers/UsbCameraPickerView.xaml(.cs)` + ViewModel | ✅ |
| `SamplesWpfHardware/LabelControls/LabelUsbCameraPickerView.xaml(.cs)` | ✅ |
| TreeView entries | ✅ |

### 3.5 Tests

| Task | Status |
|------|--------|
| `UsbCameraServiceTests` (with mocked `DeviceInformation`) | ⬜ *deferred — needs WinRT mock harness* |
| `UsbCameraInfoTests` (POCO, INPC, ToString) | ✅ |

---

## 4. Auto-detect & device-state UX

> Cross-cutting design for how all three pickers visualise live device state: connected, disconnected, and in-use. Goal: the user never has to click *Refresh* and never picks a device that's silently unusable.

### 4.1 Device states (per item)

| State | Meaning | Trigger |
|-------|---------|---------|
| `Available` | Device is present and free to open | `DeviceWatcher.Added` or initial enumeration |
| `JustConnected` | Available, appeared within the last ~3 s | `DeviceWatcher.Added` after first enumeration |
| `InUse` | Device is held exclusively by another process | `*Device.FromIdAsync()` returns `null`, or `HardwareInUse` HRESULT |
| `Disconnected` | Was selected; now physically gone | `DeviceWatcher.Removed` while it was the bound `Value` |
| `Unknown` | Enumerated but couldn't probe (permissions, transient error) | exception during probe — fail-open, treat as `Available` for selection |

### 4.2 Detection strategy (per device class)

| Concern | Serial | USB | Camera |
|---------|--------|-----|--------|
| Connect / disconnect | `DeviceWatcher` (free, always on) | `DeviceWatcher` | `DeviceWatcher` |
| In-use — passive | Track ports the consuming app opened via the service itself | `InterfaceEnabled` AQS prop | Catch `MediaCapture.InitializeAsync` HRESULT when the consumer opens it |
| In-use — active probe | `SerialDevice.FromIdAsync(id) is null` | `UsbDevice.FromIdAsync(id) is null` | **No auto-probe** — `MediaCapture.InitializeAsync` is too heavy. Only on explicit user click ("Test camera") |
| Probe cadence | On enumeration + on selection change. **Never** on a timer | Same | On demand only |
| Acquiring app (best-effort) | — | — | `AppDiagnosticInfo.RequestInfoForAppAsync` (Win10 19041+, requires capability declaration) |

**Rule:** active probing is **opt-in** via DP `DetectInUseState` (default `false` for serial/usb, `false` for camera with no auto-probe ever). Passive detection (catching errors when the consuming app actually opens the device) is **always on**.

### 4.3 Visual treatment

```
┌─────────────────────────────────────────────────────────┐
│ ● COM3   Arduino Uno              Available         🔄 │  ← normal
│ ● COM4   USB-Serial CH340         In use            🔄 │  ← amber dot, italics, tooltip
│ ● COM5   Prolific USB-to-Serial   ★ New                │  ← briefly highlighted (3 s)
└─────────────────────────────────────────────────────────┘
```

| Element | Treatment |
|---------|-----------|
| Status dot | `Available`=green, `InUse`=amber, `Disconnected`=red, `JustConnected`=green + pulse animation 3 s |
| Item text | Strikethrough on `Disconnected`; italic on `InUse` |
| Tooltip | "In use by *ProcessName*" when known, else "In use by another application" |
| Status text column | Localised: "Available" / "In use" / "Disconnected" / "New" |
| Refresh affordance | Always-visible 🔄 button next to the dropdown — manual override even when auto-refresh is on |
| Empty state | "No devices found" + a *Refresh* link, no dot |
| Theming | Dot/text colours bound to theme resources (light + dark) |

### 4.4 Selected-item ("Value") behaviour

This is the part most pickers get wrong — what happens to a *bound* `Value` when the world changes.

| Transition | Default behaviour | Configurable? |
|------------|-------------------|---------------|
| Selected device disconnects | Keep `Value` set to the now-stale info; raise new event `DeviceLost`; show inline warning "Device disconnected" under the dropdown; do **not** auto-clear (preserves user intent so reconnect rebinds silently) | DP `ClearValueOnDisconnect` (default `false`) |
| Disconnected device reappears | Auto-rebind to same `DeviceId`; raise `DeviceReconnected`; clear inline warning | DP `AutoRebindOnReconnect` (default `true`) |
| Selected device becomes in-use | Keep `Value`; show amber warning under dropdown "In use by another application"; consumer's open call still fails loudly (don't pretend it's fine) | — |
| Selected device frees up | Clear in-use warning silently | — |
| New device appears, nothing selected | Do **not** auto-select — show "New" badge for 3 s, let user choose | DP `AutoSelectFirstAvailable` (default `false`) |
| New device appears, something selected | Show "New" badge briefly; never rebind | — |

**Why these defaults:** the picker is a *user-intent* control. Auto-clearing on disconnect destroys intent (USB jiggle = lost setting). Auto-selecting on appear creates surprise rebinding. Consumers who want different policies opt in via DPs.

### 4.5 New events on each picker

| Event | Args | Fires when |
|-------|------|------------|
| `DeviceLost` | `RoutedPropertyChangedEventArgs<TInfo?>` | Bound `Value` device disconnected |
| `DeviceReconnected` | `RoutedPropertyChangedEventArgs<TInfo?>` | Previously-lost device came back |
| `DeviceStateChanged` | `DeviceStateChangedEventArgs` (Old/New state, DeviceId) | Any tracked device transitions state |

### 4.6 New DPs on each picker

| DP | Type | Default | Notes |
|----|------|---------|-------|
| `AutoRefreshOnDeviceChange` | `bool` | `true` | Subscribes to `DeviceWatcher`. Resolves §6.5. |
| `DetectInUseState` | `bool` | `false` (serial/usb), `false` (camera, never auto-probes) | Opt-in active probe |
| `ClearValueOnDisconnect` | `bool` | `false` | Preserves user intent by default |
| `AutoRebindOnReconnect` | `bool` | `true` | By `DeviceId` match |
| `AutoSelectFirstAvailable` | `bool` | `false` | Avoids surprise rebinding |
| `JustConnectedHighlightDuration` | `TimeSpan` | `3 s` | Set `0` to disable highlight |
| `ItemStatusTemplate` | `DataTemplate?` | `null` | Override the dot+text default |

### 4.7 Localised strings (added to §5 resx scope)

`Available`, `InUse`, `Disconnected`, `New`, `NoDevicesFound`, `Refresh`, `DeviceDisconnected`, `DeviceInUse`, `DeviceInUseBy` (format string), `TestCamera` — invariant + `da-DK` + `de-DE`.

### 4.8 Implementation tasks

| Task | Status |
|------|--------|
| `Models/DeviceState.cs` enum + `DeviceStateChangedEventArgs` + `IDeviceInfo` | ✅ |
| `Services/Internal/DeviceWatcherHost.cs` — wraps `DeviceWatcher`, marshals events to UI thread | ✅ |
| `Services/Internal/JustConnectedTimer.cs` — schedules `JustConnected` → `Available` transition | ✅ |
| ~~`DeviceStateTracker`~~ deferred — each service maintains its own `ObservableCollection<TInfo>` and mutates `Item.State` directly (POCO is the state via INPC) | ✅ |
| Default `ItemTemplate` (status dot + name + status text) — themed | ✅ |
| Inline warning area under dropdown for selected-item state | ✅ |
| `JustConnected` pulse animation (opacity + scale, repeats while State=JustConnected) | ✅ |
| Wire up `DeviceLost` / `DeviceReconnected` on all three pickers | ✅ |
| Wire up `DeviceStateChanged` on all three pickers | ✅ |
| Wire up the seven new DPs on all three pickers | ✅ |
| Sample app: dedicated demo view "Hot-plug & in-use" with two USB-serial dongles or a camera to demonstrate state transitions visually | ✅ |
| Tests: simulate connect/disconnect/in-use sequences against the stubbed services and assert event firing + visual state | ⬜ |

---

## 5. Cross-cutting / shared

| Task | Status |
|------|--------|
| `docs/Hardware/@Readme.md` (category-level overview) | ✅ |
| Per-control `*_Readme.md` next to each picker (`SerialPortPicker_Readme.md`, `UsbPortPicker_Readme.md`, `UsbCameraPicker_Readme.md`) | ✅ |
| Update top-level `CLAUDE.md` "Project Overview" with `Atc.Wpf.Hardware` | ✅ |
| Update `docs/sample-app.md` with the new `Wpf.Hardware` category | ✅ |
| Theming check: Light/Dark coverage for new controls — uses `DynamicResource AtcApps.Brushes.*` so theme-switch works automatically | ✅ |
| **Localization — `en-US` / `da-DK` / `de-DE`** for all user-visible strings: | ✅ |
| &nbsp;&nbsp;• `Resources/Miscellaneous.resx` (+ `.da-DK` / `.de-DE`) — Watermarks, "Refresh", "No devices found", "Select serial port…", "Select USB device…", "Select camera…", state strings ("Available", "In use", "Disconnected", "New", "In use by {0}", "Test camera") | ✅ |
| &nbsp;&nbsp;• `Resources/Validations.resx` (+ `.da-DK` / `.de-DE`) — "Device is required", "Device is no longer available", "Device is currently in use" | ✅ |
| &nbsp;&nbsp;• No literal strings in XAML/C# — all user-visible strings go through resx | ✅ |
| &nbsp;&nbsp;• Runtime smoke-test for `da-DK` / `de-DE` cultures | ⬜ *deferred — requires UI test harness* |
| `CHANGELOG.md` entry under `[Unreleased] / Added` | ✅ |

---

## 6. Other relevant pickers (candidates — not committed)

> Listed for inspiration / future scope. Not in current sprint unless promoted.

| Picker | Use case | Source API | Status |
|--------|----------|------------|--------|
| `AudioInputPicker` (mic) | Choose recording device | `DeviceClass.AudioCapture` via `Windows.Devices.Enumeration` | ✅ |
| `AudioOutputPicker` (speakers / headset) | Choose playback device | `DeviceClass.AudioRender` via `Windows.Devices.Enumeration` | ✅ |
| `PrinterPicker` | Choose installed printer | `PrinterSettings.InstalledPrinters` / `PrintQueue` | ⬜ |
| `BluetoothDevicePicker` | Choose paired/discovered BT device | `Windows.Devices.Bluetooth` | ⬜ |
| `NetworkAdapterPicker` | Choose NIC for binding | `NetworkInterface.GetAllNetworkInterfaces()` | ⬜ |
| `DrivePicker` / `VolumePicker` | Choose disk/volume | `System.IO.DriveInfo.GetDrives()` + 2 s polling for hot-plug | ✅ |
| `DisplayPicker` / `MonitorPicker` | Choose display | `Screen.AllScreens` / `MonitorEnumProc` | ⬜ |
| `ProcessPicker` | Choose running process (e.g., for attach/capture) | `Process.GetProcesses()` | ⬜ |
| `WindowPicker` | Choose top-level window (capture targets) | `EnumWindows` | ⬜ |
| `TimeZonePicker` | Choose IANA / Windows time zone | `TimeZoneInfo.GetSystemTimeZones()` | ✅ |
| `CulturePicker` / `LanguagePicker` | Already partly covered by `LabelLanguageSelector` — verify gap | 🔵 |

---

## 7. Open questions (please review)

1. **Project name** — `Atc.Wpf.Hardware` vs `Atc.Wpf.Devices`? (Network is `Atc.Wpf.Network` → "Hardware" feels parallel.)
2. **Should `Atc.Wpf.Hardware` reference `Atc.Wpf.Forms`** so Label\* wrappers live in the same assembly? (Forms already references Controls — adding Hardware → Forms keeps Label\* together.)
   - Alt: keep Label\* inside `Atc.Wpf.Hardware` (self-contained module) — current proposal.
3. **`Value` type richness** — POCO with metadata, or just `string` (port name / device id)? POCO gives binding flexibility; string is simpler.
4. ~~**DirectShow vs MediaFoundation** for `UsbCameraPicker`~~ → **Resolved** (§3.1): MediaFoundation via WinRT `Windows.Devices.Enumeration` + `DeviceWatcher`. TFM `net10.0-windows10.0.19041.0`. Applies to all three pickers.
5. ~~**Hot-plug auto-refresh** default~~ → **Resolved** (§4.6): `AutoRefreshOnDeviceChange="True"` default; manual *Refresh* button always available as override.
6. **Live preview on `UsbCameraPicker`** — in scope for v1 or defer?
7. **External NuGet wrapping?** — Should hardware enumeration live in a non-WPF `Atc.Hardware` package (mirroring `Atc.Network`)?
8. **In-use active probing for serial/USB** — default off (`DetectInUseState="False"`), opt-in. Confirm — or do you want it on by default given the UX value?

---

## 8. Suggested execution order

1. Foundation (§0) — get project + naming approved.
2. **Auto-detect & device-state plumbing (§4.1–4.2, §4.8 first half)** — `DeviceWatcherHost`, `DeviceStateTracker`, `DeviceState` enum. Built once, reused by all three pickers.
3. `SerialPortPicker` end-to-end (§1) — simplest API surface, validates the pattern + applies §4 visual treatment first.
4. `UsbPortPicker` (§2) — reuses §4 plumbing.
5. `UsbCameraPicker` (§3) — adds media capabilities and the on-demand "Test camera" probe.
6. Cross-cutting polish (§5) — docs, theming, localization triplets.
7. Promote one or two from §6 if scope allows.
