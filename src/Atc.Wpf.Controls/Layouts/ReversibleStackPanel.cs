namespace Atc.Wpf.Controls.Layouts;

/// <summary>A <see cref="StackPanel"/> that can arrange its children in reverse order.</summary>
public sealed class ReversibleStackPanel : StackPanel
{
    /// <summary>Identifies the <see cref="ReverseOrder"/> dependency property.</summary>
    public static readonly DependencyProperty ReverseOrderProperty = DependencyProperty.Register(
        nameof(ReverseOrder),
        typeof(bool),
        typeof(ReversibleStackPanel),
        new FrameworkPropertyMetadata(
            defaultValue: false,
            FrameworkPropertyMetadataOptions.AffectsArrange));

    /// <summary>Gets or sets a value indicating whether the children are arranged in reverse order.</summary>
    public bool ReverseOrder
    {
        get => (bool)GetValue(ReverseOrderProperty);
        set => SetValue(ReverseOrderProperty, value);
    }

    /// <inheritdoc />
    protected override Size ArrangeOverride(Size arrangeSize)
    {
        double x = 0;
        double y = 0;

        var children = ReverseOrder
            ? InternalChildren
                .Cast<UIElement>()
                .Reverse()
            : InternalChildren.Cast<UIElement>();

        foreach (var child in children)
        {
            Size size;

            if (Orientation == Orientation.Horizontal)
            {
                size = new Size(child.DesiredSize.Width, System.Math.Max(arrangeSize.Height, child.DesiredSize.Height));
                child.Arrange(new Rect(new Point(x, y), size));
                x += size.Width;
            }
            else
            {
                size = new Size(System.Math.Max(arrangeSize.Width, child.DesiredSize.Width), child.DesiredSize.Height);
                child.Arrange(new Rect(new Point(x, y), size));
                y += size.Height;
            }
        }

        return Orientation == Orientation.Horizontal
            ? new Size(x, arrangeSize.Height)
            : new Size(arrangeSize.Width, y);
    }
}