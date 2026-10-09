namespace Atc.Wpf.Forms;

/// <summary>
/// Layout settings for a <see cref="LabelInputFormPanel"/>.
/// </summary>
public sealed class LabelInputFormPanelSettings
{
    /// <summary>
    /// Gets or sets the maximum size of the panel.
    /// </summary>
    public Size MaxSize { get; set; } = new(1920, 1200);

    /// <summary>
    /// Gets or sets the orientation of the label controls.
    /// </summary>
    public Orientation ControlOrientation { get; set; } = Orientation.Vertical;

    /// <summary>
    /// Gets or sets a value indicating whether the controls are wrapped in a group box per group identifier.
    /// </summary>
    public bool UseGroupBox { get; set; }

    /// <summary>
    /// Gets or sets the width of the label controls.
    /// </summary>
    public int ControlWidth { get; set; } = 320;

    /// <inheritdoc />
    public override string ToString()
        => $"{nameof(MaxSize)}: {MaxSize}, {nameof(ControlOrientation)}: {ControlOrientation}, {nameof(UseGroupBox)}: {UseGroupBox}, {nameof(ControlWidth)}: {ControlWidth}";
}