// ReSharper disable StringLiteralTypo
namespace Atc.Wpf.FontIcons;

[SuppressMessage("Usage", "CA2201:Do not raise reserved exception types", Justification = "OK.")]
[SuppressMessage("Major Code Smell", "S112:General exceptions should never be thrown", Justification = "OK.")]
[SuppressMessage("Maintainability", "CA1508:Avoid dead conditional code", Justification = "OK - LoadFonts do not ensure fontFamilies is not null.")]
public static class ResourceFontHelper
{
    private static List<FontFamily>? fontFamilies;

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