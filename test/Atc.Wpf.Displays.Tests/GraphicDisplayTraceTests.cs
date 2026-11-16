namespace Atc.Wpf.Displays.Tests;

/// <summary>
/// The emulators write an unknown command byte to <see cref="Trace"/> once per distinct value and controller instance (global constraints).
/// </summary>
[Collection(nameof(TraceTestGroup))]
public sealed class GraphicDisplayTraceTests
{
    public static TheoryData<string> Controllers
        => [nameof(Ssd1306Controller), nameof(Ssd1331Controller), nameof(Ssd1351Controller), nameof(Ssd1680Controller), nameof(Max7219Controller), nameof(St7920Controller), nameof(TftController)];

    [Theory]
    [MemberData(nameof(Controllers))]
    public void UnknownByte_SentTwice_IsTracedOnce(string name)
    {
        var (first, _) = UnknownBytes(name);
        using var capture = TraceCapture.Start();
        using var controller = Create(name);

        controller.WriteCommand(first);
        controller.WriteCommands([first, first]);

        Assert.Equal(1, capture.Count(name, Hex(first)));
    }

    [Theory]
    [MemberData(nameof(Controllers))]
    public void TwoDistinctUnknownBytes_AreTracedOnceEach(string name)
    {
        var (first, second) = UnknownBytes(name);
        using var capture = TraceCapture.Start();
        using var controller = Create(name);

        controller.WriteCommands([first, second, first, second]);

        Assert.Equal(1, capture.Count(name, Hex(first)));
        Assert.Equal(1, capture.Count(name, Hex(second)));
    }

    [Theory]
    [MemberData(nameof(Controllers))]
    public void UnknownByte_NewInstance_IsTracedAgain(string name)
    {
        var (first, _) = UnknownBytes(name);
        using var capture = TraceCapture.Start();
        using var controller = Create(name);
        using var other = Create(name);

        controller.WriteCommand(first);
        other.WriteCommand(first);
        controller.WriteCommand(first);

        Assert.Equal(2, capture.Count(name, Hex(first)));
    }

    [Fact]
    public void TftEspiSt7789Init_TracesNothing()
    {
        // ref: §ST7789.2, LIB-TFTESPI ST7789_Init.h (⚠️ derived from the library, not read byte for byte): TFT_eSPI sends the ST7789 vendor
        // commands RAMCTRL 0xB0, PORCTRL 0xB2, GCTRL 0xB7, VCOMS 0xBB, LCMCTRL 0xC0, VDVVRHEN 0xC2, VRHS 0xC3, VDVS 0xC4, FRCTRL2 0xC6,
        // PWCTRL1 0xD0 and the gamma tables 0xE0 / 0xE1 (and 0xB6 in some versions); they are consumed with their data, not traced.
        using var capture = TraceCapture.Start();
        using var controller = new TftController(new ColorFrameBuffer(240, 320), TftChip.St7789);

        controller.WriteCommands([0x11, 0x13]);
        Send(controller, 0x36, 0x00);
        Send(controller, 0xB6, 0x0A, 0x82);
        Send(controller, 0xB0, 0x00, 0xE0);
        Send(controller, 0x3A, 0x55);
        Send(controller, 0xB2, 0x0C, 0x0C, 0x00, 0x33, 0x33);
        Send(controller, 0xB7, 0x35);
        Send(controller, 0xBB, 0x28);
        Send(controller, 0xC0, 0x0C);
        Send(controller, 0xC2, 0x01, 0xFF);
        Send(controller, 0xC3, 0x10);
        Send(controller, 0xC4, 0x20);
        Send(controller, 0xC6, 0x0F);
        Send(controller, 0xD0, 0xA4, 0xA1);
        Send(controller, 0xE0, 0xD0, 0x00, 0x02, 0x07, 0x0A, 0x28, 0x32, 0x44, 0x42, 0x06, 0x0E, 0x12, 0x14, 0x17);
        Send(controller, 0xE1, 0xD0, 0x00, 0x02, 0x07, 0x0A, 0x28, 0x31, 0x54, 0x47, 0x0E, 0x1C, 0x17, 0x1B, 0x1E);
        controller.WriteCommand(0x21);
        Send(controller, 0x2A, 0x00, 0x00, 0x00, 0xEF);
        Send(controller, 0x2B, 0x00, 0x00, 0x01, 0x3F);
        controller.WriteCommand(0x29);

        Assert.Equal(0, capture.Count(nameof(TftController)));
    }

    private static (byte First, byte Second) UnknownBytes(string name)
        => name switch
        {
            // ref: §SSD1306.2, §SSD1331.2, §SSD1351.2, §SSD1680.2, §TFT.2: bytes that are not commands of the chip.
            // §ST7920.2: 0xF1 and 0xF2 are DDRAM address codes above 0xBF (⚠️ unverified, masked with 0x3F and traced).
            nameof(TftController) => (0x90, 0x91),
            _ => (0xF1, 0xF2),
        };

    private static IGraphicDisplayController Create(string name)
        => name switch
        {
            nameof(Ssd1306Controller) => new Ssd1306Controller(new MonochromeFrameBuffer()),
            nameof(Ssd1331Controller) => new Ssd1331Controller(new ColorFrameBuffer(96, 64)),
            nameof(Ssd1351Controller) => new Ssd1351Controller(new ColorFrameBuffer(128, 128)),
            nameof(Ssd1680Controller) => new Ssd1680Controller(new EPaperFrameBuffer()),
            nameof(Max7219Controller) => new Max7219Controller(new Max7219ChainBuffer()),
            nameof(St7920Controller) => new St7920Controller(new MonochromeFrameBuffer()),
            _ => new TftController(new ColorFrameBuffer(240, 320), TftChip.St7789),
        };

    private static string Hex(byte value)
        => string.Create(CultureInfo.InvariantCulture, $"0x{value:X2}");

    private static void Send(
        TftController controller,
        byte command,
        params byte[] data)
    {
        controller.WriteCommand(command);
        controller.WriteData(data);
    }
}