namespace Atc.Wpf.Controls.Media;

/// <summary>
/// Provides data for the <see cref="SvgImage.ImageFailed"/> event, shaped like the
/// <see cref="ExceptionRoutedEventArgs"/> of <see cref="System.Windows.Controls.Image.ImageFailed"/>.
/// </summary>
public sealed class SvgImageFailedEventArgs : RoutedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SvgImageFailedEventArgs"/> class.
    /// </summary>
    /// <param name="routedEvent">The routed event being raised.</param>
    /// <param name="source">The <see cref="SvgImage"/> that failed to load its image.</param>
    /// <param name="errorException">The exception that made the image fail.</param>
    public SvgImageFailedEventArgs(
        RoutedEvent routedEvent,
        object source,
        Exception errorException)
        : base(
            routedEvent,
            source)
    {
        ArgumentNullException.ThrowIfNull(errorException);

        ErrorException = errorException;
    }

    /// <summary>
    /// Gets the exception that made the image fail, e.g. a missing file, a missing resource or malformed SVG.
    /// </summary>
    public Exception ErrorException { get; }
}