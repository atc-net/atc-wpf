// ReSharper disable IdentifierTypo
// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Wpf.Controls.Resources;

internal static class ResourceHelper
{
    // Selectors on different UI threads can load flags at the same time.
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, BitmapImage> CacheFlags = new(StringComparer.Ordinal);
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<RenderFlagIndicatorType, Dictionary<string, BitmapImage>> CachedResults = new();

    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
    public static Dictionary<string, BitmapImage> GetFlags(
        RenderFlagIndicatorType renderFlagIndicatorType)
    {
        if (CachedResults.TryGetValue(renderFlagIndicatorType, out var cached))
        {
            return cached;
        }

        var filterStyle = string.Empty;
        var filterSize = string.Empty;

        switch (renderFlagIndicatorType)
        {
            case RenderFlagIndicatorType.None:
                break;
            case RenderFlagIndicatorType.Flat16:
                filterStyle = "Flat";
                filterSize = "16";
                break;
            case RenderFlagIndicatorType.Shiny16:
                filterStyle = "Shiny";
                filterSize = "16";
                break;
            default:
                throw new SwitchCaseDefaultException(renderFlagIndicatorType);
        }

        var assembly = Assembly.GetAssembly(typeof(LanguageSelector))!;

        var flagLocations = assembly
            .GetManifestResourceNames()
            .Where(x => x.StartsWith("Atc.Wpf.Controls.Resources.Flags", StringComparison.Ordinal) &&
                        x.Contains(filterStyle, StringComparison.Ordinal) &&
                        x.Contains(filterSize, StringComparison.Ordinal))
            .ToList();

        var flagBitmaps = new Dictionary<string, BitmapImage>(StringComparer.Ordinal);
        foreach (var flagLocation in flagLocations)
        {
            if (CacheFlags.TryGetValue(flagLocation, out var flag))
            {
                flagBitmaps.Add(flagLocation, flag);
            }
            else
            {
                using var stream = assembly.GetManifestResourceStream(flagLocation)!;
                stream.Position = 0;

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = null;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze();

                flagBitmaps.Add(flagLocation, CacheFlags.GetOrAdd(flagLocation, bitmap));
            }
        }

        return CachedResults.GetOrAdd(renderFlagIndicatorType, flagBitmaps);
    }
}