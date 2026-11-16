namespace Atc.Wpf.Displays;

/// <summary>
/// The drawing algorithms shared by the frame buffers: clipped lines, rectangle outlines and HD44780 glyph text.
/// </summary>
internal static class DisplayRasterizer
{
    private const long FarCoordinate = 100_000;
    private const int GlyphWidth = 5;
    private const int GlyphAdvance = 6;
    private const int GlyphHeight = 8;
    private const byte GlyphLeftBit = 0x10;

    // Bresenham line, both ends included; a line with far-away ends is clipped to the buffer first.
    public static bool DrawLine<T>(
        ref T sink,
        int x0,
        int y0,
        int x1,
        int y1,
        int width,
        int height)
        where T : struct, IPixelSink
    {
        long ax = x0;
        long ay = y0;
        long bx = x1;
        long by = y1;
        var farthest = System.Math.Max(
            System.Math.Max(System.Math.Abs(ax), System.Math.Abs(ay)),
            System.Math.Max(System.Math.Abs(bx), System.Math.Abs(by)));
        if (farthest > FarCoordinate &&
            !ClipFarLine(ref ax, ref ay, ref bx, ref by, width, height))
        {
            return false;
        }

        var changed = false;
        var x = ax;
        var y = ay;
        var dx = System.Math.Abs(bx - ax);
        var dy = -System.Math.Abs(by - ay);
        var stepX = ax < bx ? 1 : -1;
        var stepY = ay < by ? 1 : -1;
        var error = dx + dy;

        while (true)
        {
            changed |= sink.Plot(x, y);

            if (x == bx && y == by)
            {
                break;
            }

            var doubled = 2 * error;
            if (doubled >= dy)
            {
                error += dy;
                x += stepX;
            }

            if (doubled <= dx)
            {
                error += dx;
                y += stepY;
            }
        }

        return changed;
    }

    // The outline of a rectangle; the part outside the buffer is clipped.
    public static bool DrawRectangleOutline<T>(
        ref T sink,
        int x,
        int y,
        int width,
        int height,
        int bufferWidth,
        int bufferHeight)
        where T : struct, IPixelSink
    {
        if (width <= 0 || height <= 0)
        {
            return false;
        }

        var right = (long)x + width - 1;
        var bottom = (long)y + height - 1;
        var firstX = System.Math.Max(x, 0);
        var lastX = (int)System.Math.Min(right, bufferWidth - 1L);
        var firstY = System.Math.Max(y, 0);
        var lastY = (int)System.Math.Min(bottom, bufferHeight - 1L);

        var changed = false;
        for (var column = firstX; column <= lastX; column++)
        {
            changed |= sink.Plot(column, y);
            changed |= sink.Plot(column, bottom);
        }

        for (var row = firstY; row <= lastY; row++)
        {
            changed |= sink.Plot(x, row);
            changed |= sink.Plot(right, row);
        }

        return changed;
    }

    // Draws text with HD44780 glyphs; every glyph dot becomes a scale x scale square.
    public static bool DrawText<T>(
        ref T sink,
        int x,
        int y,
        string text,
        CharacterRom rom,
        int scale)
        where T : struct, IPixelSink
    {
        var changed = false;
        var column = (long)x;
        var row = (long)y;
        foreach (var c in text)
        {
            if (c == '\r')
            {
                continue;
            }

            if (c == '\n')
            {
                column = x;
                row += (long)GlyphHeight * scale;
                continue;
            }

            var glyph = CharacterRomTables.GetGlyph(rom, CharacterRomTables.Map(rom, c));
            for (var glyphRow = 0; glyphRow < glyph.Length; glyphRow++)
            {
                for (var glyphColumn = 0; glyphColumn < GlyphWidth; glyphColumn++)
                {
                    if ((glyph[glyphRow] & (GlyphLeftBit >> glyphColumn)) == 0)
                    {
                        continue;
                    }

                    changed |= sink.Fill(
                        column + ((long)glyphColumn * scale),
                        row + ((long)glyphRow * scale),
                        scale,
                        scale);
                }
            }

            column += (long)GlyphAdvance * scale;
        }

        return changed;
    }

    // Clips a rectangle to the buffer; false when nothing of it is inside.
    public static bool TryClip(
        long x,
        long y,
        long width,
        long height,
        int bufferWidth,
        int bufferHeight,
        out int firstX,
        out int lastX,
        out int firstY,
        out int lastY)
    {
        var left = System.Math.Max(x, 0);
        var right = System.Math.Min(x + width - 1, bufferWidth - 1L);
        var top = System.Math.Max(y, 0);
        var bottom = System.Math.Min(y + height - 1, bufferHeight - 1L);

        // Compared in long: the casts below happen only after the values are clamped into the buffer.
        var inside = width > 0 && height > 0 && left <= right && top <= bottom;
        firstX = inside ? (int)left : 0;
        lastX = inside ? (int)right : -1;
        firstY = inside ? (int)top : 0;
        lastY = inside ? (int)bottom : -1;
        return inside;
    }

    // Liang-Barsky clip of a line with far-away ends to the buffer, so that Bresenham does not walk billions of pixels.
    // Returns false when the line misses the buffer. Lines with ends near the buffer are not clipped and keep their exact pixels.
    private static bool ClipFarLine(
        ref long x0,
        ref long y0,
        ref long x1,
        ref long y1,
        int width,
        int height)
    {
        double t0 = 0;
        double t1 = 1;
        var dx = (double)x1 - x0;
        var dy = (double)y1 - y0;
        Span<double> p = [-dx, dx, -dy, dy];
        Span<double> q = [x0, (width - 1d) - x0, y0, (height - 1d) - y0];
        for (var i = 0; i < 4; i++)
        {
            if (p[i] == 0)
            {
                if (q[i] < 0)
                {
                    return false;
                }

                continue;
            }

            var r = q[i] / p[i];
            if (p[i] < 0)
            {
                t0 = System.Math.Max(t0, r);
            }
            else
            {
                t1 = System.Math.Min(t1, r);
            }
        }

        if (t0 > t1)
        {
            return false;
        }

        var nx0 = (long)System.Math.Round(x0 + (t0 * dx));
        var ny0 = (long)System.Math.Round(y0 + (t0 * dy));
        var nx1 = (long)System.Math.Round(x0 + (t1 * dx));
        var ny1 = (long)System.Math.Round(y0 + (t1 * dy));
        x0 = nx0;
        y0 = ny0;
        x1 = nx1;
        y1 = ny1;
        return true;
    }
}