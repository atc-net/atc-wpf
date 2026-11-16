namespace Atc.Wpf.Displays.Tests;

public sealed class DotMatrixDisplayBaseTests
{
    private static readonly Color PanelColor = Color.FromRgb(0x00, 0x00, 0x00);
    private static readonly Color LitColor = Color.FromRgb(0xFF, 0xFF, 0xFF);

    [StaFact]
    public void Render_LitAndUnlitDots_UseTheirBrushes()
    {
        var sut = CreateSut(new DotMatrixGeometry(2, 1, 0, 0, 1.0, 0, 0), (0, 0));

        var pixels = Render(sut, 140, 100);
        var layout = CreateLayout(sut, 140, 100);

        Assert.Equal(LitColor, pixels(Centre(layout.GetDotRect(0, 0))));
        var ghost = pixels(Centre(layout.GetDotRect(1, 0)));
        Assert.NotEqual(LitColor, ghost);
        Assert.NotEqual(PanelColor, ghost);
        Assert.InRange(ghost.R, 1, 0x40);
    }

    [StaFact]
    public void Render_CellGaps_LeavePanelBetweenCells()
    {
        var lit = Enumerable.Range(0, 10)
            .SelectMany(x => Enumerable.Range(0, 8).Select(y => (x, y)))
            .ToArray();
        var sut = CreateSut(new DotMatrixGeometry(10, 8, 5, 8, 0.2, 3.0, 0), lit);

        var pixels = Render(sut, 300, 200);
        var layout = CreateLayout(sut, 300, 200);

        var lastOfFirstCell = layout.GetDotRect(4, 3);
        var firstOfSecondCell = layout.GetDotRect(5, 3);
        Assert.True(firstOfSecondCell.Left - lastOfFirstCell.Right > 10, "The cell gap is too narrow to sample.");
        var gap = new Point((lastOfFirstCell.Right + firstOfSecondCell.Left) / 2, Centre(lastOfFirstCell).Y);

        Assert.Equal(PanelColor, pixels(gap));
        Assert.Equal(LitColor, pixels(Centre(lastOfFirstCell)));
        Assert.Equal(LitColor, pixels(Centre(firstOfSecondCell)));
    }

    [StaFact]
    public void Render_KeepsTheAspectRatioAndCentres()
    {
        var sut = CreateSut(new DotMatrixGeometry(1, 1, 0, 0, 0, 0, 0), (0, 0));
        sut.ShowBezel = true;
        sut.BezelBrush = new SolidColorBrush(Color.FromRgb(0x40, 0x40, 0x40));

        var pixels = Render(sut, 100, 300);

        var opaqueRows = Enumerable.Range(0, 300)
            .Where(y => pixels(new Point(50.5, y + 0.5)).A > 0)
            .ToArray();
        Assert.NotEmpty(opaqueRows);
        Assert.InRange(opaqueRows[0], 99, 101);
        Assert.InRange(299 - opaqueRows[^1], 99, 101);

        var litColumns = Enumerable.Range(0, 100).Count(x => pixels(new Point(x + 0.5, 150.5)) == LitColor);
        var litRows = Enumerable.Range(0, 300).Count(y => pixels(new Point(50.5, y + 0.5)) == LitColor);
        Assert.True(litColumns > 5, $"The dot is too small: {litColumns} px.");
        Assert.InRange(litRows, litColumns - 1, litColumns + 1);
    }

    [StaFact]
    public void Brightness_ScalesTheLitDotIntensity()
    {
        var geometry = new DotMatrixGeometry(1, 1, 0, 0, 0, 0, 0);
        var full = CreateSut(geometry, (0, 0));
        var half = CreateSut(geometry, (0, 0));
        half.Brightness = 0.5;

        var centre = Centre(CreateLayout(full, 100, 100).GetDotRect(0, 0));
        var fullPixel = Render(full, 100, 100)(centre);
        var halfPixel = Render(half, 100, 100)(centre);

        Assert.Equal(LitColor, fullPixel);
        Assert.True(halfPixel.R < fullPixel.R - 0x40, $"The dimmed dot is not darker: {halfPixel}.");
        Assert.True(halfPixel.R > 0x40, $"The dimmed dot is too dark: {halfPixel}.");
    }

