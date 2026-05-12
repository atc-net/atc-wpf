# ValueConverters in Atc.Wpf

## 📑 Table of Contents

- [ValueConverters in Atc.Wpf](#valueconverters-in-atcwpf)
  - [📑 Table of Contents](#-table-of-contents)
  - [🧹 Usage](#-usage)
  - [#️⃣ ValueConverters - Bool to \[...\]](#️⃣-valueconverters---bool-to-)
  - [#️⃣ ValueConverters - String to \[...\]](#️⃣-valueconverters---string-to-)
  - [#️⃣ ValueConverters - ICollection to \[...\]](#️⃣-valueconverters---icollection-to-)
  - [#️⃣ ValueConverters - Object to \[...\]](#️⃣-valueconverters---object-to-)
  - [#️⃣ ValueConverters - Markup to \[...\]](#️⃣-valueconverters---markup-to-)
  - [#️⃣ ValueConverters - Enum to \[...\]](#️⃣-valueconverters---enum-to-)
  - [#️⃣ ValueConverters - Number to \[...\]](#️⃣-valueconverters---number-to-)
  - [#️⃣ ValueConverters - Time to \[...\]](#️⃣-valueconverters---time-to-)
  - [#️⃣ ValueConverters - Others to \[...\]](#️⃣-valueconverters---others-to-)
  - [#️⃣ ValueConverters - Math](#️⃣-valueconverters---math)
  - [#️⃣ ValueConverters - JSON](#️⃣-valueconverters---json)
  - [#️⃣ ValueConverters - Method](#️⃣-valueconverters---method)
  - [🧩 Advanced parameter shapes](#-advanced-parameter-shapes)
    - [Enum -\> Visibility](#enum---visibility)
    - [Double -\> GridLength](#double---gridlength)
    - [Bool -\> Width](#bool---width)
    - [Thickness — side selection (Binding / Filter / ToDouble)](#thickness--side-selection-binding--filter--todouble)
    - [String -\> Bool (Regex)](#string---bool-regex)
    - [Enum-Description -\> String — case formatter](#enum-description---string--case-formatter)
    - [MultiBinding — Window button visibility](#multibinding--window-button-visibility)
    - [LogLevel / LogCategoryType — per-state palette overrides](#loglevel--logcategorytype--per-state-palette-overrides)

## 🧹 Usage

To use a converter in WPF by ResourceDictionary and and key:

```xml
<!-- Namespace mapping -->
xmlns:atcToolkitValueConverters="clr-namespace:Atc.XamlToolkit.ValueConverters;assembly=Atc.XamlToolkit.Wpf"

<!-- Resources -->
<UserControl.Resources>
    <ResourceDictionary>
        <atcValueConverters:BoolToVisibilityVisibleValueConverter x:Key="BoolToVisibilityVisibleValueConverter" />
    </ResourceDictionary>
</UserControl.Resources>

<!-- Usage -->
<StackPanel Visibility="{Binding IsVisible<br/>Converter={StaticResource BoolToVisibilityVisibleValueConverter}}" />
```

Or by the ValueConverter's Instance:

```xml
<StackPanel Visibility="{Binding IsVisible<br/>Converter={x:Static atcValueConverters:BoolToVisibilityVisibleValueConverter.Instance}}" />
```

## #️⃣ ValueConverters - Bool to [...]

| Category                  | Type                                                     | Convert Examples                        | ConvertBack Examples                    |
| ------------------------- | -------------------------------------------------------- | --------------------------------------- | --------------------------------------- |
| Bool -> Bool              | BoolToInverseBoolValueConverter                          | True -> False<br/>False -> True         | False -> True<br/>False -> False        |
| Bool -> Visibility        | BoolToVisibilityCollapsedValueConverter                  | True -> Collapsed<br/>False -> Visible  | Collapsed -> True<br/>Visible -> False  |
| Bool -> Visibility        | BoolToVisibilityVisibleValueConverter                    | True -> Visible<br/>False -> Collapsed  | Visible -> True<br/>Collapsed -> False  |
| Bool -> Opacity           | BoolToOpacityValueConverter                              | True -> 1.0<br/>False -> 0.0            | Not supported                           |
| Bool -> Opacity           | BoolToInverseOpacityValueConverter                       | True -> 0.0<br/>False -> 1.0            | Not supported                           |
| Bool -> Object *(stateful)* | BoolToObjectValueConverter                             | True -&gt; TrueValue<br/>False -&gt; FalseValue<br/>*(set both via XAML resource)* | Not supported                           |
| Bool -> With              | BoolToWidthValueConverter                                | true<br/>10 -> 10<br/>true<br/>"Auto" -> * | Not supported                        |
| Bool[] -> Bool            | MultiBoolToBoolValueConverter                            | All-True -> True                        | Not supported                           |
| Bool[] -> Visibility      | MultiBoolToVisibilityVisibleValueConverter               | All-True -> Visible                     | Not supported                           |
| (Bool, Double) -> Double  | BoolAndDoubleToDoubleMultiValueConverter                 | (true, 150) -> 150<br/>(false, 150) -> 0 | Not supported                          |

## #️⃣ ValueConverters - String to [...]

| Category                  | Type                                                     | Convert Examples                        | ConvertBack Examples                    |
| ------------------------- | -------------------------------------------------------- | --------------------------------------- | --------------------------------------- |
| String -> Brush           | ColorNameToBrushValueConverter                           | "Green" -> Brushs.Green                 | Brushs.Green -> "Green"                 |
| String -> Color           | ColorNameToColorValueConverter                           | "Green" -> Colors.Green                 | Colors.Green -> "Green"                 |
| String -> "NumericFormat" | StandardNumericFormatTypeToFormatStringValueConverter    | StandardNumericFormatType -> String     | Not supported                           |
| String -> Bool            | StringNullOrEmptyToBoolValueConverter                    | NULL or empty -> True                   | Not supported                           |
| String -> Bool            | StringNullOrEmptyToInverseBoolValueConverter             | NULL or empty -> False                  | Not supported                           |
| String -> Visibility      | StringNullOrEmptyToVisibilityCollapsedValueConverter     | NULL or empty -> Collapsed              | Not supported                           |
| String -> Visibility      | StringNullOrEmptyToVisibilityVisibleValueConverter       | NULL or empty -> Visible                | Not supported                           |
| String -> Visibility      | StringToVisibilityVisibleValueConverter                  | "Active" + param "Active" -> Visible    | Not supported                           |
| String -> Visibility      | StringToVisibilityCollapsedValueConverter                | "Active" + param "Active" -> Collapsed  | Not supported                           |
| String -> Bool            | StringEqualsToBoolValueConverter                         | "Active" + param "Active" -> True<br/>"Active" + param "active" -> True (case-insensitive)<br/>"Active" + param "Inactive" -> False | Not supported                           |
| String -> String          | PathToFilenameValueConverter                             | "C:\foo\bar.txt" -> "bar.txt"<br/>"C:\foo\bar.txt" + param "WithoutExtension" -> "bar" | Not supported                           |
| String -> List<String>    | StringToSplitStringListValueConverter                    | String -> List<String>                  | Not supported                           |
| String -> String          | ToLowerValueConverter                                    | String -> String                        | Binding.DoNothing                       |
| String -> String          | ToUpperValueConverter                                    | String -> String                        | Binding.DoNothing                       |

## #️⃣ ValueConverters - ICollection to [...]

| Category                  | Type                                                     | Convert Examples                        | ConvertBack Examples                    |
| ------------------------- | -------------------------------------------------------- | --------------------------------------- | --------------------------------------- |
| ICollection -> Bool       | CollectionNullOrEmptyToBoolValueConverter                | NULL or empty -> True                   | Not supported                           |
| ICollection -> Bool       | CollectionNullOrEmptyToInverseBoolValueConverter         | NULL or empty -> False                  | Not supported                           |
| ICollection -> Visibility | CollectionNullOrEmptyToVisibilityCollapsedValueConverter | NULL or empty -> Collapsed              | Not supported                           |
| ICollection -> Visibility | CollectionNullOrEmptyToVisibilityVisibleValueConverter   | NULL or empty -> Visible                | Not supported                           |

## #️⃣ ValueConverters - Object to [...]

| Category                  | Type                                                     | Convert Examples                        | ConvertBack Examples                    |
| ------------------------- | -------------------------------------------------------- | --------------------------------------- | --------------------------------------- |
| Object -> Bool            | IsNotNullValueConverter                                  | ! Null -> True<br/>Null -> False        | Not supported                           |
| Object -> Bool            | IsNullValueConverter                                     | Null -> True<br/>! Null -> False        | Not supported                           |
| Null -> X                 | NullCheckValueConverter                                  | NULL -> Parameter if set                | Not supported                           |
| Null -> UnsetValue        | NullToUnsetValueConverter                                | NULL -> DependencyProperty.UnsetValue   | Object -> DependencyProperty.UnsetValue |
| Object -> Bool            | ObjectNotNullToBoolValueConverter                        | NotNULL -> True                         | Not supported                           |
| Object -> Visibility      | ObjectNotNullToVisibilityCollapsedValueConverter         | NotNULL -> Collapsed                    | Not supported                           |
| Object -> Visibility      | ObjectNotNullToVisibilityVisibleValueConverter           | NotNULL -> Visible                      | Not supported                           |
| Object -> Visibility      | ObjectNullToVisibilityCollapsedValueConverter            | NULL -> Collapsed                       | Not supported                           |
| Object -> Visibility      | ObjectNullToVisibilityVisibleValueConverter              | NULL -> Visible                         | Not supported                           |
| Object[] -> Visibility    | MultiObjectNullToVisibilityCollapsedValueConverter       | All-NULL -> Collapsed                   | Not supported                           |
| Object -> Bool            | ObjectNullToBoolValueConverter                           | NULL => True                            | Not supported                           |
| Object -> String          | ObjectToTypeNameValueConverter                                     | "hello" -> "String"<br/>42 -> "Int32"<br/>"hello" + param "Full" -> "System.String" | Not supported                           |

## #️⃣ ValueConverters - Markup to [...]

| Category                  | Type                                                     | Convert Examples                        | ConvertBack Examples                    |
| ------------------------- | -------------------------------------------------------- | --------------------------------------- | --------------------------------------- |
| Base converter            | MarkupMultiValueConverterBase                            | Base converter - no examples            | Base converter - no examples            |
|                           | MarkupValueConverter                                     |                                         |                                         |
|                           | MarkupValueConverterBase                                 |                                         |                                         |

## #️⃣ ValueConverters - Enum to [...]

| Category                  | Type                                                     | Convert Examples                        | ConvertBack Examples                    |
| ------------------------- | -------------------------------------------------------- | --------------------------------------- | --------------------------------------- |
| Enum -> String            | EnumDescriptionToStringValueConverter                    | DayOfWeek.Monday -> Monday              | Not supported                           |
| Enum -> Bool              | EnumToBoolValueConverter                                 | DayOfWeek.Monday + param "Monday" -> True<br/>DayOfWeek.Tuesday + param "Monday,Tuesday" -> True<br/>DayOfWeek.Wednesday + param "Monday,Tuesday" -> False | Not supported                           |
| Enum -> Bool              | EnumToInverseBoolValueConverter                          | DayOfWeek.Monday + param "Monday" -> False<br/>DayOfWeek.Wednesday + param "Monday,Tuesday" -> True | Not supported                           |
| Enum -> Bool *(flags)*    | EnumFlagsToBoolValueConverter                            | (Read \| Write) + param Read -> True<br/>Read + param (Read \| Write) -> False<br/>Permissions + param "Read,Write" -> True (if both set) | Not supported                           |
| Enum -> Visibility        | EnumToVisibilityVisibleValueConverter                    | Status.Active + param "Active" -> Visible<br/>DayOfWeek.Tuesday + param "Monday,Tuesday" -> Visible<br/>DayOfWeek.Wednesday + param "Monday,Tuesday" -> Collapsed | Not supported                         |
| Enum -> Visibility        | EnumToVisibilityCollapsedValueConverter                  | Status.Active + param "Active" -> Collapsed<br/>DayOfWeek.Tuesday + param "Monday,Tuesday" -> Collapsed<br/>DayOfWeek.Wednesday + param "Monday,Tuesday" -> Visible | Not supported                       |
| Enum -> Visibility *(flags)* | EnumFlagsToVisibilityVisibleValueConverter            | (Read \| Write) + param Read -> Visible<br/>Read + param Admin -> Collapsed | Not supported                           |
| Enum -> Visibility *(flags)* | EnumFlagsToVisibilityCollapsedValueConverter          | (Read \| Write) + param Read -> Collapsed<br/>Read + param Admin -> Visible | Not supported                           |

## #️⃣ ValueConverters - Number to [...]

| Category                  | Type                                                     | Convert Examples                        | ConvertBack Examples                    |
| ------------------------- | -------------------------------------------------------- | --------------------------------------- | --------------------------------------- |
| Number -> File size       | NumberToFileSizeStringValueConverter                     | 1024 -> "1 KB"<br/>1572864 -> "1.5 MB"<br/>1073741824 -> "1 GB" | Not supported                           |
| Number -> Visibility      | NumericComparisonToVisibilityVisibleValueConverter              | 10 + param "&gt;=5" -&gt; Visible<br/>3 + param "between:1,10" -&gt; Visible<br/>0 + param "&gt;5" -&gt; Collapsed | Not supported                           |
| Number -> Visibility      | NumericComparisonToVisibilityCollapsedValueConverter            | 10 + param "&gt;=5" -&gt; Collapsed<br/>0 + param "&gt;5" -&gt; Visible | Not supported                           |
| Number -> String *(two-way)* | NumberToPercentStringValueConverter                                    | 0.42 -> "42 %"<br/>0.1234 + param 2 -> "12.34 %" | "42%" -> 0.42<br/>"42 %" -> 0.42        |

## #️⃣ ValueConverters - Time to [...]

| Category                  | Type                                                     | Convert Examples                        | ConvertBack Examples                    |
| ------------------------- | -------------------------------------------------------- | --------------------------------------- | --------------------------------------- |
| TimeSpan -> String        | TimeSpanToHumanReadableStringValueConverter              | TimeSpan.FromMinutes(75) -> "1h 15m"<br/>TimeSpan.FromSeconds(90) -> "1m 30s" | Not supported                           |
| DateTime -> String        | DateTimeToRelativeStringValueConverter                   | DateTime.UtcNow.AddMinutes(-5) -> "5 minutes ago"<br/>DateTime.UtcNow.AddHours(2) -> "in 2 hours" | Not supported                           |

## #️⃣ ValueConverters - Others to [...]

| Category                  | Type                                                     | Convert Examples                        | ConvertBack Examples                    |
| ------------------------- | -------------------------------------------------------- | --------------------------------------- | --------------------------------------- |
|                           | BackgroundToForegroundValueConverter                     |                                         |                                         |
| Brush -> Color            | BrushToColorValueConverter                               | Brushs.Green -> Colors.Green            | Colors.Green -> Brushs.Green            |
| Brush -> Color-Name       | BrushToColorNameValueConverter                           | Brushs.Red -> "Red"                     | "Red" -> Brushs.Red                     |
| byte[] -> ImageSource     | ByteArrayToImageSourceValueConverter                     | byte[] -> BitmapImage                   | BitmapSource -> byte[] (PNG)            |
| Color -> Brush            | ColorToBrushValueConverter                               | Colors.Green -> Brushs.Green            | Brushs.Green -> Colors.Green            |
| Color -> SolidColor       | ColorToSolidColorValueConverter                          | Colors.Green -> Colors.Green            | Not supported                           |
| Color -> String           | ColorHexToColorValueConverter                            | "#FF00FF00" -> "Green"                  | "Green" -> "#FF00FF00"              |
| Double -> GridLength      | DoubleToGridLengthValueConverter                         | 100.0 -> GridLength(100, Pixel)         | GridLength -> double                    |
| Hex-Brush -> Brush-Key    | HexBrushToBrushKeyValueConverter                         | "#FF00FF00" -> "Green"                  | "Green" -> Brushs.Green               |
| Hex-Color -> Color-Key    | HexColorToColorKeyValueConverter                         | "#FF00FF00" -> "Green"                  | "Green" -> Color.Green                |
| Int -> Visibility         | IntegerGreaterThenZeroToVisibilityVisibleValueConverter  | 0 -> Collapsed<br/>1 -> Visible         | Not supported                           |
| Int -> TimeSpan           | IntegerToTimeSpanValueConverter                          | 100 -> TimeSpan.FromMilliseconds(100)   | TimeSpan -> int (milliseconds)          |
| LogCategoryType -> Brush  | LogCategoryTypeToBrushValueConverter                     | Information -> DodgerBlue               | Not supported                           |
| LogCategoryType -> Color  | LogCategoryTypeToColorValueConverter                     | Information -> DodgerBlue               | Not supported                           |
| LogCategoryType -> Image  | LogCategoryTypeToResourceImageValueConverter *(Components)* | Information -> BitmapImage           | Not supported                           |
| LogLevel -> Brush         | LogLevelToBrushValueConverter                            | Information -> DodgerBlue               | Not supported                           |
| LogLevel -> Color         | LogLevelToColorValueConverter                            | Information -> DodgerBlue               | Not supported                           |
|                           | ObservableDictionaryToDictionaryOfStringsValueConverter  |                                         |                                         |
|                           | ThicknessBindingValueConverter                           |                                         | DependencyProperty.UnsetValue           |
|                           | ThicknessFilterValueConverter                            |                                         | DependencyProperty.UnsetValue           |
| Thickness -> Double       | ThicknessToDoubleValueConverter                          | Thickness.Left -> double                | double -> Thickness (single side)       |
| DateTime (UTC) -> DateTime (Local) | UtcToLocalDateTimeValueConverter                | UTC -> Local                            | Local -> UTC                            |
| String -> Bool (Regex)    | RegexValidationValueConverter                            | "test@example.com" + pattern -> True    | Not supported                           |
| Errors -> String          | ValidationErrorsToFirstValidationErrorContentValueConverter |                                      | Not supported                           |
| Errors -> String          | ValidationErrorsToStringValueConverter                   |                                         | Not supported                           |
|                           | WindowResizeModeMinMaxButtonVisibilityMultiValueConverter|                                         |                                         |

## #️⃣ ValueConverters - Math

| Category                  | Type                                                     | Convert Examples                        | ConvertBack Examples                    |
| ------------------------- | -------------------------------------------------------- | --------------------------------------- | --------------------------------------- |
|                           | MathAddValueConverter                                    |                                         |                                         |
|                           | MathDivideValueConverter                                 |                                         |                                         |
|                           | MathMultiplyValueConverter                               |                                         |                                         |
|                           | MathSubtractValueConverter                               |                                         |                                         |
|                           | MathValueConverter                                       |                                         |                                         |

## #️⃣ ValueConverters - JSON

| Category                  | Type                                                     | Convert Examples                        | ConvertBack Examples                    |
| ------------------------- | -------------------------------------------------------- | --------------------------------------- | --------------------------------------- |
| JsonNode -> Length        | JsonArrayLengthConverter                                 | JsonArrayNode -> "[5]"                  | Not supported                           |
| JsonNode -> Children      | JsonNodeChildrenConverter                                | JsonNode -> IEnumerable<JsonNode>       | Not supported                           |
| JsonPropertyNode -> Brush | JsonPropertyTypeToColorConverter                         | JsonPropertyNode -> SolidColorBrush     | Not supported                           |
| JsonValueNode -> String   | JsonValueDisplayConverter                                | JsonValueNode -> DisplayValue           | Not supported                           |
| JsonValueNode -> Brush    | JsonValueTypeToColorConverter                            | JsonValueNode -> SolidColorBrush        | Not supported                           |

## #️⃣ ValueConverters - Method

| Category                  | Type                                                     | Convert Examples                        | ConvertBack Examples                    |
| ------------------------- | -------------------------------------------------------- | --------------------------------------- | --------------------------------------- |
| Object -> Object          | MethodToValueConverter                                   | Object.Method() -> Result               | Not supported                           |

## 🧩 Advanced parameter shapes

This section collects XAML usage examples for converters whose `ConverterParameter` accepts richer shapes (multi-value, type-safe enums, etc.).

### Enum -> Visibility

Both `EnumToVisibilityVisibleValueConverter` and `EnumToVisibilityCollapsedValueConverter` accept the parameter in four shapes:

| Shape                                    | Example                                                                                             |
| ---------------------------------------- | --------------------------------------------------------------------------------------------------- |
| Single `Enum`                            | `ConverterParameter="{x:Static sys:DayOfWeek.Monday}"`                                              |
| Single string (member name)              | `ConverterParameter=Monday` *(case-insensitive)*                                                    |
| Comma-separated string (multi-value)     | `ConverterParameter="Monday,Tuesday"` *(items are trimmed and case-insensitive)*                    |
| `IEnumerable` of `Enum`/string           | An `x:Array` of enum literals, or the [`EnumValuesExtension`](../MarkupExtensions) markup extension |

Multi-value examples in XAML:

```xml
<!-- Comma-separated string -->
Visibility="{Binding Day,
    Converter={x:Static atcValueConverters:EnumToVisibilityVisibleValueConverter.Instance},
    ConverterParameter='Monday,Tuesday'}"

<!-- Type-safe via x:Array -->
<Binding Path="Day"
         Converter="{x:Static atcValueConverters:EnumToVisibilityVisibleValueConverter.Instance}">
    <Binding.ConverterParameter>
        <x:Array Type="sys:DayOfWeek">
            <sys:DayOfWeek>Monday</sys:DayOfWeek>
            <sys:DayOfWeek>Tuesday</sys:DayOfWeek>
        </x:Array>
    </Binding.ConverterParameter>
</Binding>

<!-- Type-safe and compact via EnumValuesExtension + x:Static -->
Visibility="{Binding Day,
    Converter={x:Static atcValueConverters:EnumToVisibilityVisibleValueConverter.Instance},
    ConverterParameter={atc:EnumValues {x:Static sys:DayOfWeek.Monday},
                                       {x:Static sys:DayOfWeek.Tuesday}}}"

<!-- Compact attribute syntax via EnumValuesExtension + Type/Values -->
Visibility="{Binding Day,
    Converter={x:Static atcValueConverters:EnumToVisibilityVisibleValueConverter.Instance},
    ConverterParameter={atc:EnumValues Type={x:Type sys:DayOfWeek}, Values='Monday,Tuesday'}}"
```

### Double -> GridLength

`DoubleToGridLengthValueConverter` reads a magic-string parameter to choose the `GridUnitType`. Comparison is case-insensitive (uppercased internally).

| `ConverterParameter` | Result                                                |
| -------------------- | ----------------------------------------------------- |
| *(none)* / `Pixel`   | `GridLength(value, Pixel)` — value used as pixels     |
| `Star` / `*`         | `GridLength(value, Star)` — value used as star ratio  |
| `Auto`               | `GridLength(1, Auto)` — input value ignored           |

```xml
<!-- Pixel width (default) -->
<ColumnDefinition Width="{Binding LeftPaneWidth,
    Converter={x:Static atcValueConverters:DoubleToGridLengthValueConverter.Instance}}" />

<!-- Star ratio -->
<ColumnDefinition Width="{Binding StarRatio,
    Converter={x:Static atcValueConverters:DoubleToGridLengthValueConverter.Instance},
    ConverterParameter=Star}" />

<!-- Auto (binding value is ignored, but the binding is what drives re-evaluation) -->
<ColumnDefinition Width="{Binding ResetTrigger,
    Converter={x:Static atcValueConverters:DoubleToGridLengthValueConverter.Instance},
    ConverterParameter=Auto}" />
```

### Bool -> Width

`BoolToWidthValueConverter` returns either `0` (when the bound `bool` is `false`) or the width specified by `ConverterParameter` (when `true`). The parameter is parsed by WPF's `LengthConverter`, so it accepts the same forms as `Width=`.

| `ConverterParameter` | Result when value is `true`                          |
| -------------------- | ---------------------------------------------------- |
| *(none)* / `Auto`    | `double.NaN` (equivalent to `Width="Auto"`)          |
| `"100"`              | `100`                                                |
| `"50px"`             | `50`                                                 |

```xml
<!-- Collapse to width 0 when bound to false, otherwise Auto -->
<Border Width="{Binding IsExpanded,
    Converter={x:Static atcValueConverters:BoolToWidthValueConverter.Instance}}" />

<!-- Fixed 200px when true, 0 when false -->
<Border Width="{Binding IsExpanded,
    Converter={x:Static atcValueConverters:BoolToWidthValueConverter.Instance},
    ConverterParameter=200}" />
```

### Thickness — side selection (Binding / Filter / ToDouble)

`ThicknessBindingValueConverter`, `ThicknessFilterValueConverter` and `ThicknessToDoubleValueConverter` all accept a `LeftTopRightBottomType` enum to pick *which* side of a `Thickness` to act on. Each converter has an equivalent CLR property (`IgnoreThicknessSide`, `Filter`, `TakeThicknessSide`) used as a fallback when `ConverterParameter` is not a `LeftTopRightBottomType`.

| Converter                          | What it does                                                                  |
| ---------------------------------- | ----------------------------------------------------------------------------- |
| `ThicknessBindingValueConverter`   | Zeroes the selected side, passes the rest through (mask-out)                  |
| `ThicknessFilterValueConverter`    | Keeps only the selected side, zeroes the rest (mask-in)                       |
| `ThicknessToDoubleValueConverter`  | Extracts the selected side as a `double` (two-way; can build back a Thickness) |

```xml
<!-- Pattern A: via ConverterParameter (per-binding override) -->
<Border Padding="{Binding Source={x:Static SystemParameters.WindowResizeBorderThickness},
    Converter={x:Static atcValueConverters:ThicknessBindingValueConverter.Instance},
    ConverterParameter={x:Static atc:LeftTopRightBottomType.Top}}" />

<!-- Pattern B: via a configured instance (handy as a StaticResource) -->
<UserControl.Resources>
    <atcValueConverters:ThicknessFilterValueConverter x:Key="KeepBottomMargin"
                                                      Filter="Bottom" />
</UserControl.Resources>
<Border Margin="{Binding ChromeMargin,
    Converter={StaticResource KeepBottomMargin}}" />

<!-- Two-way extract of a single side -->
<TextBox Text="{Binding Margin,
    Converter={x:Static atcValueConverters:ThicknessToDoubleValueConverter.Instance},
    ConverterParameter={x:Static atc:LeftTopRightBottomType.Left}}" />
```

### String -> Bool (Regex)

`RegexValidationValueConverter` returns `true` when the bound string matches the regex pattern passed in `ConverterParameter`. An invalid regex or empty input returns `false`. Pattern execution is bounded by a 1-second timeout.

```xml
<!-- Email -->
IsValid="{Binding Email,
    Converter={x:Static atcValueConverters:RegexValidationValueConverter.Instance},
    ConverterParameter='^[\\w.-]+@[\\w.-]+\\.\\w+$'}"

<!-- Phone (US-style) -->
IsValid="{Binding Phone,
    Converter={x:Static atcValueConverters:RegexValidationValueConverter.Instance},
    ConverterParameter='^\\d{3}-\\d{3}-\\d{4}$'}"
```

> Note: backslashes inside an XML attribute should be doubled (`\\d`, `\\w`).

### Enum-Description -> String — case formatter

`EnumDescriptionToStringValueConverter` resolves an enum's `[Description]` text, then optionally applies a *case formatter* identified by `ConverterParameter`:

| `ConverterParameter` | Effect                                                        |
| -------------------- | ------------------------------------------------------------- |
| *(none)*             | Description as-is                                             |
| `U`                  | Entire string `UPPERCASE`                                     |
| `u`                  | First character uppercased: `Hello world`                     |
| `L`                  | Entire string `lowercase`                                     |
| `l`                  | First character lowercased: `hELLO`                           |
| `Ul`                 | Upper, but first char lowercased: `hELLO`                     |
| `Lu`                 | Lower, but first char uppercased: `Hello`                     |
| `<Format>:`          | Applies `<Format>` then appends `:`                           |
| `<Format>.`          | Applies `<Format>` then appends `.`                           |

```xml
<!-- "Monday" -->
Text="{Binding Day,
    Converter={x:Static atcValueConverters:EnumDescriptionToStringValueConverter.Instance}}"

<!-- "MONDAY" -->
Text="{Binding Day,
    Converter={x:Static atcValueConverters:EnumDescriptionToStringValueConverter.Instance},
    ConverterParameter=U}"

<!-- "monday:" (lowercase + colon) -->
Text="{Binding Day,
    Converter={x:Static atcValueConverters:EnumDescriptionToStringValueConverter.Instance},
    ConverterParameter='L:'}"
```

### MultiBinding — Window button visibility

`WindowResizeModeMinMaxButtonVisibilityMultiValueConverter` is an `IMultiValueConverter` driven by a fixed-order `MultiBinding`. The `ConverterParameter` must be a `WindowResizeModeButtonType` (`Close` / `Min` / `Max`) and selects which button's `Visibility` to compute.

| Multi-binding index | Type                | Meaning                                       |
| ------------------- | ------------------- | --------------------------------------------- |
| `[0]`               | `bool`              | Whether the button is enabled at all          |
| `[1]`               | `bool`              | `UseNoneWindowStyle` — hides chrome entirely  |
| `[2]`               | `ResizeMode`        | Window resize mode (consulted for Min/Max)    |

```xml
<Button Name="MinimizeButton">
    <Button.Visibility>
        <MultiBinding Converter="{x:Static atcValueConverters:WindowResizeModeMinMaxButtonVisibilityMultiValueConverter.Instance}"
                      ConverterParameter="{x:Static atc:WindowResizeModeButtonType.Min}">
            <Binding Path="ShowMinButton"        RelativeSource="{RelativeSource AncestorType=Window}" />
            <Binding Path="UseNoneWindowStyle"   RelativeSource="{RelativeSource AncestorType=Window}" />
            <Binding Path="ResizeMode"           RelativeSource="{RelativeSource AncestorType=Window}" />
        </MultiBinding>
    </Button.Visibility>
</Button>
```

### LogLevel / LogCategoryType — per-state palette overrides

`LogLevelToColorValueConverter` and `LogCategoryTypeToColorValueConverter` are the
source of truth for their respective color palettes. Each per-state entry is exposed
as a *mutable* static property so consumers can override individual colors without
subclassing. The paired brush converters (`LogLevelToBrushValueConverter` and
`LogCategoryTypeToBrushValueConverter`) derive from these colors via a cache that
rebuilds the frozen `SolidColorBrush` whenever its source color changes — so a
single color override propagates to both converters automatically.

| Member                | Purpose                                                                                       |
| --------------------- | --------------------------------------------------------------------------------------------- |
| `XxxColor` *(get/set)* | Current color for a single state, e.g. `WarningColor`, `SecurityColor`                       |
| `DefaultXxxColor`     | `static readonly` — the built-in value, useful for reverting one state                        |
| `FallbackColor`       | Returned for `null` or unmapped enum values (e.g. `LogLevel.None`). Default `Colors.DeepPink` |
| `SetColor(state, c)`  | Switch-style setter mirroring the property; unmapped states write to `FallbackColor`          |
| `GetColor(state)`     | Reads the current color for any state                                                         |
| `ResetToDefaults()`   | Restores every entry to its `DefaultXxxColor`                                                 |
| `GetBrush(state)`     | On the brush converter — frozen brush built from the current color (cached)                   |

```csharp
// Application startup — override individual entries before any binding fires
LogLevelToColorValueConverter.WarningColor = Colors.Orange;
LogLevelToColorValueConverter.SetColor(LogLevel.Critical, Colors.DarkRed);

LogCategoryTypeToColorValueConverter.SecurityColor = Colors.Teal;
LogCategoryTypeToColorValueConverter.SetColor(LogCategoryType.UI, Colors.MediumPurple);

// Restore the built-in palette later if needed
LogLevelToColorValueConverter.ResetToDefaults();
LogCategoryTypeToColorValueConverter.ResetToDefaults();
```

> **Note:** WPF bindings do not auto-refresh when these static properties change —
> the binding source value (the `LogLevel` / `LogCategoryType`) hasn't changed.
> Set overrides at application startup *before* any binding fires. To refresh a
> live UI, clear and re-add items in the bound collection so each row re-resolves.