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

    private static FrameworkElement CreateImage(Type imageType)
        => (FrameworkElement)Activator.CreateInstance(imageType)!;

    private static DependencyProperty GetDependencyProperty(
        Type imageType,
        string fieldName)
        => (DependencyProperty)imageType
            .GetField(fieldName, BindingFlags.Public | BindingFlags.Static)!
            .GetValue(null)!;
}