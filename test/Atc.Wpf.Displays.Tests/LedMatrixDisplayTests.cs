namespace Atc.Wpf.Displays.Tests;

public sealed class LedMatrixDisplayTests
{
    private const int RenderWidth = 640;
    private const int RenderHeight = 160;
    private const int Modules = 4;
    private const int Columns = Modules * 8;
    private const int Rows = 8;
    private static readonly Color PanelColor = Color.FromRgb(0x00, 0x00, 0x00);
    private static readonly Color LitColor = Color.FromRgb(0xFF, 0xFF, 0xFF);

    [StaFact]
    public void Fc16_FirstDeviceIsTheRightmostModule()
    {
        // ref: §MAX7219.7 (module 0 = rightmost, nearest DIN; FC-16 visual: digit r, bit 7 - x)
        // ref: §P3.X item 7 (FC-16 module 0 digit 0 bit 0 = row 0 of the rightmost column of the display;
        //      module 0 digit 0 bit 7 = row 0 of the leftmost column of module 0)
        var sut = CreateSut(Max7219MatrixWiring.Fc16);
        PowerUp(sut.Buffer);

        sut.Buffer.SetDigit(0, 0, 0x81);

        var lit = RenderLitDots(sut);
        Assert.Equal([(31, 0), (24, 0)], lit.OrderByDescending(d => d.X));
    }

    [StaTheory]
    [InlineData(Max7219MatrixWiring.Fc16, 0, 0x01, 31, 0)]
    [InlineData(Max7219MatrixWiring.Parola, 0, 0x01, 24, 0)]
    [InlineData(Max7219MatrixWiring.ICStation, 7, 0x01, 24, 0)]
    [InlineData(Max7219MatrixWiring.ICStation, 0, 0x01, 24, 7)]
    [InlineData(Max7219MatrixWiring.Generic, 0, 0x80, 31, 0)]
    [InlineData(Max7219MatrixWiring.Generic, 0, 0x01, 31, 7)]
    public void EachWiring_MapsBit0ToTheNotedCorner(
        Max7219MatrixWiring wiring,
        int digit,
        byte value,
        int expectedX,
        int expectedY)
    {
        // ref: §P3.X item 7 (Parola digit 0 bit 0 = row 0, leftmost column of the module; ICStation digit 7 bit 0 = row 0,
        //      leftmost column; Generic digit 0 bit 7 = row 0, rightmost column of the module)
        // ref: §MAX7219.7 visual table (Generic: digit 7 - x, bit 7 - r, so digit 0 bit 0 = x 7, r 7)
        // Module 0 is the rightmost module: display columns 24..31 of a 4-module chain.
        var sut = CreateSut(wiring);
        PowerUp(sut.Buffer);

        sut.Buffer.SetDigit(0, digit, value);

        var lit = RenderLitDots(sut);
        Assert.Equal([(expectedX, expectedY)], lit);
    }

    [StaFact]
    public void Intensity_MapsToBrightness()
    {
        // ref: §MAX7219.1 (intensity 0..15), §MAX7219.2 (power-on intensity 0), §MAX7219.5 (n = 0 is dim but not off;
        //      display test overrides intensity)
        var sut = new LedMatrixDisplay();

        Assert.Equal(0.25, sut.Brightness, 6);

        sut.Buffer.SetIntensity(0, 15);
        Assert.Equal(1.0, sut.Brightness, 6);

        sut.Buffer.SetIntensity(0, 7);
        Assert.Equal(0.25 + (0.75 * 7 / 15.0), sut.Brightness, 6);

        sut.Buffer.SetDisplayTest(0, isDisplayTest: true);
        Assert.Equal(1.0, sut.Brightness, 6);
    }

    [StaFact]
    public void Intensity_ChangedOnAWorkerThread_ReachesBrightness()
    {
        var sut = new LedMatrixDisplay();
        var buffer = sut.Buffer;

        Task.Run(() => buffer.SetIntensity(0, 15)).Wait();
        Pump();

        Assert.Equal(1.0, sut.Brightness, 6);
    }

    [StaFact]
    public void Shutdown_IsDark()
    {
        // ref: §MAX7219.5 (shutdown: display blank, registers unchanged), §MAX7219.2 (data remains unaltered)
        var sut = CreateSut(Max7219MatrixWiring.Fc16);
        PowerUp(sut.Buffer);
        FillDigits(sut.Buffer, 0xFF);
        Assert.Equal(Columns * Rows, RenderLitDots(sut).Count);

        for (var device = 0; device < Modules; device++)
        {
            sut.Buffer.SetShutdown(device, isShutdown: true);
        }

        Assert.Empty(RenderLitDots(sut));

        sut.Buffer.SetShutdown(2, isShutdown: false);

        var lit = RenderLitDots(sut);
        Assert.Equal(64, lit.Count);
        Assert.All(lit, d => Assert.InRange(d.X, 8, 15));
    }

