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
    /// Gets or sets the width of the flyout panel (for Left/Right/Center positions).
    /// Default is 400.
    /// </summary>
    public double Width { get; set; } = 400;

    /// <summary>
    /// Gets or sets the height of the flyout panel (for Top/Bottom/Center positions).
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
    /// Gets or sets whether the flyout is pinned (prevents light dismiss).
    /// Default is false.
    /// </summary>
    public bool IsPinned { get; set; }

    /// <summary>
    /// Gets or sets whether the flyout can be resized by dragging the edge.
    /// Default is false.
    /// </summary>
    public bool IsResizable { get; set; }

    /// <summary>
    /// Gets or sets the minimum width when resizing.
    /// Default is 200.
    /// </summary>
    public double MinWidth { get; set; } = 200;

    /// <summary>
    /// Gets or sets the maximum width when resizing.
    /// Default is 800.
    /// </summary>
    public double MaxWidth { get; set; } = 800;

    /// <summary>
    /// Gets or sets the minimum height when resizing.
    /// Default is 150.
    /// </summary>
    public double MinHeight { get; set; } = 150;

    /// <summary>
    /// Gets or sets the maximum height when resizing.
    /// Default is 600.
    /// </summary>
    public double MaxHeight { get; set; } = 600;

    /// <summary>
    /// Gets or sets the easing function for animations.
    /// Default is null (uses CubicEase).
    /// </summary>
    public IEasingFunction? EasingFunction { get; set; }

    /// <summary>
    /// Gets or sets whether focus is trapped within the flyout when open.
    /// When enabled, Tab/Shift+Tab cycles through focusable elements within the flyout.
    /// Default is true.
    /// </summary>
    public bool IsFocusTrapEnabled { get; set; } = true;

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
    /// Creates options for a centered flyout (modal-like).
    /// </summary>
    public static FlyoutOptions Center => new()
    {
        Position = FlyoutPosition.Center,
        Width = 500,
        Height = 400,
        CornerRadius = new CornerRadius(8),
    };

    /// <summary>
    /// Creates options for a modal flyout (no light dismiss).
    /// </summary>
    public static FlyoutOptions Modal => new()
    {
        IsLightDismissEnabled = false,
        CloseOnEscape = false,
    };

    /// <summary>
    /// Creates options for a resizable flyout.
    /// </summary>
    public static FlyoutOptions Resizable => new()
    {
        IsResizable = true,
    };

    /// <summary>
    /// Creates options for a pinned flyout (prevents light dismiss).
    /// </summary>
    public static FlyoutOptions Pinned => new()
    {
        IsPinned = true,
    };
}