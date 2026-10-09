// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global
namespace Atc.Wpf.Theming;

/// <summary>
/// Specifies a theme brush resource, resolved as <c>AtcApps.Brushes.{Name}</c>
/// (Bootstrap members as <c>AtcApps.Brushes.Bootstrap.{Color}{Shade}</c>).
/// </summary>
public enum AtcAppsBrushKeyType
{
    /// <summary>The highlight brush.</summary>
    Highlight,

    /// <summary>The base accent brush.</summary>
    AccentBase,

    /// <summary>The primary accent brush.</summary>
    Accent,

    /// <summary>The accent brush, variant 2.</summary>
    Accent2,

    /// <summary>The accent brush, variant 3.</summary>
    Accent3,

    /// <summary>The accent brush, variant 4.</summary>
    Accent4,

    /// <summary>The theme background brush.</summary>
    ThemeBackground,

    /// <summary>The theme background brush, level 1.</summary>
    ThemeBackground1,

    /// <summary>The theme background brush, level 2.</summary>
    ThemeBackground2,

    /// <summary>The theme background brush, level 3.</summary>
    ThemeBackground3,

    /// <summary>The theme background brush, level 4.</summary>
    ThemeBackground4,

    /// <summary>The theme background brush, level 5.</summary>
    ThemeBackground5,

    /// <summary>The theme background brush, level 6.</summary>
    ThemeBackground6,

    /// <summary>The theme background brush, level 7.</summary>
    ThemeBackground7,

    /// <summary>The theme foreground brush, level 7.</summary>
    ThemeForeground7,

    /// <summary>The theme foreground brush, level 6.</summary>
    ThemeForeground6,

    /// <summary>The theme foreground brush, level 5.</summary>
    ThemeForeground5,

    /// <summary>The theme foreground brush, level 4.</summary>
    ThemeForeground4,

    /// <summary>The theme foreground brush, level 3.</summary>
    ThemeForeground3,

    /// <summary>The theme foreground brush, level 2.</summary>
    ThemeForeground2,

    /// <summary>The theme foreground brush, level 1.</summary>
    ThemeForeground1,

    /// <summary>The theme foreground brush.</summary>
    ThemeForeground,

    /// <summary>The foreground brush that contrasts best with the accent color.</summary>
    IdealForeground,

    /// <summary>The gray brush, level 1.</summary>
    Gray1,

    /// <summary>The gray brush, level 2.</summary>
    Gray2,

    /// <summary>The gray brush, level 3.</summary>
    Gray3,

    /// <summary>The gray brush, level 4.</summary>
    Gray4,

    /// <summary>The gray brush, level 5.</summary>
    Gray5,

    /// <summary>The gray brush, level 6.</summary>
    Gray6,

    /// <summary>The gray brush, level 7.</summary>
    Gray7,

    /// <summary>The gray brush, level 8.</summary>
    Gray8,

    /// <summary>The gray brush, level 9.</summary>
    Gray9,

    /// <summary>The gray brush, level 10.</summary>
    Gray10,

    /// <summary>The selection overlay background brush.</summary>
    SelectionOverlayBackground,

    /// <summary>The selection overlay border brush.</summary>
    SelectionOverlayBorder,

    /// <summary>The Bootstrap blue 100 brush.</summary>
    BootstrapBlue100,

    /// <summary>The Bootstrap blue 200 brush.</summary>
    BootstrapBlue200,

    /// <summary>The Bootstrap blue 300 brush.</summary>
    BootstrapBlue300,

    /// <summary>The Bootstrap blue 400 brush.</summary>
    BootstrapBlue400,

    /// <summary>The Bootstrap blue 500 brush.</summary>
    BootstrapBlue500,

    /// <summary>The Bootstrap blue 600 brush.</summary>
    BootstrapBlue600,

    /// <summary>The Bootstrap blue 700 brush.</summary>
    BootstrapBlue700,

    /// <summary>The Bootstrap blue 800 brush.</summary>
    BootstrapBlue800,

    /// <summary>The Bootstrap blue 900 brush.</summary>
    BootstrapBlue900,

    /// <summary>The Bootstrap indigo 100 brush.</summary>
    BootstrapIndigo100,

    /// <summary>The Bootstrap indigo 200 brush.</summary>
    BootstrapIndigo200,

