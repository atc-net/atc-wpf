namespace Atc.Wpf.Theming.Tests.Decorators;

/// <summary>
/// <see cref="ClipBorder"/> sizes like a Border, clips its child to the inner rounded shape, and draws the
/// border and background without a seam between them.
/// </summary>
public sealed class ClipBorderTests
{
    private const int Width = 100;
    private const int Height = 60;

    private static readonly Color BorderColor = Color.FromRgb(0xE0, 0x10, 0x10);
    private static readonly Color BackgroundColor = Color.FromRgb(0x10, 0x10, 0xE0);

    [StaFact]
    public void Measure_AddsBorderAndPaddingToTheChild()
    {
        var sut = new ClipBorder
        {
            BorderThickness = new Thickness(1, 2, 3, 4),
            Padding = new Thickness(5, 6, 7, 8),
            Child = new Border { Width = 50, Height = 20 },
        };

        sut.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

        Assert.Equal(new Size(50 + 1 + 3 + 5 + 7, 20 + 2 + 4 + 6 + 8), sut.DesiredSize);
    }

    [StaFact]
    public void Measure_WithoutAChild_IsBorderPlusPadding()
    {
        var sut = new ClipBorder
        {
            BorderThickness = new Thickness(2),
            Padding = new Thickness(3),
        };

        sut.Measure(new Size(200, 200));

        Assert.Equal(new Size(10, 10), sut.DesiredSize);
    }

    [StaFact]
    public void Measure_GivesTheChildTheSpaceInsideBorderAndPadding()
    {
        var child = new Border();
        var sut = new ClipBorder
        {
            BorderThickness = new Thickness(2),
            Padding = new Thickness(3),
            Child = child,
        };

        sut.Measure(new Size(100, 40));
        sut.Arrange(new Rect(0, 0, 100, 40));

        Assert.Equal(new Rect(5, 5, 90, 30), LayoutInformation.GetLayoutSlot(child));
    }

    [StaFact]
    public void Arrange_ClipsTheChildToTheRoundedInnerShape()
    {
        var child = new Border();
        var sut = new ClipBorder
        {
            BorderThickness = new Thickness(2),
            CornerRadius = new CornerRadius(20),
            Child = child,
        };

        Layout(sut);

        var clip = Assert.IsType<StreamGeometry>(child.Clip);
        Assert.False(clip.FillContains(new Point(1, 1)));
        Assert.True(clip.FillContains(new Point(Width / 2.0, Height / 2.0)));
        Assert.True(clip.FillContains(new Point(Width / 2.0, 1)));
    }

    [StaFact]
    public void Arrange_SquareCorners_ClipKeepsTheCorners()
    {
        var child = new Border();
        var sut = new ClipBorder { Child = child };

        Layout(sut);

        Assert.True(Assert.IsType<StreamGeometry>(child.Clip).FillContains(new Point(1, 1)));
    }

    [StaTheory]
    [InlineData(-1.0, 0.0)]
    [InlineData(double.NaN, 0.0)]
    [InlineData(double.PositiveInfinity, 0.0)]
    public void InvalidThickness_IsRejected(
        double left,
        double top)
    {
        var sut = new ClipBorder();

        Assert.Throws<ArgumentException>(() => sut.BorderThickness = new Thickness(left, top, 0, 0));
        Assert.Throws<ArgumentException>(() => sut.Padding = new Thickness(left, top, 0, 0));
    }

    [StaFact]
    public void NegativeCornerRadius_IsRejected()
        => Assert.Throws<ArgumentException>(() => new ClipBorder().CornerRadius = new CornerRadius(-1));

    [StaFact]
    public void Render_OpaqueBorderAndBackground()
    {
        var pixels = Render(new ClipBorder
        {
            BorderThickness = new Thickness(10),
            BorderBrush = new SolidColorBrush(BorderColor),
            Background = new SolidColorBrush(BackgroundColor),
        });

        Assert.Equal(BorderColor, pixels(5, 30));
        Assert.Equal(BackgroundColor, pixels(50, 30));
    }

