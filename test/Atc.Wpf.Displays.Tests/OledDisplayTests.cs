namespace Atc.Wpf.Displays.Tests;

public sealed class OledDisplayTests
{
    private const int RenderWidth = 640;
    private const int RenderHeight = 320;
    private const int FrameBytes = 128 * 8;
    private static readonly Color PanelColor = Color.FromRgb(0x00, 0x00, 0x00);
    private static readonly Color LitColor = Color.FromRgb(0xFF, 0xFF, 0xFF);

    [StaFact]
    public void Text_LightsTheGlyphPixels()
    {
        var sut = CreateSut();

        sut.Text = "A\nB";

        // 'A' in the A00 ROM: row 0 is 0x0E (.XXX.), so x = 0 is off and x = 1..3 are on.
        var glyphA = CharacterRomTables.GetGlyph(CharacterRom.A00, (byte)'A');
        var glyphB = CharacterRomTables.GetGlyph(CharacterRom.A00, (byte)'B');
        for (var y = 0; y < 8; y++)
        {
            for (var x = 0; x < 5; x++)
            {
                Assert.Equal((glyphA[y] & (0x10 >> x)) != 0, sut.Buffer.GetPixel(x, y));
                Assert.Equal((glyphB[y] & (0x10 >> x)) != 0, sut.Buffer.GetPixel(x, 8 + y));
            }
        }

        var pixels = Render(sut);
        var layout = CreateLayout(sut);
        Assert.Equal(LitColor, pixels(Centre(layout.GetDotRect(1, 0))));
        Assert.Equal(PanelColor, pixels(Centre(layout.GetDotRect(0, 0))));
    }

    [StaFact]
    public void Text_SetAgain_ClearsTheOldPixels()
    {
        var sut = new OledDisplay { Text = "HELLO" };
        sut.Buffer.SetPixel(100, 50, isOn: true);

        sut.Text = "I";

        Assert.False(sut.Buffer.GetPixel(100, 50));
        Assert.False(sut.Buffer.GetPixel(6, 0) || sut.Buffer.GetPixel(7, 0) || sut.Buffer.GetPixel(8, 0));
    }

    [StaFact]
    public void YellowBlue_TopRowsYellowRestBlue()
    {
        var sut = new OledDisplay { Color = OledDisplayColor.YellowBlue, ShowBezel = false };
        sut.Buffer.Contrast = 255;
        sut.Buffer.SetPixel(0, 0, isOn: true);
        sut.Buffer.SetPixel(0, 20, isOn: true);
        var (_, blue) = OledDisplayPalette.Get(OledDisplayColor.YellowBlue);
        var (_, yellow) = OledDisplayPalette.Get(OledDisplayColor.Yellow);

        var pixels = Render(sut);
        var layout = CreateLayout(sut);

        Assert.Equal(yellow, pixels(Centre(layout.GetDotRect(0, 0))));
        Assert.Equal(blue, pixels(Centre(layout.GetDotRect(0, 20))));
    }

    [StaFact]
    public void YellowZoneRows_IsCoercedToThePixelRows()
    {
        var sut = new OledDisplay { PixelRows = 32, YellowZoneRows = 50 };

        Assert.Equal(32, sut.YellowZoneRows);

        sut.YellowZoneRows = -3;

        Assert.Equal(0, sut.YellowZoneRows);
    }

    [StaFact]
    public void PixelSize_IsCoerced()
    {
        var sut = new OledDisplay { PixelColumns = 500, PixelRows = 0 };

        Assert.Equal(132, sut.PixelColumns);
        Assert.Equal(1, sut.PixelRows);
        Assert.Equal(132, sut.Buffer.Width);
        Assert.Equal(1, sut.Buffer.Height);
    }

