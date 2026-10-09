// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
namespace Atc.Wpf.Theming;

/// <summary>
/// Specifies a theme color resource, resolved as <c>AtcApps.Colors.{Name}</c>
/// (Bootstrap members as <c>AtcApps.Colors.Bootstrap.{Color}{Shade}</c>).
/// </summary>
public enum AtcAppsColorKeyType
{
    /// <summary>The highlight color.</summary>
    Highlight,

    /// <summary>The base accent color.</summary>
    AccentBase,

    /// <summary>The primary accent color.</summary>
    Accent,

    /// <summary>The accent color, variant 2.</summary>
    Accent2,

    /// <summary>The accent color, variant 3.</summary>
    Accent3,

    /// <summary>The accent color, variant 4.</summary>
    Accent4,

    /// <summary>The theme background color.</summary>
    ThemeBackground,

    /// <summary>The theme background color, level 1.</summary>
    ThemeBackground1,

    /// <summary>The theme background color, level 2.</summary>
    ThemeBackground2,

    /// <summary>The theme background color, level 3.</summary>
    ThemeBackground3,

    /// <summary>The theme background color, level 4.</summary>
    ThemeBackground4,

    /// <summary>The theme background color, level 5.</summary>
    ThemeBackground5,

    /// <summary>The theme background color, level 6.</summary>
    ThemeBackground6,

    /// <summary>The theme background color, level 7.</summary>
    ThemeBackground7,

    /// <summary>The theme foreground color, level 7.</summary>
    ThemeForeground7,

    /// <summary>The theme foreground color, level 6.</summary>
    ThemeForeground6,

    /// <summary>The theme foreground color, level 5.</summary>
    ThemeForeground5,

    /// <summary>The theme foreground color, level 4.</summary>
    ThemeForeground4,

    /// <summary>The theme foreground color, level 3.</summary>
    ThemeForeground3,

    /// <summary>The theme foreground color, level 2.</summary>
    ThemeForeground2,

    /// <summary>The theme foreground color, level 1.</summary>
    ThemeForeground1,

    /// <summary>The theme foreground color.</summary>
    ThemeForeground,

    /// <summary>The foreground color that contrasts best with the accent color.</summary>
    IdealForeground,

    /// <summary>The gray color, level 1.</summary>
    Gray1,

    /// <summary>The gray color, level 2.</summary>
    Gray2,

    /// <summary>The gray color, level 3.</summary>
    Gray3,

    /// <summary>The gray color, level 4.</summary>
    Gray4,

    /// <summary>The gray color, level 5.</summary>
    Gray5,

    /// <summary>The gray color, level 6.</summary>
    Gray6,

    /// <summary>The gray color, level 7.</summary>
    Gray7,

    /// <summary>The gray color, level 8.</summary>
    Gray8,

    /// <summary>The gray color, level 9.</summary>
    Gray9,

    /// <summary>The gray color, level 10.</summary>
    Gray10,

    /// <summary>The selection overlay background color.</summary>
    SelectionOverlayBackground,

    /// <summary>The selection overlay border color.</summary>
    SelectionOverlayBorder,

    /// <summary>The Bootstrap blue 100 color.</summary>
    BootstrapBlue100,

    /// <summary>The Bootstrap blue 200 color.</summary>
    BootstrapBlue200,

    /// <summary>The Bootstrap blue 300 color.</summary>
    BootstrapBlue300,

    /// <summary>The Bootstrap blue 400 color.</summary>
    BootstrapBlue400,

    /// <summary>The Bootstrap blue 500 color.</summary>
    BootstrapBlue500,

    /// <summary>The Bootstrap blue 600 color.</summary>
    BootstrapBlue600,

    /// <summary>The Bootstrap blue 700 color.</summary>
    BootstrapBlue700,

    /// <summary>The Bootstrap blue 800 color.</summary>
    BootstrapBlue800,

    /// <summary>The Bootstrap blue 900 color.</summary>
    BootstrapBlue900,

    /// <summary>The Bootstrap indigo 100 color.</summary>
    BootstrapIndigo100,

    /// <summary>The Bootstrap indigo 200 color.</summary>
    BootstrapIndigo200,

