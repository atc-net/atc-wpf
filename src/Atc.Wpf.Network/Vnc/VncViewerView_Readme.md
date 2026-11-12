# 🖥️ VncViewerView

A view that displays and controls a remote desktop over the VNC (RFB) protocol.

## 🔍 Overview

`VncViewerView` is a `UserControl` that renders the remote framebuffer of a `VncViewerViewModel` in an `Image` (uniform stretch, nearest-neighbor scaling) and forwards mouse and keyboard input to the remote host. A context menu offers **Connect** and **Disconnect**: Connect opens an `InputFormDialogBox` asking for host, port (default `5900`) and password, then connects through `VncConnectionService`. While connecting or disconnecting a busy overlay with the current `StatusMessage` is shown, and a placeholder text is displayed when not connected.

## 📍 Namespace

```csharp
using Atc.Wpf.Network.Vnc;
```

## 🚀 Usage

### XAML

```xml
<UserControl
    xmlns:vnc="clr-namespace:Atc.Wpf.Network.Vnc;assembly=Atc.Wpf.Network">

    <vnc:VncViewerView DataContext="{Binding VncViewer}" />
</UserControl>
```

### View Model

```csharp
public VncViewerViewModel VncViewer { get; } = new();
```

### Connecting from Code

```csharp
await VncViewer.ConnectAsync("192.168.1.50", 5900, password: null);

// Later
await VncViewer.DisconnectAsync();
```

## ⚙️ Properties

The view is driven by `VncViewerViewModel`:

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IsConnected` | `bool` | `false` | `true` while connected to a VNC server |
| `StatusMessage` | `string` | `""` | Connection status text (connecting, connected, connection lost, failure message, ...) |
| `IsBusy` | `bool` | `false` | Inherited from `ViewModelBase`; `true` while connecting/disconnecting |
| `CurrentFrame` | `WriteableBitmap?` | `null` | Read-only. The current remote framebuffer |
| `FramebufferWidth` | `int` | `0` | Read-only. Framebuffer width in pixels |
| `FramebufferHeight` | `int` | `0` | Read-only. Framebuffer height in pixels |
| `ConnectCommand` | `RelayCommandAsync` | - | Shows the connect dialog and connects; enabled when not busy and not connected |
| `DisconnectCommand` | `RelayCommandAsync` | - | Disconnects; enabled when not busy and connected |
| `ErrorMessage` / `HasError` | `string?` / `bool` | `null` / `false` | Why connecting failed, or that the connection was lost (not set by a disconnect you asked for) |
| `RetryCommand` | `RelayCommandAsync` | - | Connects again to the last server with the same port and password |
| `ScaleMode` | `VncScaleMode` | `Fit` | `Fit` (whole screen, letterboxed), `Fill` (fills the view, edges cut off) or `Original` (1:1 with scroll bars); the mouse is mapped for each |
| `Stretch` / `IsScrollable` | `Stretch` / `bool` | `Uniform` / `false` | Read-only, from `ScaleMode` (used by the view) |
| `ViewOnly` | `bool` | `false` | Shows the screen without sending mouse or keyboard input |
| `ShareClipboard` | `bool` | `false` | Opt-in: server clipboard text is copied to the local clipboard, and `SendClipboardCommand` sends the local clipboard text |
| `SendClipboardCommand` | `RelayCommandAsync` | - | Sends the local clipboard text; enabled when connected and `ShareClipboard` is on |
| `ClipboardSetter` / `ClipboardGetter` | delegates | `Clipboard.SetText` / `Clipboard.GetText` | Replace them in tests or to filter what is shared |

### Methods

| Method | Description |
|--------|-------------|
| `ConnectAsync(string host, int port, string? password)` | Connects without showing the dialog |
| `DisconnectAsync()` | Disconnects from the server |
| `SendPointerEventAsync(byte buttonMask, int x, int y)` | Sends a pointer event (used by the view) |
| `SendKeyEventAsync(uint keySym, bool pressed)` | Sends a key event (used by the view) |

## ⚡ Events

| Event | Args | Description |
|-------|------|-------------|
| `FramebufferUpdated` | `EventArgs` | Raised by the view model when the remote framebuffer changes; the view refreshes the image on the dispatcher |

The view's toolbar has the scale mode, **View only**, **Share clipboard**, **Send clipboard** and **Retry**; errors show under it.

`VncPointerMapping.MapToFramebuffer(position, imageSize, width, height, scaleMode)` maps a mouse position to the remote screen for each scale mode.

## 📝 Notes

- Mouse coordinates are translated from the uniformly scaled image to framebuffer coordinates; left, middle and right buttons map to button mask bits `1`, `2` and `4` (press/release is handled for the left and right buttons, mouse-move includes all three)
- Keys are translated to X11 key symbols via `KeySymMapper`; mapped keys are marked handled so they are not processed locally
- Input is only sent while `IsConnected` is `true`
- `ConnectCommand` uses `Application.Current.MainWindow` as dialog owner and does nothing if it is `null`
- The view model implements `IDisposable`; dispose it when the hosting view is discarded
- The library has no XAML namespace mapping, so use a `clr-namespace:Atc.Wpf.Network.Vnc;assembly=Atc.Wpf.Network` prefix

## 🔗 Related Controls

- **NetworkScannerView** - Discover hosts on the local network
- **InputFormDialogBox** - Used for the connect dialog

## 🎮 Sample Application

See the VncViewer sample in the Atc.Wpf.Sample application under **Wpf.Network > Network > VncViewer** for interactive examples.
