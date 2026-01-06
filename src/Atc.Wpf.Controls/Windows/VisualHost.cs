namespace Atc.Wpf.Controls.Windows;

/// <summary>
/// VisualHost.
/// </summary>
public sealed class VisualHost : FrameworkElement
{
    private readonly VisualCollection visualCollection;

    /// <summary>
    /// Initializes a new instance of the <see cref="VisualHost" /> class.
    /// </summary>
    public VisualHost()
    {
        this.visualCollection = new VisualCollection(this);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VisualHost" /> class.
    /// </summary>
    /// <param name="visual">The visual.</param>
    public VisualHost(Visual visual)
        : this()
        => Add(visual);

    /// <summary>
    /// Provide a required override for the VisualChildrenCount property.
    /// Gets the number of visual child elements within this element.
    /// </summary>
    /// <returns>The number of visual child elements for this element.</returns>
    protected override int VisualChildrenCount => visualCollection.Count;

    /// <summary>
    /// Adds the specified visual.
    /// </summary>
    /// <param name="visual">The visual.</param>
    public void Add(Visual visual)
    {
        ArgumentNullException.ThrowIfNull(visual);

        visualCollection.Add(visual);
    }

    /// <summary>
    /// Clears this instance.
    /// </summary>
    public void Clear()
        => visualCollection.Clear();

    /// <summary>
    /// Provide a required override for the GetVisualChild method.
    /// Overrides <see cref="GetVisualChild(int)" />, and returns a child at the specified index from a collection of child elements.
    /// </summary>
    /// <param name="index">The zero-based index of the requested child element in the collection.</param>
    /// <returns>
    /// The requested child element. This should not return null; if the provided index is out of range, an exception is thrown.
    /// </returns>
    protected override Visual GetVisualChild(int index)
    {
        if (index < 0 || index >= visualCollection.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        return visualCollection[index];
    }
}