    /// <summary>The Bootstrap indigo 300 color.</summary>
    BootstrapIndigo300,

    /// <summary>The Bootstrap indigo 400 color.</summary>
    BootstrapIndigo400,

    /// <summary>The Bootstrap indigo 500 color.</summary>
    BootstrapIndigo500,

    /// <summary>The Bootstrap indigo 600 color.</summary>
    BootstrapIndigo600,

    /// <summary>The Bootstrap indigo 700 color.</summary>
    BootstrapIndigo700,

    /// <summary>The Bootstrap indigo 800 color.</summary>
    BootstrapIndigo800,

    /// <summary>The Bootstrap indigo 900 color.</summary>
    BootstrapIndigo900,

    /// <summary>The Bootstrap purple 100 color.</summary>
    BootstrapPurple100,

    /// <summary>The Bootstrap purple 200 color.</summary>
    BootstrapPurple200,

    /// <summary>The Bootstrap purple 300 color.</summary>
    BootstrapPurple300,

    /// <summary>The Bootstrap purple 400 color.</summary>
    BootstrapPurple400,

    /// <summary>The Bootstrap purple 500 color.</summary>
    BootstrapPurple500,

    /// <summary>The Bootstrap purple 600 color.</summary>
    BootstrapPurple600,

    /// <summary>The Bootstrap purple 700 color.</summary>
    BootstrapPurple700,

    /// <summary>The Bootstrap purple 800 color.</summary>
    BootstrapPurple800,

    /// <summary>The Bootstrap purple 900 color.</summary>
    BootstrapPurple900,

    /// <summary>The Bootstrap pink 100 color.</summary>
    BootstrapPink100,

    /// <summary>The Bootstrap pink 200 color.</summary>
    BootstrapPink200,

    /// <summary>The Bootstrap pink 300 color.</summary>
    BootstrapPink300,

    /// <summary>The Bootstrap pink 400 color.</summary>
    BootstrapPink400,

    /// <summary>The Bootstrap pink 500 color.</summary>
    BootstrapPink500,

    /// <summary>The Bootstrap pink 600 color.</summary>
    BootstrapPink600,

    /// <summary>The Bootstrap pink 700 color.</summary>
    BootstrapPink700,

    /// <summary>The Bootstrap pink 800 color.</summary>
    BootstrapPink800,

    /// <summary>The Bootstrap pink 900 color.</summary>
    BootstrapPink900,

    /// <summary>The Bootstrap red 100 color.</summary>
    BootstrapRed100,

    /// <summary>The Bootstrap red 200 color.</summary>
    BootstrapRed200,

    /// <summary>The Bootstrap red 300 color.</summary>
    BootstrapRed300,

    /// <summary>The Bootstrap red 400 color.</summary>
    BootstrapRed400,

    /// <summary>The Bootstrap red 500 color.</summary>
    BootstrapRed500,

    /// <summary>The Bootstrap red 600 color.</summary>
    BootstrapRed600,

    /// <summary>The Bootstrap red 700 color.</summary>
    BootstrapRed700,

    /// <summary>The Bootstrap red 800 color.</summary>
    BootstrapRed800,

    /// <summary>The Bootstrap red 900 color.</summary>
    BootstrapRed900,

    /// <summary>The Bootstrap orange 100 color.</summary>
    BootstrapOrange100,

    /// <summary>The Bootstrap orange 200 color.</summary>
    BootstrapOrange200,

    /// <summary>The Bootstrap orange 300 color.</summary>
    BootstrapOrange300,

    /// <summary>The Bootstrap orange 400 color.</summary>
    BootstrapOrange400,

    /// <summary>The Bootstrap orange 500 color.</summary>
    BootstrapOrange500,

    /// <summary>The Bootstrap orange 600 color.</summary>
    BootstrapOrange600,

    /// <summary>The Bootstrap orange 700 color.</summary>
    BootstrapOrange700,

    /// <summary>The Bootstrap orange 800 color.</summary>
    BootstrapOrange800,

    /// <summary>The Bootstrap orange 900 color.</summary>
    BootstrapOrange900,

    /// <summary>The Bootstrap yellow 100 color.</summary>
    BootstrapYellow100,

