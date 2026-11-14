namespace Atc.Wpf.Controls.Tests.Displays;

public sealed class CharacterDisplayTests
{
    private const int RenderWidth = 800;
    private const int RenderHeight = 220;
    private static readonly Color PanelColor = Color.FromRgb(0x00, 0x00, 0x00);
    private static readonly Color LitColor = Color.FromRgb(0xFF, 0xFF, 0xFF);
    private static readonly byte[] FullBlock = [0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F];

    [StaFact]
    public void Text_ShowsEachLineFromColumnZero()
    {
        var sut = new CharacterDisplay();

        sut.Text = "AB\nCD";

        Assert.Equal(
            "AB              \nCD              ",
            sut.Buffer.GetVisibleText(CharacterRom.A00));
    }

    [StaFact]
    public void Text_WithCrLfAndExtraLines_ShowsOnlyTheRows()
    {
        var sut = new CharacterDisplay();

        sut.Text = "A\r\nB\r\nC";

        Assert.Equal(
            "A               \nB               ",
            sut.Buffer.GetVisibleText(CharacterRom.A00));
    }

    [StaFact]
    public void Text_SetAgain_BlanksTheOldCharacters()
    {
        var sut = new CharacterDisplay { Text = "HELLO\nWORLD" };

        sut.Text = "HI";

        Assert.Equal(
            "HI              \n                ",
            sut.Buffer.GetVisibleText(CharacterRom.A00));
    }

    [StaFact]
    public void SettingText_KeepsCustomGlyphsAndCursor()
    {
        var sut = new CharacterDisplay();
        byte[] glyph = [0x01, 0x02, 0x04, 0x08, 0x10, 0x08, 0x04, 0x02];
        sut.DefineCharacter(2, glyph);
        sut.CursorMode = CharacterDisplayCursor.Underline;
        sut.CursorRow = 1;
        sut.CursorColumn = 3;

        sut.Text = "HI\nTHERE";

        Assert.Equal(glyph, sut.Buffer.GetCustomGlyph(2));
        Assert.Equal(CharacterDisplayCursor.Underline, sut.Buffer.CursorMode);
        Assert.Equal((1, 3), sut.Buffer.GetCursorCell());
        Assert.Equal(0, sut.Buffer.ShiftOffset);
    }

    [StaFact]
    public void ChangingTheSize_KeepsTheContentAtItsPositions()
    {
        var sut = new CharacterDisplay { Text = "HELLO" };

        sut.Columns = 20;

        Assert.Equal(20, sut.Buffer.Columns);
        Assert.Equal(
            "HELLO               \n                    ",
            sut.Buffer.GetVisibleText(CharacterRom.A00));
    }

    [StaFact]
    public void GrowingTheDisplay_ShowsTheTextAgain()
    {
        var sut = new CharacterDisplay
        {
            Columns = 8,
            Rows = 1,
            Text = "HELLO WORLD",
        };
        Assert.Equal("HELLO WO", sut.Buffer.GetVisibleText(CharacterRom.A00));

        sut.Columns = 16;

        Assert.Equal("HELLO WORLD     ", sut.Buffer.GetVisibleText(CharacterRom.A00));
    }

    [StaFact]
    public void GrowingTheRows_ShowsTheLinesAgain()
    {
        var sut = new CharacterDisplay { Text = "ONE\nTWO\nTHREE" };

        sut.Rows = 4;

        Assert.Equal(
            "ONE             \nTWO             \nTHREE           \n                ",
            sut.Buffer.GetVisibleText(CharacterRom.A00));
    }

    [StaFact]
    public void CustomCharacters_AreAppliedToTheGlyphRam()
    {
        byte[] first = [0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11, 0x11];
        byte[] second = [0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04];
        var sut = new CharacterDisplay();

        sut.CustomCharacters = [first, second];

        Assert.Equal(first, sut.Buffer.GetCustomGlyph(0));
        Assert.Equal(second, sut.Buffer.GetCustomGlyph(1));
    }

