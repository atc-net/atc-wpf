# 🌐 LabelIPAddressBox, LabelIPRangeBox and LabelMacAddressBox

Labelled text fields for network addresses, with the right validation already set.

## 🔍 Overview

All three are `LabelTextBox`es (label, mandatory asterisk, information popup, validation message) whose `ValidationFormat` is preset:

| Control | `ValidationFormat` | Accepts | Message when invalid |
|---------|--------------------|---------|----------------------|
| `LabelIPAddressBox` | `IPAddress` | IPv4 or IPv6 address | "Invalid IP address" |
| `LabelIPRangeBox` | `IPv4Cidr` | One or more IPv4 ranges in CIDR notation, separated by `,` or `;`, such as `192.168.1.0/24` or `192.168.1.0/24, 10.0.0.0/24` | "Invalid IPv4 range (CIDR), such as 192.168.1.0/24" |
| `LabelMacAddressBox` | `MacAddress` | 12 hexadecimal digits, separated by `-`, `:`, `.` or nothing | "Invalid MAC address" |

The messages are in English, Danish and German. Setting `ValidationFormat` yourself still wins (for example `IPv4Address` on a `LabelIPAddressBox`).

## 📍 Namespace

```xml
xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas"
```

## 🚀 Usage

```xml
<atc:LabelIPAddressBox LabelText="Server" Text="{Binding ServerAddress}" IsMandatory="True" />
<atc:LabelIPRangeBox LabelText="Scan range" Text="{Binding Range}" />
<atc:LabelMacAddressBox LabelText="MAC address" Text="{Binding MacAddress}" />
```

## ⚙️ Properties

The same as `LabelTextBox` (see `LabelTextBox_Readme.md`). `TextBoxValidationRuleType` has two new values, usable on any `LabelTextBox`: `MacAddress` and `IPv4Cidr`.

## 🔗 Related Controls

- **LabelTextBox** - The base control
- **LabelEndpointBox** - Host and port, or a URL
- **WakeOnLanView** and **NetworkScannerSettingsView** (Atc.Wpf.Network) - Use these fields

## 🎮 Sample Application

See **Wpf.Forms > LabelNetworkAddressBoxes** in the Atc.Wpf.Sample application.