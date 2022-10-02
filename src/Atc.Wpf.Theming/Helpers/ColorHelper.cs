namespace Atc.Wpf.Theming.Helpers;

/// <summary>
/// A Helper class for the Color-Struct.
/// </summary>
public class ColorHelper
{
    /// <summary>
    /// Gets a static default instance of <see cref="ColorHelper"/>.
    /// </summary>
    public static readonly ColorHelper DefaultInstance = new();

    /// <summary>
    /// Gets a static default instance of <see cref="ColorHelper"/> with the original WPF color names.
    /// </summary>
    public static readonly ColorHelper DefaultInstanceInvariant = new(CultureInfo.InvariantCulture);

    /// <summary>
    /// Creates a new Instance of the ColorHelper with the default translations.
    /// </summary>
    public ColorHelper()
        : this(CultureInfo.CurrentUICulture, typeof(ColorNames))
    {
    }

    /// <summary>
    /// Creates a new Instance of the ColorHelper.
    /// </summary>
    /// <param name="culture">The <see cref="CultureInfo"/> used to get the local color names</param>
    public ColorHelper(CultureInfo? culture)
        : this(culture, typeof(ColorNames))
    {
    }

    /// <summary>
    /// Creates a new Instance of the ColorHelper.
    /// </summary>
    /// <param name="culture">The <see cref="CultureInfo"/> used to get the local color names</param>
    /// <param name="resourceDictionaryType">A type from which the <see cref="ResourceManager"/> derives all information for finding .resources files.</param>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
    public ColorHelper(CultureInfo? culture, Type? resourceDictionaryType)
    {
        ColorNamesDictionary = new Dictionary<Color, string>();

        if (culture is null || resourceDictionaryType is null)
        {
            foreach (var propertyInfo in typeof(Colors).GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                try
                {
                    var color = (Color)(propertyInfo.GetValue(obj: null) ?? default(Color));
                    if (!ColorNamesDictionary.ContainsKey(color))
                    {
                        ColorNamesDictionary.Add(color, propertyInfo.Name);
                    }
                }
                catch (Exception)
                {
                    Trace.TraceError($"Color from {propertyInfo.Name} could not be added to dictionary!");
                }
            }
        }
        else
        {
            var rm = new ResourceManager(resourceDictionaryType);
            var resourceSet = rm.GetResourceSet(
                culture,
                createIfNotExists: true,
                tryParents: true);

            if (resourceSet is null)
            {
                return;
            }

            foreach (var entry in resourceSet.OfType<DictionaryEntry>())
            {
                try
                {
                    if (ColorConverter.ConvertFromString(entry.Key.ToString()) is Color color)
                    {
                        ColorNamesDictionary.Add(color, entry.Value!.ToString()!);
                    }
                }
                catch (Exception)
                {
                    Trace.TraceError($"{entry.Key} is not a valid color key!");
                }
            }
        }
    }

    /// <summary>
    /// This function tries to convert a given string into a Color in the following order:
    ///    1. If the string starts with '#' the function tries to get the color from the hex-code
    ///    2. else the function tries to find the color in the color names Dictionary
    ///    3. If 1. + 2. were not successful the function adds a '#' sign and tries 1. + 2. again
    /// </summary>
    /// <param name="colorName">The localized name of the color, the hex-code of the color or the internal color name</param>
    /// <param name="colorNamesDictionary">Optional: The dictionary where the ColorName should be looked up</param>
    /// <returns>the Color if successful, else null</returns>
    public virtual Color? ColorFromString(
        string? colorName,
        Dictionary<Color, string>? colorNamesDictionary)
    {
        Color? result = null;

        try
        {
            if (string.IsNullOrWhiteSpace(colorName))
            {
                return null;
            }

            colorNamesDictionary ??= ColorNamesDictionary;

            if (!colorName.StartsWith('#') &&
                colorNamesDictionary.Any(x => string.Equals(x.Value, colorName, StringComparison.OrdinalIgnoreCase)))
            {
                result = colorNamesDictionary.FirstOrDefault(x => string.Equals(x.Value, colorName, StringComparison.OrdinalIgnoreCase)).Key;
            }

            result ??= ColorConverter.ConvertFromString(colorName) as Color?;
        }
        catch (FormatException)
        {
            if (colorName is not null && !result.HasValue && !colorName.StartsWith('#'))
            {
                result = ColorFromString("#" + colorName);
            }
        }

        return result;
    }

    /// <summary>
    /// This function tries to convert a given string into a Color in the following order:
    ///    1. If the string starts with '#' the function tries to get the color from the hex-code
    ///    2. else the function tries to find the color in the default <see cref="ColorNamesDictionary"/>
    ///    3. If 1. + 2. were not successful the function adds a '#' sign and tries 1. + 2. again
    /// </summary>
    /// <param name="colorName">The localized name of the color, the hex-code of the color or the internal color name</param>
    /// <returns>the Color if successful, else null</returns>
    public Color? ColorFromString(
        string colorName)
        => ColorFromString(colorName, colorNamesDictionary: null);

    /// <summary>
    /// A Dictionary with localized Color Names
    /// </summary>
    public Dictionary<Color, string> ColorNamesDictionary { get; }

    /// <summary>
    /// Searches for the localized name of a given <paramref name="color"/>
    /// </summary>
    /// <param name="color">color</param>
    /// <param name="colorNamesDictionary">Optional: The dictionary where the ColorName should be looked up</param>
    /// <param name="useAlphaChannel">Set this value to <see langword="false"/> if the alpha-channel should be omitted</param>
    /// <returns>the local color name or null if the given color doesn't have a name</returns>
    public virtual string? GetColorName(
        Color? color,
        Dictionary<Color, string>? colorNamesDictionary,
        bool useAlphaChannel)
    {
        if (color is null)
        {
            return null;
        }

        colorNamesDictionary ??= ColorNamesDictionary;

        var colorHex = useAlphaChannel ? color.ToString() : $"#{color.Value.R:X2}{color.Value.G:X2}{color.Value.B:X2}";

        return colorNamesDictionary.TryGetValue(color.Value, out var name) ? $"{name} ({colorHex})" : $"{colorHex}";
    }

    /// <summary>
    /// Searches for the localized name of a given <paramref name="color"/> by using the default <see cref="ColorNamesDictionary"/>
    /// </summary>
    /// <param name="color">color</param>
    /// <returns>the local color name or null if the given color doesn't have a name</returns>
    public string? GetColorName(
        Color? color)
        => GetColorName(color, colorNamesDictionary: null, useAlphaChannel: true);
}