    [StaFact]
    public void CustomCharacter_IsDrawnForItsCode()
    {
        var sut = CreateSut();
        sut.DefineCharacter(0, FullBlock);
        sut.Text = "\u0000";

        var pixels = Render(sut);
        var layout = CreateLayout(sut);

        for (var y = 0; y < 8; y++)
        {
            for (var x = 0; x < 5; x++)
            {
                Assert.Equal(LitColor, pixels(Centre(layout.GetDotRect(x, y))));
            }
        }

        Assert.NotEqual(LitColor, pixels(Centre(layout.GetDotRect(5, 0))));
    }

    [StaFact]
    public void RomCharacter_IsDrawnFromTheRom()
    {
        var sut = CreateSut();
        sut.Text = "A";

        var pixels = Render(sut);
        var layout = CreateLayout(sut);
        var glyph = CharacterRomTables.GetGlyph(CharacterRom.A00, (byte)'A').ToArray();

        for (var y = 0; y < 8; y++)
        {
            for (var x = 0; x < 5; x++)
            {
                var isLit = (glyph[y] & (0x10 >> x)) != 0;
                Assert.Equal(isLit, pixels(Centre(layout.GetDotRect(x, y))) == LitColor);
            }
        }
    }

    [StaTheory]
    [InlineData(0, 2)]
    [InlineData(41, 2)]
    [InlineData(16, 0)]
    [InlineData(16, 5)]
    public void Columns_OutOfRange_IsRejected(
        int columns,
        int rows)
    {
        var sut = new CharacterDisplay();

        Assert.ThrowsAny<ArgumentException>(() =>
        {
            sut.Columns = columns;
            sut.Rows = rows;
        });
    }

    [StaFact]
    public void BoundBuffer_DrivenByTheEmulator_IsShown()
    {
        var shared = new CharacterDisplayBuffer(20, 4);
        var sut = CreateSut();
        sut.Buffer = shared;
        Render(sut);
        var (litX, litY) = FindDot(CharacterRomTables.GetGlyph(CharacterRom.A00, (byte)'Q'), isLit: true);

        new Hd44780Controller(shared).Data((byte)'Q');
        Pump();

        var pixels = Render(sut);
        var layout = CreateLayout(sut);
        Assert.Same(shared, sut.Buffer);
        Assert.Equal(16, shared.Columns);
        Assert.Equal(2, shared.Rows);
        Assert.Equal(LitColor, pixels(Centre(layout.GetDotRect(litX, litY))));
    }

    [StaFact]
    public void Buffer_SetToNull_FallsBackToTheOwnBuffer()
    {
        var sut = new CharacterDisplay();
        var own = sut.Buffer;
        sut.Buffer = new CharacterDisplayBuffer(16, 2);

        sut.Buffer = null!;

        Assert.Same(own, sut.Buffer);
    }

    [StaFact]
    public void ChangesFromAWorkerThread_RedrawOnTheUiThread()
    {
        var sut = CreateSut();
        var (litX, litY) = FindDot(CharacterRomTables.GetGlyph(CharacterRom.A00, (byte)'Z'), isLit: true);
        var layout = CreateLayout(sut);
        var dot = Centre(layout.GetDotRect(litX, 8 + litY));
        Assert.NotEqual(LitColor, Render(sut)(dot));
        var buffer = sut.Buffer;
        Exception? workerException = null;

        // A plain thread joined synchronously: the dispatcher is pumped below, on this STA thread.
        var worker = new Thread(() =>
        {
            try
            {
                buffer.WriteVisible(1, 0, "Z", CharacterRom.A00);
            }
            catch (InvalidOperationException ex)
            {
                workerException = ex;
            }
        });
        worker.Start();
        worker.Join();
        Pump();

        Assert.Null(workerException);
        Assert.Equal(LitColor, Render(sut)(dot));
    }

    [StaFact]
    public void BlinkingCursor_RunsATimerOnlyWhileLoaded()
    {
        var sut = new CharacterDisplay { CursorMode = CharacterDisplayCursor.Blink };
        Assert.False(sut.IsBlinkTimerRunning);

        var window = new Window
        {
            Content = sut,
            Left = -10000,
            Top = -10000,
            Width = 400,
            Height = 150,
            ShowActivated = false,
            ShowInTaskbar = false,
            WindowStyle = WindowStyle.None,
        };

        try
        {
            window.Show();
            Pump();
            Assert.True(sut.IsBlinkTimerRunning);

            sut.CursorMode = CharacterDisplayCursor.Underline;
            Assert.False(sut.IsBlinkTimerRunning);

            sut.CursorMode = CharacterDisplayCursor.UnderlineAndBlink;
            Assert.True(sut.IsBlinkTimerRunning);

            sut.Visibility = Visibility.Collapsed;
            Pump();
            Assert.False(sut.IsBlinkTimerRunning);

            sut.Visibility = Visibility.Visible;
            Pump();
            Assert.True(sut.IsBlinkTimerRunning);
        }
        finally
        {
            window.Close();
            Pump();
        }

        Assert.False(sut.IsBlinkTimerRunning);
    }

