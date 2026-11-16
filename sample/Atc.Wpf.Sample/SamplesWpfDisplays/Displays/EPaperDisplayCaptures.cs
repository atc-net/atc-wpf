namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

/// <summary>
/// Pictures and captures of an ESP32 sketch that drives an SSD1680 e-paper module with GxEPD2: init, clear screen, a picture with a full
/// refresh and (on black/white panels) a partial refresh, then hibernate.
/// </summary>
/// <remarks>
/// The command bytes are the library's, restated from §LIB.GXEPD2 (no library code is copied). Pictures are drawn in the landscape
/// orientation of GxEPD2 <c>setRotation(1)</c> (the control's default rotation 90) and sent native, as GxEPD2 does (§SSD1680.8).
/// </remarks>
public static class EPaperDisplayCaptures
{
    private const int TextScale = 2;
    private const int Margin = 8;

    /// <summary>
    /// Draws a picture in the native RAM layout: text lines in black, twice the size of the 5 x 8 font, and on a black/white/red panel
    /// a red bar under them.
    /// </summary>
    /// <param name="columns">The native pixel columns of the buffer (the short side; 128 for both sizes).</param>
    /// <param name="visibleColumns">The native columns the glass shows (122 on the 2.13" panels); the picture is drawn into them only.</param>
    /// <param name="rows">The native pixel rows (the long side).</param>
    /// <param name="hasRed">Whether to draw the red bar.</param>
    /// <param name="lines">The text lines.</param>
    /// <returns>The black plane (0 = black) and the red plane (1 = red), <c>ceil(columns / 8)</c> bytes per row.</returns>
    public static (byte[] Black, byte[] Red) CreatePicture(
        int columns,
        int visibleColumns,
        int rows,
        bool hasRed,
        IReadOnlyList<string> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);

        var scratch = new MonochromeFrameBuffer(MonochromeFrameBuffer.MaxWidth, MonochromeFrameBuffer.MaxHeight);
        for (var i = 0; i < lines.Count; i++)
        {
            scratch.DrawText(0, i * 10, lines[i]);
        }

        var bytesPerRow = (columns + 7) / 8;
        var black = new byte[bytesPerRow * rows];
        var red = new byte[black.Length];
        Array.Fill(black, (byte)0xFF);

        // setRotation(1): the user pixel (ux, uy) of the rows x visibleColumns landscape is native (visibleColumns - 1 - uy, ux):
        // GxEPD2 rotates against the visible width its graphics base class was constructed with (§SSD1680.8); padding bits stay white.
        var redTop = Margin + (lines.Count * 10 * TextScale) + 4;
        for (var uy = 0; uy < visibleColumns; uy++)
        {
            for (var ux = 0; ux < rows; ux++)
            {
                var sx = (ux - Margin) / TextScale;
                var sy = (uy - Margin) / TextScale;
                var isBlack = ux >= Margin &&
                              uy >= Margin &&
                              sx < MonochromeFrameBuffer.MaxWidth &&
                              sy < MonochromeFrameBuffer.MaxHeight &&
                              scratch.GetPixel(sx, sy);
                var isRed = hasRed &&
                            uy >= redTop &&
                            uy < redTop + 12 &&
                            ux >= Margin &&
                            ux < rows - Margin;
                var nx = visibleColumns - 1 - uy;
                var index = (ux * bytesPerRow) + (nx / 8);
                var bit = (byte)(0x80 >> (nx % 8));
                if (isBlack)
                {
                    black[index] &= (byte)~bit;
                }

                if (isRed)
                {
                    red[index] |= bit;
                }
            }
        }