    [StaFact]
    public void IsInverted_XorsWithTheBufferInvert()
    {
        // ref: §SSD1306.2 (0xA6 / 0xA7: A7 shows RAM 0 as on)
        var sut = CreateSut();
        sut.Buffer.SetPixel(0, 0, isOn: true);
        var layout = CreateLayout(sut);
        var onPixel = Centre(layout.GetDotRect(0, 0));
        var offPixel = Centre(layout.GetDotRect(1, 0));

        sut.IsInverted = true;
        var inverted = Render(sut);
        Assert.Equal(PanelColor, inverted(onPixel));
        Assert.Equal(LitColor, inverted(offPixel));

        sut.Buffer.IsInverted = true;
        var both = Render(sut);
        Assert.Equal(LitColor, both(onPixel));
        Assert.Equal(PanelColor, both(offPixel));

        sut.IsInverted = false;
        var chipOnly = Render(sut);
        Assert.Equal(PanelColor, chipOnly(onPixel));
        Assert.Equal(LitColor, chipOnly(offPixel));
    }

    [StaFact]
    public void DisplayOff_ShowsOnlyThePanel()
    {
        var sut = CreateSut();
        sut.Buffer.Fill();
        sut.Buffer.IsEntireDisplayOn = true;
        sut.IsInverted = true;

        sut.Buffer.IsDisplayOn = false;

        var pixels = Render(sut);
        var layout = CreateLayout(sut);
        Assert.Equal(PanelColor, pixels(Centre(layout.GetDotRect(0, 0))));
        Assert.Equal(PanelColor, pixels(Centre(layout.GetDotRect(127, 63))));
    }

    [StaFact]
    public void EntireDisplayOn_LightsEveryPixel()
    {
        // ref: §SSD1306.2 (0xA5: all pixels on regardless of RAM)
        var sut = CreateSut();
        sut.Buffer.Contrast = 255;

        sut.Buffer.IsEntireDisplayOn = true;

        var pixels = Render(sut);
        var layout = CreateLayout(sut);
        Assert.Equal(LitColor, pixels(Centre(layout.GetDotRect(0, 0))));
        Assert.Equal(LitColor, pixels(Centre(layout.GetDotRect(127, 63))));
        Assert.Equal(LitColor, pixels(Centre(layout.GetDotRect(64, 31))));
    }

    [StaFact]
    public void Contrast_MapsToBrightness()
    {
        // ref: §SSD1306.2 (0x81 contrast 0-255, reset 0x7F)
        var sut = new OledDisplay();

        Assert.Equal(0.4 + (0.6 * 0x7F / 255.0), sut.Brightness, 6);

        sut.Buffer.Contrast = 0;
        Assert.Equal(0.4, sut.Brightness, 6);

        sut.Buffer.Contrast = 255;
        Assert.Equal(1.0, sut.Brightness, 6);
    }

    [StaFact]
    public void Contrast_ChangedOnAWorkerThread_ReachesBrightness()
    {
        var sut = new OledDisplay();

        var buffer = sut.Buffer;

        Task.Run(() => buffer.Contrast = 0).Wait();
        Pump();

        Assert.Equal(0.4, sut.Brightness, 6);
    }

    [StaFact]
    public void Contrast_SetBackByAWorkerWhileBrightnessIsApplied_ReachesBrightness()
    {
        // A worker sets the contrast A -> C -> A. The C reaches the UI thread; while the UI thread applies C (after reading it,
        // before recording it as applied) the worker sets A back. The worker then sees A as already applied and posts nothing,
        // so the UI thread must notice the change itself.
        var sut = new OledDisplay();
        var buffer = sut.Buffer;
        Assert.Equal(0x7F, buffer.Contrast);

        Task.Run(() => buffer.Contrast = 0).Wait();
        sut.ContrastReadForTests = () =>
        {
            sut.ContrastReadForTests = null;
            Task.Run(() => buffer.Contrast = 0x7F).Wait();
        };
        Pump();
        Pump();

        Assert.Null(sut.ContrastReadForTests);
        Assert.Equal(0.4 + (0.6 * 0x7F / 255.0), sut.Brightness, 6);
    }

    [StaFact]
    public void ExternalBuffer_IsResizedToThePixelSize()
    {
        var sut = new OledDisplay { PixelColumns = 64, PixelRows = 32 };
        var buffer = new MonochromeFrameBuffer(128, 64) { Contrast = 255 };

        sut.Buffer = buffer;

        Assert.Same(buffer, sut.Buffer);
        Assert.Equal(64, buffer.Width);
        Assert.Equal(32, buffer.Height);
        Assert.Equal(1.0, sut.Brightness, 6);
    }

