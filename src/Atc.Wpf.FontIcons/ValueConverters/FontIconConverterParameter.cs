namespace Atc.Wpf.FontIcons.ValueConverters;

/// <summary>
/// Resolves the <c>ConverterParameter</c> of the font-icon value converters into a foreground brush and an em-size.
/// </summary>
internal static class FontIconConverterParameter
{
    private static readonly BrushConverter BrushConverter = new();

    /// <summary>
    /// Resolves <paramref name="parameter"/>: a <see cref="Brush"/> gives the brush, a <see cref="double"/> gives the em-size,
    /// and a string (as XAML passes <c>ConverterParameter=24</c> or <c>ConverterParameter=Red</c>) is parsed as an
    /// invariant-culture number or as a brush. Anything else gives the defaults.
    /// </summary>
    /// <param name="parameter">The converter parameter.</param>
    /// <param name="defaultBrush">The brush used when the parameter does not give one.</param>
    /// <param name="defaultEmSize">The em-size used when the parameter does not give one.</param>
    /// <returns>The brush and the em-size to render with.</returns>
    public static (Brush Brush, double EmSize) Resolve(
        object? parameter,
        Brush defaultBrush,
        double defaultEmSize)
    {
        switch (parameter)
        {
            case Brush brush:
                return (brush, defaultEmSize);
            case double emSize:
                return (defaultBrush, emSize);
            case string text when !string.IsNullOrWhiteSpace(text):
                if (double.TryParse(
                        text,
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out var parsedEmSize))
                {
                    return (defaultBrush, parsedEmSize);
                }

                var parsedBrush = TryParseBrush(text);
                return parsedBrush is null
                    ? (defaultBrush, defaultEmSize)
                    : (parsedBrush, defaultEmSize);
            default:
                return (defaultBrush, defaultEmSize);
        }
    }

    private static Brush? TryParseBrush(string text)
    {
        try
        {
            if (BrushConverter.ConvertFromInvariantString(text) is not Brush brush)
            {
                return null;
            }

            if (brush.CanFreeze)
            {
                brush.Freeze();
            }

            return brush;
        }
        catch (FormatException)
        {
            return null;
        }
        catch (NotSupportedException)
        {
            return null;
        }
    }
}