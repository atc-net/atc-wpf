### General Project Info
[![Github top language](https://img.shields.io/github/languages/top/atc-net/atc-wpf)](https://github.com/atc-net/atc-wpf)
[![Github stars](https://img.shields.io/github/stars/atc-net/atc-wpf)](https://github.com/atc-net/atc-wpf)
[![Github forks](https://img.shields.io/github/forks/atc-net/atc-wpf)](https://github.com/atc-net/atc-wpf)
[![Github size](https://img.shields.io/github/repo-size/atc-net/atc-wpf)](https://github.com/atc-net/atc-wpf)
[![Issues Open](https://img.shields.io/github/issues/atc-net/atc-wpf.svg?logo=github)](https://github.com/atc-net/atc-wpf/issues)

### Packages
[![Github Version](https://img.shields.io/static/v1?logo=github&color=blue&label=github&message=latest)](https://github.com/orgs/atc-net/packages?repo_name=atc-wpf)
[![NuGet Version](https://img.shields.io/nuget/v/Atc.Wpf.svg?logo=nuget)](https://www.nuget.org/profiles/atc-net)

### Build Status
![Pre-Integration](https://github.com/atc-net/atc-wpf/workflows/Pre-Integration/badge.svg)
![Post-Integration](https://github.com/atc-net/atc-wpf/workflows/Post-Integration/badge.svg)
![Release](https://github.com/atc-net/atc-wpf/workflows/Release/badge.svg)

### Code Quality
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=atc-wpf&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=atc-wpf)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=atc-wpf&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=atc-wpf)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=atc-wpf&metric=security_rating)](https://sonarcloud.io/dashboard?id=atc-wpf)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=atc-wpf&metric=bugs)](https://sonarcloud.io/dashboard?id=atc-wpf)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=atc-wpf&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=atc-wpf)

# ATC.Net WPF

This is a base library for building WPF application with the MVVM design pattern.


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


# The workflow setup for this repository
[Read more on Git-Flow](docs/GitFlow.md)

# Contributing

Please refer to each project's style and contribution guidelines for submitting patches and additions. In general, we follow the "fork-and-pull" Git workflow. [Read more here](https://gist.github.com/Chaser324/ce0505fbed06b947d962).

 1. **Fork** the repo on GitHub
 2. **Clone** the project to your own machine
 3. **Commit** changes to your own branch
 4. **Push** your work back up to your fork
 5. Submit a **Pull request** so that we can review your changes

NOTE: Be sure to merge the latest from "upstream" before making a pull request!

# Coding Guidelines

This repository is adapting the [ATC-Coding-Rules](https://github.com/atc-net/atc-coding-rules) which is defined and based on .editorconfig's and a range of Roslyn Analyzers.
