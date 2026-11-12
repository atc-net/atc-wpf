# 🔌 TcpClientView and UdpClientView

Small TCP and UDP client test tools: connect to a device, send text or hex, and see what goes out and comes back.

## 🔍 Overview

`TcpClientView` and `UdpClientView` wrap Atc.Network's `TcpClient` and `UdpClient`. Enter a host and port, connect, type text (with an optional line ending) or hexadecimal bytes, and send. Sent data (`→`), received data (`←`, as text or hex) and errors (`[ERROR]`, shown in the error colour) appear in a `TerminalViewer` with copy and clear. The TCP view adds TLS and message framing (none, by a termination such as a line feed, or length-prefixed).

## 📍 Namespace

```xml
xmlns:clients="clr-namespace:Atc.Wpf.Network.Clients;assembly=Atc.Wpf.Network"
```

## 🚀 Usage

```xml
<clients:TcpClientView DataContext="{Binding TcpClient}" />
<clients:UdpClientView DataContext="{Binding UdpClient}" />
```

```csharp
public TcpClientViewModel TcpClient { get; } = new() { Host = "192.168.1.50", Port = 4001 };
public UdpClientViewModel UdpClient { get; } = new() { Host = "192.168.1.60", Port = 5005 };
```

## ⚙️ Properties

Both view models (`NetworkClientViewModelBase`):

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Host` / `Port` | `string?` / `int` | `null` / `0` | Where to connect |
| `IsConnected` | `bool` | `false` | Connected |
| `SendText` | `string?` | `null` | Text, or hex bytes when `SendAsHex` (`48 65 6C`, `48-65-6C`, `0x48 0x65`, `48656C`) |
| `SendAsHex` / `ShowReceivedAsHex` | `bool` | `false` | Send hex / show received bytes as hex |
| `LineEnding` | `TerminationType` | `None` | Added to sent text: `LineFeed`, `CarriageReturn`, `CarriageReturnLineFeed`, `EndOfText`, `EndOfTransmission` |
| `TextEncoding` | `Encoding` | UTF-8 | For sent and received text |
| `ErrorMessage` / `HasError` | `string?` / `bool` | `null` / `false` | Connect failure, invalid hex, send failure |
| `TerminalId` | `string` | unique | The `TerminalViewer` the traffic goes to |
| `Output` | `Action<string>` | to the terminal | Replace it to log elsewhere or in tests |

`TcpClientViewModel` adds `UseTls` (server name = `Host`), `MessageFraming` and `FramingTermination`, and `ClientFactory` (`Func<string, int, TcpClientConfig, ITcpClient>`). `UdpClientViewModel` adds `ClientFactory` (`Func<IPAddress, int, IUdpClient>`) and `AddressResolver` (DNS by default).

### Commands

| Command | Description |
|---------|-------------|
| `ConnectCommand` | Connects; enabled for a host and a port from 1 to 65535 |
| `DisconnectCommand` | Disconnects |
| `SendCommand` | Sends `SendText`; enabled when connected and there is text |

## 📝 Notes

- UDP "connect" opens a socket to the address; nothing is sent until you send
- The views are test tools, not a protocol implementation; for a real device protocol use Atc.Network's clients in code

## 🔗 Related Controls

- **TerminalViewer** (Atc.Wpf.Components) - Shows the traffic
- **NetworkScannerView** - Find the device first

## 🎮 Sample Application

See **Wpf.Network > Clients > TcpClient** and **UdpClient** in the Atc.Wpf.Sample application.