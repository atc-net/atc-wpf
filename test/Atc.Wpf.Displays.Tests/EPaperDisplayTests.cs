namespace Atc.Wpf.Displays.Tests;

public sealed class EPaperDisplayTests
{
    private const int RenderWidth = 640;
    private const int RenderHeight = 320;
    private const int SmallColumns = 16;
    private const int SmallRows = 8;
    private static readonly TimeSpan FlashPhase = TimeSpan.FromMilliseconds(200);
    private static readonly Color Paper = Color.FromRgb(0xE8, 0xE6, 0xDF);
    private static readonly Color Ink = Color.FromRgb(0x1A, 0x1A, 0x1A);
    private static readonly Color RedInk = Color.FromRgb(0xB2, 0x22, 0x22);

    [StaFact]
    public void Defaults_AreThe29InchPanelInLandscape()
    {
        var sut = new EPaperDisplay();

        Assert.Equal(128, sut.PixelColumns);
        Assert.Equal(296, sut.PixelRows);
        Assert.Equal(90, sut.Rotation);
        Assert.False(sut.HasRed);
        Assert.True(sut.ShowRefreshFlash);
        Assert.Equal(TimeSpan.FromMilliseconds(600), sut.RefreshFlashDuration);
        Assert.Equal(128, sut.Buffer.Width);
        Assert.Equal(296, sut.Buffer.Height);
    }

    [StaFact]
    public void Refresh_Full_PlaysTheFlashThenShowsTheImage()
    {
        var time = new ManualTimeProvider();
        var sut = CreateSut(time);
        var window = CreateOffScreenWindow(sut);
        try
        {
            window.Show();
            Pump();
            var layout = CreateLayout(sut);
            Assert.Equal(Paper, Render(sut)(Centre(layout.GetDotRect(0, 0))));

            // Pixel (0, 0) black, pixels (1..7, 0) white.
            sut.Buffer.WriteRam(EPaperPlane.Black, 0, 0, 0x7F);
            sut.Buffer.Refresh(EPaperRefreshKind.Full);

            // Phase 1 of 3: the new picture inverted.
            var pixels = Render(sut);
            Assert.Equal(Paper, pixels(Centre(layout.GetDotRect(0, 0))));
            Assert.Equal(Ink, pixels(Centre(layout.GetDotRect(1, 0))));

            // Phase 2 of 3: all white.
            time.Advance(FlashPhase);
            pixels = Render(sut);
            Assert.Equal(Paper, pixels(Centre(layout.GetDotRect(0, 0))));
            Assert.Equal(Paper, pixels(Centre(layout.GetDotRect(1, 0))));

            // Phase 3: the picture; the flash timer is gone.
            time.Advance(FlashPhase);
            pixels = Render(sut);
            Assert.Equal(Ink, pixels(Centre(layout.GetDotRect(0, 0))));
            Assert.Equal(Paper, pixels(Centre(layout.GetDotRect(1, 0))));
            Assert.Equal(0, time.ActiveTimerCount);
        }
        finally
        {
            window.Close();
            Pump();
        }
    }

    [StaFact]
    public void Refresh_FullDuringAFlash_RestartsTheFlashWithTheNewestImage()
    {
        var time = new ManualTimeProvider();
        var sut = CreateSut(time);
        var window = CreateOffScreenWindow(sut);
        try
        {
            window.Show();
            Pump();
            var layout = CreateLayout(sut);

            sut.Buffer.WriteRam(EPaperPlane.Black, 0, 0, 0x7F);
            sut.Buffer.Refresh(EPaperRefreshKind.Full);
            Pump();
            time.Advance(FlashPhase);

            // Second picture: pixel (1, 0) black instead of (0, 0).
            sut.Buffer.WriteRam(EPaperPlane.Black, 0, 0, 0xBF);
            sut.Buffer.Refresh(EPaperRefreshKind.Full);

            var pixels = Render(sut);
            Assert.Equal(Ink, pixels(Centre(layout.GetDotRect(0, 0))));
            Assert.Equal(Paper, pixels(Centre(layout.GetDotRect(1, 0))));

            // The first flash would have ended here; the restarted one is in its white phase.
            time.Advance(FlashPhase);
            Assert.Equal(Paper, Render(sut)(Centre(layout.GetDotRect(1, 0))));

            time.Advance(FlashPhase);
            pixels = Render(sut);
            Assert.Equal(Paper, pixels(Centre(layout.GetDotRect(0, 0))));
            Assert.Equal(Ink, pixels(Centre(layout.GetDotRect(1, 0))));
            Assert.Equal(0, time.ActiveTimerCount);
        }
        finally
        {
            window.Close();
            Pump();
        }
    }

