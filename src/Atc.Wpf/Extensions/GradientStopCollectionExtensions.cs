// ReSharper disable once CheckNamespace
namespace System.Windows.Media;

public static class GradientStopCollectionExtensions
{
    /// <summary>
    /// Gets the color of a gradient stop collection with the given index.
    /// </summary>
    /// <param name="collection">Collection of colors.</param>
    /// <param name="offset">The offset</param>
    /// <returns>The color at the offset</returns>
    public static Color GetColorAtOffset(
        this GradientStopCollection collection,
        double offset)
    {
        var stops = collection
            .OrderBy(x => x.Offset)
            .ToArray();

        switch (offset)
        {
            case <= 0:
                return stops[0].Color;
            case >= 1:
                return stops[^1].Color;
        }

        var left = stops[0];
        GradientStop? right = null;

        foreach (var stop in stops)
        {
            if (stop.Offset >= offset)
            {
                right = stop;
                break;
            }

            left = stop;
        }

        right ??= stops[0];

        var percent = Math.Round((offset - left.Offset) / (right.Offset - left.Offset), 3);
        var a = (byte)(((right.Color.A - left.Color.A) * percent) + left.Color.A);
        var r = (byte)(((right.Color.R - left.Color.R) * percent) + left.Color.R);
        var g = (byte)(((right.Color.G - left.Color.G) * percent) + left.Color.G);
        var b = (byte)(((right.Color.B - left.Color.B) * percent) + left.Color.B);
        return Color.FromArgb(a, r, g, b);
    }
}