    [StaFact]
    public void BlinkingCursor_FromTheEmulator_StartsTheTimer()
    {
        var sut = new CharacterDisplay();
        var window = new Window
        {
            Content = sut,
            Left = -10000,
            Top = -10000,
            Width = 400,
            Height = 150,
            ShowActivated = false,
            ShowInTaskbar = false,
            WindowStyle = WindowStyle.None,
        };

        try
        {
            window.Show();
            Pump();
            Assert.False(sut.IsBlinkTimerRunning);

            // Display on, cursor off, blink on.
            new Hd44780Controller(sut.Buffer).Command(0x0D);
            Pump();

            Assert.True(sut.IsBlinkTimerRunning);
        }
        finally
        {
            window.Close();
            Pump();
        }
    }

    [StaFact]
    public void UnderlineCursor_LightsTheCursorRowOfItsCell()
    {
        var sut = CreateSut();
        sut.CursorRow = 1;
        sut.CursorColumn = 2;
        sut.CursorMode = CharacterDisplayCursor.Underline;

        var pixels = Render(sut);
        var layout = CreateLayout(sut);

        for (var x = 10; x < 15; x++)
        {
            Assert.Equal(LitColor, pixels(Centre(layout.GetDotRect(x, 15))));
            Assert.NotEqual(LitColor, pixels(Centre(layout.GetDotRect(x, 14))));
        }

        Assert.NotEqual(LitColor, pixels(Centre(layout.GetDotRect(9, 15))));
        Assert.NotEqual(LitColor, pixels(Centre(layout.GetDotRect(15, 15))));
    }

    [StaFact]
    public void BlinkCursor_ShowsABlockOnTheCursorCell()
    {
        var sut = CreateSut();
        sut.CursorMode = CharacterDisplayCursor.Blink;

        var pixels = Render(sut);
        var layout = CreateLayout(sut);

        for (var y = 0; y < 8; y++)
        {
            for (var x = 0; x < 5; x++)
            {
                Assert.Equal(LitColor, pixels(Centre(layout.GetDotRect(x, y))));
            }
        }
    }

    [StaFact]
    public void Vfd_DrawsGlyphRowSevenFromTheRom()
    {
        var sut = CreateSut();
        sut.Kind = CharacterDisplayKind.Vfd;
        sut.GlowRadius = 0;
        sut.DotCornerRadius = 0;
        sut.CharacterRom = CharacterRom.A02;
        sut.Text = "A";
        var rowSeven = CharacterRomTables.GetGlyph(CharacterRom.A02, CharacterRomTables.Map(CharacterRom.A02, 'A'))[7];
        Assert.NotEqual(0, rowSeven);
        var litX = Enumerable.Range(0, 5).First(x => (rowSeven & (0x10 >> x)) != 0);

        var pixels = Render(sut);
        var layout = CreateLayout(sut);

        Assert.Equal(LitColor, pixels(Centre(layout.GetDotRect(litX, 7))));
    }

    [StaFact]
    public void Vfd_CursorUnderline_IsAddedToTheGlyph()
    {
        var sut = CreateSut();
        sut.Kind = CharacterDisplayKind.Vfd;
        sut.GlowRadius = 0;
        sut.DotCornerRadius = 0;
        sut.Text = "A";
        sut.CursorMode = CharacterDisplayCursor.Underline;

        var pixels = Render(sut);
        var layout = CreateLayout(sut);
        var (litX, litY) = FindDot(CharacterRomTables.GetGlyph(CharacterRom.A00, (byte)'A'), isLit: true);

        Assert.Equal(LitColor, pixels(Centre(layout.GetDotRect(litX, litY))));
        for (var x = 0; x < 5; x++)
        {
            Assert.Equal(LitColor, pixels(Centre(layout.GetDotRect(x, 7))));
        }
    }

