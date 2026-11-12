# ⏰ WakeOnLanView

Wakes a sleeping device by sending a Wake-on-LAN magic packet to its MAC address.

## 🔍 Overview

`WakeOnLanView` takes a MAC address (`00-11-22-33-44-55`, `00:11:22:33:44:55`, `001122334455` or `0011.2233.4455`), a broadcast address and a UDP port, and sends the magic packet with Atc.Network's `WakeOnLanHelper`. The network scanner offers the same as a row action (**Wake-on-LAN**) for a scanned host's MAC address.

## 📍 Namespace

```xml
xmlns:diagnostics="clr-namespace:Atc.Wpf.Network.Diagnostics;assembly=Atc.Wpf.Network"
```

## 🚀 Usage

```xml
<diagnostics:WakeOnLanView DataContext="{Binding WakeOnLan}" />
```

```csharp
public WakeOnLanViewModel WakeOnLan { get; } = new() { MacAddress = "00-11-22-33-44-55" };
```

## ⚙️ Properties

`WakeOnLanViewModel`:

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `MacAddress` | `string?` | `null` | The device's MAC address |
| `BroadcastAddress` | `string` | `255.255.255.255` | Where the packet goes; a subnet's broadcast address (such as `192.168.1.255`) also works |
| `Port` | `int` | `9` | UDP port (some devices listen on 7) |
| `StatusMessage` | `string?` | `null` | "Magic packet sent to …" after a send |
| `ErrorMessage` / `HasError` | `string?` / `bool` | `null` / `false` | The send failed |
| `Sender` | delegate | `WakeOnLanHelper.SendMagicPacket` | Replace it in tests |

### Commands and Methods

| Member | Description |
|--------|-------------|
| `SendCommand` | Sends the packet; enabled for a valid MAC address, broadcast address and port |
| `IsValidMacAddress(string?)` | 12 hexadecimal digits, optionally separated by `-`, `:` or `.` |

## 🔘 WakeOnLanButton

A `Button` that sends the packet when clicked, for a MAC address you already have (a settings page, a device list). It is disabled while `MacAddress` isn't valid, and raises `PacketSent` (with `Error` when sending failed).

```xml
<diagnostics:WakeOnLanButton
    Content="Wake"
    MacAddress="{Binding Device.MacAddress}"
    BroadcastAddress="192.168.1.255" />
```

| Property / event | Type | Default | Description |
|------------------|------|---------|-------------|
| `MacAddress` | `string?` | `null` | The device's MAC address |
| `BroadcastAddress` | `string` | `255.255.255.255` | Where the packet goes |
| `Port` | `int` | `9` | UDP port |
| `PacketSent` | `EventHandler<WakeOnLanSentEventArgs>` | | `MacAddress` and `Error` (`null` when sent) |
| `SendAsync()` | method | | Sends as a click does |

## 📝 Notes

- Wake-on-LAN must be on in the device's BIOS/UEFI and network adapter settings
- Sending succeeds even when no device wakes: UDP gives no answer

## 🔗 Related Controls

- **NetworkScannerView** - Its **Wake-on-LAN** row action uses the scanned MAC address

## 🎮 Sample Application

See **Wpf.Network > Diagnostics > WakeOnLan** in the Atc.Wpf.Sample application.