namespace Atc.Wpf.Controls.Layouts;

[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "OK.")]
public sealed class PanelEx : Panel
{
    protected override Size MeasureOverride(Size availableSize)
    {
        var maxSize = default(Size);

        foreach (UIElement child in InternalChildren)
        {
            if (child is null)
            {
                continue;
            }

            child.Measure(availableSize);
            maxSize.Width = System.Math.Max(maxSize.Width, child.DesiredSize.Width);
            maxSize.Height = System.Math.Max(maxSize.Height, child.DesiredSize.Height);
        }

        return maxSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        foreach (UIElement child in InternalChildren)
        {
            child?.Arrange(new Rect(finalSize));
        }

        return finalSize;
    }
}