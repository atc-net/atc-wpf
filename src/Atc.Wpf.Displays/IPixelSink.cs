namespace Atc.Wpf.Displays;

/// <summary>
/// The target of <see cref="DisplayRasterizer"/>: writes pixels of a frame buffer, which clips them to its size.
/// </summary>
internal interface IPixelSink
{
    // Sets one pixel; returns true when it changed.
    bool Plot(
        long x,
        long y);

    // Fills a rectangle; returns true when a pixel changed.
    bool Fill(
        long x,
        long y,
        long width,
        long height);
}