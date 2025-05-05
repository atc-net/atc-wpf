namespace Atc.Wpf.Controls.Layouts;

public sealed partial class ReversibleStackPanel : StackPanel
{
    [DependencyProperty]
    private bool reverseOrder;

    protected override Size ArrangeOverride(
        Size arrangeSize)
    {
        double x = 0;
        double y = 0;

        var children = ReverseOrder
            ? InternalChildren.Cast<UIElement>().Reverse()
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