# Atc.Wpf.FontIcons

Font-based icon rendering for WPF. Embeds glyph fonts (FontAwesome, Material, Octicons, Foundation, ElegantIcons, Mahapps, Modern, …) and exposes them as `FontIcon` controls and image-source helpers, so icons stay crisp at any DPI without bitmap assets.

## What lives here

| Area | Path | Highlights |
|---|---|---|
| Font-icon controls | `Controls/` | `FontIcon`, `FontIconImage`, font-family enums per icon set |
| Embedded fonts | `Fonts/` | OTF/TTF resources (FontAwesome, Material, Octicons, …) |
| Helpers | `Helpers/` | `ResourceFontHelper` — lazy-loads font-family lists once per icon set |
| Generated lookups | output of `Atc.Wpf.Generator.FontIconResources` | strongly-typed glyph constants per icon set |

## Install

```xml
<PackageReference Include="Atc.Wpf.FontIcons" Version="2.*" />
```

Use a font icon in XAML:

```xml
<atc:FontIcon
    FontIconType="FontAwesomeSolid"
    Glyph="{x:Static atcicons:FontAwesomeSolidGlyph.House}"
    Width="24"
    Height="24" />
```

## Docs

- Font icons overview & supported icon sets: [`docs/FontIcons/@Readme.md`](../../docs/FontIcons/@Readme.md)

## Dependencies

- [`Atc.Wpf`](../Atc.Wpf/README.md) — core
