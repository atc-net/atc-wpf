namespace Atc.Wpf.Controls.Flyouts;

/// <summary>
/// Configuration options for displaying a flyout.
/// </summary>
public sealed class FlyoutOptions
{
    /// <summary>
    /// Gets or sets the position from which the flyout slides in.
    /// Default is <see cref="FlyoutPosition.Right"/>.
    /// </summary>
    public FlyoutPosition Position { get; set; } = FlyoutPosition.Right;

    /// <summary>
    /// Gets or sets the width of the flyout panel (for Left/Right positions).
    /// Default is 400.
    /// </summary>
    public double Width { get; set; } = 400;

    /// <summary>
    /// Gets or sets the height of the flyout panel (for Top/Bottom positions).
    /// Default is 300.
    /// </summary>
    public double Height { get; set; } = 300;

    /// <summary>
    /// Gets or sets whether light dismiss is enabled (click outside or Escape to close).
    /// Default is true.
    /// </summary>
    public bool IsLightDismissEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to show an overlay behind the flyout.
    /// Default is true.
    /// </summary>
    public bool ShowOverlay { get; set; } = true;

    /// <summary>
    /// Gets or sets the overlay opacity (0.0 to 1.0).
    /// Default is 0.5.
    /// </summary>
    public double OverlayOpacity { get; set; } = 0.5;

    /// <summary>
    /// Gets or sets whether to show the close button in the header.
    /// Default is true.
    /// </summary>
    public bool ShowCloseButton { get; set; } = true;

    /// <summary>
    /// Gets or sets whether Escape key closes the flyout.
    /// Default is true.
    /// </summary>
    public bool CloseOnEscape { get; set; } = true;

    /// <summary>
    /// Gets or sets the animation duration in milliseconds.
    /// Default is 300.
    /// </summary>
    public double AnimationDuration { get; set; } = 300;

    /// <summary>
    /// Gets or sets the corner radius of the flyout panel.
    /// Default is 0.
    /// </summary>
    public CornerRadius CornerRadius { get; set; }

    /// <summary>
    /// Gets or sets the padding inside the flyout content area.
    /// Default is 16 on all sides.
    /// </summary>
    public Thickness Padding { get; set; } = new(16);

    /// <summary>
    /// Creates default options for a right-side flyout.
    /// </summary>
    public static FlyoutOptions Default => new();

    /// <summary>
    /// Creates options for a wide flyout (600px).
    /// </summary>
    public static FlyoutOptions Wide => new() { Width = 600 };

    /// <summary>
    /// Creates options for a narrow flyout (300px).
    /// </summary>
    public static FlyoutOptions Narrow => new() { Width = 300 };

    /// <summary>
    /// Creates options for a left-side flyout.
    /// </summary>
    public static FlyoutOptions Left
        => new() { Position = FlyoutPosition.Left };

    /// <summary>
    /// Creates options for a top flyout.
    /// </summary>
    public static FlyoutOptions Top
        => new() { Position = FlyoutPosition.Top };

    /// <summary>
    /// Creates options for a bottom flyout.
    /// </summary>
    public static FlyoutOptions Bottom
        => new() { Position = FlyoutPosition.Bottom };

    /// <summary>
    /// Creates options for a modal flyout (no light dismiss).
    /// </summary>
    public static FlyoutOptions Modal => new()
    {
        IsLightDismissEnabled = false,
        CloseOnEscape = false,
    };
}