    [StaFact]
    public void BlinkingCursorFromTheBuffer_WhileCollapsed_DoesNotStartTheTimer()
    {
        var sut = new CharacterDisplay();
        var window = CreateOffScreenWindow(sut);

        try
        {
            window.Show();
            Pump();
            sut.Visibility = Visibility.Collapsed;
            Pump();

            sut.Buffer.CursorMode = CharacterDisplayCursor.Blink;
            Pump();

            Assert.False(sut.IsBlinkTimerRunning);
        }
        finally
        {
            window.Close();
            Pump();
        }
    }

    [StaFact]
    public void SwappingToABufferWithoutBlink_StopsTheTimer()
    {
        // The blink comes from the buffer (as from an emulator); a CursorMode set on the control would be carried to the new buffer.
        var sut = new CharacterDisplay();
        sut.Buffer.CursorMode = CharacterDisplayCursor.Blink;
        var window = CreateOffScreenWindow(sut);

        try
        {
            window.Show();
            Pump();
            Assert.True(sut.IsBlinkTimerRunning);

            sut.Buffer = new CharacterDisplayBuffer(16, 2);
            Pump();

            Assert.False(sut.IsBlinkTimerRunning);
        }
        finally
        {
            window.Close();
            Pump();
        }
    }

    [StaFact]
    public void BufferChange_DuringARender_IsDrawnByTheNextFrame()
    {
        var sut = CreateSut();
        var (litX, litY) = FindDot(CharacterRomTables.GetGlyph(CharacterRom.A00, (byte)'Z'), isLit: true);
        var dot = Centre(CreateLayout(sut).GetDotRect(litX, 8 + litY));
        Assert.NotEqual(LitColor, Render(sut)(dot));

        // The write lands after the snapshot was taken but before the base clears its dirty flag, as a worker write can.
        sut.AfterSnapshotForTests = () =>
        {
            sut.AfterSnapshotForTests = null;
            sut.Buffer.WriteVisible(1, 0, "Z", CharacterRom.A00);
        };
        sut.InvalidateVisual();
        sut.UpdateLayout();
        Assert.Null(sut.AfterSnapshotForTests);

        Pump();

        Assert.Equal(LitColor, Render(sut)(dot));
    }

    [StaFact]
    public void DisplayOff_ShowsNoLitDots_ButKeepsThePanel()
    {
        var sut = CreateSut();
        sut.GhostOpacity = 0.2;
        sut.DefineCharacter(0, FullBlock);
        sut.Text = "\u0000";
        sut.CursorMode = CharacterDisplayCursor.Underline;

        sut.IsDisplayOn = false;

        var pixels = Render(sut);
        var layout = CreateLayout(sut);
        var dot = pixels(Centre(layout.GetDotRect(0, 0)));
        var lastOfFirstCell = layout.GetDotRect(4, 3);
        var firstOfSecondCell = layout.GetDotRect(5, 3);
        var gap = new Point((lastOfFirstCell.Right + firstOfSecondCell.Left) / 2, Centre(lastOfFirstCell).Y);

        Assert.False(sut.Buffer.IsDisplayOn);
        Assert.NotEqual(LitColor, dot);
        Assert.NotEqual(PanelColor, dot);
        Assert.NotEqual(LitColor, pixels(Centre(layout.GetDotRect(0, 7))));
        Assert.Equal(PanelColor, pixels(gap));
    }

    [StaFact]
    public void LcdBacklightOff_UsesTheReflectivePalette()
    {
        var backlit = new CharacterDisplay();
        var unlit = new CharacterDisplay { IsBacklightOn = false };
        var layout = CreateLayout(backlit);
        var lastOfFirstCell = layout.GetDotRect(4, 3);
        var firstOfSecondCell = layout.GetDotRect(5, 3);
        var gap = new Point((lastOfFirstCell.Right + firstOfSecondCell.Left) / 2, Centre(lastOfFirstCell).Y);

        var backlitPanel = Render(backlit)(gap);
        var unlitPanel = Render(unlit)(gap);

        Assert.NotEqual(backlitPanel, unlitPanel);
        Assert.Equal(CharacterDisplayPalette.Get(CharacterDisplayKind.Lcd, CharacterDisplayColor.YellowGreen, isBacklightOn: false).Panel, unlitPanel);
        Assert.True(unlit.GhostOpacity < backlit.GhostOpacity, "The reflective panel shows weaker ghost dots.");
    }