    /// <summary>The Bootstrap indigo 300 brush.</summary>
    BootstrapIndigo300,

    /// <summary>The Bootstrap indigo 400 brush.</summary>
    BootstrapIndigo400,

    /// <summary>The Bootstrap indigo 500 brush.</summary>
    BootstrapIndigo500,

    /// <summary>The Bootstrap indigo 600 brush.</summary>
    BootstrapIndigo600,

    /// <summary>The Bootstrap indigo 700 brush.</summary>
    BootstrapIndigo700,

    /// <summary>The Bootstrap indigo 800 brush.</summary>
    BootstrapIndigo800,

    /// <summary>The Bootstrap indigo 900 brush.</summary>
    BootstrapIndigo900,

    /// <summary>The Bootstrap purple 100 brush.</summary>
    BootstrapPurple100,

    /// <summary>The Bootstrap purple 200 brush.</summary>
    BootstrapPurple200,

    /// <summary>The Bootstrap purple 300 brush.</summary>
    BootstrapPurple300,

    /// <summary>The Bootstrap purple 400 brush.</summary>
    BootstrapPurple400,

    /// <summary>The Bootstrap purple 500 brush.</summary>
    BootstrapPurple500,

    /// <summary>The Bootstrap purple 600 brush.</summary>
    BootstrapPurple600,

    /// <summary>The Bootstrap purple 700 brush.</summary>
    BootstrapPurple700,

    /// <summary>The Bootstrap purple 800 brush.</summary>
    BootstrapPurple800,

    /// <summary>The Bootstrap purple 900 brush.</summary>
    BootstrapPurple900,

    /// <summary>The Bootstrap pink 100 brush.</summary>
    BootstrapPink100,

    /// <summary>The Bootstrap pink 200 brush.</summary>
    BootstrapPink200,

    /// <summary>The Bootstrap pink 300 brush.</summary>
    BootstrapPink300,

    /// <summary>The Bootstrap pink 400 brush.</summary>
    BootstrapPink400,

    /// <summary>The Bootstrap pink 500 brush.</summary>
    BootstrapPink500,

    /// <summary>The Bootstrap pink 600 brush.</summary>
    BootstrapPink600,

    /// <summary>The Bootstrap pink 700 brush.</summary>
    BootstrapPink700,

    /// <summary>The Bootstrap pink 800 brush.</summary>
    BootstrapPink800,

    /// <summary>The Bootstrap pink 900 brush.</summary>
    BootstrapPink900,

    /// <summary>The Bootstrap red 100 brush.</summary>
    BootstrapRed100,

    /// <summary>The Bootstrap red 200 brush.</summary>
    BootstrapRed200,

    /// <summary>The Bootstrap red 300 brush.</summary>
    BootstrapRed300,

    /// <summary>The Bootstrap red 400 brush.</summary>
    BootstrapRed400,

    /// <summary>The Bootstrap red 500 brush.</summary>
    BootstrapRed500,

    /// <summary>The Bootstrap red 600 brush.</summary>
    BootstrapRed600,

    /// <summary>The Bootstrap red 700 brush.</summary>
    BootstrapRed700,

    /// <summary>The Bootstrap red 800 brush.</summary>
    BootstrapRed800,

    /// <summary>The Bootstrap red 900 brush.</summary>
    BootstrapRed900,

    /// <summary>The Bootstrap orange 100 brush.</summary>
    BootstrapOrange100,

    /// <summary>The Bootstrap orange 200 brush.</summary>
    BootstrapOrange200,

    /// <summary>The Bootstrap orange 300 brush.</summary>
    BootstrapOrange300,

    /// <summary>The Bootstrap orange 400 brush.</summary>
    BootstrapOrange400,

    /// <summary>The Bootstrap orange 500 brush.</summary>
    BootstrapOrange500,

    /// <summary>The Bootstrap orange 600 brush.</summary>
    BootstrapOrange600,

    /// <summary>The Bootstrap orange 700 brush.</summary>
    BootstrapOrange700,

    /// <summary>The Bootstrap orange 800 brush.</summary>
    BootstrapOrange800,

    /// <summary>The Bootstrap orange 900 brush.</summary>
    BootstrapOrange900,

    /// <summary>The Bootstrap yellow 100 brush.</summary>
    BootstrapYellow100,

