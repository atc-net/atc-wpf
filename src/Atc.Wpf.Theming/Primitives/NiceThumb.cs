namespace Atc.Wpf.Theming.Primitives;

/// <summary>
/// A <see cref="Thumb"/> that captures the touch device while it is being dragged.
/// </summary>
public sealed class NiceThumb : Thumb, INiceThumb
{
    private TouchDevice? currentDevice;

    /// <inheritdoc />
    protected override void OnPreviewTouchDown(TouchEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);
        ReleaseCurrentDevice();
        CaptureCurrentDevice(e);
    }

    /// <inheritdoc />
    protected override void OnPreviewTouchUp(TouchEventArgs e)
    {
        ReleaseCurrentDevice();
    }

    /// <inheritdoc />
    protected override void OnLostTouchCapture(TouchEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (currentDevice is null)
        {
            return;
        }

        CaptureCurrentDevice(e);
    }

    private void ReleaseCurrentDevice()
    {
        if (currentDevice is null)
        {
            return;
        }

        var temp = currentDevice;
        currentDevice = null;
        ReleaseTouchCapture(temp);
    }

    private void CaptureCurrentDevice(TouchEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        var gotTouch = CaptureTouch(e.TouchDevice);
        if (gotTouch)
        {
            currentDevice = e.TouchDevice;
        }
    }
}