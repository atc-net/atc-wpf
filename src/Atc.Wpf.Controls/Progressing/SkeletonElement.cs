namespace Atc.Wpf.Controls.Progressing;

/// <summary>An animated placeholder shape used to build skeleton loading screens.</summary>
public sealed partial class SkeletonElement : Control
{
    [DependencyProperty(DefaultValue = SkeletonShape.Rectangle)]
    private SkeletonShape shape;

    [DependencyProperty(DefaultValue = SkeletonAnimationType.Shimmer)]
    private SkeletonAnimationType animationType;

    [DependencyProperty(DefaultValue = "new CornerRadius(4)")]
    private CornerRadius cornerRadius;

    [DependencyProperty(DefaultValue = true)]
    private bool isActive;

    static SkeletonElement()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(SkeletonElement),
            new FrameworkPropertyMetadata(typeof(SkeletonElement)));
    }
}