    [StaFact]
    public void Render_RoundedCorners_LeaveTheCornerEmpty()
    {
        var pixels = Render(new ClipBorder
        {
            BorderThickness = new Thickness(10),
            CornerRadius = new CornerRadius(30),
            BorderBrush = new SolidColorBrush(BorderColor),
            Background = new SolidColorBrush(BackgroundColor),
        });

        Assert.Equal(0, pixels(1, 1).A);
        Assert.Equal(BorderColor, pixels(50, 4));
        Assert.Equal(BackgroundColor, pixels(50, 30));
    }

    [StaFact]
    public void Render_BorderOnly_LeavesTheInsideEmpty()
    {
        var pixels = Render(new ClipBorder
        {
            BorderThickness = new Thickness(10),
            BorderBrush = new SolidColorBrush(BorderColor),
        });

        Assert.Equal(BorderColor, pixels(5, 30));
        Assert.Equal(0, pixels(50, 30).A);
    }

    [StaFact]
    public void Render_BackgroundOnly_FillsTheInside()
    {
        var pixels = Render(new ClipBorder
        {
            BorderThickness = new Thickness(10),
            Background = new SolidColorBrush(BackgroundColor),
        });

        Assert.Equal(BackgroundColor, pixels(50, 30));
        Assert.Equal(0, pixels(5, 30).A);
    }

    [StaFact]
    public void Render_TranslucentBackground_DoesNotShowTheBorderThroughIt()
    {
        var pixels = Render(new ClipBorder
        {
            BorderThickness = new Thickness(10),
            BorderBrush = new SolidColorBrush(BorderColor),
            Background = new SolidColorBrush(Color.FromArgb(0x80, BackgroundColor.R, BackgroundColor.G, BackgroundColor.B)),
        });

        Assert.Equal(BorderColor, pixels(5, 30));
        var inside = pixels(50, 30);
        Assert.InRange(inside.A, 0x70, 0x90);
        Assert.True(inside.R < 0x30, $"The border colour shows through the background: {inside}.");
    }

    [StaFact]
    public void Render_TranslucentBorder_DoesNotDrawOverTheBackground()
    {
        var pixels = Render(new ClipBorder
        {
            BorderThickness = new Thickness(10),
            BorderBrush = new SolidColorBrush(Color.FromArgb(0x80, BorderColor.R, BorderColor.G, BorderColor.B)),
            Background = new SolidColorBrush(BackgroundColor),
        });

        Assert.InRange(pixels(5, 30).A, 0x70, 0x90);
        Assert.Equal(BackgroundColor, pixels(50, 30));
    }

    [StaFact]
    public void Render_OptimizeClipRendering_FillsEverythingWithTheBorderBrush()
    {
        var pixels = Render(new ClipBorder
        {
            BorderThickness = new Thickness(10),
            BorderBrush = new SolidColorBrush(BorderColor),
            Background = new SolidColorBrush(BackgroundColor),
            OptimizeClipRendering = true,
        });

        Assert.Equal(BorderColor, pixels(5, 30));
        Assert.Equal(BorderColor, pixels(50, 30));
    }

    [StaFact]
    public void Render_SameBrushForBorderAndBackground_FillsEverything()
    {
        var brush = new SolidColorBrush(BorderColor);
        var pixels = Render(new ClipBorder
        {
            BorderThickness = new Thickness(10),
            BorderBrush = brush,
            Background = brush,
        });

        Assert.Equal(BorderColor, pixels(5, 30));
        Assert.Equal(BorderColor, pixels(50, 30));
    }

    private static void Layout(UIElement element)
    {
        element.Measure(new Size(Width, Height));
        element.Arrange(new Rect(0, 0, Width, Height));
    }

    private static Func<int, int, Color> Render(ClipBorder sut)
    {
        sut.Width = Width;
        sut.Height = Height;
        Layout(sut);

        var bitmap = new RenderTargetBitmap(Width, Height, 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(sut);
        var bytes = new byte[Width * Height * 4];
        bitmap.CopyPixels(bytes, Width * 4, 0);

        return (x, y) =>
        {
            var i = ((y * Width) + x) * 4;
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