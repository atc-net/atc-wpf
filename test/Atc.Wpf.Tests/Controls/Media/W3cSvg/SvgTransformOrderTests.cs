namespace Atc.Wpf.Tests.Controls.Media.W3cSvg;

/// <summary>
/// SVG applies a transform list right-to-left: in <c>transform="A B"</c> the shape is first transformed by B and then by A.
/// A WPF <see cref="TransformGroup"/> applies its children first-to-last, so the parsed list must be added in reverse.
/// </summary>
public sealed class SvgTransformOrderTests
{
    private const string SvgOpen = """<svg viewBox="-50 -50 100 100" xmlns="http://www.w3.org/2000/svg">""";

    private static readonly Rect ShapeRect = new(0, 0, 4, 2);

    [StaFact]
    public void RotateThenTranslate_TranslatesFirstAndRotatesSecond()
    {
        // Arrange - rotate(90) translate(10,0): translate the shape, then rotate the result about the origin
        var expected = new TranslateTransform(10, 0).Value;
        expected.Append(new RotateTransform(90).Value);

        // Act
        var group = RenderTransformedRect("rotate(90) translate(10,0)");

        // Assert
        AssertMatrix(expected, group.Transform.Value);
        AssertRect(new Rect(-2, 10, 2, 4), group.Bounds);
    }

    [StaFact]
    public void TranslateThenRotate_RotatesFirstAndTranslatesSecond()
    {
        // Arrange - translate(10,0) rotate(90): rotate the shape about the origin, then translate the result
        var expected = new RotateTransform(90).Value;
        expected.Append(new TranslateTransform(10, 0).Value);

        // Act
        var group = RenderTransformedRect("translate(10,0) rotate(90)");

        // Assert
        AssertMatrix(expected, group.Transform.Value);
        AssertRect(new Rect(8, 0, 2, 4), group.Bounds);
    }

    [StaFact]
    public void TheTwoOrders_ProduceDifferentPlacements()
    {
        var rotateFirstInList = RenderTransformedRect("rotate(90) translate(10,0)");
        var translateFirstInList = RenderTransformedRect("translate(10,0) rotate(90)");

        Assert.NotEqual(rotateFirstInList.Bounds, translateFirstInList.Bounds);
    }

    [StaFact]
    public void TranslateThenScale_StillRendersAsBefore()
    {
        // Arrange - translate(10,0) scale(2): scale the shape, then translate the result
        var expected = new ScaleTransform(2, 2).Value;
        expected.Append(new TranslateTransform(10, 0).Value);

        // Act
        var group = RenderTransformedRect("translate(10,0) scale(2)");

        // Assert
        AssertMatrix(expected, group.Transform.Value);
        AssertRect(new Rect(10, 0, 8, 4), group.Bounds);
    }

    [StaFact]
    public void ThreeTransforms_AreComposedRightToLeft()
    {
        // Arrange - translate(10,0) rotate(90) scale(2): scale, then rotate, then translate
        var expected = new ScaleTransform(2, 2).Value;
        expected.Append(new RotateTransform(90).Value);
        expected.Append(new TranslateTransform(10, 0).Value);

        // Act
        var group = RenderTransformedRect("translate(10,0) rotate(90) scale(2)");

        // Assert
        AssertMatrix(expected, group.Transform.Value);
        AssertRect(Rect.Transform(ShapeRect, expected), group.Bounds);
    }

    private static DrawingGroup RenderTransformedRect(string transform)
    {
        var drawing = new SvgRender().LoadXmlDrawing(
            $"""{SvgOpen}<rect x="{ShapeRect.X}" y="{ShapeRect.Y}" width="{ShapeRect.Width}" height="{ShapeRect.Height}" transform="{transform}" fill="red" /></svg>""");

        return DrawingGroups(drawing).Single(x => x.Transform is not null);
    }

    private static void AssertMatrix(
        Matrix expected,
        Matrix actual)
    {
        const int precision = 9;
        Assert.Equal(expected.M11, actual.M11, precision);
        Assert.Equal(expected.M12, actual.M12, precision);
        Assert.Equal(expected.M21, actual.M21, precision);
        Assert.Equal(expected.M22, actual.M22, precision);
        Assert.Equal(expected.OffsetX, actual.OffsetX, precision);
        Assert.Equal(expected.OffsetY, actual.OffsetY, precision);
    }

    private static void AssertRect(
        Rect expected,
        Rect actual)
    {
        const int precision = 9;
        Assert.Equal(expected.X, actual.X, precision);
        Assert.Equal(expected.Y, actual.Y, precision);
        Assert.Equal(expected.Width, actual.Width, precision);
        Assert.Equal(expected.Height, actual.Height, precision);
    }

    private static IEnumerable<DrawingGroup> DrawingGroups(DrawingGroup group)
    {
        foreach (var child in group.Children.OfType<DrawingGroup>())
        {
            yield return child;
            foreach (var nested in DrawingGroups(child))
            {
                yield return nested;
            }
        }
    }
}