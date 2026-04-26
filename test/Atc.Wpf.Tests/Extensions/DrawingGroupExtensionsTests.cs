namespace Atc.Wpf.Tests.Extensions;

public sealed class DrawingGroupExtensionsTests
{
    [Fact]
    public void ApplyTransform_assigns_the_transform_when_none_exists()
    {
        var group = new DrawingGroup();
        var rotate = new RotateTransform(45);

        group.ApplyTransform(rotate);

        Assert.Same(rotate, group.Transform);
    }

    [Fact]
    public void ApplyTransform_appends_to_an_existing_TransformGroup()
    {
        var existing = new TransformGroup();
        existing.Children.Add(new RotateTransform(10));
        var group = new DrawingGroup { Transform = existing };

        group.ApplyTransform(new ScaleTransform(2, 2));

        var transformGroup = Assert.IsType<TransformGroup>(group.Transform);
        Assert.Equal(2, transformGroup.Children.Count);
        Assert.IsType<RotateTransform>(transformGroup.Children[0]);
        Assert.IsType<ScaleTransform>(transformGroup.Children[1]);
    }

    [Fact]
    public void ApplyTransform_wraps_existing_non_group_transform_into_a_TransformGroup()
    {
        var existing = new RotateTransform(10);
        var group = new DrawingGroup { Transform = existing };

        group.ApplyTransform(new ScaleTransform(2, 2));

        var transformGroup = Assert.IsType<TransformGroup>(group.Transform);
        Assert.Equal(2, transformGroup.Children.Count);
        Assert.Same(existing, transformGroup.Children[0]);
        Assert.IsType<ScaleTransform>(transformGroup.Children[1]);
    }

    [Fact]
    public void ApplyTransform_throws_for_null_drawing()
    {
        Assert.Throws<ArgumentNullException>(() =>
            DrawingGroupExtensions.ApplyTransform(
                objDrawing: null!,
                new RotateTransform(0)));
    }
}