    [StaFact]
    public void DisplayTest_LightsAll()
    {
        // ref: §MAX7219.5 (display test: every LED lit; overrides shutdown, scan limit, decode and intensity)
        // The buffer is in the power-on state: shut down, scan limit 0, blank digits (§MAX7219.2).
        var sut = CreateSut(Max7219MatrixWiring.Fc16);
        Assert.Empty(RenderLitDots(sut));

        for (var device = 0; device < Modules; device++)
        {
            sut.Buffer.SetDisplayTest(device, isDisplayTest: true);
        }

        Assert.Equal(Columns * Rows, RenderLitDots(sut).Count);
        Assert.Equal(1.0, sut.Brightness, 6);
    }

    [StaFact]
    public void ScanLimit_HidesDigitsBeyond()
    {
        // ref: §MAX7219.5 (a digit is displayed only if its index <= scan limit); FC-16 digit = row (§MAX7219.7)
        var sut = CreateSut(Max7219MatrixWiring.Fc16);
        PowerUp(sut.Buffer);
        FillDigits(sut.Buffer, 0xFF);

        sut.Buffer.SetScanLimit(0, 2);

        var lit = RenderLitDots(sut);
        Assert.Equal((3 * 64) + (3 * 8), lit.Count);
        Assert.DoesNotContain(lit, d => d.X >= 24 && d.Y > 2);
    }

    [StaFact]
    public void ScanLimit_OnGenericWiring_HidesColumns()
    {
        // ref: §MAX7219.7 Generic: digit 7 - x, so digits 0..2 are module columns 7, 6, 5 (display columns 31, 30, 29 of device 0);
        // ref: §MAX7219.5 (digits above the scan limit are dark)
        var sut = CreateSut(Max7219MatrixWiring.Generic);
        PowerUp(sut.Buffer);
        FillDigits(sut.Buffer, 0xFF);

        sut.Buffer.SetScanLimit(0, 2);

        var lit = RenderLitDots(sut);
        Assert.Equal((3 * 64) + (3 * 8), lit.Count);
        Assert.DoesNotContain(lit, d => d.X is >= 24 and < 29);
        Assert.Contains((29, 7), lit);
    }

    [StaFact]
    public void DecodeMode_ShowsTheCodeBPattern()
    {
        // ref: §MAX7219.4 (Code B '1' = B C = 0x30; D7 = decimal point, independent of the decoder)
        // ref: §MAX7219.7 FC-16: digit r, bit 7 - x -> bits 5 and 4 are module columns 2 and 3, bit 7 column 0;
        //      device 0 is the rightmost module (display columns 24..31).
        var sut = CreateSut(Max7219MatrixWiring.Fc16);
        PowerUp(sut.Buffer);

        // Decode digit 0 only: a decoded digit holding 0 would show Code B '0' (0x7E).
        sut.Buffer.SetDecodeMode(0, 0x01);

        sut.Buffer.SetDigit(0, 0, 0x01);

        Assert.Equal([(26, 0), (27, 0)], RenderLitDots(sut));

        sut.Buffer.SetDigit(0, 0, 0x81);

        Assert.Equal([(24, 0), (26, 0), (27, 0)], RenderLitDots(sut));
    }

    [StaTheory]
    [InlineData(Max7219MatrixWiring.Fc16)]
    [InlineData(Max7219MatrixWiring.Generic)]
    public void Text_WritesTheGlyphThroughTheWiring(Max7219MatrixWiring wiring)
    {
        // 'A' in the A00 ROM: row 0 is 0x0E (.XXX.), so display columns 1..3 of row 0 are on, in the leftmost module (device 3).
        // ref: §MAX7219.7 FC-16: digit r, bit 7 - x -> digit 0 bits 6, 5, 4 = 0x70.
        //      Generic: digit 7 - x, bit 7 - r -> digits 6, 5, 4 have bit 7 set.
        // ref: §LIB.MDMAX72XX.1 (begin(): test off, scan limit 7, intensity 7, decode off, shutdown off)
        var sut = new LedMatrixDisplay { Wiring = wiring };

        sut.Text = "A";

        var buffer = sut.Buffer;
        Assert.False(buffer.IsShutdown(3));
        Assert.Equal(7, buffer.GetScanLimit(3));
        Assert.Equal(7, buffer.GetIntensity(3));
        if (wiring == Max7219MatrixWiring.Fc16)
        {
            Assert.Equal(0x70, buffer.GetDigit(3, 0));
        }
        else
        {
            Assert.Equal(0x00, buffer.GetDigit(3, 7) & 0x80);
            Assert.Equal(0x80, buffer.GetDigit(3, 6) & 0x80);
            Assert.Equal(0x80, buffer.GetDigit(3, 5) & 0x80);
            Assert.Equal(0x80, buffer.GetDigit(3, 4) & 0x80);
            Assert.Equal(0x00, buffer.GetDigit(3, 3) & 0x80);
        }

        var lit = RenderLitDots(CopyLook(sut));
        Assert.Contains((1, 0), lit);
        Assert.Contains((3, 0), lit);
        Assert.DoesNotContain((0, 0), lit);
    }

