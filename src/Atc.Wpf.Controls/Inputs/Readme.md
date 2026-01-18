# Inputs

Input controls are unlabeled input components that provide core functionality without the label wrapper. These controls are used as building blocks for Label Controls but
can also be used independently for more flexible UI layouts.

## Table of Contents

- [Inputs](#inputs)
  - [Table of Contents](#table-of-contents)
  - [Overview](#overview)
  - [Common Features](#common-features)
  - [Quick Reference](#quick-reference)
  - [Text Input Controls](#text-input-controls)
    - [RichTextBoxEx](#richtextboxex)
      - [Features](#features)
      - [Usage Example](#usage-example)
  - [Number Input Controls](#number-input-controls)
    - [IntegerBox](#integerbox)
      - [Features](#features-1)
      - [Properties](#properties)
      - [Usage Example](#usage-example-1)
    - [DecimalBox](#decimalbox)
      - [Features](#features-2)
      - [Properties](#properties-1)
      - [Usage Example](#usage-example-2)
    - [IntegerXyBox](#integerxybox)
      - [Features](#features-3)
      - [Properties](#properties-2)
      - [Usage Example](#usage-example-3)
    - [DecimalXyBox](#decimalxybox)
      - [Features](#features-4)
      - [Properties](#properties-3)
      - [Usage Example](#usage-example-4)
    - [PixelSizeBox](#pixelsizebox)
      - [Features](#features-5)
      - [Properties](#properties-4)
      - [Usage Example](#usage-example-5)
  - [Time Selection Controls](#time-selection-controls)
    - [ClockPanelPicker](#clockpanelpicker)
      - [Features](#features-6)
      - [Usage Example](#usage-example-6)
  - [Toggle Controls](#toggle-controls)
    - [ToggleSwitch](#toggleswitch)
      - [Features](#features-7)
      - [Properties](#properties-5)
      - [Usage Example](#usage-example-7)
  - [Color Controls](#color-controls)
    - [ColorPicker](#colorpicker)
      - [Features](#features-8)
      - [Properties](#properties-6)
      - [Usage Example](#usage-example-8)
  - [Network Controls](#network-controls)
    - [EndpointBox](#endpointbox)
      - [Features](#features-11)
      - [Properties](#properties-9)
      - [Usage Example](#usage-example-11)
      - [Validation](#validation)
  - [Usage in Label Controls](#usage-in-label-controls)
  - [Theming](#theming)
    - [Theme Support](#theme-support)
    - [Example Theme Configuration](#example-theme-configuration)
  - [Summary](#summary)

## Overview

Base controls provide specialized input functionality without labels, making them ideal for:

- Custom layouts where you want to position labels yourself
- Building composite controls
- Situations where labels are not needed
- Creating your own labeled control wrappers

All base controls support:
- Data binding with `INotifyPropertyChanged`
- WPF commanding patterns
- Light and Dark theme support
- Validation and error handling

## Common Features

Base controls are designed to be lightweight and focused on their core functionality:

- **No Label Overhead** - Just the input control without label infrastructure
- **Flexible Layout** - Place them anywhere in your UI
- **Direct Value Access** - Bind directly to the value property
- **Event Support** - Full event support for value changes
- **Theme Compatible** - Work seamlessly with Atc.Wpf.Theming

## Quick Reference

Here's a complete list of all available Base Controls organized by category:

| Category | Controls |
|----------|----------|
| **Text Input** | RichTextBoxEx |
| **Number Input** | IntegerBox, DecimalBox, IntegerXyBox, DecimalXyBox, PixelSizeBox |
| **Time Selection** | ClockPanelPicker |
| **Toggle** | ToggleSwitch |
| **Color** | ColorPicker |
| **Network** | EndpointBox |

> **Note:** DirectoryPicker and FilePicker have been moved to the [Pickers](../Pickers/Readme.md) folder.

## Text Input Controls

### RichTextBoxEx

An extended RichTextBox with additional functionality.

#### Features

- Extended rich text editing capabilities
- Document binding support
- Enhanced formatting options

#### Usage Example

```xml
<atc:RichTextBoxEx 
    x:Name="richTextBox"
    Document="{Binding DocumentContent, Mode=TwoWay}" />
```

## Number Input Controls

### IntegerBox

A text box that only accepts integer values with validation.

#### Features

- Integer-only input
- Automatic validation
- Minimum and maximum value support
- Numeric formatting

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Value` | `int?` | `null` | The integer value |
| `MinimumValue` | `int` | `int.MinValue` | Minimum allowed value |
| `MaximumValue` | `int` | `int.MaxValue` | Maximum allowed value |

#### Usage Example

```xml
<!-- Simple integer input -->
<atc:IntegerBox
    Value="{Binding Quantity, Mode=TwoWay}"
    MinimumValue="1"
    MaximumValue="100" />

<!-- Port number input -->
<atc:IntegerBox
    Value="{Binding Port, Mode=TwoWay}"
    MinimumValue="1"
    MaximumValue="65535" />
```

### DecimalBox

A text box that only accepts decimal values with validation.

#### Features

- Decimal number input
- Automatic validation
- Precision control
- Minimum and maximum value support

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Value` | `decimal?` | `null` | The decimal value |
| `MinimumValue` | `decimal` | `decimal.MinValue` | Minimum allowed value |
| `MaximumValue` | `decimal` | `decimal.MaxValue` | Maximum allowed value |

#### Usage Example

```xml
<!-- Price input -->
<atc:DecimalBox
    Value="{Binding Price, Mode=TwoWay}"
    MinimumValue="0"
    MaximumValue="9999.99" />

<!-- Percentage input -->
<atc:DecimalBox
    Value="{Binding Percentage, Mode=TwoWay}"
    MinimumValue="0"
    MaximumValue="100" />
```

### IntegerXyBox

A control for entering X and Y integer coordinates.

#### Features

- Two integer input fields (X and Y)
- Independent validation for each axis
- Coordinate pair value

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ValueX` | `int?` | `null` | The X coordinate |
| `ValueY` | `int?` | `null` | The Y coordinate |
| `MinimumValue` | `int` | `int.MinValue` | Minimum value for both axes |
| `MaximumValue` | `int` | `int.MaxValue` | Maximum value for both axes |

#### Usage Example

```xml
<!-- Screen position -->
<atc:IntegerXyBox
    ValueX="{Binding PositionX, Mode=TwoWay}"
    ValueY="{Binding PositionY, Mode=TwoWay}"
    MinimumValue="0"
    MaximumValue="3840" />
```

### DecimalXyBox

A control for entering X and Y decimal coordinates.

#### Features

- Two decimal input fields (X and Y)
- Independent validation for each axis
- Precision control
- Coordinate pair value

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ValueX` | `decimal?` | `null` | The X coordinate |
| `ValueY` | `decimal?` | `null` | The Y coordinate |
| `MinimumValue` | `decimal` | `decimal.MinValue` | Minimum value for both axes |
| `MaximumValue` | `decimal` | `decimal.MaxValue` | Maximum value for both axes |

#### Usage Example

```xml
<!-- Geographic coordinates -->
<atc:DecimalXyBox
    ValueX="{Binding Longitude, Mode=TwoWay}"
    ValueY="{Binding Latitude, Mode=TwoWay}"
    MinimumValue="-180"
    MaximumValue="180" />
```

### PixelSizeBox

A control for entering pixel dimensions (width and height).

#### Features

- Width and height input fields
- Pixel unit indication
- Dimension validation
- Common size presets

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Width` | `int?` | `null` | The width in pixels |
| `Height` | `int?` | `null` | The height in pixels |
| `MinimumValue` | `int` | `1` | Minimum dimension value |
| `MaximumValue` | `int` | `int.MaxValue` | Maximum dimension value |

#### Usage Example

```xml
<!-- Image size -->
<atc:PixelSizeBox
    Width="{Binding ImageWidth, Mode=TwoWay}"
    Height="{Binding ImageHeight, Mode=TwoWay}"
    MinimumValue="1"
    MaximumValue="4096" />

<!-- Window size -->
<atc:PixelSizeBox
    Width="{Binding WindowWidth, Mode=TwoWay}"
    Height="{Binding WindowHeight, Mode=TwoWay}"
    MinimumValue="800"
    MaximumValue="3840" />
```

## Time Selection Controls

### ClockPanelPicker

A visual clock panel for selecting time values.

#### Features

- Visual clock interface
- 12-hour and 24-hour format support
- Hour and minute selection
- AM/PM selection for 12-hour format

#### Usage Example

```xml
<!-- 24-hour format -->
<atc:ClockPanelPicker
    SelectedTime="{Binding AppointmentTime, Mode=TwoWay}"
    Is24HourFormat="True" />

<!-- 12-hour format with AM/PM -->
<atc:ClockPanelPicker
    SelectedTime="{Binding MeetingTime, Mode=TwoWay}"
    Is24HourFormat="False" />
```

## Toggle Controls

### ToggleSwitch

A modern toggle switch for on/off states.

#### Features

- Smooth toggle animation
- On/Off text labels
- IsOn state binding
- Customizable colors
- Size variations

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IsOn` | `bool` | `false` | The toggle state (on/off) |
| `ContentDirection` | `FlowDirection` | `LeftToRight` | Direction of content flow |
| `OnText` | `string` | `"On"` | Text displayed when toggled on |
| `OffText` | `string` | `"Off"` | Text displayed when toggled off |
| `OnBackground` | `Brush` | Theme color | Background when toggled on |
| `OffBackground` | `Brush` | Theme color | Background when toggled off |
| `ThumbBrush` | `Brush` | White | Color of the toggle thumb |

#### Usage Example

```xml
<!-- Basic toggle -->
<atc:ToggleSwitch
    IsOn="{Binding IsEnabled, Mode=TwoWay}" />

<!-- Custom labels -->
<atc:ToggleSwitch
    IsOn="{Binding DarkModeEnabled, Mode=TwoWay}"
    OnText="Dark"
    OffText="Light" />

<!-- Custom colors -->
<atc:ToggleSwitch
    IsOn="{Binding NotificationsEnabled, Mode=TwoWay}"
    OnBackground="Green"
    OffBackground="Red" />
```

## Color Controls

### ColorPicker

A control for selecting colors with various input methods.

#### Features

- RGB/HSV color selection
- Hex color code input
- Alpha channel support
- Color preview
- Recent colors history

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `SelectedColor` | `Color` | `Colors.Black` | The selected color |
| `ShowAlphaChannel` | `bool` | `true` | Show/hide alpha channel slider |

#### Usage Example

```xml
<!-- Basic color picker -->
<atc:ColorPicker
    SelectedColor="{Binding BackgroundColor, Mode=TwoWay}" />

<!-- With alpha channel -->
<atc:ColorPicker
    SelectedColor="{Binding OverlayColor, Mode=TwoWay}"
    ShowAlphaChannel="True" />

<!-- Without alpha channel -->
<atc:ColorPicker
    SelectedColor="{Binding BorderColor, Mode=TwoWay}"
    ShowAlphaChannel="False" />
```

## Network Controls

### EndpointBox

A control for entering network endpoints with protocol, host, and port.

#### Features

- Protocol selection (HTTP, HTTPS, FTP, FTPS, TCP, UDP, OPC TCP)
- Host input with validation
- Port input with range validation
- Combined URI output
- Network validation rules (IPv4, IPv6, etc.)
- Clear button support

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `NetworkProtocol` | `NetworkProtocolType` | `Https` | The network protocol |
| `Host` | `string` | `""` | The host address |
| `Port` | `int` | `80` | The port number |
| `Value` | `Uri?` | `null` | The complete endpoint as a URI |
| `WatermarkText` | `string` | `"localhost"` | Placeholder text for host field |
| `ShowClearTextButton` | `bool` | `true` | Show/hide clear button |
| `HideUpDownButtons` | `bool` | `true` | Show/hide port increment/decrement buttons |
| `NetworkValidation` | `NetworkValidationRule` | `None` | Additional host validation (IPv4, IPv6, etc.) |
| `MinimumPort` | `int` | `1` | Minimum port number |
| `MaximumPort` | `int` | `65535` | Maximum port number |

#### Usage Example

```xml
<!-- Basic HTTP endpoint -->
<atc:EndpointBox
    NetworkProtocol="Http"
    Host="{Binding ServerHost, Mode=TwoWay}"
    Port="{Binding ServerPort, Mode=TwoWay}" />

<!-- With URI binding -->
<atc:EndpointBox
    Value="{Binding ApiEndpoint, Mode=TwoWay}"
    NetworkProtocol="Https" />

<!-- OPC TCP with IPv4 validation -->
<atc:EndpointBox
    NetworkProtocol="OpcTcp"
    NetworkValidation="IPv4Address"
    Host="{Binding OpcHost, Mode=TwoWay}"
    Port="{Binding OpcPort, Mode=TwoWay}"
    MinimumPort="4840"
    MaximumPort="4850" />
```

#### Validation

The control provides multiple levels of validation:

1. **Port Range Validation** - Validates port is within `MinimumPort` and `MaximumPort`
2. **Protocol-Based Host Validation** - Validates host format based on the selected `NetworkProtocol`
3. **Network Validation** - Additional validation based on `NetworkValidation` property (e.g., IPv4Address format)

## Usage in Label Controls

Base controls are used as the core input mechanism for Label Controls. You can use the same base control in multiple contexts:

```xml
<!-- As a standalone control -->
<atc:IntegerBox Value="{Binding Count}" />

<!-- Within a Label Control -->
<atc:LabelIntegerBox
    LabelText="Count"
    Value="{Binding Count}" />
```

This design allows for maximum flexibility - use base controls directly when you want full layout control,
or use label controls when you want the convenience of integrated labels,
validation messages, and mandatory indicators.

## Theming

All base controls support both Light and Dark themes through the Atc.Wpf.Theming package.

### Theme Support

- Automatic theme switching based on application theme
- Consistent styling across all base controls
- Custom theme colors for focused/hover states
- High contrast mode support

### Example Theme Configuration

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="pack://application:,,,/Atc.Wpf.Theming;component/Themes/Generic.xaml" />
            <ResourceDictionary Source="pack://application:,,,/Atc.Wpf.Controls;component/Styles/Controls.xaml" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

## Summary

The Base Controls library provides a focused set of unlabeled input controls for WPF applications:

- **12 specialized controls** covering numbers, text, time, colors, and network endpoints
- **Lightweight design** - No label infrastructure overhead
- **Flexible usage** - Use standalone or as building blocks for custom controls
- **Theme support** - Light and Dark themes out of the box
- **Validation** - Built-in validation for appropriate control types
- **MVVM ready** - Full data binding and command support

Base controls are the foundation of the Label Controls library,
providing the core input functionality that can be composed into more complex UI elements.

> **See also:** [Pickers](../Pickers/Readme.md) for DirectoryPicker and FilePicker controls.
