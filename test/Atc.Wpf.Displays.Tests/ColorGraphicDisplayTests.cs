namespace Atc.Wpf.Displays.Tests;

public sealed class ColorGraphicDisplayTests
{
    private const int RenderWidth = 640;
    private const int RenderHeight = 320;
    private const int SmallColumns = 16;
    private const int SmallRows = 8;
    private const int Backlight = 14;
    private static readonly Color Red = Color.FromRgb(0xFF, 0x00, 0x00);
    private static readonly Color Mid = Color.FromRgb(0x80, 0x40, 0x20);
    private static readonly Color Blue = Color.FromRgb(0x00, 0x00, 0xFF);
    private static readonly Color TftBlack = Color.FromRgb(Backlight, Backlight, Backlight);

    [StaFact]
    public void Frame_ShowsThePixelColours()
    {
        var sut = CreateSut(ColorPanelKind.Oled);
        sut.Buffer.SetPixel(0, 0, Red);
        sut.Buffer.SetPixel(1, 0, Mid);
        sut.Buffer.SetPixel(SmallColumns - 1, SmallRows - 1, Blue);

        var pixels = Render(sut);
        var layout = CreateLayout(sut);

        Assert.Equal(Red, pixels(Centre(layout.GetDotRect(0, 0))));
        Assert.Equal(Mid, pixels(Centre(layout.GetDotRect(1, 0))));
        Assert.Equal(Blue, pixels(Centre(layout.GetDotRect(SmallColumns - 1, SmallRows - 1))));
        Assert.Equal(Colors.Black, pixels(Centre(layout.GetDotRect(2, 0))));
    }

    [StaFact]
    public void TftPanel_LiftsBlackToTheBacklight()
    {
        var sut = CreateSut(ColorPanelKind.Tft);
        sut.Buffer.SetPixel(0, 0, Red);
        sut.Buffer.SetPixel(1, 0, Mid);

        var pixels = Render(sut);
        var layout = CreateLayout(sut);

        // c' = 14 + c * 241 / 255: 0 -> 14, 255 -> 255, 0x80 -> 134, 0x40 -> 74, 0x20 -> 44.
        AssertClose(TftBlack, pixels(Centre(layout.GetDotRect(2, 0))));
        AssertClose(Color.FromRgb(255, Backlight, Backlight), pixels(Centre(layout.GetDotRect(0, 0))));
        AssertClose(Color.FromRgb(134, 74, 44), pixels(Centre(layout.GetDotRect(1, 0))));
    }

    [StaFact]
    public void OledPanel_KeepsTrueBlack()
    {
        var sut = CreateSut(ColorPanelKind.Oled);
        sut.Buffer.SetPixel(0, 0, Red);

        var pixels = Render(sut);
        var layout = CreateLayout(sut);

        Assert.Equal(Colors.Black, pixels(Centre(layout.GetDotRect(2, 0))));
        Assert.Equal(Red, pixels(Centre(layout.GetDotRect(0, 0))));
    }

    [StaFact]
    public void PanelKind_ChangedAfterAFrame_RecolorsTheFrame()
    {
        var sut = CreateSut(ColorPanelKind.Oled);
        var layout = CreateLayout(sut);
        Assert.Equal(Colors.Black, Render(sut)(Centre(layout.GetDotRect(2, 0))));

        sut.PanelKind = ColorPanelKind.Tft;

        AssertClose(TftBlack, Render(sut)(Centre(layout.GetDotRect(2, 0))));
    }

    [StaFact]
    public void PixelSize_ChangedAfterAFrame_RecreatesThePicture()
    {
        var sut = CreateSut(ColorPanelKind.Oled);
        sut.Buffer.SetPixel(0, 0, Red);
        Assert.Equal(Red, Render(sut)(Centre(CreateLayout(sut).GetDotRect(0, 0))));

        sut.PixelColumns = 8;
        sut.Buffer.SetPixel(7, 7, Blue);

        var pixels = Render(sut);
        var layout = CreateLayout(sut);
        Assert.Equal(Red, pixels(Centre(layout.GetDotRect(0, 0))));
        Assert.Equal(Blue, pixels(Centre(layout.GetDotRect(7, 7))));
    }

    [StaFact]
    public void ExternalBuffer_WithNoChangedRows_IsShownWhole()
    {
        var sut = CreateSut(ColorPanelKind.Oled);
        var external = new ColorFrameBuffer(SmallColumns, SmallRows);
        external.SetPixel(0, 0, Blue);
        _ = external.TryTakeDirtyRows(out _, out _);
        var centre = Centre(CreateLayout(sut).GetDotRect(0, 0));
        Assert.Equal(Colors.Black, Render(sut)(centre));

        sut.Buffer = external;

        Assert.Equal(Blue, Render(sut)(centre));
    }

