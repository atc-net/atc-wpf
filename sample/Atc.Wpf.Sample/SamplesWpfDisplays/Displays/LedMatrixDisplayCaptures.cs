namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

/// <summary>
/// Captures of an ESP32 sketch that shows "Hello ESP32" on a chain of MAX7219 8 x 8 LED matrix modules, as MD_MAX72XX, LedControl and
/// U8g2 send it: one latch (CS) frame per step, the first pair for the farthest (leftmost) device (§MAX7219.6).
/// </summary>
/// <remarks>
/// The register values are the libraries', restated from §LIB.MDMAX72XX, §LIB.LEDCONTROL and §LIB.U8G2MAX7219 (no library code is copied).
/// </remarks>
public static class LedMatrixDisplayCaptures
{
    private const int ModuleSize = 8;
    private const string SceneText = "Hello ESP32";

    /// <summary>
    /// Creates the capture of a library.
    /// </summary>
    /// <param name="library">The library.</param>
    /// <param name="moduleCount">The number of modules, 1 to 8.</param>
    /// <param name="wiring">The module wiring (used by MD_MAX72XX; LedControl and U8g2 always send the FC-16 orientation).</param>
    /// <returns>The latch frames in the order they are sent.</returns>
    public static IReadOnlyList<byte[]> CreateCapture(
        LedMatrixReplayLibrary library,
        int moduleCount,
        Max7219MatrixWiring wiring)
        => library switch
        {
            LedMatrixReplayLibrary.LedControl => CreateLedControl(moduleCount),
            LedMatrixReplayLibrary.U8g2 => CreateU8g2(moduleCount),
            _ => CreateMdMax72xx(moduleCount, wiring),
        };

    /// <summary>
    /// Computes the digit registers that show text on a chain: <c>[device][digit]</c>, device 0 the rightmost module (§MAX7219.7).
    /// </summary>
    /// <param name="text">The text, drawn with the 5 x 8 font from the left.</param>
    /// <param name="moduleCount">The number of modules.</param>
    /// <param name="wiring">The module wiring.</param>
    /// <param name="offset">The scroll offset in pixels; the text repeats.</param>
    /// <returns>The registers.</returns>
    public static byte[][] CreateRegisters(
        string text,
        int moduleCount,
        Max7219MatrixWiring wiring,
        int offset = 0)
    {
        ArgumentNullException.ThrowIfNull(text);

        var scratch = new MonochromeFrameBuffer(MonochromeFrameBuffer.MaxWidth, ModuleSize);
        scratch.DrawText(0, 0, text);
        var textWidth = System.Math.Clamp(text.Length * 6, 1, MonochromeFrameBuffer.MaxWidth);
        var width = moduleCount * ModuleSize;
        var registers = new byte[moduleCount][];
        for (var device = 0; device < moduleCount; device++)
        {
            registers[device] = new byte[ModuleSize];
        }

        for (var column = 0; column < width; column++)
        {
            var source = offset == 0 ? column : (column + offset) % textWidth;
            if (source >= MonochromeFrameBuffer.MaxWidth)
            {
                continue;
            }

            var device = moduleCount - 1 - (column / ModuleSize);
            var x = column % ModuleSize;
            for (var r = 0; r < ModuleSize; r++)
            {
                if (!scratch.GetPixel(source, r))
                {
                    continue;
                }

                // ref: §MAX7219.7, the visual table: module column x (0 = leftmost LED), row r (0 = top).
                var (digit, bit) = wiring switch
                {
                    Max7219MatrixWiring.Generic => (7 - x, 7 - r),
                    Max7219MatrixWiring.Parola => (r, x),
                    Max7219MatrixWiring.ICStation => (7 - r, x),
                    _ => (r, 7 - x),
                };
                registers[device][digit] |= (byte)(1 << bit);
            }
        }

        return registers;
    }

    // A full-length frame with the same pair for every device (MD_MAX72XX control(), U8g2 init lines).
    private static byte[] AllDevices(
        int moduleCount,
        byte address,
        byte value)
        => [.. Enumerable.Repeat(new[] { address, value }, moduleCount).SelectMany(pair => pair)];