    [StaFact]
    public void Refresh_PartialDuringAFlash_ChangesThePictureTheFlashEndsOn()
    {
        var time = new ManualTimeProvider();
        var sut = CreateSut(time);
        var window = CreateOffScreenWindow(sut);
        try
        {
            window.Show();
            Pump();
            var layout = CreateLayout(sut);

            sut.Buffer.WriteRam(EPaperPlane.Black, 0, 0, 0x7F);
            sut.Buffer.Refresh(EPaperRefreshKind.Full);
            Pump();

            sut.Buffer.WriteRam(EPaperPlane.Black, 0, 0, 0xBF);
            sut.Buffer.Refresh(EPaperRefreshKind.Partial);
            Pump();
            Assert.Equal(1, time.ActiveTimerCount);

            time.Advance(FlashPhase * 2);
            var pixels = Render(sut);
            Assert.Equal(Paper, pixels(Centre(layout.GetDotRect(0, 0))));
            Assert.Equal(Ink, pixels(Centre(layout.GetDotRect(1, 0))));
        }
        finally
        {
            window.Close();
            Pump();
        }
    }

    [StaFact]
    public void Refresh_Partial_ShowsTheImageWithoutFlash()
    {
        var time = new ManualTimeProvider();
        var sut = CreateSut(time);
        var window = CreateOffScreenWindow(sut);
        try
        {
            window.Show();
            Pump();
            var layout = CreateLayout(sut);

            sut.Buffer.WriteRam(EPaperPlane.Black, 0, 0, 0x7F);
            sut.Buffer.Refresh(EPaperRefreshKind.Partial);

            var pixels = Render(sut);
            Assert.Equal(Ink, pixels(Centre(layout.GetDotRect(0, 0))));
            Assert.Equal(Paper, pixels(Centre(layout.GetDotRect(1, 0))));
            Assert.Equal(0, time.ActiveTimerCount);
        }
        finally
        {
            window.Close();
            Pump();
        }
    }

    [StaFact]
    public void ShowRefreshFlashOff_ShowsImmediately()
    {
        var time = new ManualTimeProvider();
        var sut = CreateSut(time);
        sut.ShowRefreshFlash = false;
        var window = CreateOffScreenWindow(sut);
        try
        {
            window.Show();
            Pump();
            var layout = CreateLayout(sut);

            sut.Buffer.WriteRam(EPaperPlane.Black, 0, 0, 0x7F);
            sut.Buffer.Refresh(EPaperRefreshKind.Full);

            var pixels = Render(sut);
            Assert.Equal(Ink, pixels(Centre(layout.GetDotRect(0, 0))));
            Assert.Equal(Paper, pixels(Centre(layout.GetDotRect(1, 0))));
            Assert.Equal(0, time.ActiveTimerCount);
        }
        finally
        {
            window.Close();
            Pump();
        }
    }

    [StaFact]
    public void Unloaded_DuringAFlash_StopsTheTimerAndShowsTheImage()
    {
        var time = new ManualTimeProvider();
        var sut = CreateSut(time);
        var window = CreateOffScreenWindow(sut);
        window.Show();
        Pump();

        sut.Buffer.WriteRam(EPaperPlane.Black, 0, 0, 0x7F);
        sut.Buffer.Refresh(EPaperRefreshKind.Full);
        Pump();
        Assert.Equal(1, time.ActiveTimerCount);

        window.Close();
        Pump();

        Assert.Equal(0, time.ActiveTimerCount);

        // Shown again, the control shows the picture, not the rest of the flash.
        window.Content = null;
        var second = CreateOffScreenWindow(sut);
        try
        {
            second.Show();
            Pump();
            Assert.Equal(Ink, Render(sut)(Centre(CreateLayout(sut).GetDotRect(0, 0))));
            Assert.Equal(0, time.ActiveTimerCount);
        }
        finally
        {
            second.Close();
            Pump();
        }
    }

