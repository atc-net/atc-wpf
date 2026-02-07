namespace Atc.Wpf.Controls.Progressing;

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