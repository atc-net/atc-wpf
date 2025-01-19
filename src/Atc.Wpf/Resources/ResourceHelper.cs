// ReSharper disable IdentifierTypo
// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Wpf.Resources;

internal static class ResourceHelper
{
    private static readonly Dictionary<string, BitmapImage> CacheFlags = new(StringComparer.Ordinal);

    public static Dictionary<string, BitmapImage> GetFlags(
        RenderFlagIndicatorType renderFlagIndicatorType)
    {
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
            .Where(x => x.StartsWith("Atc.Wpf.Resources.Flags", StringComparison.Ordinal) &&
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

                flagBitmaps.Add(flagLocation, bitmap);

                CacheFlags.Add(flagLocation, bitmap);
            }
        }

        return flagBitmaps;
    }
}