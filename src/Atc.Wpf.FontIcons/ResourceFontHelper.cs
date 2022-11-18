namespace Atc.Wpf.FontIcons;

[SuppressMessage("Usage", "CA2201:Do not raise reserved exception types", Justification = "OK.")]
[SuppressMessage("Major Code Smell", "S112:General exceptions should never be thrown", Justification = "OK.")]
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

        return fontFamilies.First(x => x.Source.Equals("./#Font Awesome 5 Brands", StringComparison.CurrentCultureIgnoreCase));
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

        return fontFamilies.First(x => x.Source.Equals("./#Font Awesome 5 Free", StringComparison.CurrentCultureIgnoreCase));
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

        return fontFamilies.First(x => x.Source.Equals("./#Font Awesome 5 Free Solid", StringComparison.CurrentCultureIgnoreCase));
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

        return fontFamilies.First(x => x.Source.Equals("./#GlyphIcons Halflings", StringComparison.CurrentCultureIgnoreCase));
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

        return fontFamilies.First(x => x.Source.Equals("./#IcoFont", StringComparison.CurrentCultureIgnoreCase));
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

        return fontFamilies.First(x => x.Source.Equals("./#Material Design Icons", StringComparison.CurrentCultureIgnoreCase));
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

        return fontFamilies.First(x => x.Source.Equals("./#Weather Icons", StringComparison.CurrentCultureIgnoreCase));
    }

    [SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded", Justification = "OK.")]
    private static void LoadFonts()
    {
        fontFamilies = Fonts.GetFontFamilies(
            new Uri("pack://application:,,,/Atc.Wpf.FontIcons;component/Resources/fonts/#")).ToList();
    }
}