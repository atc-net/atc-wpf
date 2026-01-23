namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// Provides data for the <see cref="Carousel.SlideChanged"/> event.
/// </summary>
public sealed class CarouselSlideChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CarouselSlideChangedEventArgs"/> class.
    /// </summary>
    /// <param name="oldIndex">The previous slide index.</param>
    /// <param name="newIndex">The new slide index.</param>
    public CarouselSlideChangedEventArgs(
        int oldIndex,
        int newIndex)
    {
        OldIndex = oldIndex;
        NewIndex = newIndex;
    }

    /// <summary>
    /// Gets the previous slide index.
    /// </summary>
    public int OldIndex { get; }

    /// <summary>
    /// Gets the new slide index.
    /// </summary>
    public int NewIndex { get; }
}