    /// <summary>The Bootstrap yellow 200 brush.</summary>
    BootstrapYellow200,

    /// <summary>The Bootstrap yellow 300 brush.</summary>
    BootstrapYellow300,

    /// <summary>The Bootstrap yellow 400 brush.</summary>
    BootstrapYellow400,

    /// <summary>The Bootstrap yellow 500 brush.</summary>
    BootstrapYellow500,

    /// <summary>The Bootstrap yellow 600 brush.</summary>
    BootstrapYellow600,

    /// <summary>The Bootstrap yellow 700 brush.</summary>
    BootstrapYellow700,

    /// <summary>The Bootstrap yellow 800 brush.</summary>
    BootstrapYellow800,

    /// <summary>The Bootstrap yellow 900 brush.</summary>
    BootstrapYellow900,

    /// <summary>The Bootstrap green 100 brush.</summary>
    BootstrapGreen100,

    /// <summary>The Bootstrap green 200 brush.</summary>
    BootstrapGreen200,

    /// <summary>The Bootstrap green 300 brush.</summary>
    BootstrapGreen300,

    /// <summary>The Bootstrap green 400 brush.</summary>
    BootstrapGreen400,

    /// <summary>The Bootstrap green 500 brush.</summary>
    BootstrapGreen500,

    /// <summary>The Bootstrap green 600 brush.</summary>
    BootstrapGreen600,

    /// <summary>The Bootstrap green 700 brush.</summary>
    BootstrapGreen700,

    /// <summary>The Bootstrap green 800 brush.</summary>
    BootstrapGreen800,

    /// <summary>The Bootstrap green 900 brush.</summary>
    BootstrapGreen900,

    /// <summary>The Bootstrap teal 100 brush.</summary>
    BootstrapTeal100,

    /// <summary>The Bootstrap teal 200 brush.</summary>
    BootstrapTeal200,

    /// <summary>The Bootstrap teal 300 brush.</summary>
    BootstrapTeal300,

    /// <summary>The Bootstrap teal 400 brush.</summary>
    BootstrapTeal400,

    /// <summary>The Bootstrap teal 500 brush.</summary>
    BootstrapTeal500,

    /// <summary>The Bootstrap teal 600 brush.</summary>
    BootstrapTeal600,

    /// <summary>The Bootstrap teal 700 brush.</summary>
    BootstrapTeal700,

    /// <summary>The Bootstrap teal 800 brush.</summary>
    BootstrapTeal800,

    /// <summary>The Bootstrap teal 900 brush.</summary>
    BootstrapTeal900,

    /// <summary>The Bootstrap cyan 100 brush.</summary>
    BootstrapCyan100,

    /// <summary>The Bootstrap cyan 200 brush.</summary>
    BootstrapCyan200,

    /// <summary>The Bootstrap cyan 300 brush.</summary>
    BootstrapCyan300,

    /// <summary>The Bootstrap cyan 400 brush.</summary>
    BootstrapCyan400,

    /// <summary>The Bootstrap cyan 500 brush.</summary>
    BootstrapCyan500,

    /// <summary>The Bootstrap cyan 600 brush.</summary>
    BootstrapCyan600,

    /// <summary>The Bootstrap cyan 700 brush.</summary>
    BootstrapCyan700,

    /// <summary>The Bootstrap cyan 800 brush.</summary>
    BootstrapCyan800,

    /// <summary>The Bootstrap cyan 900 brush.</summary>
    BootstrapCyan900,

    /// <summary>The Bootstrap gray 100 brush.</summary>
    BootstrapGray100,

    /// <summary>The Bootstrap gray 200 brush.</summary>
    BootstrapGray200,

    /// <summary>The Bootstrap gray 300 brush.</summary>
    BootstrapGray300,

    /// <summary>The Bootstrap gray 400 brush.</summary>
    BootstrapGray400,

    /// <summary>The Bootstrap gray 500 brush.</summary>
    BootstrapGray500,

    /// <summary>The Bootstrap gray 600 brush.</summary>
    BootstrapGray600,

    /// <summary>The Bootstrap gray 700 brush.</summary>
    BootstrapGray700,

    /// <summary>The Bootstrap gray 800 brush.</summary>
    BootstrapGray800,

    /// <summary>The Bootstrap gray 900 brush.</summary>
    BootstrapGray900,
}