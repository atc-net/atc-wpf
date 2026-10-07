# Atc.Wpf.Hardware

WPF pickers for hardware devices and system resources, with live device-state detection.

## 🔍 Overview

Each picker lists the current devices or resources, keeps the list up to date while it is loaded, and shows the state of every entry: **Available**, **JustConnected**, **InUse** or **Disconnected**. When the selected device disappears, the picker raises `DeviceLost` and can clear or keep the selection; when it comes back, it can re-select it automatically (`DeviceReconnected`).

| Picker | Selects | Change detection |
|--------|---------|------------------|
| `SerialPortPicker` | Serial (COM) ports, optional in-use probe | `DeviceWatcher` |
| `UsbPortPicker` | USB devices, filterable by device class | `DeviceWatcher` |
| `UsbCameraPicker` | Cameras, optional live preview and format selection | `DeviceWatcher` |
| `AudioInputPicker` | Microphones, optional live level meter | `DeviceWatcher` |
| `AudioOutputPicker` | Speakers, optional test tone | `DeviceWatcher` |
| `BluetoothDevicePicker` | Bluetooth devices | `DeviceWatcher` |
| `DrivePicker` | Drives | Polling |
| `DisplayPicker` | Monitors | Polling |
| `PrinterPicker` | Print queues | Polling |
| `NetworkAdapterPicker` | Network adapters | Polling |
| `ProcessPicker` | Running processes | Polling |
| `WindowPicker` | Top-level windows | Polling |

Every picker has a `Label*` wrapper (for example `LabelSerialPortPicker`) with label, mandatory marker and validation text for forms.

## 📦 Installation

```xml
<PackageReference Include="Atc.Wpf.Hardware" Version="4.*" />
```

Requires Windows 10 version 2004 (build 19041) or later - the package targets `net10.0-windows10.0.19041.0`.

## 🚀 Quick Start

```xml
<Window xmlns:atc="https://github.com/atc-net/atc-wpf/tree/main/schemas">
    <atc:SerialPortPicker
        Value="{Binding SelectedPort, Mode=TwoWay}"
        WatermarkText="Select a COM port" />
</Window>
```

Polling pickers expose their service tuning, for example:

```xml
<atc:ProcessPicker PollingInterval="00:00:05" OnlyWithMainWindow="True" />
```

To share one watcher between several pickers, or to supply a test double, pass the service to the picker's constructor (`new ProcessPicker(IProcessService service)`).

## 📖 Documentation

- **[Hardware guide](../../docs/Hardware/@Readme.md)** - shared properties, device-state behaviour and localization
- One `*_Readme.md` per picker in [`Pickers/`](Pickers/)

## 🎮 Sample Application

`sample/Atc.Wpf.Sample` → **Wpf.Hardware** in the sample tree.