    // A full-length frame for one device, no-ops for the others (LedControl spiTransfer); the device's pair is at (N - 1 - device) * 2.
    private static byte[] OneDevice(
        int moduleCount,
        int device,
        byte address,
        byte value)
    {
        var frame = new byte[moduleCount * 2];
        frame[(moduleCount - 1 - device) * 2] = address;
        frame[((moduleCount - 1 - device) * 2) + 1] = value;
        return frame;
    }

    // One frame per digit carrying a pair for every device, the farthest first.
    private static IEnumerable<byte[]> DigitRows(byte[][] registers)
    {
        var moduleCount = registers.Length;
        for (var digit = 0; digit < ModuleSize; digit++)
        {
            var frame = new byte[moduleCount * 2];
            for (var i = 0; i < moduleCount; i++)
            {
                var device = moduleCount - 1 - i;
                frame[i * 2] = (byte)(digit + 1);
                frame[(i * 2) + 1] = registers[device][digit];
            }

            yield return frame;
        }
    }

    // ref: §LIB.MDMAX72XX.1: begin() (test off, scan limit 7, intensity 7, decode off, clear, shutdown off), then flushBufferAll().
    private static List<byte[]> CreateMdMax72xx(
        int moduleCount,
        Max7219MatrixWiring wiring)
    {
        var frames = new List<byte[]>
        {
            AllDevices(moduleCount, 0x0F, 0x00),
            AllDevices(moduleCount, 0x0B, 0x07),
            AllDevices(moduleCount, 0x0A, 0x07),
            AllDevices(moduleCount, 0x09, 0x00),
        };

        for (var row = 0; row < ModuleSize; row++)
        {
            frames.Add(AllDevices(moduleCount, (byte)(row + 1), 0x00));
        }

        frames.Add(AllDevices(moduleCount, 0x0C, 0x01));
        frames.AddRange(DigitRows(CreateRegisters(SceneText, moduleCount, wiring)));
        return frames;
    }

    // ref: §LIB.LEDCONTROL: per device the constructor frames (test off, scan limit 7, decode off, clear, shutdown on), then the
    // sketch's shutdown(addr, false), setIntensity(addr, 8) and setRow(addr, row, value) per row.
    private static List<byte[]> CreateLedControl(int moduleCount)
    {
        var frames = new List<byte[]>();
        for (var device = 0; device < moduleCount; device++)
        {
            frames.Add(OneDevice(moduleCount, device, 0x0F, 0x00));
            frames.Add(OneDevice(moduleCount, device, 0x0B, 0x07));
            frames.Add(OneDevice(moduleCount, device, 0x09, 0x00));
            for (var row = 0; row < ModuleSize; row++)
            {
                frames.Add(OneDevice(moduleCount, device, (byte)(row + 1), 0x00));
            }

            frames.Add(OneDevice(moduleCount, device, 0x0C, 0x00));
        }

        var registers = CreateRegisters(SceneText, moduleCount, Max7219MatrixWiring.Fc16);
        for (var device = 0; device < moduleCount; device++)
        {
            frames.Add(OneDevice(moduleCount, device, 0x0C, 0x01));
            frames.Add(OneDevice(moduleCount, device, 0x0A, 0x08));
            for (var row = 0; row < ModuleSize; row++)
            {
                frames.Add(OneDevice(moduleCount, device, (byte)(row + 1), registers[device][row]));
            }
        }

        return frames;
    }

    // ref: §LIB.U8G2MAX7219: init (test off, shutdown, decode off, intensity 10, scan limit 7), power save off, then one frame per
    // tile row with tile 0 (the leftmost module) first.
    private static List<byte[]> CreateU8g2(int moduleCount)
    {
        var frames = new List<byte[]>
        {
            AllDevices(moduleCount, 0x0F, 0x00),
            AllDevices(moduleCount, 0x0C, 0x00),
            AllDevices(moduleCount, 0x09, 0x00),
            AllDevices(moduleCount, 0x0A, 0x0A),
            AllDevices(moduleCount, 0x0B, 0x07),
            AllDevices(moduleCount, 0x0C, 0x01),
        };

        frames.AddRange(DigitRows(CreateRegisters(SceneText, moduleCount, Max7219MatrixWiring.Fc16)));
        return frames;
    }
}