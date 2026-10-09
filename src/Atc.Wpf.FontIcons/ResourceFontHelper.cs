// ReSharper disable StringLiteralTypo
namespace Atc.Wpf.FontIcons;

/// <summary>
/// Provides access to the icon font families embedded in the Atc.Wpf.FontIcons assembly.
/// </summary>
[SuppressMessage("Usage", "CA2201:Do not raise reserved exception types", Justification = "OK.")]
[SuppressMessage("Major Code Smell", "S112:General exceptions should never be thrown", Justification = "OK.")]
[SuppressMessage("Maintainability", "CA1508:Avoid dead conditional code", Justification = "OK - LoadFonts do not ensure fontFamilies is not null.")]
public static class ResourceFontHelper
{
    private static List<FontFamily>? fontFamilies;

    /// <summary>
    /// Gets the embedded Font Awesome 5 Brands font family.
    /// </summary>
    public static FontFamily GetAwesomeBrand()
    {
        // ReSharper disable once InvertIf
        if (fontFamilies is null)
        {
            LoadFonts();
            if (fontFamilies is null)
            {
                throw new Exception("fontFamilies is not loaded");
            }
        }

        return fontFamilies.First(x => x.Source.Equals("./#Font Awesome 5 Brands", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets the embedded Font Awesome 5 Free (regular) font family.
    /// </summary>
    public static FontFamily GetAwesomeRegular()
    {
        // ReSharper disable once InvertIf
        if (fontFamilies is null)
        {
            LoadFonts();
            if (fontFamilies is null)
            {
                throw new Exception("fontFamilies is not loaded");
            }
        }

        return fontFamilies.First(x => x.Source.Equals("./#Font Awesome 5 Free", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets the embedded Font Awesome 5 Free Solid font family.
    /// </summary>
    public static FontFamily GetAwesomeSolid()
    {
        // ReSharper disable once InvertIf
        if (fontFamilies is null)
        {
            LoadFonts();
            if (fontFamilies is null)
            {
                throw new Exception("fontFamilies is not loaded");
            }
        }

        return fontFamilies.First(x => x.Source.Equals("./#Font Awesome 5 Free Solid", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets the embedded Font Awesome 7 Brands font family.
    /// </summary>
    public static FontFamily GetAwesome7Brand()
    {
        // ReSharper disable once InvertIf
        if (fontFamilies is null)
        {
            LoadFonts();
            if (fontFamilies is null)
            {
                throw new Exception("fontFamilies is not loaded");
            }
        }

        return fontFamilies.First(x => x.Source.Equals("./#Font Awesome 7 Brands", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets the embedded Font Awesome 7 Free font family (used for both regular and solid icons).
    /// </summary>
    public static FontFamily GetAwesome7Free()
    {
        // ReSharper disable once InvertIf
        if (fontFamilies is null)
        {
            LoadFonts();
            if (fontFamilies is null)
            {
                throw new Exception("fontFamilies is not loaded");
            }
        }

        return fontFamilies.First(x => x.Source.Equals("./#Font Awesome 7 Free", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets the embedded Bootstrap (GlyphIcons Halflings) font family.
    /// </summary>
    public static FontFamily GetBootstrap()
    {
        // ReSharper disable once InvertIf
        if (fontFamilies is null)
        {
            LoadFonts();
            if (fontFamilies is null)
            {
                throw new Exception("fontFamilies is not loaded");
            }
        }

        return fontFamilies.First(x => x.Source.Equals("./#GlyphIcons Halflings", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets the embedded IcoFont font family.
    /// </summary>
    public static FontFamily GetIcoFont()
    {
        // ReSharper disable once InvertIf
        if (fontFamilies is null)
        {
            LoadFonts();
            if (fontFamilies is null)
            {
                throw new Exception("fontFamilies is not loaded");
            }
        }

        return fontFamilies.First(x => x.Source.Equals("./#IcoFont", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets the embedded Material Design Icons font family.
    /// </summary>
    public static FontFamily GetMaterialDesign()
    {
        // ReSharper disable once InvertIf
        if (fontFamilies is null)
        {
            LoadFonts();
            if (fontFamilies is null)
            {
                throw new Exception("fontFamilies is not loaded");
            }
        }

        return fontFamilies.First(x => x.Source.Equals("./#Material Design Icons", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets the embedded Weather Icons font family.
    /// </summary>
    public static FontFamily GetWeather()
    {
        // ReSharper disable once InvertIf
        if (fontFamilies is null)
        {
            LoadFonts();
            if (fontFamilies is null)
            {
                throw new Exception("fontFamilies is not loaded");
            }
        }

        return fontFamilies.First(x => x.Source.Equals("./#Weather Icons", StringComparison.OrdinalIgnoreCase));
    }

    [SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded", Justification = "OK.")]
    private static void LoadFonts()
    {
        fontFamilies = Fonts.GetFontFamilies(
            new Uri("pack://application:,,,/Atc.Wpf.FontIcons;component/Resources/fonts/#")).ToList();
    }
}