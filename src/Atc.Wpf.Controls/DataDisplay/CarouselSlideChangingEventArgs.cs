namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// Provides data for the <see cref="Carousel.SlideChanging"/> event.
/// </summary>
public sealed class CarouselSlideChangingEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CarouselSlideChangingEventArgs"/> class.
    /// </summary>
    /// <param name="currentIndex">The current slide index.</param>
    /// <param name="targetIndex">The target slide index.</param>
    public CarouselSlideChangingEventArgs(
        int currentIndex,
        int targetIndex)
    {
        CurrentIndex = currentIndex;
        TargetIndex = targetIndex;
    }

    /// <summary>
    /// Gets the current slide index.
    /// </summary>
    public int CurrentIndex { get; }

    /// <summary>
    /// Gets the target slide index.
    /// </summary>
    public int TargetIndex { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the slide change should be canceled.
    /// </summary>
    public bool Cancel { get; set; }
}