    [StaFact]
    public void Refresh_Full_WhenNotLoaded_ShowsTheImageWithoutATimer()
    {
        var time = new ManualTimeProvider();
        var sut = CreateSut(time);

        sut.Buffer.WriteRam(EPaperPlane.Black, 0, 0, 0x7F);
        sut.Buffer.Refresh(EPaperRefreshKind.Full);

        Assert.Equal(Ink, Render(sut)(Centre(CreateLayout(sut).GetDotRect(0, 0))));
        Assert.Equal(0, time.ActiveTimerCount);
    }

    [StaFact]
    public void Rotation90_MapsNativeTopLeftToScreenBottomLeft()
    {
        // ref: §SSD1680.8, GxEPD2 setRotation(1): (nx, ny) = (WIDTH - 1 - uy, ux), WIDTH = 16 here.
        // Native (0, 0): ux = ny = 0, uy = WIDTH - 1 - nx = 15, so screen (0, 15), the bottom-left of the 8 x 16 screen.
        // Native (15, 0): ux = 0, uy = 0, the screen top-left.
        var sut = CreateSut();
        sut.Rotation = 90;
        sut.Buffer.WriteRam(EPaperPlane.Black, 0, 0, 0x7F);
        sut.Buffer.WriteRam(EPaperPlane.Black, 1, 0, 0xFE);
        sut.Buffer.Refresh(EPaperRefreshKind.Partial);

        var pixels = Render(sut);
        var layout = CreateLayout(sut);

        Assert.Equal(SmallRows, layout.DotColumns);
        Assert.Equal(SmallColumns, layout.DotRows);
        Assert.Equal(Ink, pixels(Centre(layout.GetDotRect(0, 15))));
        Assert.Equal(Ink, pixels(Centre(layout.GetDotRect(0, 0))));
        Assert.Equal(Paper, pixels(Centre(layout.GetDotRect(7, 0))));
        Assert.Equal(Paper, pixels(Centre(layout.GetDotRect(7, 15))));
        Assert.Equal(Paper, pixels(Centre(layout.GetDotRect(0, 14))));
    }

    [StaFact]
    public void Rotation270_MatchesAdafruitRotation0_NativeTopLeftAtScreenTopRight()
    {
        // ref: §SSD1680.8: GxEPD2 setRotation(3) = Adafruit_EPD rotation 0: (nx, ny) = (uy, HEIGHT - 1 - ux), HEIGHT = 8 here.
        // Native (0, 0): uy = 0, ux = HEIGHT - 1 - ny = 7, so screen (7, 0), the top-right of the 8 x 16 screen.
        var sut = CreateSut();
        sut.Rotation = 270;
        sut.Buffer.WriteRam(EPaperPlane.Black, 0, 0, 0x7F);
        sut.Buffer.Refresh(EPaperRefreshKind.Partial);

        var pixels = Render(sut);
        var layout = CreateLayout(sut);

        Assert.Equal(Ink, pixels(Centre(layout.GetDotRect(7, 0))));
        Assert.Equal(Paper, pixels(Centre(layout.GetDotRect(0, 0))));
        Assert.Equal(Paper, pixels(Centre(layout.GetDotRect(0, 15))));
    }

    [StaFact]
    public void Rotation180_MapsNativeTopLeftToScreenBottomRight()
    {
        // ref: §SSD1680.8, GxEPD2 setRotation(2): (nx, ny) = (WIDTH - 1 - ux, HEIGHT - 1 - uy).
        var sut = CreateSut();
        sut.Rotation = 180;
        sut.Buffer.WriteRam(EPaperPlane.Black, 0, 0, 0x7F);
        sut.Buffer.Refresh(EPaperRefreshKind.Partial);

        var pixels = Render(sut);
        var layout = CreateLayout(sut);

        Assert.Equal(Ink, pixels(Centre(layout.GetDotRect(SmallColumns - 1, SmallRows - 1))));
        Assert.Equal(Paper, pixels(Centre(layout.GetDotRect(0, 0))));
    }

