namespace Atc.Wpf.Controls.Media.W3cSvg.FileLoaders;

/// <summary>
/// Defines a loader for external files (such as images) referenced from an SVG document.
/// </summary>
public interface IExternalFileLoader
{
    /// <summary>
    /// Opens the file referenced by an SVG document.
    /// </summary>
    /// <param name="hRef">The reference to the external file, as written in the SVG.</param>
    /// <param name="svgFilename">The path of the SVG file that contains the reference.</param>
    /// <returns>A stream with the file content, or <see langword="null"/> if the file could not be resolved.</returns>
    Stream? LoadFile(
        string hRef,
        string svgFilename);
}