    /// <summary>The Bootstrap yellow 200 color.</summary>
    BootstrapYellow200,

    /// <summary>The Bootstrap yellow 300 color.</summary>
    BootstrapYellow300,

    /// <summary>The Bootstrap yellow 400 color.</summary>
    BootstrapYellow400,

    /// <summary>The Bootstrap yellow 500 color.</summary>
    BootstrapYellow500,

    /// <summary>The Bootstrap yellow 600 color.</summary>
    BootstrapYellow600,

    /// <summary>The Bootstrap yellow 700 color.</summary>
    BootstrapYellow700,

    /// <summary>The Bootstrap yellow 800 color.</summary>
    BootstrapYellow800,

    /// <summary>The Bootstrap yellow 900 color.</summary>
    BootstrapYellow900,

    /// <summary>The Bootstrap green 100 color.</summary>
    BootstrapGreen100,

    /// <summary>The Bootstrap green 200 color.</summary>
    BootstrapGreen200,

    /// <summary>The Bootstrap green 300 color.</summary>
    BootstrapGreen300,

    /// <summary>The Bootstrap green 400 color.</summary>
    BootstrapGreen400,

    /// <summary>The Bootstrap green 500 color.</summary>
    BootstrapGreen500,

    /// <summary>The Bootstrap green 600 color.</summary>
    BootstrapGreen600,

    /// <summary>The Bootstrap green 700 color.</summary>
    BootstrapGreen700,

    /// <summary>The Bootstrap green 800 color.</summary>
    BootstrapGreen800,

    /// <summary>The Bootstrap green 900 color.</summary>
    BootstrapGreen900,

    /// <summary>The Bootstrap teal 100 color.</summary>
    BootstrapTeal100,

    /// <summary>The Bootstrap teal 200 color.</summary>
    BootstrapTeal200,

    /// <summary>The Bootstrap teal 300 color.</summary>
    BootstrapTeal300,

    /// <summary>The Bootstrap teal 400 color.</summary>
    BootstrapTeal400,

    /// <summary>The Bootstrap teal 500 color.</summary>
    BootstrapTeal500,

    /// <summary>The Bootstrap teal 600 color.</summary>
    BootstrapTeal600,

    /// <summary>The Bootstrap teal 700 color.</summary>
    BootstrapTeal700,

    /// <summary>The Bootstrap teal 800 color.</summary>
    BootstrapTeal800,

    /// <summary>The Bootstrap teal 900 color.</summary>
    BootstrapTeal900,

    /// <summary>The Bootstrap cyan 100 color.</summary>
    BootstrapCyan100,

    /// <summary>The Bootstrap cyan 200 color.</summary>
    BootstrapCyan200,

    /// <summary>The Bootstrap cyan 300 color.</summary>
    BootstrapCyan300,

    /// <summary>The Bootstrap cyan 400 color.</summary>
    BootstrapCyan400,

    /// <summary>The Bootstrap cyan 500 color.</summary>
    BootstrapCyan500,

    /// <summary>The Bootstrap cyan 600 color.</summary>
    BootstrapCyan600,

    /// <summary>The Bootstrap cyan 700 color.</summary>
    BootstrapCyan700,

    /// <summary>The Bootstrap cyan 800 color.</summary>
    BootstrapCyan800,

    /// <summary>The Bootstrap cyan 900 color.</summary>
    BootstrapCyan900,

    /// <summary>The Bootstrap gray 100 color.</summary>
    BootstrapGray100,

    /// <summary>The Bootstrap gray 200 color.</summary>
    BootstrapGray200,

    /// <summary>The Bootstrap gray 300 color.</summary>
    BootstrapGray300,

    /// <summary>The Bootstrap gray 400 color.</summary>
    BootstrapGray400,

    /// <summary>The Bootstrap gray 500 color.</summary>
    BootstrapGray500,

    /// <summary>The Bootstrap gray 600 color.</summary>
    BootstrapGray600,

    /// <summary>The Bootstrap gray 700 color.</summary>
    BootstrapGray700,

    /// <summary>The Bootstrap gray 800 color.</summary>
    BootstrapGray800,

    /// <summary>The Bootstrap gray 900 color.</summary>
    BootstrapGray900,
}