    [StaFact]
    public void ExternalBuffer_TakesOverANonEmptyText()
    {
        var sut = new OledDisplay { Text = "A" };
        var buffer = new MonochromeFrameBuffer();

        sut.Buffer = buffer;

        Assert.True(buffer.GetPixel(1, 0));
    }

    [StaFact]
    public void Buffer_SetToNull_ReturnsToTheOwnBuffer()
    {
        var sut = new OledDisplay();
        var own = sut.Buffer;
        sut.Buffer = new MonochromeFrameBuffer();

        sut.Buffer = null!;

        Assert.Same(own, sut.Buffer);
    }

    [StaFact]
    public void OldBuffer_ChangesNoLongerReachTheControl()
    {
        var sut = new OledDisplay();
        var old = sut.Buffer;
        sut.Buffer = new MonochromeFrameBuffer { Contrast = 255 };

        old.Contrast = 0;

        Assert.Equal(1.0, sut.Brightness, 6);
    }

    [StaFact]
    public void FramesFromAWorkerThread_AreCoalescedIntoOneRedraw()
    {
        var sut = CreateSut();
        sut.Width = RenderWidth;
        sut.Height = RenderHeight;
        var window = CreateOffScreenWindow(sut);
        try
        {
            window.Show();
            Pump();

            var buffer = sut.Buffer;
            var snapshots = 0;
            sut.SnapshotTakenForTests = () => snapshots++;

            Task.Run(() =>
            {
                for (var i = 0; i < 100; i++)
                {
                    var frame = new byte[FrameBytes];
                    frame[i] = 0x01;
                    buffer.LoadFrame(frame);
                }
            }).Wait();

            Pump();

            Assert.Equal(1, snapshots);
            sut.SnapshotTakenForTests = null;

            // The last frame lit only pixel (99, 0).
            Assert.True(sut.Buffer.GetPixel(99, 0));
            Assert.False(sut.Buffer.GetPixel(98, 0));
            var pixels = Render(sut);
            var layout = CreateLayout(sut);
            Assert.Equal(LitColor, pixels(Centre(layout.GetDotRect(99, 0))));
            Assert.Equal(PanelColor, pixels(Centre(layout.GetDotRect(98, 0))));
        }
        finally
        {
            window.Close();
            Pump();
        }
    }

    [StaFact]
    public void Render_AtZeroSize_DoesNotThrow()
    {
        var sut = new OledDisplay { Text = "HI" };
        sut.Measure(new Size(0, 0));
        sut.Arrange(new Rect(0, 0, 0, 0));

        var bitmap = new RenderTargetBitmap(1, 1, 96, 96, PixelFormats.Pbgra32);
        var exception = Record.Exception(() => bitmap.Render(sut));

        Assert.Null(exception);
    }

    [StaFact]
    public void AutomationPeer_NamesTheResolution()
    {
        var sut = new OledDisplay { PixelColumns = 128, PixelRows = 32 };

        var peer = UIElementAutomationPeer.CreatePeerForElement(sut);

        Assert.IsType<OledDisplayAutomationPeer>(peer);
        Assert.Equal("OLED 128x32", peer.GetName());
        Assert.Equal(nameof(OledDisplay), peer.GetClassName());
    }

    private static OledDisplay CreateSut()
    {
        var sut = new OledDisplay
        {
            ShowBezel = false,
            PanelBrush = new SolidColorBrush(PanelColor),
            LitDotBrush = new SolidColorBrush(LitColor),
        };
        sut.Buffer.Contrast = 255;
        return sut;
    }

    private static Window CreateOffScreenWindow(OledDisplay content)
        => new()
        {
            Content = content,
            Left = -10000,
            Top = -10000,
            SizeToContent = SizeToContent.WidthAndHeight,
            ShowActivated = false,
            ShowInTaskbar = false,
            WindowStyle = WindowStyle.None,
        };

    private static void Pump()
        => Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);

    private static DotMatrixLayout CreateLayout(OledDisplay sut)
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