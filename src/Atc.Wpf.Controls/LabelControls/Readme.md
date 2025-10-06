# Label Controls

Label controls provide a consistent way to display labeled input fields with validation, mandatory indicators, and other UI features.

## Table of Contents

- [Label Controls](#label-controls)
  - [Table of Contents](#table-of-contents)
  - [Overview](#overview)
  - [Common Features](#common-features)
  - [Quick Reference](#quick-reference)
  - [Text Input Controls](#text-input-controls)
    - [LabelTextBox](#labeltextbox)
      - [Features](#features)
      - [Properties](#properties)
      - [Usage Example](#usage-example)
    - [LabelPasswordBox](#labelpasswordbox)
      - [Features](#features-1)
      - [Properties](#properties-1)
      - [Usage Example](#usage-example-1)
  - [Number Input Controls](#number-input-controls)
    - [LabelIntegerBox](#labelintegerbox)
      - [Features](#features-2)
      - [Properties](#properties-2)
      - [Usage Example](#usage-example-2)
    - [LabelDecimalBox](#labeldecimalbox)
      - [Features](#features-3)
      - [Properties](#properties-3)
      - [Usage Example](#usage-example-3)
    - [LabelIntegerXyBox](#labelintegerxybox)
      - [Features](#features-4)
      - [Usage Example](#usage-example-4)
    - [LabelDecimalXyBox](#labeldecimalxybox)
      - [Features](#features-5)
      - [Usage Example](#usage-example-5)
    - [LabelPixelSizeBox](#labelpixelsizebox)
      - [Features](#features-6)
      - [Usage Example](#usage-example-6)
  - [Date and Time Controls](#date-and-time-controls)
    - [LabelDatePicker](#labeldatepicker)
      - [Features](#features-7)
      - [Usage Example](#usage-example-7)
    - [LabelTimePicker](#labeltimepicker)
      - [Features](#features-8)
      - [Usage Example](#usage-example-8)
    - [LabelDateTimePicker](#labeldatetimepicker)
      - [Features](#features-9)
      - [Usage Example](#usage-example-9)
  - [Selection Controls](#selection-controls)
    - [LabelCheckBox](#labelcheckbox)
      - [Features](#features-10)
      - [Properties](#properties-4)
      - [Usage Example](#usage-example-10)
    - [LabelComboBox](#labelcombobox)
      - [Features](#features-11)
      - [Properties](#properties-5)
      - [Usage Example](#usage-example-11)
    - [LabelToggleSwitch](#labeltoggleswitch)
      - [Features](#features-12)
      - [Usage Example](#usage-example-12)
  - [Selector Controls](#selector-controls)
    - [LabelAccentColorSelector](#labelaccentcolorselector)
      - [Features](#features-13)
      - [Usage Example](#usage-example-13)
    - [LabelCountrySelector](#labelcountryselector)
      - [Features](#features-14)
      - [Usage Example](#usage-example-14)
    - [LabelFontFamilySelector](#labelfontfamilyselector)
      - [Features](#features-15)
      - [Usage Example](#usage-example-15)
    - [LabelLanguageSelector](#labellanguageselector)
      - [Features](#features-16)
      - [Usage Example](#usage-example-16)
    - [LabelThemeSelector](#labelthemeselector)
      - [Features](#features-17)
      - [Usage Example](#usage-example-17)
    - [LabelThemeAndAccentColorSelectors](#labelthemeandaccentcolorselectors)
      - [Features](#features-18)
      - [Usage Example](#usage-example-18)
    - [LabelWellKnownColorSelector](#labelwellknowncolorselector)
      - [Features](#features-19)
      - [Usage Example](#usage-example-19)
  - [Picker Controls](#picker-controls)
    - [LabelColorPicker](#labelcolorpicker)
      - [Features](#features-20)
      - [Usage Example](#usage-example-20)
    - [LabelDirectoryPicker](#labeldirectorypicker)
      - [Features](#features-21)
      - [Usage Example](#usage-example-21)
    - [LabelFilePicker](#labelfilepicker)
      - [Features](#features-22)
      - [Usage Example](#usage-example-22)
  - [Slider Controls](#slider-controls)
    - [LabelSlider](#labelslider)
      - [Features](#features-23)
      - [Usage Example](#usage-example-23)
  - [Network and Endpoint Controls](#network-and-endpoint-controls)
    - [LabelEndpointBox](#labelendpointbox)
      - [Features](#features-24)
      - [Properties](#properties-6)
      - [Usage Example](#usage-example-24)
      - [Validation](#validation)
      - [Events](#events)
  - [Information Display Controls](#information-display-controls)
    - [LabelContent](#labelcontent)
      - [Features](#features-25)
      - [Usage Example](#usage-example-25)
    - [LabelTextInfo](#labeltextinfo)
      - [Features](#features-26)
      - [Usage Example](#usage-example-26)
  - [Base Controls](#base-controls)
  - [Validation Behavior](#validation-behavior)
    - [Validation Implementation](#validation-implementation)
  - [Theming](#theming)
    - [Theme Support](#theme-support)
    - [Example Theme Configuration](#example-theme-configuration)
  - [Summary](#summary)

## Overview

All label controls inherit from `LabelControl` base class and follow a consistent pattern:

- Label text on the left (or top)
- Input control on the right (or bottom)
- Optional mandatory indicator (asterisk)
- Validation message display
- Information tooltip support

## Common Features

All label controls share these properties from `ILabelControl`:

| Property | Type | Description |
|----------|------|-------------|
| `LabelText` | `string` | The text to display in the label |
| `IsMandatory` | `bool` | Whether the field is required |
| `ShowAsteriskOnMandatory` | `bool` | Show asterisk (*) for mandatory fields |
| `MandatoryColor` | `Brush` | Color for the mandatory indicator |
| `ValidationText` | `string` | Validation error message to display |
| `ValidationColor` | `Brush` | Color for validation messages |
| `Identifier` | `string` | Unique identifier for the control |
| `GroupIdentifier` | `string` | Group identifier for related controls |
| `Orientation` | `Orientation` | Layout orientation (Horizontal/Vertical) |
| `HideAreas` | `LabelControlHideAreas` | Areas to hide (Label, Information, Validation) |

## Quick Reference

Here's a complete list of all available Label Controls organized by category:

| Category | Controls |
|----------|----------|
| **Text Input** | LabelTextBox, LabelPasswordBox |
| **Number Input** | LabelIntegerBox, LabelDecimalBox, LabelIntegerXyBox, LabelDecimalXyBox, LabelPixelSizeBox |
| **Date/Time** | LabelDatePicker, LabelTimePicker, LabelDateTimePicker |
| **Selection** | LabelCheckBox, LabelComboBox, LabelToggleSwitch |
| **Selectors** | LabelAccentColorSelector, LabelCountrySelector, LabelFontFamilySelector, LabelLanguageSelector, LabelThemeSelector, LabelThemeAndAccentColorSelectors, LabelWellKnownColorSelector |
| **Pickers** | LabelColorPicker, LabelDirectoryPicker, LabelFilePicker |
| **Slider** | LabelSlider |
| **Network** | LabelEndpointBox |
| **Information Display** | LabelContent, LabelTextInfo |

## Text Input Controls

### LabelTextBox

A labeled text box with validation support for text input.

#### Features

- Single-line text input
- Watermark (placeholder) text
- Text alignment and trimming options
- Maximum and minimum length validation
- Clear button support
- Regular expression validation
- Multiple validation rules (Email, Uri, Numeric, etc.)

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Text` | `string` | `""` | The text content |
| `WatermarkText` | `string` | `""` | Placeholder text when empty |
| `TextAlignment` | `TextAlignment` | `Left` | Text alignment (Left, Center, Right) |
| `TextTrimming` | `TextTrimming` | `None` | Text trimming behavior |
| `MaxLength` | `int` | `100` | Maximum allowed characters |
| `MinLength` | `int` | `0` | Minimum required characters |
| `ShowClearTextButton` | `bool` | `true` | Show/hide the clear button |
| `RegexPattern` | `string?` | `null` | Regular expression for validation |
| `ValidationRule` | `TextBoxValidationRuleType` | `None` | Predefined validation rule (Email, Uri, etc.) |

#### Usage Example

```xml
<!-- Basic text box -->
<atc:LabelTextBox
    LabelText="Name"
    Text="{Binding UserName, Mode=TwoWay}"
    WatermarkText="Enter your name"
    IsMandatory="True" />

<!-- Email validation -->
<atc:LabelTextBox
    LabelText="Email"
    Text="{Binding Email, Mode=TwoWay}"
    ValidationRule="Email"
    WatermarkText="user@example.com" />

<!-- With regex pattern -->
<atc:LabelTextBox
    LabelText="Product Code"
    Text="{Binding ProductCode, Mode=TwoWay}"
    RegexPattern="^[A-Z]{3}-\d{4}$"
    MinLength="8"
    MaxLength="8" />
```

### LabelPasswordBox

A labeled password box with secure text entry and validation.

#### Features

- Secure password entry (masked characters)
- Watermark text
- Maximum and minimum length validation
- Clear button support
- Regular expression validation
- Validation rules for password complexity

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Password` | `string` | `""` | The password value |
| `WatermarkText` | `string` | `""` | Placeholder text when empty |
| `TextAlignment` | `TextAlignment` | `Left` | Text alignment |
| `TextTrimming` | `TextTrimming` | `None` | Text trimming behavior |
| `MaxLength` | `int` | `100` | Maximum allowed characters |
| `MinLength` | `int` | `0` | Minimum required characters |
| `ShowClearTextButton` | `bool` | `true` | Show/hide the clear button |
| `RegexPattern` | `string?` | `null` | Regular expression for validation |

#### Usage Example

```xml
<!-- Basic password box -->
<atc:LabelPasswordBox
    LabelText="Password"
    Password="{Binding UserPassword, Mode=TwoWay}"
    WatermarkText="Enter password"
    IsMandatory="True"
    MinLength="8" />

<!-- With complexity validation -->
<atc:LabelPasswordBox
    LabelText="New Password"
    Password="{Binding NewPassword, Mode=TwoWay}"
    MinLength="12"
    RegexPattern="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&amp;])" />
```

## Number Input Controls

### LabelIntegerBox

A labeled integer input box with range validation.

#### Features

- Integer-only input
- Minimum and maximum value validation
- Text alignment
- Value change events

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Value` | `int?` | `null` | The integer value |
| `WatermarkText` | `string` | `""` | Placeholder text |
| `TextAlignment` | `TextAlignment` | `Left` | Text alignment |
| `TextTrimming` | `TextTrimming` | `None` | Text trimming behavior |
| `MinimumValue` | `int` | `int.MinValue` | Minimum allowed value |
| `MaximumValue` | `int` | `int.MaxValue` | Maximum allowed value |

#### Usage Example

```xml
<!-- Basic integer input -->
<atc:LabelIntegerBox
    LabelText="Age"
    Value="{Binding Age, Mode=TwoWay}"
    MinimumValue="0"
    MaximumValue="150" />

<!-- Port number with validation -->
<atc:LabelIntegerBox
    LabelText="Port"
    Value="{Binding ServerPort, Mode=TwoWay}"
    MinimumValue="1"
    MaximumValue="65535"
    IsMandatory="True" />
```

### LabelDecimalBox

A labeled decimal input box with precision and range validation.

#### Features

- Decimal number input
- Configurable decimal places
- Minimum and maximum value validation
- Text alignment

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Value` | `decimal?` | `null` | The decimal value |
| `WatermarkText` | `string` | `""` | Placeholder text |
| `TextAlignment` | `TextAlignment` | `Left` | Text alignment |
| `MinimumValue` | `decimal` | `decimal.MinValue` | Minimum allowed value |
| `MaximumValue` | `decimal` | `decimal.MaxValue` | Maximum allowed value |

#### Usage Example

```xml
<!-- Price input -->
<atc:LabelDecimalBox
    LabelText="Price"
    Value="{Binding Price, Mode=TwoWay}"
    MinimumValue="0"
    MaximumValue="99999.99"
    IsMandatory="True" />

<!-- Percentage input -->
<atc:LabelDecimalBox
    LabelText="Tax Rate %"
    Value="{Binding TaxRate, Mode=TwoWay}"
    MinimumValue="0"
    MaximumValue="100" />
```

### LabelIntegerXyBox

A labeled control for entering X and Y integer coordinates.

#### Features

- Two integer input fields (X and Y)
- Independent range validation for each axis
- Coordinate pair value output

#### Usage Example

```xml
<!-- Screen position -->
<atc:LabelIntegerXyBox
    LabelText="Position"
    ValueX="{Binding PositionX, Mode=TwoWay}"
    ValueY="{Binding PositionY, Mode=TwoWay}"
    MinimumValue="0"
    MaximumValue="1920" />
```

### LabelDecimalXyBox

A labeled control for entering X and Y decimal coordinates.

#### Features

- Two decimal input fields (X and Y)
- Independent range validation for each axis
- Coordinate pair value output
- Precision control

#### Usage Example

```xml
<!-- Map coordinates -->
<atc:LabelDecimalXyBox
    LabelText="Coordinates"
    ValueX="{Binding Longitude, Mode=TwoWay}"
    ValueY="{Binding Latitude, Mode=TwoWay}"
    MinimumValue="-180"
    MaximumValue="180" />
```

### LabelPixelSizeBox

A labeled control for entering pixel dimensions (width and height).

#### Features

- Width and height input fields
- Pixel unit display
- Range validation for dimensions

#### Usage Example

```xml
<!-- Image size -->
<atc:LabelPixelSizeBox
    LabelText="Image Size"
    Width="{Binding ImageWidth, Mode=TwoWay}"
    Height="{Binding ImageHeight, Mode=TwoWay}"
    MinimumValue="1"
    MaximumValue="4096" />
```

## Date and Time Controls

### LabelDatePicker

A labeled date picker control for selecting dates.

#### Features

- Calendar popup for date selection
- Date format display
- Minimum and maximum date validation
- Watermark text

#### Usage Example

```xml
<!-- Birth date -->
<atc:LabelDatePicker
    LabelText="Birth Date"
    SelectedDate="{Binding BirthDate, Mode=TwoWay}"
    IsMandatory="True" />

<!-- Date range validation -->
<atc:LabelDatePicker
    LabelText="Start Date"
    SelectedDate="{Binding StartDate, Mode=TwoWay}"
    MinimumDate="{x:Static sys:DateTime.Today}" />
```

### LabelTimePicker

A labeled time picker control for selecting time values.

#### Features

- Time selection interface
- Hours, minutes, seconds input
- 12/24 hour format support

#### Usage Example

```xml
<!-- Appointment time -->
<atc:LabelTimePicker
    LabelText="Time"
    SelectedTime="{Binding AppointmentTime, Mode=TwoWay}"
    IsMandatory="True" />
```

### LabelDateTimePicker

A labeled control combining date and time selection.

#### Features

- Combined date and time picker
- Calendar and time selection
- DateTime value output

#### Usage Example

```xml
<!-- Event timestamp -->
<atc:LabelDateTimePicker
    LabelText="Event DateTime"
    SelectedDateTime="{Binding EventDateTime, Mode=TwoWay}"
    IsMandatory="True" />
```

## Selection Controls

### LabelCheckBox

A labeled checkbox for boolean selection.

#### Features

- Three-state support (checked, unchecked, indeterminate)
- Click event handling
- IsChecked binding

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `IsChecked` | `bool?` | `null` | The checked state |

#### Usage Example

```xml
<!-- Simple checkbox -->
<atc:LabelCheckBox
    LabelText="Accept Terms"
    IsChecked="{Binding AcceptTerms, Mode=TwoWay}"
    IsMandatory="True" />

<!-- Three-state checkbox -->
<atc:LabelCheckBox
    LabelText="Select All"
    IsChecked="{Binding SelectAllState, Mode=TwoWay}" />
```

### LabelComboBox

A labeled dropdown/combo box for item selection.

#### Features

- Single item selection from a list
- Data binding support
- Display member path for complex objects
- Selected value/item tracking

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ItemsSource` | `IEnumerable` | `null` | Collection of items to display |
| `SelectedItem` | `object?` | `null` | The currently selected item |

#### Usage Example

```xml
<!-- Simple dropdown -->
<atc:LabelComboBox
    LabelText="Country"
    ItemsSource="{Binding Countries}"
    SelectedItem="{Binding SelectedCountry, Mode=TwoWay}"
    DisplayMemberPath="Name"
    IsMandatory="True" />

<!-- Enum binding -->
<atc:LabelComboBox
    LabelText="Status"
    ItemsSource="{Binding StatusOptions}"
    SelectedItem="{Binding CurrentStatus, Mode=TwoWay}" />
```

### LabelToggleSwitch

A labeled toggle switch for binary on/off states.

#### Features

- Visual on/off toggle
- IsOn binding
- Switch state change events

#### Usage Example

```xml
<!-- Feature toggle -->
<atc:LabelToggleSwitch
    LabelText="Enable Notifications"
    IsOn="{Binding NotificationsEnabled, Mode=TwoWay}" />

<!-- Dark mode toggle -->
<atc:LabelToggleSwitch
    LabelText="Dark Mode"
    IsOn="{Binding IsDarkMode, Mode=TwoWay}" />
```

## Selector Controls

### LabelAccentColorSelector

A labeled control for selecting accent colors for themes.

#### Features

- Predefined accent color palette
- Visual color preview
- Theme integration

#### Usage Example

```xml
<atc:LabelAccentColorSelector
    LabelText="Accent Color"
    SelectedColor="{Binding AccentColor, Mode=TwoWay}" />
```

### LabelCountrySelector

A labeled control for selecting countries from a list.

#### Features

- Comprehensive country list
- Country code support (ISO codes)
- Flag display (if available)

#### Usage Example

```xml
<atc:LabelCountrySelector
    LabelText="Country"
    SelectedCountry="{Binding UserCountry, Mode=TwoWay}"
    IsMandatory="True" />
```

### LabelFontFamilySelector

A labeled control for selecting font families.

#### Features

- System font family list
- Font preview
- Search/filter capability

#### Usage Example

```xml
<atc:LabelFontFamilySelector
    LabelText="Font"
    SelectedFontFamily="{Binding EditorFont, Mode=TwoWay}" />
```

### LabelLanguageSelector

A labeled control for selecting languages.

#### Features

- Supported language list
- Culture code support
- Localization integration

#### Usage Example

```xml
<atc:LabelLanguageSelector
    LabelText="Language"
    SelectedLanguage="{Binding ApplicationLanguage, Mode=TwoWay}"
    IsMandatory="True" />
```

### LabelThemeSelector

A labeled control for selecting application themes.

#### Features

- Light/Dark theme selection
- Custom theme support
- Theme preview

#### Usage Example

```xml
<atc:LabelThemeSelector
    LabelText="Theme"
    SelectedTheme="{Binding ApplicationTheme, Mode=TwoWay}" />
```

### LabelThemeAndAccentColorSelectors

A labeled control combining theme and accent color selection.

#### Features

- Unified theme and accent color picker
- Real-time preview
- Coordinated color scheme

#### Usage Example

```xml
<atc:LabelThemeAndAccentColorSelectors
    LabelText="Appearance"
    SelectedTheme="{Binding Theme, Mode=TwoWay}"
    SelectedAccentColor="{Binding AccentColor, Mode=TwoWay}" />
```

### LabelWellKnownColorSelector

A labeled control for selecting from predefined well-known colors.

#### Features

- Standard color palette (Web colors, named colors)
- Color preview
- Hex/RGB display

#### Usage Example

```xml
<atc:LabelWellKnownColorSelector
    LabelText="Highlight Color"
    SelectedColor="{Binding HighlightColor, Mode=TwoWay}" />
```

## Picker Controls

### LabelColorPicker

A labeled color picker control for selecting custom colors.

#### Features

- RGB/HSV color selection
- Alpha channel support
- Hex color code input
- Color preview

#### Usage Example

```xml
<!-- Custom color selection -->
<atc:LabelColorPicker
    LabelText="Background Color"
    SelectedColor="{Binding BackgroundColor, Mode=TwoWay}" />

<!-- With alpha channel -->
<atc:LabelColorPicker
    LabelText="Overlay Color"
    SelectedColor="{Binding OverlayColor, Mode=TwoWay}"
    ShowAlphaChannel="True" />
```

### LabelDirectoryPicker

A labeled control for selecting directories/folders.

#### Features

- Browse folder dialog
- Path display and validation
- Recent folders support

#### Usage Example

```xml
<!-- Output directory -->
<atc:LabelDirectoryPicker
    LabelText="Output Directory"
    SelectedPath="{Binding OutputDirectory, Mode=TwoWay}"
    IsMandatory="True" />

<!-- With initial directory -->
<atc:LabelDirectoryPicker
    LabelText="Project Folder"
    SelectedPath="{Binding ProjectPath, Mode=TwoWay}"
    InitialDirectory="C:\Projects" />
```

### LabelFilePicker

A labeled control for selecting files.

#### Features

- Browse file dialog
- File filter support
- Multiple file selection
- File path validation

#### Usage Example

```xml
<!-- Single file selection -->
<atc:LabelFilePicker
    LabelText="Configuration File"
    SelectedFile="{Binding ConfigFile, Mode=TwoWay}"
    Filter="JSON Files (*.json)|*.json|All Files (*.*)|*.*"
    IsMandatory="True" />

<!-- Multiple files -->
<atc:LabelFilePicker
    LabelText="Attachments"
    SelectedFiles="{Binding Attachments, Mode=TwoWay}"
    AllowMultiple="True" />
```

## Slider Controls

### LabelSlider

A labeled slider control for selecting numeric values within a range.

#### Features

- Visual range selection
- Minimum and maximum bounds
- Tick marks and labels
- Value tooltip

#### Usage Example

```xml
<!-- Volume control -->
<atc:LabelSlider
    LabelText="Volume"
    Value="{Binding Volume, Mode=TwoWay}"
    Minimum="0"
    Maximum="100"
    TickFrequency="10"
    IsSnapToTickEnabled="True" />

<!-- Opacity control -->
<atc:LabelSlider
    LabelText="Opacity"
    Value="{Binding Opacity, Mode=TwoWay}"
    Minimum="0"
    Maximum="1"
    TickFrequency="0.1" />
```

## Network and Endpoint Controls

### LabelEndpointBox

A labeled control for entering network endpoints (protocol + host + port) with validation.

#### Features

- Protocol selection (HTTP, HTTPS, FTP, FTPS, TCP, UDP, OPC TCP)
- Host input with validation
- Port input with range validation
- Combined URI output
- Network validation rules (IPv4, IPv6, etc.)
- Clear button support
- Deferred validation (only shows errors after user interaction)

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `NetworkProtocol` | `NetworkProtocolType` | `Https` | The network protocol (Http, Https, Ftp, Ftps, Tcp, Udp, OpcTcp) |
| `Host` | `string` | `""` | The host address (domain, IP address, or hostname) |
| `Port` | `int` | `80` | The port number |
| `Value` | `Uri?` | `null` | The complete endpoint as a URI (read/write) |
| `WatermarkText` | `string` | `"localhost"` | Placeholder text for the host field |
| `ShowClearTextButton` | `bool` | `true` | Show/hide the clear button in host field |
| `HideUpDownButtons` | `bool` | `true` | Show/hide increment/decrement buttons for port |
| `NetworkValidation` | `NetworkValidationRule` | `None` | Additional validation rule for host (IPv4Address, IPv6Address, etc.) |
| `MinimumPort` | `int` | `1` | Minimum allowed port number |
| `MaximumPort` | `int` | `65535` | Maximum allowed port number |

#### Usage Example

```xml
<!-- Basic usage with protocol and default port -->
<atc:LabelEndpointBox
    LabelText="API Endpoint"
    NetworkProtocol="Https"
    Host="api.example.com"
    Port="443" />

<!-- OPC TCP endpoint with IPv4 validation -->
<atc:LabelEndpointBox
    LabelText="OPC Server"
    NetworkProtocol="OpcTcp"
    NetworkValidation="IPv4Address"
    Host="192.168.1.10"
    Port="4840"
    IsMandatory="True"
    WatermarkText="192.168.1.1" />

<!-- Binding to a Uri property in ViewModel -->
<atc:LabelEndpointBox
    LabelText="Service Endpoint"
    Value="{Binding ServiceEndpointUri, Mode=TwoWay}"
    NetworkProtocol="Http" />

<!-- With port range restrictions -->
<atc:LabelEndpointBox
    LabelText="Custom Port"
    NetworkProtocol="Tcp"
    Host="localhost"
    Port="8080"
    MinimumPort="8000"
    MaximumPort="9000"
    HideUpDownButtons="False" />
```

#### Validation

The control provides multiple levels of validation:

1. **Required Field Validation** - Shows "Field is required" if `IsMandatory="True"` and host is empty (only after user interaction)
2. **Port Range Validation** - Validates port is within `MinimumPort` and `MaximumPort`
3. **Protocol-Based Host Validation** - Validates host format based on the selected `NetworkProtocol`
4. **Network Validation** - Additional validation based on `NetworkValidation` property (e.g., IPv4Address format)

#### Events

| Event | Type | Description |
|-------|------|-------------|
| `NetworkProtocolLostFocus` | `EventHandler<ValueChangedEventArgs<NetworkProtocolType?>>` | Fired when network protocol changes |
| `HostLostFocus` | `EventHandler<ValueChangedEventArgs<string?>>` | Fired when host value changes |
| `PortLostFocus` | `EventHandler<ValueChangedEventArgs<int?>>` | Fired when port value changes |
| `ValueLostFocus` | `EventHandler<ValueChangedEventArgs<Uri?>>` | Fired when the complete URI value changes |

## Information Display Controls

### LabelContent

A labeled control for displaying any WPF content.

#### Features

- Generic content display with label
- Supports any WPF UIElement as content
- Flexible layout options

#### Usage Example

```xml
<!-- Display status icon -->
<atc:LabelContent LabelText="Status">
    <atc:LabelContent.Content>
        <Image Source="{Binding StatusIcon}" Width="32" Height="32" />
    </atc:LabelContent.Content>
</atc:LabelContent>

<!-- Display custom control -->
<atc:LabelContent LabelText="Progress">
    <atc:LabelContent.Content>
        <ProgressBar Value="{Binding ProgressValue}" Width="200" />
    </atc:LabelContent.Content>
</atc:LabelContent>
```

### LabelTextInfo

A labeled control for displaying read-only text information.

#### Features

- Read-only text display
- Text selection support
- Text wrapping and trimming options
- Copy-to-clipboard support

#### Usage Example

```xml
<!-- Display calculated value -->
<atc:LabelTextInfo
    LabelText="Total"
    Text="{Binding TotalAmount, StringFormat=C}" />

<!-- Display system information -->
<atc:LabelTextInfo
    LabelText="Version"
    Text="{Binding ApplicationVersion}" />

<!-- Multi-line information -->
<atc:LabelTextInfo
    LabelText="Description"
    Text="{Binding Description}"
    TextWrapping="Wrap"
    MaxWidth="400" />
```

## Base Controls

The following base controls (without labels) are also available in `Atc.Wpf.Controls.BaseControls`:

- `EndpointBox` - Network endpoint input (used by `LabelEndpointBox`)
- `IntegerBox` - Integer input with validation
- `DecimalBox` - Decimal input with validation
- `DateTimePicker` - Date and time selection
- `TimePicker` - Time selection
- `ColorPicker` - Color selection
- `ToggleSwitch` - On/off toggle

These base controls provide the core functionality and can be used independently without the label wrapper.

## Validation Behavior

All label controls support **deferred validation** pattern:

- Validation errors are not shown on initial load for empty required fields
- Errors only appear after the user has interacted with the field (after losing focus)
- This provides a better user experience by not showing errors for untouched fields
- The validation state is tracked using an internal `isDirty` flag

### Validation Implementation

```csharp
// Example from LabelEndpointBox
private bool isDirty;

private void OnHostTextBoxLostFocus(object sender, RoutedEventArgs e)
{
    isDirty = true;
    ValidateEndpoint();
}

private void ValidateEndpoint()
{
    // Only show "Field is required" after user interaction
    if (IsMandatory && string.IsNullOrWhiteSpace(Host) && isDirty)
    {
        ValidationText = "Field is required.";
        return;
    }
    // ... other validation logic
}
```

## Theming

All label controls support both Light and Dark themes through the Atc.Wpf.Theming package.

### Theme Support

- Automatic theme switching based on application theme
- Consistent styling across all label controls
- Custom theme colors for validation, mandatory indicators, and labels
- High contrast mode support

### Example Theme Configuration

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="pack://application:,,,/Atc.Wpf.Theming;component/Themes/Generic.xaml" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

## Summary

The Label Controls library provides a comprehensive set of labeled input and display controls for WPF applications:

- **25+ specialized controls** covering text, numbers, dates, selections, pickers, and more
- **Consistent API** - all controls inherit from `LabelControl` with common properties
- **Built-in validation** - with deferred validation pattern for better UX
- **Theme support** - Light and Dark themes out of the box
- **Flexible layout** - Horizontal and Vertical orientations
- **Rich features** - Watermarks, clear buttons, range validation, regex patterns, and more

All controls follow WPF best practices and support data binding, commanding, and MVVM patterns.