    [StaFact]
    public void Inverted_InvertsTheColours()
    {
        // ref: §TFT.2 (0x21 INVON / 0x20 INVOFF invert the shown colour, RAM unchanged)
        var sut = CreateSut(ColorPanelKind.Oled);
        sut.Buffer.SetPixel(0, 0, Red);
        sut.Buffer.SetPixel(1, 0, Mid);

        sut.Buffer.IsInverted = true;

        var pixels = Render(sut);
        var layout = CreateLayout(sut);
        Assert.Equal(Color.FromRgb(0x00, 0xFF, 0xFF), pixels(Centre(layout.GetDotRect(0, 0))));
        Assert.Equal(Color.FromRgb(0x7F, 0xBF, 0xDF), pixels(Centre(layout.GetDotRect(1, 0))));
        Assert.Equal(Colors.White, pixels(Centre(layout.GetDotRect(2, 0))));

        sut.Buffer.IsInverted = false;

        Assert.Equal(Red, Render(sut)(Centre(layout.GetDotRect(0, 0))));
    }

    [StaFact]
    public void DisplayOff_ShowsOnlyThePanel()
    {
        var sut = CreateSut(ColorPanelKind.Tft);
        sut.Buffer.Clear(Colors.White);
        var layout = CreateLayout(sut);
        var centre = Centre(layout.GetDotRect(3, 3));

        sut.Buffer.IsDisplayOn = false;
        AssertClose(TftBlack, Render(sut)(centre));

        sut.Buffer.IsDisplayOn = true;
        Assert.Equal(Colors.White, Render(sut)(centre));

        sut.Buffer.IsSleeping = true;
        AssertClose(TftBlack, Render(sut)(centre));
    }

    [StaFact]
    public void Brightness_DimsThePicture()
    {
        var sut = CreateSut(ColorPanelKind.Oled);
        sut.Buffer.Clear(Colors.White);
        var centre = Centre(CreateLayout(sut).GetDotRect(3, 3));

        sut.Buffer.Brightness = 0;
        Assert.Equal(Colors.Black, Render(sut)(centre));

        sut.Buffer.Brightness = 128;
        var half = Render(sut)(centre);
        Assert.InRange(half.R, 120, 136);
    }

    [StaFact]
    public void PixelGrid_DrawnOnlyWhenPixelsAreLarge()
    {
        // 16 x 8 pixels at 640 x 320: a pixel is 26.7 DIP and a gap 3.2 DIP, centred on the pixel border.
        var large = CreateSut(ColorPanelKind.Oled);
        large.Buffer.Clear(Colors.White);
        var largeLayout = CreateLayout(large);
        var border = new Point(largeLayout.GetDotRect(1, 0).X, Centre(largeLayout.GetDotRect(1, 0)).Y);

        Assert.Equal(Colors.Black, Render(large)(border));

        large.ShowPixelGrid = false;
        Assert.Equal(Colors.White, Render(large)(border));

        // 480 x 8 pixels at 640 x 320: a pixel is 1.3 DIP, under the 4 DIP threshold.
        var small = CreateSut(ColorPanelKind.Oled, columns: 480);
        small.Buffer.Clear(Colors.White);
        var smallLayout = CreateLayout(small);
        var smallBorder = new Point(smallLayout.GetDotRect(101, 0).X, Centre(smallLayout.GetDotRect(101, 4)).Y);

        Assert.True(small.ShowPixelGrid);
        Assert.Equal(Colors.White, Render(small)(smallBorder));
    }

