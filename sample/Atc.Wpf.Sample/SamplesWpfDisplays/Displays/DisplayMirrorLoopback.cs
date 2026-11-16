namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

/// <summary>
/// Generates display mirroring traffic in-process, as a device would send it over a serial port, and feeds it to a receiver as a byte stream:
/// an OLED frame stream (display id 1), a colour frame stream (id 2), HD44780 traffic through a PCF8574 I2C backpack (id 3),
/// an e-paper refreshed every few seconds (id 4), scrolling text on a MAX7219 LED matrix (id 5) and ST7920 text plus graphics sent
/// serial-framed, as the U8g2 tee sends it (id 6).
/// Each display is described once a second, and now and then a few noise bytes and a damaged packet show the resync and CRC counters.
/// </summary>
public sealed class DisplayMirrorLoopback : IDisposable
{
    /// <summary>The display id of the OLED.</summary>
    public const byte OledId = 1;

    /// <summary>The display id of the colour panel.</summary>
    public const byte ColorId = 2;

    /// <summary>The display id of the character LCD.</summary>
    public const byte LcdId = 3;

    /// <summary>The display id of the e-paper.</summary>
    public const byte EPaperId = 4;

    /// <summary>The display id of the LED matrix.</summary>
    public const byte MatrixId = 5;

    /// <summary>The display id of the ST7920 graphic LCD.</summary>
    public const byte GraphicLcdId = 6;

    private const int FramesPerSecond = 20;
    private const int OledWidth = 128;
    private const int OledHeight = 64;
    private const int ColorWidth = 128;
    private const int ColorHeight = 160;
    private const int LcdColumns = 16;
    private const int LcdRows = 2;
    private const int ChunkLength = 4096;
    private const int BallRadius = 6;
    private const byte LcdAddress = 0x27;
    private const byte PinRs = 0x01;
    private const byte PinEnable = 0x04;
    private const byte PinBacklight = 0x08;
    private const string MatrixText = "Atc.Wpf mirror * ";

    // A few bytes that are no packet, including a magic byte with a wrong second byte.
    private static readonly byte[] Noise = [0x00, 0x55, 0xA7, 0x13, 0xFF, 0xA7];
    private static readonly byte[] LcdRowAddresses = [0x00, 0x40];

    private readonly DisplayMirrorReceiver receiver;
    private readonly Action<Exception>? onError;
    private readonly MonochromeFrameBuffer oledScratch = new(OledWidth, OledHeight);
    private readonly MonochromeFrameBuffer graphicLcdScratch = new(128, 32);
    private readonly byte[] colorPixels = new byte[ColorWidth * ColorHeight * 2];
    private readonly List<byte> stream = [];
    private CancellationTokenSource? cancellation;
    private Task? loop;
    private int tick;
    private double ballX = 30;
    private double ballY = 20;
    private double ballVelocityX = 2.3;
    private double ballVelocityY = 1.6;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayMirrorLoopback"/> class.
    /// </summary>
    /// <param name="receiver">The receiver to feed.</param>
    /// <param name="onError">Called (on the generator's thread) when the generator stops because of an error.</param>
    public DisplayMirrorLoopback(
        DisplayMirrorReceiver receiver,
        Action<Exception>? onError = null)
    {
        ArgumentNullException.ThrowIfNull(receiver);

        this.receiver = receiver;
        this.onError = onError;
    }