    [StaTheory]
    [InlineData(-90, 270)]
    [InlineData(360, 0)]
    [InlineData(450, 90)]
    [InlineData(100, 90)]
    [InlineData(140, 180)]
    public void Rotation_IsCoercedToAQuarterTurn(
        int value,
        int expected)
    {
        var sut = new EPaperDisplay { Rotation = value };

        Assert.Equal(expected, sut.Rotation);
    }

    [StaFact]
    public void RedPixels_UseRedInk()
    {
        var sut = CreateSut();
        sut.HasRed = true;
        sut.Buffer.WriteRam(EPaperPlane.Black, 0, 0, 0x3F);
        sut.Buffer.WriteRam(EPaperPlane.Red, 0, 0, 0x80);
        sut.Buffer.Refresh(EPaperRefreshKind.Partial);

        var pixels = Render(sut);
        var layout = CreateLayout(sut);

        // ref: §SSD1680.4 item 7: red where the red bit is 1, else the black bit.
        Assert.True(sut.Buffer.HasRed);
        Assert.Equal(RedInk, pixels(Centre(layout.GetDotRect(0, 0))));
        Assert.Equal(Ink, pixels(Centre(layout.GetDotRect(1, 0))));
        Assert.Equal(Paper, pixels(Centre(layout.GetDotRect(2, 0))));
    }

    [StaFact]
    public void RedPixels_OnABlackWhitePanel_AreNotShown()
    {
        // ref: §SSD1680.4 item 3: a BW panel ignores plane 2.
        var sut = CreateSut();
        sut.Buffer.WriteRam(EPaperPlane.Black, 0, 0, 0x3F);
        sut.Buffer.WriteRam(EPaperPlane.Red, 0, 0, 0xC0);
        sut.Buffer.Refresh(EPaperRefreshKind.Partial);

        var pixels = Render(sut);
        var layout = CreateLayout(sut);

        Assert.Equal(Ink, pixels(Centre(layout.GetDotRect(0, 0))));
        Assert.Equal(Ink, pixels(Centre(layout.GetDotRect(1, 0))));
        Assert.Equal(Paper, pixels(Centre(layout.GetDotRect(2, 0))));
    }

    [StaFact]
    public void InkBrushes_AreUsedForThePixels()
    {
        var sut = CreateSut();
        sut.PaperBrush = Brushes.White;
        sut.InkBrush = Brushes.Navy;
        sut.Buffer.WriteRam(EPaperPlane.Black, 0, 0, 0x7F);
        sut.Buffer.Refresh(EPaperRefreshKind.Partial);

        var pixels = Render(sut);
        var layout = CreateLayout(sut);

        Assert.Equal(Colors.Navy, pixels(Centre(layout.GetDotRect(0, 0))));
        Assert.Equal(Colors.White, pixels(Centre(layout.GetDotRect(1, 0))));
    }

    [StaFact]
    public void RamWritesWithoutRefresh_DoNotChangeTheScreen()
    {
        var sut = CreateSut();
        var layout = CreateLayout(sut);
        Assert.Equal(Paper, Render(sut)(Centre(layout.GetDotRect(0, 0))));

        sut.Buffer.WriteRam(EPaperPlane.Black, 0, 0, 0x00);
        sut.Buffer.LoadRam(new byte[sut.Buffer.BytesPerRow * SmallRows], []);

        var pixels = Render(sut);
        Assert.Equal(Paper, pixels(Centre(layout.GetDotRect(0, 0))));
        Assert.Equal(Paper, pixels(Centre(layout.GetDotRect(SmallColumns - 1, SmallRows - 1))));
    }

    [StaFact]
    public void FramesFromAWorkerThread_AreCoalesced()
    {
        var sut = CreateSut(new ManualTimeProvider());
        sut.Width = RenderWidth;
        sut.Height = RenderHeight;
        var window = CreateOffScreenWindow(sut);
        try
        {
            window.Show();
            Pump();

            var buffer = sut.Buffer;
            var copies = 0;
            sut.FrameCopiedForTests = () => copies++;

            Task.Run(() =>
            {
                for (var i = 0; i < 100; i++)
                {
                    // Pixel (i % 16, i / 16 % 8) black, the rest white.
                    var frame = new byte[2 * SmallRows];
                    Array.Fill(frame, (byte)0xFF);
                    var index = i % (SmallColumns * SmallRows);
                    frame[((index / SmallColumns) * 2) + ((index % SmallColumns) / 8)] = (byte)~(0x80 >> (index % 8));
                    buffer.LoadRam(frame, []);
                    buffer.Refresh(EPaperRefreshKind.Partial);
                }
            }).Wait();

            Pump();

            Assert.Equal(1, copies);
            sut.FrameCopiedForTests = null;

            // The last frame blackened only pixel 99 = (3, 6).
            var pixels = Render(sut);
            var layout = CreateLayout(sut);
            Assert.Equal(Ink, pixels(Centre(layout.GetDotRect(3, 6))));
            Assert.Equal(Paper, pixels(Centre(layout.GetDotRect(2, 6))));
        }
        finally
        {
            window.Close();
            Pump();
        }
    }