    [StaFact]
    public void Render_Glow_LightsThePanelAroundLitDots()
    {
        var geometry = new DotMatrixGeometry(3, 1, 0, 0, 1.0, 0, 0);
        var plain = CreateSut(geometry, (1, 0));
        var glowing = CreateSut(geometry, (1, 0));
        glowing.GlowRadius = 0.5;

        var dot = CreateLayout(plain, 180, 100).GetDotRect(1, 0);
        var besideDot = new Point(dot.Right + (dot.Width / 4), Centre(dot).Y);

        Assert.Equal(PanelColor, Render(plain, 180, 100)(besideDot));
        Assert.True(Render(glowing, 180, 100)(besideDot).R > 0x20, "The glow does not reach past the dot.");
    }

    [StaFact]
    public void Render_RoundDots_LeaveTheCornersToThePanel()
    {
        var sut = CreateSut(new DotMatrixGeometry(1, 1, 0, 0, 0, 0, 0), (0, 0));
        sut.DotCornerRadius = 0.5;

        var pixels = Render(sut, 100, 100);
        var dot = CreateLayout(sut, 100, 100).GetDotRect(0, 0);

        Assert.Equal(LitColor, pixels(Centre(dot)));
        Assert.Equal(PanelColor, pixels(new Point(dot.Left + 1, dot.Top + 1)));
    }

    [StaTheory]
    [InlineData(0.1, 0.25)]
    [InlineData(0.5, 0.5)]
    [InlineData(2.0, 1.0)]
    [InlineData(double.NaN, 1.0)]
    public void Brightness_IsCoercedToTheDimmingRange(
        double value,
        double expected)
    {
        var sut = new TestDotMatrixDisplay { Brightness = value };

        Assert.Equal(expected, sut.Brightness);
    }

    [StaFact]
    public void Measure_Unconstrained_ReturnsTheNaturalSize()
    {
        var geometry = new DotMatrixGeometry(20, 16, 5, 8, 0.1, 1.0, 1.0);
        var sut = CreateSut(geometry);

        sut.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

        var natural = DotMatrixLayout.GetNaturalSize(geometry, showBezel: false);
        Assert.True(natural.Width > 0);
        Assert.Equal(natural, sut.DesiredSize);
    }

    [StaFact]
    public void Measure_Constrained_FitsAndKeepsTheAspectRatio()
    {
        var geometry = new DotMatrixGeometry(20, 16, 5, 8, 0.1, 1.0, 1.0);
        var sut = CreateSut(geometry);

        sut.Measure(new Size(double.PositiveInfinity, 50));

        var natural = DotMatrixLayout.GetNaturalSize(geometry, showBezel: false);
        Assert.Equal(50, sut.DesiredSize.Height, 6);
        Assert.Equal(natural.Width * 50 / natural.Height, sut.DesiredSize.Width, 6);
    }

    [StaFact]
    public void Render_AtZeroSize_DrawsNothingAndDoesNotThrow()
    {
        var sut = CreateSut(new DotMatrixGeometry(2, 1, 0, 0, 1.0, 0, 0), (0, 0));

        sut.Measure(default);
        sut.Arrange(default);
        var bitmap = new RenderTargetBitmap(1, 1, 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(sut);
        var bytes = new byte[4];
        bitmap.CopyPixels(bytes, 4, 0);

        Assert.Equal(0, bytes[3]);
    }

    [StaFact]
    public void InvalidateDots_FromAWorkerThread_RedrawsOnTheUiThreadOnce()
    {
        var sut = CreateSut(new DotMatrixGeometry(2, 1, 0, 0, 1.0, 0, 0), (0, 0));
        Layout(sut, 140, 100);
        var renderCount = sut.RenderCount;
        Exception? workerException = null;

        // A plain thread joined synchronously: the dispatcher is pumped below, on this STA thread.
        var worker = new Thread(() =>
        {
            try
            {
                for (var i = 0; i < 10; i++)
                {
                    sut.RaiseInvalidateDots();
                }
            }
            catch (InvalidOperationException ex)
            {
                workerException = ex;
            }
        });
        worker.Start();
        worker.Join();

        Assert.Null(workerException);
        Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);
        sut.UpdateLayout();

        Assert.Equal(renderCount + 1, sut.RenderCount);
        Assert.Equal(Environment.CurrentManagedThreadId, sut.RenderThreadId);
    }