    /// <summary>
    /// Gets the descriptions the loopback sends.
    /// </summary>
    public static IReadOnlyList<(byte DisplayId, DisplayMirrorDescribe Describe)> Displays { get; } =
    [
        (OledId, new DisplayMirrorDescribe(DisplayMirrorDisplayKind.MonoOled, DisplayMirrorChip.Ssd1306, OledWidth, OledHeight, "Loopback OLED")),
        (ColorId, new DisplayMirrorDescribe(DisplayMirrorDisplayKind.ColorPanel, DisplayMirrorChip.St7735, ColorWidth, ColorHeight, "Loopback TFT")),
        (LcdId, new DisplayMirrorDescribe(DisplayMirrorDisplayKind.CharacterLcd, DisplayMirrorChip.Hd44780, LcdColumns, LcdRows, "Loopback LCD")),
        (EPaperId, new DisplayMirrorDescribe(DisplayMirrorDisplayKind.EPaper, DisplayMirrorChip.Ssd1680, DisplayMirrorLoopbackTraffic.EPaperWidth, DisplayMirrorLoopbackTraffic.EPaperHeight, EPaperDisplayPreset.All[3].Name)),
        (MatrixId, new DisplayMirrorDescribe(DisplayMirrorDisplayKind.LedMatrix, DisplayMirrorChip.Max7219, DisplayMirrorLoopbackTraffic.MatrixModules * 8, 8, LedMatrixDisplayPreset.All[1].Name)),
        (GraphicLcdId, new DisplayMirrorDescribe(DisplayMirrorDisplayKind.GraphicLcd, DisplayMirrorChip.St7920, GraphicLcdDisplay.PixelColumns, GraphicLcdDisplay.PixelRows, GraphicLcdDisplayPreset.All[0].Name)),
    ];

    /// <summary>
    /// Gets a value indicating whether the generator runs.
    /// </summary>
    public bool IsRunning => loop is { IsCompleted: false };

    /// <summary>
    /// Starts the generator on a background task.
    /// </summary>
    public void Start()
    {
        if (IsRunning)
        {
            return;
        }

        cancellation?.Dispose();
        cancellation = new CancellationTokenSource();
        var token = cancellation.Token;
        loop = Task.Run(() => RunAsync(token), CancellationToken.None);
    }

    /// <summary>
    /// Stops the generator and waits (up to a second) until it has fed its last bytes, so a new source can start on a clean stream.
    /// </summary>
    /// <returns><see langword="true"/> when the generator is not running any more.</returns>
    public bool Stop()
    {
        cancellation?.Cancel();

        // The loop never blocks on the UI thread (the targets hand their events over with BeginInvoke), so waiting here cannot deadlock.
        // It never faults either: its errors go to the error callback.
        return loop is null ||
               loop.Wait(TimeSpan.FromSeconds(1), CancellationToken.None);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Stop();
        cancellation?.Dispose();
        cancellation = null;
    }

    /// <summary>
    /// Builds the bytes of one tick (1/20 s) of traffic; the first tick only describes every display.
    /// </summary>
    /// <returns>The stream bytes.</returns>
    public byte[] NextTick()
    {
        stream.Clear();

        // As a device does: describe the displays now and then, so a receiver that starts later still learns them.
        if (tick % FramesPerSecond == 0)
        {
            foreach (var (displayId, describe) in Displays)
            {
                Add(displayId, DisplayMirrorMessageType.Describe, describe.ToPayload());
            }

            // The first tick only describes: the receiving side creates its targets on the UI thread, a moment later.
            if (tick == 0)
            {
                tick++;
                return [.. stream];
            }
        }

        // The LCD is set up again once a second (a moment after the Describe, so the receiving side has had time to create its target).
        if (tick % FramesPerSecond == 2)
        {
            Add(LcdId, DisplayMirrorMessageType.I2cWrite, CreateLcdInit());
        }

        var oledPacketStart = stream.Count;
        Add(OledId, DisplayMirrorMessageType.MonoFrame, CreateOledFrame());
        if (tick % (FramesPerSecond * 5) == FramesPerSecond * 4)
        {
            // A damaged packet: one payload byte flipped, so its CRC fails and it is dropped.
            stream[oledPacketStart + DisplayMirrorProtocol.HeaderLength + 8] ^= 0x5A;
        }

        Add(ColorId, DisplayMirrorMessageType.ColorFrame, CreateColorFrame());

        AddFamilyTraffic();

        if (tick % 4 == 0)
        {
            Add(LcdId, DisplayMirrorMessageType.I2cWrite, CreateLcdContent());
        }

        if (tick % (FramesPerSecond * 3) == FramesPerSecond)
        {
            stream.AddRange(Noise);
        }

        tick++;
        return [.. stream];
    }