    [StaFact]
    public void UpdateBitmap_DoesNotAllocatePerFrame()
    {
        var sut = new EPaperDisplay { HasRed = true };
        var buffer = sut.Buffer;
        var black = new byte[buffer.BytesPerRow * buffer.Height];
        var red = new byte[black.Length];
        for (var i = 0; i < 10; i++)
        {
            sut.UpdateBitmap();
        }

        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var i = 0; i < 20; i++)
        {
            black[i] = 0x0F;
            red[i + 1] = 0xF0;
            buffer.LoadRam(black, red);
            buffer.Refresh(EPaperRefreshKind.Partial);
            sut.UpdateBitmap();
        }

        var perFrame = (GC.GetAllocatedBytesForCurrentThread() - before) / 20;
        Assert.True(perFrame < 1024, $"Allocated {perFrame} bytes per frame.");
    }

    [StaFact]
    public void Text_DrawsBlackIntoTheRamAndRefreshes_InTheRotatedOrientation()
    {
        var sut = new EPaperDisplay { PixelColumns = SmallColumns, PixelRows = SmallRows, Rotation = 90 };

        sut.Text = "A";

        // 'A' in the A00 ROM: row 0 is 0x0E (.XXX.), so screen (1, 0) is black and screen (0, 0) white.
        // ref: §SSD1680.8, GxEPD2 setRotation(1): screen (ux, uy) is native (WIDTH - 1 - uy, ux) = (15, ux).
        Assert.Equal(EPaperInk.White, sut.Buffer.GetShownPixel(15, 0));
        Assert.Equal(EPaperInk.Black, sut.Buffer.GetShownPixel(15, 1));
        Assert.False(sut.Buffer.GetRamPixel(EPaperPlane.Black, 15, 1));

        sut.Rotation = 0;

        Assert.Equal(EPaperInk.Black, sut.Buffer.GetShownPixel(1, 0));
        Assert.Equal(EPaperInk.White, sut.Buffer.GetShownPixel(15, 1));
    }

    [StaFact]
    public void Text_IsAFullRefresh()
    {
        var sut = new EPaperDisplay();
        var kinds = new List<EPaperRefreshKind>();
        sut.Buffer.Refreshed += (_, e) => kinds.Add(e.Kind);

        sut.Text = "HI";

        Assert.Equal([EPaperRefreshKind.Full], kinds);
    }

    [StaFact]
    public void ExternalBuffer_IsResizedTakesOverHasRedAndTheText()
    {
        var sut = new EPaperDisplay { PixelColumns = SmallColumns, PixelRows = SmallRows, Rotation = 0, HasRed = true, Text = "A" };
        var buffer = new EPaperFrameBuffer();

        sut.Buffer = buffer;

        Assert.Same(buffer, sut.Buffer);
        Assert.Equal(SmallColumns, buffer.Width);
        Assert.Equal(SmallRows, buffer.Height);
        Assert.True(buffer.HasRed);
        Assert.Equal(EPaperInk.Black, buffer.GetShownPixel(1, 0));
    }

    [StaFact]
    public void Buffer_SetToNull_ReturnsToTheOwnBuffer()
    {
        var sut = new EPaperDisplay();
        var own = sut.Buffer;
        sut.Buffer = new EPaperFrameBuffer();

        sut.Buffer = null!;

        Assert.Same(own, sut.Buffer);
    }

    [StaFact]
    public void PixelSize_IsCoerced()
    {
        var sut = new EPaperDisplay { PixelColumns = 500, PixelRows = 0 };

        Assert.Equal(400, sut.PixelColumns);
        Assert.Equal(1, sut.PixelRows);
        Assert.Equal(400, sut.Buffer.Width);
        Assert.Equal(1, sut.Buffer.Height);
    }

    [StaFact]
    public void Render_AtZeroSize_DoesNotThrow()
    {
        var sut = new EPaperDisplay { Text = "HI" };
        sut.Measure(new Size(0, 0));
        sut.Arrange(new Rect(0, 0, 0, 0));

        var bitmap = new RenderTargetBitmap(1, 1, 96, 96, PixelFormats.Pbgra32);
        var exception = Record.Exception(() => bitmap.Render(sut));

        Assert.Null(exception);
    }

    [StaFact]
    public void AutomationPeer_NamesTheResolution()
    {
        var sut = new EPaperDisplay { PixelColumns = 122, PixelRows = 250 };

        var peer = UIElementAutomationPeer.CreatePeerForElement(sut);

        Assert.IsType<EPaperDisplayAutomationPeer>(peer);
        Assert.Equal("Display 122x250", peer.GetName());
        Assert.Equal(nameof(EPaperDisplay), peer.GetClassName());
    }

    [StaFact]
    public void VisibleColumns_GxEpd2SetRotation1On128WideBuffer_LandsOnTheUprightGlassPixel()
    {
        // ref: §SSD1680.1 (2.13": 128-bit rows, bits 122-127 padding, GxEPD2 RAM X offset 0) and §SSD1680.8 (GxEPD2 setRotation(1)).
        // GxEPD2_BW::drawPixel r1: x = WIDTH - x - 1 after the swap, where the bare WIDTH is Adafruit_GFX::WIDTH, the constructor width
        // GxEPD2_Type::WIDTH_VISIBLE = 122: (nx, ny) = (121 - uy, ux). The buffer stays GxEPD2_Type::WIDTH = 128 bits wide.
        // User (10, 20): nx = 101, ny = 10 -> byte 12, bit 5 (mask 0x04). User (0, 0): nx = 121 -> byte 15, bit 1 (mask 0x40).
        var sut = new EPaperDisplay
        {
            PixelColumns = 128,
            PixelRows = 250,
            VisibleColumns = 122,
            Rotation = 90,
            ShowBezel = false,
            FlashTimeProvider = new ManualTimeProvider(),
        };
        sut.Buffer.WriteRam(EPaperPlane.Black, 12, 10, 0xFB);
        sut.Buffer.WriteRam(EPaperPlane.Black, 15, 0, 0xBF);
        sut.Buffer.Refresh(EPaperRefreshKind.Partial);

        var pixels = Render(sut);
        var layout = CreateLayout(sut, columns: 250, rows: 122);

        Assert.Equal(Ink, pixels(Centre(layout.GetDotRect(10, 20))));
        Assert.Equal(Ink, pixels(Centre(layout.GetDotRect(0, 0))));
        Assert.Equal(Paper, pixels(Centre(layout.GetDotRect(10, 14))));
        Assert.Equal(Paper, pixels(Centre(layout.GetDotRect(10, 26))));
        Assert.Equal(Paper, pixels(Centre(layout.GetDotRect(1, 0))));
    }

    [StaFact]
    public void VisibleColumns_HidesTheColumnsPastIt()
    {
        // Native columns 9 (visible) and 12 (past VisibleColumns = 10) black; the screen is 10 x 8 and shows only column 9.
        var sut = CreateSut();
        sut.VisibleColumns = 10;
        sut.Buffer.WriteRam(EPaperPlane.Black, 1, 0, 0xB7);
        sut.Buffer.Refresh(EPaperRefreshKind.Partial);

        var pixels = Render(sut);
        var layout = CreateLayout(sut, columns: 10, rows: SmallRows);

        Assert.Equal(Ink, pixels(Centre(layout.GetDotRect(9, 0))));
        for (var x = 0; x < 9; x++)
        {
            Assert.Equal(Paper, pixels(Centre(layout.GetDotRect(x, 0))));
        }

        // The 10-column layout above only matches the picture when the control measures 10 columns; the buffer stays 16 wide.
        Assert.Equal(SmallColumns, sut.Buffer.Width);
    }

    [StaFact]
    public void VisibleColumns_IsNullByDefaultAndCoercedToThePixelColumns()
    {
        var sut = new EPaperDisplay();
        Assert.Null(sut.VisibleColumns);

        sut.VisibleColumns = 200;
        Assert.Equal(128, sut.VisibleColumns);

        sut.VisibleColumns = 0;
        Assert.Equal(1, sut.VisibleColumns);

        sut.VisibleColumns = 122;
        sut.PixelColumns = 100;
        Assert.Equal(100, sut.VisibleColumns);
    }

    [StaFact]
    public void VisibleColumns_TextAtRotation90_StartsAtTheTopOfTheGlass()
    {
        // Screen (x, y) at Rotation 90 is native (VisibleColumns - 1 - y, x): the text's top row lands on native column 9, not on the hidden 15.
        // The A00 'A' top row is 01110 (screen x 1..3).
        var sut = CreateSut();
        sut.VisibleColumns = 10;
        sut.Rotation = 90;
        sut.Text = "A";

        var pixels = Render(sut);
        var layout = CreateLayout(sut, columns: SmallRows, rows: 10);

        Assert.Equal(Paper, pixels(Centre(layout.GetDotRect(0, 0))));
        Assert.Equal(Ink, pixels(Centre(layout.GetDotRect(1, 0))));
        Assert.Equal(Ink, pixels(Centre(layout.GetDotRect(3, 0))));
        Assert.Equal(Paper, pixels(Centre(layout.GetDotRect(4, 0))));
    }

    [StaFact]
    public void Refresh_FullWithAFlashShorterThanThreeTicks_StillEndsOnThePicture()
    {
        // A 1-tick flash gives a 0-tick phase (1 / 3 rounds to 0); a zero-period timer fires once and would leave the screen white.
        var time = new ManualTimeProvider();
        var sut = CreateSut(time);
        sut.RefreshFlashDuration = TimeSpan.FromTicks(1);
        var window = CreateOffScreenWindow(sut);
        try
        {
            window.Show();
            Pump();
            var layout = CreateLayout(sut);

            sut.Buffer.WriteRam(EPaperPlane.Black, 0, 0, 0x7F);
            sut.Buffer.Refresh(EPaperRefreshKind.Full);
            Pump();

            time.Advance(TimeSpan.FromTicks(1));
            Pump();
            time.Advance(TimeSpan.FromTicks(1));
            var pixels = Render(sut);

            Assert.Equal(Ink, pixels(Centre(layout.GetDotRect(0, 0))));
            Assert.Equal(Paper, pixels(Centre(layout.GetDotRect(1, 0))));
            Assert.Equal(0, time.ActiveTimerCount);
        }
        finally
        {
            window.Close();
            Pump();
        }
    }

    private static EPaperDisplay CreateSut(ManualTimeProvider? time = null)
        => new()
        {
            PixelColumns = SmallColumns,
            PixelRows = SmallRows,
            Rotation = 0,
            ShowBezel = false,
            FlashTimeProvider = time ?? new ManualTimeProvider(),
        };

    private static Window CreateOffScreenWindow(EPaperDisplay content)
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

    private static DotMatrixLayout CreateLayout(EPaperDisplay sut)
    {
        var isQuarterTurn = sut.Rotation is 90 or 270;
        var columns = isQuarterTurn ? sut.PixelRows : sut.PixelColumns;
        var rows = isQuarterTurn ? sut.PixelColumns : sut.PixelRows;
        return DotMatrixLayout.Create(
            new DotMatrixGeometry(columns, rows, 0, 0, 0, 0, 0),
            sut.ShowBezel,
            new Size(RenderWidth, RenderHeight),
            sut.Stretch);
    }

    private static DotMatrixLayout CreateLayout(
        EPaperDisplay sut,
        int columns,
        int rows)
        => DotMatrixLayout.Create(
            new DotMatrixGeometry(columns, rows, 0, 0, 0, 0, 0),
            sut.ShowBezel,
            new Size(RenderWidth, RenderHeight),
            sut.Stretch);

    private static Point Centre(Rect rect)
        => new(rect.X + (rect.Width / 2), rect.Y + (rect.Height / 2));

    private static Func<Point, Color> Render(FrameworkElement sut)
    {
        // Buffer changes are copied by an update queued on the dispatcher.
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