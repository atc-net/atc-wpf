# 🌐 LabelEndpointBox

A labeled network endpoint form field with protocol, host and port, plus mandatory indicator and validation.

## 🔍 Overview

`LabelEndpointBox` puts an `EndpointBox` (from `Atc.Wpf.Forms.BaseControls`) in the standard `LabelContent` layout used by the other `Label*` controls. The user picks a `NetworkProtocol`, types a `Host` and sets a `Port`. The composed address is exposed as a `Uri` in `Value`. The labeled control validates the host and port and shows the result in `ValidationText`. It implements `ILabelEndpointBox`.

## 📍 Namespace

```csharp
using Atc.Wpf.Forms;
```

## 🚀 Usage

### Basic Example

```xml
<atc:LabelEndpointBox
    Host="{Binding Host}"
    LabelText="Server"
    NetworkProtocol="Https"
    Port="{Binding Port}" />
```

### IPv4 Validation

```xml
<atc:LabelEndpointBox
    Host="192.168.1.8"
    IsMandatory="True"
    LabelText="OPC UA server"
    NetworkProtocol="OpcTcp"
    NetworkValidation="IPv4Address"
    Port="4840"
    ShowClearTextButton="True"
    WatermarkText="192.168.1.1" />
```

### Bind the Composed Uri

```xml
<atc:LabelEndpointBox LabelText="Endpoint" Value="{Binding EndpointUri}" />
```

## ⚙️ Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `NetworkProtocol` | `NetworkProtocolType` | `Https` | Selected protocol. Binds two-way by default |
| `Host` | `string` | `""` | Host name or IP address. Binds two-way by default |
| `Port` | `int` | `80` | Port number. Binds two-way by default |
| `Value` | `Uri?` | `null` | The composed endpoint. Binds two-way by default |
| `MinimumPort` | `int` | `1` | Lowest valid port. Binds two-way by default |
| `MaximumPort` | `int` | `65535` | Highest valid port. Binds two-way by default |
| `NetworkValidation` | `NetworkValidationRule` | `None` | How the host is validated: `None`, `IPAddress`, `IPv4Address`, `IPv6Address`, `Hostname`, `IPv4AddressOrHostname`, `IPv6AddressOrHostname`, `IPAddressOrHostname` |
| `WatermarkText` | `string` | `localhost` | Watermark shown when the host is empty |
| `ShowClearTextButton` | `bool` | `true` | Show a clear button in the host box |
| `HideUpDownButtons` | `bool` | `true` | Hide the up/down buttons of the port box |
| `IsDirty` | `bool` | `false` | Read-only. `true` once `Host` has changed after the control was created |

### 🏷️ Inherited Label Properties

Inherits the common label properties from `LabelControlBase` (`LabelText`, `LabelPosition`, `Orientation`, `LabelWidthNumber`, `LabelWidthSizeDefinition`, `HideAreas`, `InformationText`, `InformationContent`, `InformationColor`, `ContentMinHeight`, `GroupIdentifier`, `InputDataType`) and `LabelControl` (`IsMandatory`, `ShowAsteriskOnMandatory`, `MandatoryColor`, `ValidationText`, `ValidationColor`).

## ⚡ Events

| Event | Type | Description |
|-------|------|-------------|
| `NetworkProtocolLostFocus` | `EventHandler<ValueChangedEventArgs<NetworkProtocolType?>>` | Raised when an editor loses focus after the protocol changed |
| `HostLostFocus` | `EventHandler<ValueChangedEventArgs<string?>>` | Raised when the host editor loses focus after the host changed |
| `PortLostFocus` | `EventHandler<ValueChangedEventArgs<int?>>` | Raised when the port editor loses focus after the port changed |
| `ValueLostFocus` | `EventHandler<ValueChangedEventArgs<Uri?>>` | Raised when an editor loses focus after the composed `Value` changed |

## 📝 Notes

- Validation runs in this order: mandatory host, port range (`MinimumPort` to `MaximumPort`), then the `NetworkValidation` rule on the host
- A mandatory empty host shows "Field is required" only after the host has been edited (`IsDirty`) or when `IsValid()` is called
- The events are forwarded from the inner `EndpointBox` with this control's `Identifier`; they are raised on lost focus, not on every change
- The endpoint editor itself is described in `EndpointBox_Readme.md`

## 🔗 Related Controls

- **EndpointBox** - The endpoint editor without the label
- **LabelTextBox** - Labeled free-text input
- **LabelIntegerBox** - Labeled integer input

## 🎮 Sample Application

See the LabelEndpointBox sample in the Atc.Wpf.Sample application under **Wpf.Forms > Label Controls > LabelEndpointBox** for interactive examples.
