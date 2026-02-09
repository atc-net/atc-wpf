namespace Atc.Wpf.Capture;

/// <summary>
/// Configuration settings for a capture operation.
/// </summary>
public sealed class CaptureSettings
{
    /// <summary>
    /// Gets or sets the horizontal DPI for rendering.
    /// Default is 96 (standard WPF DPI).
    /// </summary>
    public double DpiX { get; set; } = 96;

    /// <summary>
    /// Gets or sets the vertical DPI for rendering.
    /// Default is 96 (standard WPF DPI).
    /// </summary>
    public double DpiY { get; set; } = 96;
}