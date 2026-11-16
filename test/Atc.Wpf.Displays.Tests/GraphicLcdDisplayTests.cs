namespace Atc.Wpf.Displays.Tests;

public sealed class GraphicLcdDisplayTests
{
    private const int RenderWidth = 640;
    private const int RenderHeight = 320;

    [StaFact]
    public void GraphicLcd_LightsBufferPixelsWithTheLcdPalette()
    {
        var sut = CreateSut(CharacterDisplayColor.Blue);
        sut.Buffer.SetPixel(3, 5, isOn: true);
        var (panel, ink, _, _) = CharacterDisplayPalette.Get(CharacterDisplayKind.Lcd, CharacterDisplayColor.Blue, isBacklightOn: true);

        var pixels = Render(sut);
        var layout = CreateLayout(sut);

        Assert.Equal(ink, pixels(Centre(layout.GetDotRect(3, 5))));
        Assert.Equal(panel, pixels(Centre(layout.GetDotRect(4, 5))));
        Assert.Equal(panel, pixels(Centre(layout.GetDotRect(127, 63))));
    }

    [StaFact]
    public void BacklightOff_UsesTheReflectiveLook()
    {
        var sut = CreateSut(CharacterDisplayColor.Blue);
        sut.Buffer.SetPixel(10, 10, isOn: true);

        sut.IsBacklightOn = false;

        var (panel, ink, _, ghost) = CharacterDisplayPalette.Get(CharacterDisplayKind.Lcd, CharacterDisplayColor.Blue, isBacklightOn: false);
        var (litPanel, _, _, _) = CharacterDisplayPalette.Get(CharacterDisplayKind.Lcd, CharacterDisplayColor.Blue, isBacklightOn: true);
        Assert.NotEqual(litPanel, panel);
        Assert.Equal(CharacterDisplayPalette.ReflectiveGhostOpacity, ghost);

        var pixels = Render(sut);
        var layout = CreateLayout(sut);

        Assert.Equal(ink, pixels(Centre(layout.GetDotRect(10, 10))));
        Assert.Equal(panel, pixels(Centre(layout.GetDotRect(11, 10))));
    }

    [StaFact]
    public void Defaults_AreA128x64BlueLcdWithItsOwnBuffer()
    {
        var sut = new GraphicLcdDisplay();

        Assert.Equal(CharacterDisplayColor.Blue, sut.Color);
        Assert.True(sut.IsBacklightOn);
        Assert.Equal(128, sut.Buffer.Width);
        Assert.Equal(64, sut.Buffer.Height);
    }

    [StaFact]
    public void Text_DrawsIntoTheBuffer()
    {
        var sut = new GraphicLcdDisplay { Text = "A" };

        // 'A' in the A00 ROM: row 0 is 0x0E (.XXX.), so x = 0 is off and x = 1 is on.
        Assert.False(sut.Buffer.GetPixel(0, 0));
        Assert.True(sut.Buffer.GetPixel(1, 0));

        sut.Text = string.Empty;

        Assert.False(sut.Buffer.GetPixel(1, 0));
    }

    [StaFact]
    public void ExternalBuffer_IsResizedTo128x64_AndNullReturnsToTheOwnBuffer()
    {
        var sut = new GraphicLcdDisplay();
        var own = sut.Buffer;
        var external = new MonochromeFrameBuffer(32, 16);

        sut.Buffer = external;

        Assert.Same(external, sut.Buffer);
        Assert.Equal(128, external.Width);
        Assert.Equal(64, external.Height);

        sut.Buffer = null!;

        Assert.Same(own, sut.Buffer);
    }

    [StaFact]
    public void DisplayOff_ShowsOnlyThePanel()
    {
        var sut = CreateSut(CharacterDisplayColor.YellowGreen);
        sut.Buffer.SetPixel(0, 0, isOn: true);
        sut.Buffer.IsDisplayOn = false;
        var (panel, _, _, _) = CharacterDisplayPalette.Get(CharacterDisplayKind.Lcd, CharacterDisplayColor.YellowGreen, isBacklightOn: true);

        var pixels = Render(sut);
        var layout = CreateLayout(sut);

        Assert.Equal(panel, pixels(Centre(layout.GetDotRect(0, 0))));
    }

    [StaFact]
    public void St7920Controller_DrivesTheControl()
    {
        // ref: §ST7920.3, §ST7920.5: 0x0C display on (basic), 0x3E twice (RE = 1, then G = 1), GDRAM address vertical 0 then
        // horizontal 0, word 0x8000 lights glass pixel (0, 0).
        var sut = CreateSut(CharacterDisplayColor.Blue);
        using var controller = new St7920Controller(sut.Buffer);
        controller.WriteCommands([0x0C, 0x3E, 0x3E, 0x80, 0x80]);
        controller.WriteData([0x80, 0x00]);
        var (panel, ink, _, _) = CharacterDisplayPalette.Get(CharacterDisplayKind.Lcd, CharacterDisplayColor.Blue, isBacklightOn: true);

        var pixels = Render(sut);
        var layout = CreateLayout(sut);

        Assert.Equal(ink, pixels(Centre(layout.GetDotRect(0, 0))));
        Assert.Equal(panel, pixels(Centre(layout.GetDotRect(1, 0))));
    }

    [StaFact]
    public void AutomationPeer_NamesTheResolution()
    {
        var sut = new GraphicLcdDisplay();

        var peer = UIElementAutomationPeer.CreatePeerForElement(sut);

        Assert.IsType<GraphicLcdDisplayAutomationPeer>(peer);
        Assert.Equal("LCD 128x64", peer.GetName());
        Assert.Equal(nameof(GraphicLcdDisplay), peer.GetClassName());
    }

    private static GraphicLcdDisplay CreateSut(CharacterDisplayColor color)
        => new()
        {
            Color = color,
            ShowBezel = false,
            GhostOpacity = 0,
        };

    private static void Pump()
        => Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);

    private static DotMatrixLayout CreateLayout(GraphicLcdDisplay sut)
        => DotMatrixLayout.Create(sut.CurrentGeometry, sut.ShowBezel, new Size(RenderWidth, RenderHeight), sut.Stretch);

    private static Point Centre(Rect rect)
        => new(rect.X + (rect.Width / 2), rect.Y + (rect.Height / 2));

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

            // Pbgra32 is premultiplied; undo it so translucent colours compare by their own channels.
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