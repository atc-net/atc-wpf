# ValueConverters in Atc.Wpf

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
| Bool -> With              | BoolToWidthValueConverter                                | true<br/>10 -> 10<br/>true<br/>"Auto" -> * | Not supported                        |
| Bool[] -> Bool            | MultiBoolToBoolValueConverter                            | All-True -> True                        | Not supported                           |
| Bool[] -> Visibility      | MultiBoolToVisibilityVisibleValueConverter               | All-True -> Visible                     | Not supported                           |

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

## #️⃣ ValueConverters - Markup to [...]

| Category                  | Type                                                     | Convert Examples                        | ConvertBack Examples                    |
| ------------------------- | -------------------------------------------------------- | --------------------------------------- | --------------------------------------- |
| Base converter            | MarkupMultiValueConverterBase                            | Base converter - no examples            | Base converter - no examples            |
|                           | MarkupValueConverter                                     |                                         |                                         |
|                           | MarkupValueConverterBase                                 |                                         |                                         |

## #️⃣ ValueConverters - Others to [...]

| Category                  | Type                                                     | Convert Examples                        | ConvertBack Examples                    |
| ------------------------- | -------------------------------------------------------- | --------------------------------------- | --------------------------------------- |
|                           | BackgroundToForegroundValueConverter                     |                                         |                                         |
| Brush -> Color            | BrushToColorValueConverter                               | Brushs.Green -> Colors.Green            | Colors.Green -> Brushs.Green            |
| Color -> Brush            | ColorToBrushValueConverter                               | Colors.Green -> Brushs.Green            | Brushs.Green -> Colors.Green            |
| Color -> SolidColor       | ColorToSolidColorValueConverter                          | Colors.Green -> Colors.Green            | Not supported                           |
| Color -> String           | ColorHexToColorValueConverter                            | "#FF00FF00" -> "Green"                  | "Green" -> "#FF00FF00"                  |
| Hex-Brush -> Brush-Key    | HexBrushToBrushKeyValueConverter                         | "#FF00FF00" -> "Green"                  | "Green" -> Brushs.Green                 |
| Hex-Color -> Color-Key    | HexColorToColorKeyValueConverter                         | "#FF00FF00" -> "Green"                  | "Green" -> Color.Green                  |
| Enum -> String            | EnumDescriptionToStringValueConverter                    | DayOfWeek.Monday -> Monday              | Not supported                           |
| Int -> Visibility         | IntegerGreaterThenZeroToVisibilityVisibleValueConverter  | 0 -> Collapsed<br/>1 -> Visible         | Not supported                           |
| Int -> TimeSpan           | IntegerToTimeSpanValueConverter                          | 100 -> TimeSpan.FromMilliseconds(100)   | Not supported                           |
| LogCategoryType -> Brush  | LogCategoryTypeToBrushValueConverter                     | Information -> DodgerBlue               | Not supported                           |
| LogCategoryType -> Color  | LogCategoryTypeToColorValueConverter                     | Information -> DodgerBlue               | Not supported                           |
| LogLevel -> Brush         | LogLevelToBrushValueConverter                            | Information -> DodgerBlue               | Not supported                           |
| LogLevel -> Color         | LogLevelToColorValueConverter                            | Information -> DodgerBlue               | Not supported                           |
|                           | ObservableDictionaryToDictionaryOfStringsValueConverter  |                                         |                                         |
|                           | ThicknessBindingValueConverter                           |                                         | DependencyProperty.UnsetValue           |
|                           | ThicknessFilterValueConverter                            |                                         | DependencyProperty.UnsetValue           |
|                           | ThicknessToDoubleValueConverter                          |                                         | DependencyProperty.UnsetValue           |
| DateTimeOffset -> DateTime| UtcToLocalDateTimeValueConverter                         |                                         | Not supported                           |
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