        return (black, red);
    }

    /// <summary>
    /// Creates the 4-wire SPI capture GxEPD2 sends for a preset: the command bytes (D/C low) and their data bytes (D/C high).
    /// </summary>
    /// <param name="preset">The module.</param>
    /// <returns>The steps in the order they are sent.</returns>
    public static IReadOnlyList<GraphicDisplayCaptureStep> CreateGxEpd2Capture(
        EPaperDisplayPreset preset)
    {
        ArgumentNullException.ThrowIfNull(preset);

        var steps = new List<GraphicDisplayCaptureStep>();
        var planeLength = ((preset.Columns + 7) / 8) * preset.Rows;

        AddInit(steps, preset);

        // _PowerOn: E0 on the black/white classes, F8 on the colour classes; neither refreshes.
        Command(steps, 0x22, preset.HasRed ? (byte)0xF8 : (byte)0xE0);
        Command(steps, 0x20);

        // clearScreen(): 0x26 then 0x24 (white, and no colour on the wire), then a full refresh.
        var white = Enumerable.Repeat((byte)0xFF, planeLength).ToArray();
        AddWindow(steps, preset);
        Command(steps, 0x26, preset.HasRed ? new byte[planeLength] : white);
        AddWindow(steps, preset);
        Command(steps, 0x24, white);
        Command(steps, 0x22, 0xF7);
        Command(steps, 0x20);

        // The picture: writeImage to 0x24 (and the colour plane, already 1 = red on the wire), then a full refresh.
        var (black, red) = CreatePicture(
            preset.Columns,
            preset.VisibleColumns,
            preset.Rows,
            preset.HasRed,
            ["Hello ESP32", preset.LibraryClass]);
        AddWindow(steps, preset);
        Command(steps, 0x24, black);
        if (preset.HasRed)
        {
            AddWindow(steps, preset);
            Command(steps, 0x26, red);
        }

        Command(steps, 0x22, 0xF7);
        Command(steps, 0x20);

        // A partial refresh on the black/white panels (FC: display mode 2, no flash).
        if (!preset.HasRed)
        {
            var (partial, _) = CreatePicture(
                preset.Columns,
                preset.VisibleColumns,
                preset.Rows,
                hasRed: false,
                ["Hello ESP32", preset.LibraryClass, "partial refresh"]);
            AddWindow(steps, preset);
            Command(steps, 0x24, partial);
            Command(steps, 0x22, 0xFC);
            Command(steps, 0x20);
        }

        // hibernate(): _PowerOff, then deep sleep.
        Command(steps, 0x22, 0x83);
        Command(steps, 0x20);
        Command(steps, 0x10, 0x01);
        return steps;
    }

    // _InitDisplay (§LIB.GXEPD2.1 to .3); the 2.9" classes send 296 MUX lines, the 2.13" classes 250.
    private static void AddInit(
        List<GraphicDisplayCaptureStep> steps,
        EPaperDisplayPreset preset)
    {
        var lastRow = preset.Rows - 1;
        Command(steps, 0x12);
        Command(steps, 0x01, (byte)(lastRow & 0xFF), (byte)(lastRow >> 8), 0x00);
        if (preset.VisibleColumns == preset.Columns || preset.HasRed)
        {
            Command(steps, 0x11, 0x03);
        }

        Command(steps, 0x3C, 0x05);
        if (preset.HasRed)
        {
            Command(steps, 0x18, 0x80);
            Command(steps, 0x21, 0x00, 0x80);
        }
        else
        {
            Command(steps, 0x21, 0x00, 0x80);
            Command(steps, 0x18, 0x80);
        }

        AddWindow(steps, preset);
    }

    // _setPartialRamArea of the whole panel (§SSD1680.5).
    private static void AddWindow(
        List<GraphicDisplayCaptureStep> steps,
        EPaperDisplayPreset preset)
    {
        var lastRow = preset.Rows - 1;
        Command(steps, 0x11, 0x03);
        Command(steps, 0x44, 0x00, 0x0F);
        Command(steps, 0x45, 0x00, 0x00, (byte)(lastRow & 0xFF), (byte)(lastRow >> 8));
        Command(steps, 0x4E, 0x00);
        Command(steps, 0x4F, 0x00, 0x00);
    }

    private static void Command(
        List<GraphicDisplayCaptureStep> steps,
        byte command,
        params byte[] data)
    {
        steps.Add(new GraphicDisplayCaptureStep(IsData: false, [command]));
        if (data.Length > 0)
        {
            steps.Add(new GraphicDisplayCaptureStep(IsData: true, data));
        }
    }
}