    [StaFact]
    public void ColorPreset_SetsTheBrushes()
    {
        var sut = new CharacterDisplay();

        sut.Color = CharacterDisplayColor.Blue;

        var palette = CharacterDisplayPalette.Get(CharacterDisplayKind.Lcd, CharacterDisplayColor.Blue, isBacklightOn: true);
        Assert.Equal(palette.Panel, ((SolidColorBrush)sut.PanelBrush!).Color);
        Assert.Equal(palette.Ink, ((SolidColorBrush)sut.LitDotBrush!).Color);
    }

    [StaFact]
    public void Vfd_WithAnLcdOnlyPreset_FallsBackToBlueGreen()
    {
        var sut = new CharacterDisplay
        {
            Color = CharacterDisplayColor.YellowGreen,
            Kind = CharacterDisplayKind.Vfd,
        };

        var blueGreen = CharacterDisplayPalette.Get(CharacterDisplayKind.Vfd, CharacterDisplayColor.BlueGreen, isBacklightOn: true);
        Assert.Equal(blueGreen.Ink, ((SolidColorBrush)sut.LitDotBrush!).Color);
        Assert.Equal(0.35, sut.DotCornerRadius);
        Assert.Equal(0.6, sut.GlowRadius);

        sut.Kind = CharacterDisplayKind.Lcd;

        Assert.Equal(0.0, sut.DotCornerRadius);
        Assert.Equal(0.0, sut.GlowRadius);
    }

    [StaFact]
    public void ExplicitBrush_WinsOverThePreset()
    {
        var mine = new SolidColorBrush(Colors.HotPink);
        var sut = new CharacterDisplay { PanelBrush = mine };

        sut.Color = CharacterDisplayColor.Red;
        sut.Kind = CharacterDisplayKind.Vfd;
        sut.IsBacklightOn = false;

        Assert.Same(mine, sut.PanelBrush);
    }

    [StaFact]
    public void CursorMode_CanBeSetFromXaml_BeforeTheSize()
    {
        const string xaml =
            "<CharacterDisplay xmlns=\"https://github.com/atc-net/atc-wpf/tree/main/schemas\" " +
            "CursorMode=\"Blink\" CursorRow=\"3\" CursorColumn=\"18\" Columns=\"20\" Rows=\"4\" Text=\"HI\" />";

        var sut = (CharacterDisplay)XamlReader.Parse(xaml);

        Assert.Equal(CharacterDisplayCursor.Blink, sut.CursorMode);
        Assert.Equal(CharacterDisplayCursor.Blink, sut.Buffer.CursorMode);
        Assert.Equal(20, sut.Buffer.Columns);
        Assert.Equal(4, sut.Buffer.Rows);
        Assert.Equal((3, 18), sut.Buffer.GetCursorCell());
    }

    [StaFact]
    public void Write_And_Clear_ChangeTheBuffer()
    {
        var sut = new CharacterDisplay();

        sut.Write(1, 3, "OK");

        Assert.Equal(
            "                \n   OK           ",
            sut.Buffer.GetVisibleText(CharacterRom.A00));

        sut.Clear();

        Assert.Equal(
            "                \n                ",
            sut.Buffer.GetVisibleText(CharacterRom.A00));
    }

    [StaFact]
    public void Buffer_SetAfterTheContentProperties_TakesThemOver()
    {
        var glyph = FullBlock;
        var sut = new CharacterDisplay
        {
            Text = "BOOTING",
            CursorMode = CharacterDisplayCursor.Underline,
            CursorRow = 1,
            CursorColumn = 4,
            IsDisplayOn = false,
            CustomCharacters = [null!, glyph],
        };
        var buffer = new CharacterDisplayBuffer(16, 2);

        sut.Buffer = buffer;

        Assert.Equal(
            "BOOTING         \n                ",
            buffer.GetVisibleText(CharacterRom.A00));
        Assert.Equal(CharacterDisplayCursor.Underline, buffer.CursorMode);
        Assert.Equal((1, 4), buffer.GetCursorCell());
        Assert.False(buffer.IsDisplayOn);
        Assert.Equal(glyph, buffer.GetCustomGlyph(1));
    }

