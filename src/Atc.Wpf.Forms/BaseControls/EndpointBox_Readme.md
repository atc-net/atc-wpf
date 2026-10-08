# 🌐 EndpointBox

An input control for network endpoints, made of a protocol prefix, a host text box and a port box, that builds a `Uri` from the parts.

## 🔍 Overview

`EndpointBox` lets the user enter a host and port for a given network protocol (e.g. `https://`, `opc.tcp://`). The protocol is shown as a fixed prefix, the host is entered in a text box with optional watermark and clear button, and the port is entered in a numeric box limited by `MinimumPort` / `MaximumPort`.

Whenever the protocol, host or port changes, the control validates the input and updates `Value` with the resulting `Uri`, or `null` if the input is invalid. Setting `Value` from the outside splits the `Uri` back into protocol, host and port.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms.BaseControls;
```

## 🚀 Usage

### Basic Example

```xml
<atc:EndpointBox
    Host="api.example.com"
    NetworkProtocol="Https"
    Port="443" />
```

### OPC UA Endpoint

```xml
<atc:EndpointBox
    Host="192.168.1.8"
    NetworkProtocol="OpcTcp"
    Port="4840" />
```

### With Host Validation and Watermark

```xml
<atc:EndpointBox
    Host="192.168.1.8"
    NetworkProtocol="OpcTcp"
    NetworkValidation="IPv4Address"
    Port="4840"
    WatermarkText="Enter IPv4 address" />
```

### Bind to a Uri

```xml
<atc:EndpointBox NetworkProtocol="Http" Value="{Binding ServerUri}" />
```

### Listen for Changes

```xml
<atc:EndpointBox ValueChanged="OnEndpointValueChanged" />
```

```csharp
private void OnEndpointValueChanged(
    object sender,
    RoutedPropertyChangedEventArgs<Uri?> e)
{
    var newUri = e.NewValue;
}
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `NetworkProtocol` | `NetworkProtocolType` | `Https` | Protocol used for the scheme prefix. Binds two-way by default |
| `Host` | `string` | `""` | Host name or IP address. Binds two-way by default |
| `Port` | `int` | `80` | Port number. Binds two-way by default |
| `MinimumPort` | `int` | `1` | Lowest allowed port. Binds two-way by default |
| `MaximumPort` | `int` | `65535` | Highest allowed port. Binds two-way by default |
| `Value` | `Uri?` | `null` | The resulting endpoint. Binds two-way by default |
| `NetworkValidation` | `NetworkValidationRule` | `None` | Extra validation of the host (see below) |
| `WatermarkText` | `string` | `localhost` | Watermark shown in the empty host box |
| `ShowClearTextButton` | `bool` | `true` | Show a clear button in the host box |
| `HideUpDownButtons` | `bool` | `true` | Hide the up/down buttons on the port box |
| `IsDirty` | `bool` | `false` | Read-only (CLR). Set to `true` once the host has been changed |

### NetworkProtocolType

`None`, `Ftp`, `Ftps`, `Http`, `Https`, `OpcTcp`, `Tcp`, `Udp`

### NetworkValidationRule

`None`, `IPAddress`, `IPv4Address`, `IPv6Address`, `Hostname`, `IPv4AddressOrHostname`, `IPv6AddressOrHostname`, `IPAddressOrHostname`

## ⚡ Events

### Routed Events (bubbling)

| Event | Handler Type | Description |
|-------|--------------|-------------|
| `NetworkProtocolChanged` | `RoutedPropertyChangedEventHandler<NetworkProtocolType>` | Raised when `NetworkProtocol` changes |
| `HostChanged` | `RoutedPropertyChangedEventHandler<string>` | Raised when `Host` changes |
| `PortChanged` | `RoutedPropertyChangedEventHandler<int>` | Raised when `Port` changes |
| `ValueChanged` | `RoutedPropertyChangedEventHandler<Uri?>` | Raised when `Value` changes |

### CLR Events

| Event | Type | Description |
|-------|------|-------------|
| `NetworkProtocolLostFocus` | `EventHandler<ValueChangedEventArgs<NetworkProtocolType?>>` | Raised when the host or port editor loses focus and `NetworkProtocol` changed while it had focus |
| `HostLostFocus` | `EventHandler<ValueChangedEventArgs<string?>>` | Raised when the host editor loses focus and `Host` changed while it had focus |
| `PortLostFocus` | `EventHandler<ValueChangedEventArgs<int?>>` | Raised when the port editor loses focus and `Port` changed while it had focus |
| `ValueLostFocus` | `EventHandler<ValueChangedEventArgs<Uri?>>` | Raised when the host or port editor loses focus and `Value` changed while it had focus |

## 📝 Notes

- `Value` becomes `null` when the host is empty, fails protocol validation, fails `NetworkValidation`, or cannot be turned into a `Uri`
- Setting `Value` updates `Host`, `Port` and `NetworkProtocol` from the `Uri`
- The routed `*Changed` events fire on every change; the `*LostFocus` events fire once per edit, when the editor loses focus, with the value from when it got focus as `OldValue`. Values set from code do not raise them
- `Value` is recalculated when the control is loaded and when `NetworkValidation` changes

## 🔗 Related Controls

- **LabelEndpointBox** - Labeled form-field version with validation and mandatory indicator
- **IntegerBox** - Numeric input used for the port
- **NetworkScannerView** - Network scanning and discovery (Atc.Wpf.Network)

## 🎮 Sample Application

See the EndpointBox sample in the Atc.Wpf.Sample application under **Wpf.Controls > Inputs > EndpointBox** for interactive examples.
