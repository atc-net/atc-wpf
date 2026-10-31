namespace Atc.Wpf.FontIcons.Tests;

public sealed class ImageIconTests
{
    public static TheoryData<Type> ImageIconTypes => new()
    {
        typeof(ImageAwesomeSolid),
        typeof(ImageAwesomeRegular),
        typeof(ImageAwesomeBrand),
        typeof(ImageAwesomeSolid7),
        typeof(ImageAwesomeRegular7),
        typeof(ImageAwesomeBrand7),
        typeof(ImageBootstrap),
        typeof(ImageMaterialDesign),
        typeof(ImageWeather),
        typeof(ImageIcoFont),
    };

    [StaTheory]
    [MemberData(nameof(ImageIconTypes))]
    public void Rotation_OnAStaticIcon_RotatesTheImage(Type imageType)
    {
        ArgumentNullException.ThrowIfNull(imageType);

        var image = CreateImage(imageType);

        image.SetValue(GetDependencyProperty(imageType, "RotationProperty"), 90d);

        var rotateTransform = Assert.IsType<TransformGroup>(image.RenderTransform)
            .Children
            .OfType<RotateTransform>()
            .Single();
        Assert.Equal(90d, rotateTransform.Angle);
    }

    [StaTheory]
    [MemberData(nameof(ImageIconTypes))]
    public void Source_IsASharedVectorImage(Type imageType)
    {
        ArgumentNullException.ThrowIfNull(imageType);

        var first = (Image)CreateImage(imageType);
        var second = (Image)CreateImage(imageType);
        var foregroundProperty = GetDependencyProperty(imageType, "ForegroundProperty");
        first.SetValue(foregroundProperty, new SolidColorBrush(Colors.Red));
        second.SetValue(foregroundProperty, new SolidColorBrush(Colors.Red));

        Assert.IsType<DrawingImage>(first.Source);
        Assert.Same(first.Source, second.Source);
    }

    [StaFact]
    public void Source_KeepsTheSizeOfTheBitmapImage()
    {
        var image = new ImageAwesomeSolid
        {
            Icon = FontAwesomeSolidType.Square,
            Foreground = Brushes.Red,
        };
        var bitmap = (BitmapSource)ImageAwesomeSolid.CreateImageSource(FontAwesomeSolidType.Square, Brushes.Red);

        Assert.Equal(bitmap.Width, image.Source.Width, 3);
        Assert.Equal(bitmap.Height, image.Source.Height, 3);
    }

    [StaFact]
    public void Source_DrawsTheGlyphWhereTheBitmapImageDid()
    {
        var vector = RenderInkBounds(new ImageAwesomeSolid { Icon = FontAwesomeSolidType.Square, Foreground = Brushes.Red });
        var raster = RenderInkBounds(new Image { Source = ImageAwesomeSolid.CreateImageSource(FontAwesomeSolidType.Square, Brushes.Red) });

        Assert.False(vector.IsEmpty);
        Assert.InRange(System.Math.Abs(vector.Left - raster.Left), 0, 2);
        Assert.InRange(System.Math.Abs(vector.Top - raster.Top), 0, 2);
        Assert.InRange(System.Math.Abs(vector.Right - raster.Right), 0, 2);
        Assert.InRange(System.Math.Abs(vector.Bottom - raster.Bottom), 0, 2);
    }

    [StaFact]
    public void Source_ChangingForeground_UsesTheNewColor()
    {
        var image = new ImageMaterialDesign { Icon = FontMaterialDesignType.Home, Foreground = Brushes.Red };
        var redSource = image.Source;

        image.Foreground = Brushes.Blue;

        Assert.NotSame(redSource, image.Source);
        var drawing = Assert.IsType<DrawingImage>(image.Source).Drawing;
        Assert.Contains(
            EnumerateGlyphRunDrawings(drawing),
            g => g.ForegroundBrush is SolidColorBrush { Color: var c } && c == Colors.Blue);
    }

    private static Rect RenderInkBounds(Image image)
    {
        const int size = 64;
        image.Width = size;
        image.Height = size;
        image.Stretch = Stretch.Uniform;
        image.Measure(new Size(size, size));
        image.Arrange(new Rect(0, 0, size, size));

        var target = new RenderTargetBitmap(size, size, 96, 96, PixelFormats.Pbgra32);
        target.Render(image);

        var pixels = new byte[size * size * 4];
        target.CopyPixels(pixels, size * 4, 0);

        int minX = size, minY = size, maxX = -1, maxY = -1;
        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                if (pixels[(((y * size) + x) * 4) + 3] < 128)
                {
                    continue;
                }

                minX = System.Math.Min(minX, x);
                minY = System.Math.Min(minY, y);
                maxX = System.Math.Max(maxX, x);
                maxY = System.Math.Max(maxY, y);
            }
        }

        return maxX < 0
            ? Rect.Empty
            : new Rect(minX, minY, maxX - minX + 1, maxY - minY + 1);
    }

    private static IEnumerable<GlyphRunDrawing> EnumerateGlyphRunDrawings(
        Drawing drawing)
    {
        switch (drawing)
        {
            case GlyphRunDrawing glyphRunDrawing:
                yield return glyphRunDrawing;
                break;
            case DrawingGroup group:
                foreach (var child in group.Children)
                {
                    foreach (var inner in EnumerateGlyphRunDrawings(child))
                    {
                        yield return inner;
                    }
                }

                break;
        }
    }

    private static FrameworkElement CreateImage(Type imageType)
        => (FrameworkElement)Activator.CreateInstance(imageType)!;

    private static DependencyProperty GetDependencyProperty(
        Type imageType,
        string fieldName)
        => (DependencyProperty)imageType
            .GetField(fieldName, BindingFlags.Public | BindingFlags.Static)!
            .GetValue(null)!;
}