    [StaFact]
    public void Buffer_BoundLate_TakesOverTheContentProperties()
    {
        var sut = new CharacterDisplay();
        sut.SetBinding(CharacterDisplay.BufferProperty, new Binding());
        sut.Text = "HI";
        sut.CursorMode = CharacterDisplayCursor.Blink;
        sut.CustomCharacters = [FullBlock];
        var buffer = new CharacterDisplayBuffer(16, 2);

        sut.DataContext = buffer;
        DispatcherHelper.DoEvents();

        Assert.Same(buffer, sut.Buffer);
        Assert.Equal(
            "HI              \n                ",
            buffer.GetVisibleText(CharacterRom.A00));
        Assert.Equal(CharacterDisplayCursor.Blink, buffer.CursorMode);
        Assert.Equal(FullBlock, buffer.GetCustomGlyph(0));
    }

    [StaFact]
    public void Buffer_SetOnAControlWithDefaults_KeepsTheBufferState()
    {
        var sut = new CharacterDisplay();
        var buffer = new CharacterDisplayBuffer(16, 2)
        {
            CursorMode = CharacterDisplayCursor.Blink,
            IsDisplayOn = false,
            CursorAddress = 0x45,
        };
        buffer.WriteVisible(0, 0, "DEVICE", CharacterRom.A00);
        buffer.SetCgram(3, 0, 0x1F);

        sut.Buffer = buffer;

        Assert.Equal(
            "DEVICE          \n                ",
            buffer.GetVisibleText(CharacterRom.A00));
        Assert.Equal(CharacterDisplayCursor.Blink, buffer.CursorMode);
        Assert.False(buffer.IsDisplayOn);
        Assert.Equal((1, 5), buffer.GetCursorCell());
        Assert.Equal(0x1F, buffer.GetCustomGlyph(3)[0]);
    }

    [StaFact]
    public void AutomationName_ReadsTheDegreeSign()
    {
        var sut = new CharacterDisplay { Text = "21.5°C" };

        Assert.StartsWith("21.5°C", sut.GetDisplayedText(), StringComparison.Ordinal);
    }

    [StaFact]
    public void AutomationPeer_NameIsTheVisibleText()
    {
        var sut = new CharacterDisplay();
        sut.DefineCharacter(0, FullBlock);
        sut.Text = "HI\n\u0000X";

        var peer = UIElementAutomationPeer.CreatePeerForElement(sut);

        Assert.IsType<CharacterDisplayAutomationPeer>(peer);
        Assert.Equal("HI              \n X              ", peer.GetName());
    }

    private static CharacterDisplay CreateSut()
        => new()
        {
            ShowBezel = false,
            PanelBrush = new SolidColorBrush(PanelColor),
            LitDotBrush = new SolidColorBrush(LitColor),
            UnlitDotBrush = new SolidColorBrush(LitColor),
            GhostOpacity = 0,
        };

    private static (int X, int Y) FindDot(
        ReadOnlySpan<byte> glyph,
        bool isLit)
    {
        for (var y = 0; y < 7; y++)
        {
            for (var x = 0; x < 5; x++)
            {
                if (((glyph[y] & (0x10 >> x)) != 0) == isLit)
                {
                    return (x, y);
                }
            }
        }

        throw new InvalidOperationException("No such dot in the glyph.");
    }

    private static Window CreateOffScreenWindow(CharacterDisplay content)
        => new()
        {
            Content = content,
            Left = -10000,
            Top = -10000,
            Width = 400,
            Height = 150,
            ShowActivated = false,
            ShowInTaskbar = false,
            WindowStyle = WindowStyle.None,
        };

    private static void Pump()
        => Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);

    private static DotMatrixLayout CreateLayout(CharacterDisplay sut)
        => DotMatrixLayout.Create(sut.CurrentGeometry, sut.ShowBezel, new Size(RenderWidth, RenderHeight), sut.Stretch);

    private static Point Centre(Rect rect)
        => new(rect.X + (rect.Width / 2), rect.Y + (rect.Height / 2));

    private static Func<Point, Color> Render(FrameworkElement sut)
    {
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