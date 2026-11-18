namespace Atc.Wpf.FontIcons;

// The spin, rotation and flip dependency properties that every font icon and image icon control declares.
// The callbacks named here must exist in each control that uses the template.
[DependencyPropertyTemplate]
internal abstract class IconTransformMembers
{
    /// <summary>
    /// Gets or sets the current spin (angle) animation of the icon.
    /// </summary>
    [DependencyProperty(DefaultValue = false, PropertyChangedCallback = "OnSpinChanged")]
    private bool spin;

    /// <summary>
    /// Gets or sets the duration of the spinning animation (in seconds). This will stop and start the spin animation.
    /// </summary>
    [DependencyProperty(DefaultValue = 1d, PropertyChangedCallback = "OnSpinDurationChanged", CoerceValueCallback = "FontIconHelper.SpinDurationCoerceValue")]
    private double spinDuration;

    /// <summary>
    /// Gets or sets the current rotation (angle).
    /// </summary>
    [DependencyProperty(DefaultValue = 0d, PropertyChangedCallback = "OnRotationChanged", CoerceValueCallback = "FontIconHelper.RotationCoerceValue")]
    private double rotation;

    /// <summary>
    /// Gets or sets the current orientation (horizontal, vertical).
    /// </summary>
    [DependencyProperty(DefaultValue = FlipOrientationType.Normal, PropertyChangedCallback = "OnFlipOrientationChanged")]
    private FlipOrientationType flipOrientation;
}