    private static void AddNibble(
        List<byte> bytes,
        int nibble,
        bool isData)
    {
        var pins = (byte)((nibble << 4) | PinBacklight);
        if (isData)
        {
            pins |= PinRs;
        }

        // As LiquidCrystal_I2C's write4bits: the pins, then EN high and EN low (the falling edge latches the nibble).
        bytes.Add(pins);
        bytes.Add((byte)(pins | PinEnable));
        bytes.Add(pins);
    }

    private static void AddByte(
        List<byte> bytes,
        byte value,
        bool isData)
    {
        AddNibble(bytes, value >> 4, isData);
        AddNibble(bytes, value & 0x0F, isData);
    }

    // LiquidCrystal_I2C begin() without the clear: the 4-bit initialization (it works whatever mode the chip is in),
    // function set 4-bit two lines, display on, entry mode increment.
    private static byte[] CreateLcdInit()
    {
        var bytes = new List<byte> { LcdAddress, PinBacklight };
        foreach (var nibble in new[] { 0x3, 0x3, 0x3, 0x2 })
        {
            AddNibble(bytes, nibble, isData: false);
        }

        AddByte(bytes, 0x28, isData: false);
        AddByte(bytes, 0x0C, isData: false);
        AddByte(bytes, 0x06, isData: false);
        return [.. bytes];
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "An error ends the demo and is reported; it must never fault the task Stop waits for.")]
    private async Task RunAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(1000.0 / FramesPerSecond));
        try
        {
            do
            {
                var bytes = NextTick();

                // A stream arrives in chunks of any size; the receiver keeps a partial packet between them.
                for (var offset = 0; offset < bytes.Length && !cancellationToken.IsCancellationRequested; offset += ChunkLength)
                {
                    receiver.Feed(bytes.AsSpan(offset, System.Math.Min(ChunkLength, bytes.Length - offset)));
                }
            }
            while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false));
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Stopped.
        }
        catch (Exception ex)
        {
            onError?.Invoke(ex);
        }
    }

    private void Add(
        byte displayId,
        DisplayMirrorMessageType type,
        byte[] payload)
        => stream.AddRange(DisplayMirrorProtocol.Encode(displayId, type, payload));

    private byte[] CreateOledFrame()
    {
        ballX += ballVelocityX;
        ballY += ballVelocityY;
        if (ballX < BallRadius + 1 || ballX > OledWidth - BallRadius - 2)
        {
            ballVelocityX = -ballVelocityX;
            ballX = System.Math.Clamp(ballX, BallRadius + 1, OledWidth - BallRadius - 2);
        }

        if (ballY < BallRadius + 1 || ballY > OledHeight - BallRadius - 2)
        {
            ballVelocityY = -ballVelocityY;
            ballY = System.Math.Clamp(ballY, BallRadius + 1, OledHeight - BallRadius - 2);
        }

        using (oledScratch.BeginBatch())
        {
            oledScratch.Clear();
            oledScratch.DrawRectangle(0, 0, OledWidth, OledHeight);
            oledScratch.DrawText(3, 3, tick.ToString(CultureInfo.InvariantCulture));

            var centerX = (int)System.Math.Round(ballX);
            var centerY = (int)System.Math.Round(ballY);
            for (var dy = -BallRadius; dy <= BallRadius; dy++)
            {
                var half = (int)System.Math.Sqrt((BallRadius * BallRadius) - (dy * dy));
                oledScratch.DrawLine(centerX - half, centerY + dy, centerX + half, centerY + dy);
            }
        }

        return DisplayMirrorProtocol.CreateMonoFramePayload(OledWidth, OledHeight, oledScratch.GetFrame());
    }

    // The e-paper, MAX7219 and ST7920 displays of the tick.
    private void AddFamilyTraffic()
    {
        if (tick % FramesPerSecond == 2)
        {
            // The MAX7219 emulator starts shut down (power-on), so the chain is set up as MD_MAX72XX begin() does.
            foreach (var frame in DisplayMirrorLoopbackTraffic.CreateMatrixBegin())
            {
                Add(MatrixId, DisplayMirrorMessageType.Latch, frame);
            }
        }

        // A full refresh every five seconds (it flashes, as a real panel does); never at 20 fps.
        if (tick % (FramesPerSecond * 5) == 3)
        {
            Add(
                EPaperId,
                DisplayMirrorMessageType.EPaperFrame,
                DisplayMirrorLoopbackTraffic.CreateEPaperFrame(
                    ["Atc.Wpf e-paper", string.Create(CultureInfo.InvariantCulture, $"Refresh {tick / (FramesPerSecond * 5)}")],
                    isPartial: false));
        }

        // Matrix text scrolls one LED column every other tick.
        if (tick % 2 == 0)
        {
            foreach (var frame in DisplayMirrorLoopbackTraffic.CreateMatrixScroll(MatrixText, tick / 2))
            {
                Add(MatrixId, DisplayMirrorMessageType.Latch, frame);
            }
        }

        // ST7920: the text (and the display set-up) once a second, the graphics at 10 fps.
        if (tick % FramesPerSecond == 2)
        {
            Add(
                GraphicLcdId,
                DisplayMirrorMessageType.Data,
                DisplayMirrorLoopbackTraffic.CreateLcdText(
                    "Atc.Wpf ST7920",
                    string.Create(CultureInfo.InvariantCulture, $"Seconds {tick / FramesPerSecond,7}")));
        }

        if (tick % 2 == 1)
        {
            Add(GraphicLcdId, DisplayMirrorMessageType.Data, CreateGraphicLcdPicture());
        }
    }

    // A frame and a ball that follows the OLED's ball, in the bottom half of the ST7920 glass.
    private byte[] CreateGraphicLcdPicture()
    {
        using (graphicLcdScratch.BeginBatch())
        {
            graphicLcdScratch.Clear();
            graphicLcdScratch.DrawRectangle(0, 0, 128, 32);
            var centerX = (int)System.Math.Round(ballX);
            var centerY = 4 + (int)System.Math.Round(ballY * 24 / OledHeight);
            graphicLcdScratch.FillRectangle(centerX - 3, centerY - 3, 7, 7);
        }

        return DisplayMirrorLoopbackTraffic.CreateLcdGraphics(graphicLcdScratch);
    }

    // A moving colour gradient with a white ball that follows the OLED's ball, as RGB565 high byte first.
    private byte[] CreateColorFrame()
    {
        var ballCenterX = ballX * ColorWidth / OledWidth;
        var ballCenterY = ballY * ColorHeight / OledHeight;
        const double radius = 10;

        for (var y = 0; y < ColorHeight; y++)
        {
            for (var x = 0; x < ColorWidth; x++)
            {
                int pixel;
                var dx = x - ballCenterX;
                var dy = y - ballCenterY;
                if ((dx * dx) + (dy * dy) <= radius * radius)
                {
                    pixel = 0xFFFF;
                }
                else
                {
                    var red = ((x / 4) + tick) & 0x1F;
                    var green = (y * 63) / (ColorHeight - 1);
                    var blue = (((x + y) / 8) + (tick * 2)) & 0x1F;
                    pixel = (red << 11) | (green << 5) | blue;
                }

                var index = ((y * ColorWidth) + x) * 2;
                colorPixels[index] = (byte)(pixel >> 8);
                colorPixels[index + 1] = (byte)pixel;
            }
        }

        return DisplayMirrorProtocol.CreateColorFramePayload(ColorWidth, ColorHeight, colorPixels);
    }

    // lcd.setCursor(0, row) and lcd.print(line) for both rows, padded to the width so no old character is left.
    private byte[] CreateLcdContent()
    {
        string[] lines =
        [
            "Atc.Wpf mirror",
            string.Create(CultureInfo.InvariantCulture, $"Count {tick / 4,10}"),
        ];

        var bytes = new List<byte> { LcdAddress };
        for (var row = 0; row < LcdRows; row++)
        {
            AddByte(bytes, (byte)(0x80 | LcdRowAddresses[row]), isData: false);
            foreach (var c in lines[row].PadRight(LcdColumns)[..LcdColumns])
            {
                AddByte(bytes, (byte)c, isData: true);
            }
        }

        return [.. bytes];
    }
}