    [StaFact]
    public void FramesFromAWorkerThread_AreCoalescedIntoOneRedraw()
    {
        var sut = CreateSut(ColorPanelKind.Oled);
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
                    // Pixel (i % 16, i / 16) is red (0xF800, big-endian), the rest black.
                    var frame = new byte[SmallColumns * SmallRows * 2];
                    frame[i * 2] = 0xF8;
                    buffer.LoadFrameRgb565(frame);
                }
            }).Wait();

            Pump();

            Assert.Equal(1, copies);
            sut.FrameCopiedForTests = null;

            // The last frame lit only pixel 99 = (3, 6).
            var pixels = Render(sut);
            var layout = CreateLayout(sut);
            Assert.Equal(Red, pixels(Centre(layout.GetDotRect(3, 6))));
            Assert.Equal(Colors.Black, pixels(Centre(layout.GetDotRect(2, 6))));
        }
        finally
        {
            window.Close();
            Pump();
        }
    }

    [StaFact]
    public void FullFrame320x240_UpdatesWithinBudget()
    {
        const int width = 320;
        const int height = 240;
        const int frames = 50;
        var sut = new ColorGraphicDisplay { PixelColumns = width, PixelRows = height, PanelKind = ColorPanelKind.Tft };
        var buffer = sut.Buffer;
        buffer.IsInverted = true;
        var frameA = CreateGradientFrame(width, height, 0);
        var frameB = CreateGradientFrame(width, height, 1);

        for (var i = 0; i < 10; i++)
        {
            buffer.LoadFrameRgb565(i % 2 == 0 ? frameA : frameB);
            sut.UpdateBitmap();
        }

        var copies = 0;
        sut.FrameCopiedForTests = () => copies++;
        var elapsed = TimeSpan.Zero;
        var allocated = 0L;
        var stopwatch = new Stopwatch();
        for (var i = 0; i < frames; i++)
        {
            buffer.LoadFrameRgb565(i % 2 == 0 ? frameA : frameB);

            var before = GC.GetAllocatedBytesForCurrentThread();
            stopwatch.Restart();
            sut.UpdateBitmap();
            stopwatch.Stop();
            allocated += GC.GetAllocatedBytesForCurrentThread() - before;
            elapsed += stopwatch.Elapsed;
        }

        Assert.Equal(frames, copies);
        Assert.True(allocated / frames < 1024, $"Allocated {allocated / frames} bytes per frame.");

        var isOptimized = typeof(ColorGraphicDisplay).Assembly.GetCustomAttribute<DebuggableAttribute>()?.IsJITOptimizerDisabled != true;
        if (isOptimized && !Debugger.IsAttached)
        {
            var perFrame = elapsed.TotalMilliseconds / frames;
            Assert.True(perFrame <= 5.0, $"A frame took {perFrame:F2} ms.");
        }
    }

    [StaFact]
    public void Text_DrawsInTheTextColourOnBlack()
    {
        var sut = new ColorGraphicDisplay { Text = "A", TextColor = Red };

        // 'A' in the A00 ROM: row 0 is 0x0E (.XXX.).
        Assert.Equal(Colors.Black, sut.Buffer.GetPixel(0, 0));
        Assert.Equal(Red, sut.Buffer.GetPixel(1, 0));

        sut.TextColor = Blue;

        Assert.Equal(Blue, sut.Buffer.GetPixel(1, 0));
    }

    [StaFact]
    public void ExternalBuffer_IsResizedAndTakesOverTheText()
    {
        var sut = new ColorGraphicDisplay { PixelColumns = 64, PixelRows = 32, Text = "A" };
        var buffer = new ColorFrameBuffer(128, 160);

        sut.Buffer = buffer;

        Assert.Same(buffer, sut.Buffer);
        Assert.Equal(64, buffer.Width);
        Assert.Equal(32, buffer.Height);
        Assert.Equal(Colors.White, buffer.GetPixel(1, 0));
    }

    [StaFact]
    public void Buffer_SetToNull_ReturnsToTheOwnBuffer()
    {
        var sut = new ColorGraphicDisplay();
        var own = sut.Buffer;
        sut.Buffer = new ColorFrameBuffer();

        sut.Buffer = null!;

        Assert.Same(own, sut.Buffer);
    }

    [StaFact]
    public void PixelSize_IsCoerced()
    {
        var sut = new ColorGraphicDisplay { PixelColumns = 500, PixelRows = 0 };

        Assert.Equal(480, sut.PixelColumns);
        Assert.Equal(1, sut.PixelRows);
        Assert.Equal(480, sut.Buffer.Width);
        Assert.Equal(1, sut.Buffer.Height);
    }

    [StaFact]
    public void Render_AtZeroSize_DoesNotThrow()
    {
        var sut = new ColorGraphicDisplay { Text = "HI" };
        sut.Measure(new Size(0, 0));
        sut.Arrange(new Rect(0, 0, 0, 0));

        var bitmap = new RenderTargetBitmap(1, 1, 96, 96, PixelFormats.Pbgra32);
        var exception = Record.Exception(() => bitmap.Render(sut));

        Assert.Null(exception);
    }

    [StaFact]
    public void AutomationPeer_NamesTheResolution()
    {
        var sut = new ColorGraphicDisplay { PixelColumns = 240, PixelRows = 320 };

        var peer = UIElementAutomationPeer.CreatePeerForElement(sut);

        Assert.IsType<ColorGraphicDisplayAutomationPeer>(peer);
        Assert.Equal("Display 240x320", peer.GetName());
        Assert.Equal(nameof(ColorGraphicDisplay), peer.GetClassName());
    }

    private static ColorGraphicDisplay CreateSut(
        ColorPanelKind kind,
        int columns = SmallColumns,
        int rows = SmallRows)
        => new()
        {
            PixelColumns = columns,
            PixelRows = rows,
            PanelKind = kind,
            ShowBezel = false,
        };

    private static byte[] CreateGradientFrame(
        int width,
        int height,
        int seed)
    {
        var frame = new byte[width * height * 2];
        for (var i = 0; i < width * height; i++)
        {
            var value = (ushort)((i * 7) + (seed * 0x1234) + 1);
            frame[i * 2] = (byte)(value >> 8);
            frame[(i * 2) + 1] = (byte)value;
        }

        return frame;
    }

    private static void AssertClose(
        Color expected,
        Color actual)
    {
        Assert.InRange(actual.R, expected.R - 1, expected.R + 1);
        Assert.InRange(actual.G, expected.G - 1, expected.G + 1);
        Assert.InRange(actual.B, expected.B - 1, expected.B + 1);
    }

    private static Window CreateOffScreenWindow(ColorGraphicDisplay content)
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

    private static DotMatrixLayout CreateLayout(ColorGraphicDisplay sut)
        => DotMatrixLayout.Create(
            new DotMatrixGeometry(sut.PixelColumns, sut.PixelRows, 0, 0, 0, 0, 0),
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