    [StaFact]
    public void ModuleCount_ResizesTheBufferAndIsCoerced()
    {
        var sut = new LedMatrixDisplay { ModuleCount = 2 };

        Assert.Equal(2, sut.Buffer.DeviceCount);

        sut.ModuleCount = 20;

        Assert.Equal(8, sut.ModuleCount);
        Assert.Equal(8, sut.Buffer.DeviceCount);
    }

    [StaFact]
    public void Buffer_SetToNull_ReturnsToTheOwnBuffer()
    {
        var sut = new LedMatrixDisplay();
        var own = sut.Buffer;
        sut.Buffer = new Max7219ChainBuffer(2);

        sut.Buffer = null!;

        Assert.Same(own, sut.Buffer);
    }

    [StaFact]
    public void OldBuffer_ChangesNoLongerReachTheControl()
    {
        var sut = new LedMatrixDisplay();
        var old = sut.Buffer;
        var buffer = new Max7219ChainBuffer();
        buffer.SetIntensity(0, 15);
        sut.Buffer = buffer;

        old.SetIntensity(0, 0);

        Assert.Equal(1.0, sut.Brightness, 6);
    }

    [StaFact]
    public void AutomationPeer_NamesTheResolution()
    {
        var sut = new LedMatrixDisplay { ModuleCount = 4 };

        var peer = UIElementAutomationPeer.CreatePeerForElement(sut);

        Assert.IsType<LedMatrixDisplayAutomationPeer>(peer);
        Assert.Equal("LED matrix 32x8", peer.GetName());
    }

    private static LedMatrixDisplay CreateSut(Max7219MatrixWiring wiring)
        => new()
        {
            Wiring = wiring,
            ShowBezel = false,
            GhostOpacity = 0,
            GlowRadius = 0,
            PanelBrush = new SolidColorBrush(PanelColor),
            LitDotBrush = new SolidColorBrush(LitColor),
        };

    private static LedMatrixDisplay CopyLook(LedMatrixDisplay sut)
    {
        sut.ShowBezel = false;
        sut.GhostOpacity = 0;
        sut.GlowRadius = 0;
        sut.PanelBrush = new SolidColorBrush(PanelColor);
        sut.LitDotBrush = new SolidColorBrush(LitColor);
        return sut;
    }

    private static void PowerUp(Max7219ChainBuffer buffer)
    {
        for (var device = 0; device < buffer.DeviceCount; device++)
        {
            buffer.SetShutdown(device, isShutdown: false);
            buffer.SetScanLimit(device, 7);
            buffer.SetIntensity(device, 15);
        }
    }

    private static void FillDigits(
        Max7219ChainBuffer buffer,
        byte value)
    {
        for (var device = 0; device < buffer.DeviceCount; device++)
        {
            for (var digit = 0; digit < 8; digit++)
            {
                buffer.SetDigit(device, digit, value);
            }
        }
    }

    private static void Pump()
        => Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);

    private static List<(int X, int Y)> RenderLitDots(LedMatrixDisplay sut)
    {
        var pixels = Render(sut);
        var layout = DotMatrixLayout.Create(sut.CurrentGeometry, sut.ShowBezel, new Size(RenderWidth, RenderHeight), sut.Stretch);
        var lit = new List<(int X, int Y)>();
        for (var y = 0; y < layout.DotRows; y++)
        {
            for (var x = 0; x < layout.DotColumns; x++)
            {
                var rect = layout.GetDotRect(x, y);
                var centre = new Point(rect.X + (rect.Width / 2), rect.Y + (rect.Height / 2));
                var color = pixels(centre);
                if (color != PanelColor)
                {
                    lit.Add((x, y));
                }
            }
        }

        return lit;
    }

    private static Func<Point, Color> Render(FrameworkElement sut)
    {
        // Buffer changes are drawn by a redraw queued on the dispatcher.
        Pump();
        sut.Width = RenderWidth;
        sut.Height = RenderHeight;
        sut.Measure(new Size(RenderWidth, RenderHeight));
        sut.Arrange(new Rect(0, 0, RenderWidth, RenderHeight));
        sut.UpdateLayout();

        var bitmap = new RenderTargetBitmap(RenderWidth, RenderHeight, 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(sut);
        var bytes = new byte[RenderWidth * RenderHeight * 4];
        bitmap.CopyPixels(bytes, RenderWidth * 4, 0);

        return point =>
        {
            var i = (((int)point.Y * RenderWidth) + (int)point.X) * 4;
            var alpha = bytes[i + 3];
            return alpha == 0
                ? Colors.Transparent
                : Color.FromArgb(
                    alpha,
                    (byte)System.Math.Min(255, bytes[i + 2] * 255 / alpha),
                    (byte)System.Math.Min(255, bytes[i + 1] * 255 / alpha),
                    (byte)System.Math.Min(255, bytes[i] * 255 / alpha));
        };
    }
}