    [StaFact]
    public void InvalidateDots_RedrawsTheChangedDots()
    {
        var sut = CreateSut(new DotMatrixGeometry(2, 1, 0, 0, 1.0, 0, 0), (0, 0));
        Render(sut, 140, 100);
        var layout = CreateLayout(sut, 140, 100);

        sut.Lit.Clear();
        sut.Lit.Add((1, 0));
        sut.RaiseInvalidateDots();
        Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);
        var pixels = Render(sut, 140, 100);

        Assert.NotEqual(LitColor, pixels(Centre(layout.GetDotRect(0, 0))));
        Assert.Equal(LitColor, pixels(Centre(layout.GetDotRect(1, 0))));
    }

    [StaFact]
    public void TwoZones_DrawEachZoneInItsBrush()
    {
        var sut = new ZoneDotMatrixDisplay(zoneOneBelowRow: 2)
        {
            ZoneBrushes = [new SolidColorBrush(Colors.Red), new SolidColorBrush(Colors.Blue)],
        };
        var layout = DotMatrixLayout.Create(sut.Grid, showBezel: false, new Size(80, 40), sut.Stretch);

        var pixels = Render(sut, 80, 40);

        Assert.Equal(Colors.Blue, pixels(Centre(layout.GetDotRect(3, 0))));
        Assert.Equal(Colors.Blue, pixels(Centre(layout.GetDotRect(3, 1))));
        Assert.Equal(Colors.Red, pixels(Centre(layout.GetDotRect(3, 2))));
        Assert.Equal(Colors.Red, pixels(Centre(layout.GetDotRect(3, 3))));
    }

    [StaFact]
    public void DefaultZone_RendersExactlyAsBefore()
    {
        var plain = new PlainAllLitDisplay();
        var explicitDefaults = new ExplicitDefaultsAllLitDisplay();
        plain.GlowRadius = 0.5;
        explicitDefaults.GlowRadius = 0.5;
        plain.DotCornerRadius = 0.3;
        explicitDefaults.DotCornerRadius = 0.3;

        Assert.Equal(RenderBytes(plain, 80, 40), RenderBytes(explicitDefaults, 80, 40));
    }

    [StaFact]
    public void ZoneOutOfRange_IsDrawnAsZoneZero()
    {
        var sut = new ZoneDotMatrixDisplay(zoneOneBelowRow: 0)
        {
            OutOfRangeZone = 7,
            ZoneBrushes = [new SolidColorBrush(Colors.Red), new SolidColorBrush(Colors.Blue)],
        };
        var layout = DotMatrixLayout.Create(sut.Grid, showBezel: false, new Size(80, 40), sut.Stretch);

        var pixels = Render(sut, 80, 40);

        Assert.Equal(Colors.Red, pixels(Centre(layout.GetDotRect(3, 1))));
    }

    [StaFact]
    public void ZoneWithNullBrush_DrawsNoDotsForThatZone()
    {
        var sut = new ZoneDotMatrixDisplay(zoneOneBelowRow: 2)
        {
            ZoneBrushes = [new SolidColorBrush(Colors.Red), null],
        };
        var layout = DotMatrixLayout.Create(sut.Grid, showBezel: false, new Size(80, 40), sut.Stretch);

        var pixels = Render(sut, 80, 40);

        Assert.Equal(PanelColor, pixels(Centre(layout.GetDotRect(3, 0))));
        Assert.Equal(Colors.Red, pixels(Centre(layout.GetDotRect(3, 3))));
    }

    [StaTheory]
    [InlineData(0)]
    [InlineData(-3)]
    public void ZoneCountBelowOne_IsCoercedToOneZone(int zoneCount)
    {
        var sut = new ZoneDotMatrixDisplay(zoneOneBelowRow: 2)
        {
            ZoneBrushes = [new SolidColorBrush(Colors.Red), new SolidColorBrush(Colors.Blue)],
            ZoneCountOverride = zoneCount,
        };
        var layout = DotMatrixLayout.Create(sut.Grid, showBezel: false, new Size(80, 40), sut.Stretch);

        var pixels = Render(sut, 80, 40);

        Assert.Equal(Colors.Red, pixels(Centre(layout.GetDotRect(3, 0))));
        Assert.Equal(Colors.Red, pixels(Centre(layout.GetDotRect(3, 3))));
    }

    [StaFact]
    public void ZoneCountAboveFour_IsCoercedToFourZones()
    {
        var sut = new ZoneDotMatrixDisplay(zoneOneBelowRow: 0)
        {
            ZoneBrushes =
            [
                new SolidColorBrush(Colors.Red),
                new SolidColorBrush(Colors.Blue),
                new SolidColorBrush(Colors.Lime),
                new SolidColorBrush(Colors.Magenta),
                new SolidColorBrush(Colors.Cyan),
                new SolidColorBrush(Colors.Yellow),
            ],
            ZoneCountOverride = 9,
            ZoneOf = (x, _) => x switch
            {
                < 2 => 0,
                < 4 => 1,
                < 6 => 2,
                < 7 => 3,
                _ => 5,
            },
        };
        var layout = DotMatrixLayout.Create(sut.Grid, showBezel: false, new Size(80, 40), sut.Stretch);

        var pixels = Render(sut, 80, 40);

        Assert.Equal(Colors.Red, pixels(Centre(layout.GetDotRect(0, 1))));
        Assert.Equal(Colors.Blue, pixels(Centre(layout.GetDotRect(2, 1))));
        Assert.Equal(Colors.Lime, pixels(Centre(layout.GetDotRect(4, 1))));
        Assert.Equal(Colors.Magenta, pixels(Centre(layout.GetDotRect(6, 1))));
        Assert.Equal(Colors.Red, pixels(Centre(layout.GetDotRect(7, 1))));
    }

    [StaFact]
    public void NegativeZone_IsDrawnAsZoneZero()
    {
        var sut = new ZoneDotMatrixDisplay(zoneOneBelowRow: 0)
        {
            ZoneBrushes = [new SolidColorBrush(Colors.Red), new SolidColorBrush(Colors.Blue)],
            ZoneOf = (_, _) => -1,
        };
        var layout = DotMatrixLayout.Create(sut.Grid, showBezel: false, new Size(80, 40), sut.Stretch);

        var pixels = Render(sut, 80, 40);

        Assert.Equal(Colors.Red, pixels(Centre(layout.GetDotRect(3, 1))));
    }

    [StaFact]
    public void TwoZones_GlowUsesTheBrushOfItsZone()
    {
        var sut = new ZoneDotMatrixDisplay(zoneOneBelowRow: 2)
        {
            ZoneBrushes = [new SolidColorBrush(Colors.Red), new SolidColorBrush(Colors.Blue)],
            LitOf = (x, y) => x == 3 && (y == 0 || y == 3),
            GlowRadius = 0.5,
        };
        var layout = DotMatrixLayout.Create(sut.Grid, showBezel: false, new Size(80, 40), sut.Stretch);

        var pixels = Render(sut, 80, 40);

        var blueDot = layout.GetDotRect(3, 0);
        var redDot = layout.GetDotRect(3, 3);
        var besideBlue = pixels(new Point(blueDot.Right + (blueDot.Width / 4), Centre(blueDot).Y));
        var besideRed = pixels(new Point(redDot.Right + (redDot.Width / 4), Centre(redDot).Y));

        Assert.True(besideBlue.B - besideBlue.R > 0x20, $"The glow of zone 1 is not blue: {besideBlue}.");
        Assert.True(besideRed.R - besideRed.B > 0x20, $"The glow of zone 0 is not red: {besideRed}.");
    }

    private static byte[] RenderBytes(
        FrameworkElement sut,
        int width,
        int height)
    {
        Layout(sut, width, height);
        var bitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(sut);
        var bytes = new byte[width * height * 4];
        bitmap.CopyPixels(bytes, width * 4, 0);
        return bytes;
    }

    private static TestDotMatrixDisplay CreateSut(
        DotMatrixGeometry geometry,
        params (int X, int Y)[] lit)
    {
        var sut = new TestDotMatrixDisplay
        {
            Geometry = geometry,
            ShowBezel = false,
            PanelBrush = new SolidColorBrush(PanelColor),
            LitDotBrush = new SolidColorBrush(LitColor),
        };

        foreach (var dot in lit)
        {
            sut.Lit.Add(dot);
        }

        return sut;
    }

    private static DotMatrixLayout CreateLayout(
        TestDotMatrixDisplay sut,
        int width,
        int height)
        => DotMatrixLayout.Create(sut.Geometry, sut.ShowBezel, new Size(width, height), sut.Stretch);

    private static Point Centre(Rect rect)
        => new(rect.X + (rect.Width / 2), rect.Y + (rect.Height / 2));

    private static void Layout(
        FrameworkElement element,
        int width,
        int height)
    {
        element.Width = width;
        element.Height = height;
        element.Measure(new Size(width, height));
        element.Arrange(new Rect(0, 0, width, height));
        element.UpdateLayout();
    }

    private static Func<Point, Color> Render(
        FrameworkElement sut,
        int width,
        int height)
    {
        Layout(sut, width, height);

        var bitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(sut);
        var bytes = new byte[width * height * 4];
        bitmap.CopyPixels(bytes, width * 4, 0);

        return point =>
        {
            var i = (((int)point.Y * width) + (int)point.X) * 4;
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

    private class PlainAllLitDisplay : DotMatrixDisplayBase
    {
        public PlainAllLitDisplay()
        {
            ShowBezel = false;
            PanelBrush = new SolidColorBrush(PanelColor);
            LitDotBrush = new SolidColorBrush(LitColor);
        }

        protected override DotMatrixGeometry GetGeometry()
            => new(8, 4, 0, 0, 1.0, 0, 0);

        protected override bool IsDotLit(
            int x,
            int y)
            => true;
    }

    private sealed class ExplicitDefaultsAllLitDisplay : PlainAllLitDisplay
    {
        protected override int ColorZoneCount
            => 1;

        protected override int GetDotColorZone(
            int x,
            int y)
            => 0;

        protected override Brush? GetZoneLitDotBrush(int zone)
            => LitDotBrush;
    }

    private sealed class ZoneDotMatrixDisplay : DotMatrixDisplayBase
    {
        private readonly int zoneOneBelowRow;

        public ZoneDotMatrixDisplay(int zoneOneBelowRow)
        {
            this.zoneOneBelowRow = zoneOneBelowRow;
            ShowBezel = false;
            PanelBrush = new SolidColorBrush(PanelColor);
            LitDotBrush = new SolidColorBrush(LitColor);
        }

        public DotMatrixGeometry Grid { get; } = new(8, 4, 0, 0, 1.0, 0, 0);

        public int? OutOfRangeZone { get; set; }

        public int? ZoneCountOverride { get; set; }

        public Func<int, int, int>? ZoneOf { get; set; }

        public Func<int, int, bool>? LitOf { get; set; }

        public Brush?[]? ZoneBrushes { get; set; }

        protected override int ColorZoneCount
            => ZoneCountOverride ?? ZoneBrushes?.Length ?? base.ColorZoneCount;

        protected override DotMatrixGeometry GetGeometry()
            => Grid;

        protected override bool IsDotLit(
            int x,
            int y)
            => LitOf?.Invoke(x, y) ?? true;

        protected override int GetDotColorZone(
            int x,
            int y)
        {
            if (ZoneOf is { } zoneOf)
            {
                return zoneOf(x, y);
            }

            if (OutOfRangeZone is { } outOfRange)
            {
                return outOfRange;
            }

            if (ZoneBrushes is null)
            {
                return base.GetDotColorZone(x, y);
            }

            return y < zoneOneBelowRow ? 1 : 0;
        }

        protected override Brush? GetZoneLitDotBrush(int zone)
        {
            if (ZoneBrushes is { } brushes)
            {
                return zone < brushes.Length ? brushes[zone] : null;
            }

            return base.GetZoneLitDotBrush(zone);
        }
    }

    private sealed class TestDotMatrixDisplay : DotMatrixDisplayBase
    {
        public DotMatrixGeometry Geometry { get; set; } = new(1, 1, 0, 0, 0, 0, 0);

        public HashSet<(int X, int Y)> Lit { get; } = [];

        public int RenderCount { get; private set; }

        public int RenderThreadId { get; private set; }

        public void RaiseInvalidateDots()
            => InvalidateDots();

        protected override DotMatrixGeometry GetGeometry()
            => Geometry;

        protected override bool IsDotLit(
            int x,
            int y)
            => Lit.Contains((x, y));

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            RenderCount++;
            RenderThreadId = Environment.CurrentManagedThreadId;
        }
    }
}