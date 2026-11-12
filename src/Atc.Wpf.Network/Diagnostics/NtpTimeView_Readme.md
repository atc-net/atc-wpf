# 🕒 NtpTimeView

Asks an NTP server for the time and shows how far this machine's clock is off.

## 🔍 Overview

`NtpTimeView` queries an NTP server (default `pool.ntp.org`) with Atc.Network's `NtpHelper` and shows the server's time (UTC), the clock offset, the round trip time, the server's stratum and the address that answered. Useful for checking devices and servers whose clocks drift, for example before ONVIF digest authentication fails because of a wrong clock.

## 📍 Namespace

```xml
xmlns:diagnostics="clr-namespace:Atc.Wpf.Network.Diagnostics;assembly=Atc.Wpf.Network"
```

## 🚀 Usage

```xml
<diagnostics:NtpTimeView DataContext="{Binding NtpTime}" />
```

```csharp
public NtpTimeViewModel NtpTime { get; } = new() { Server = "time.windows.com" };
```

## ⚙️ Properties

`NtpTimeViewModel`:

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Server` | `string?` | `pool.ntp.org` | NTP server host name or address |
| `ServerTimeUtc` | `DateTime?` | `null` | The server's time when it answered |
| `Offset` | `TimeSpan?` | `null` | How far the local clock is behind (positive) or ahead (negative) |
| `RoundTripDelay` | `TimeSpan?` | `null` | Round trip time to the server |
| `Stratum` | `int?` | `null` | 1 = connected to a reference clock |
| `ServerAddress` | `string?` | `null` | The address that answered |
| `ErrorMessage` / `HasError` | `string?` / `bool` | `null` / `false` | No answer, or another error |
| `TimeQuery` | delegate | `NtpHelper.GetTime` | Replace it in tests |

### Commands

| Command | Description |
|---------|-------------|
| `QueryCommand` | Asks the server; enabled when a server is set |

## 🔗 Related Controls

- **TracerouteView**, **WakeOnLanView** - The other network diagnostics

## 🎮 Sample Application

See **Wpf.Network > Diagnostics > NtpTime** in the Atc.Wpf.Sample application.