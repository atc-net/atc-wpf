[![NuGet Version](https://img.shields.io/nuget/v/Atc.Wpf.svg?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/atc-wpf)

# ATC.Net WPF

This is a base library for building WPF application with the MVVM design pattern.

### Requirements

* [.NET 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)


### ValueConverters

| Category                  | Type                                                     | Convert Examples                        | ConvertBack Examples                    |
| ------------------------- | -------------------------------------------------------- | --------------------------------------- | --------------------------------------- |
| Bool -> Bool              | BoolToInverseBoolValueConverter                          | True -> False and False -> True         | False -> True and False -> False        |
| Bool -> Visibility        | BoolToVisibilityCollapsedValueConverter                  | True -> Collapsed and False -> Visible  | Collapsed -> True and Visible -> False  |
| Bool -> Visibility        | BoolToVisibilityVisibleValueConverter                    | True -> Visible and False -> Collapsed  | Visible -> True and Collapsed -> False  |
| Bool -> With              | BoolToWidthValueConverter                                | true, 10 -> 10 and true, "Auto" -> *    | Not supported                           |
| Brush -> Color            | BrushToColorValueConverter                               | Brushs.Green -> Colors.Green            | Colors.Green -> Brushs.Green            |
| ICollection -> Bool       | CollectionNullOrEmptyToBoolValueConverter                | NULL or empty -> True                   | Not supported                           |
| ICollection -> Bool       | CollectionNullOrEmptyToInverseBoolValueConverter         | NULL or empty -> False                  | Not supported                           |
| ICollection -> Visibility | CollectionNullOrEmptyToVisibilityCollapsedValueConverter | NULL or empty -> Collapsed              | Not supported                           |
| ICollection -> Visibility | CollectionNullOrEmptyToVisibilityVisibleValueConverter   | NULL or empty -> Visible                | Not supported                           |
| String -> Brush           | ColorNameToBrushValueConverter                           | "Green" -> Brushs.Green                 | Brushs.Green -> "Green"                 |
| String -> Color           | ColorNameToColorValueConverter                           | "Green" -> Colors.Green                 | Colors.Green -> "Green"                 |
| Color -> Brush            | ColorToBrushValueConverter                               | Colors.Green -> Brushs.Green            | Brushs.Green -> Colors.Green            |
| Enum -> String            | EnumDescriptionToStringValueConverter                    | DayOfWeek.Monday -> Monday              | Not supported                           |
| Bool[] -> Bool            | MultiBoolToBoolValueConverter                            | All-True -> True                        | Not supported                           |
| Bool[] -> Visibility      | MultiBoolToVisibilityVisibleValueConverter               | All-True -> Visible                     | Not supported                           |
| Object -> Bool            | ObjectNotNullToBoolValueConverter                        | NotNULL -> True                         | Not supported                           |
| Object -> Visibility      | ObjectNotNullToVisibilityVisibleValueConverter           | NotNULL -> Visible                      | Not supported                           |
| Object -> Bool            | ObjectNullToBoolValueConverter                           | NULL => True                            | Not supported                           |
| String -> Bool            | StringNullOrEmptyToBoolValueConverter                    | NULL or empty -> True                   | Not supported                           |
| String -> Bool            | StringNullOrEmptyToInverseBoolValueConverter             | NULL or empty -> False                  | Not supported                           |
| String -> Visibility      | StringNullOrEmptyToVisibilityCollapsedValueConverter     | NULL or empty -> Collapsed              | Not supported                           |
| String -> Visibility      | StringNullOrEmptyToVisibilityVisibleValueConverter       | NULL or empty -> Visible                | Not supported                           |
| Errors -> String          | ValidationErrorsToStringValueConverter                   |                                         | Not supported                           |


### Media - ShaderEffects

| Type                          | Parameters and range values                                                 |
| ----------------------------- | --------------------------------------------------------------------------- |
| ContrastAdjustShaderEffect    | Brightness (-1.0 to 1.0 default 0.0) and Contrast (-1.0 to 1.0 default 0.0) |
| DesaturateShaderEffect        | Strength (0.0 to 1.0 default 0.0)                                           |
| FadeShaderEffect              | Strength (0.0 to 1.0 default 0.0) and Color (color)                         |
| InvertColorsShaderEffect      | None                                                                        |
| MonochromeShaderEffect        | Color (color)                                                               |
| SaturateShaderEffect          | Progress                                                                    |


## How to contribute

[Contribution Guidelines](https://atc-net.github.io/introduction/about-atc#how-to-contribute)

[Coding Guidelines](https://atc-net.github.io/introduction/about-atc#coding-guidelines)
