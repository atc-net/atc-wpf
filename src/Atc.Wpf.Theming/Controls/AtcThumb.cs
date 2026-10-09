namespace Atc.Wpf.Theming.Controls;

/// <summary>
/// A <see cref="Thumb"/> that captures the touch device while it is being dragged.
/// </summary>
public sealed class AtcThumb : Thumb, IAtcThumb
{
    private TouchDevice? currentDevice;

    /// <inheritdoc />
    protected override void OnPreviewTouchDown(TouchEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnPreviewTouchDown(e);

        ReleaseCurrentDevice();
        CaptureCurrentDevice(e);
    }

    /// <inheritdoc />
    protected override void OnPreviewTouchUp(TouchEventArgs e)
    {
        base.OnPreviewTouchUp(e);

        ReleaseCurrentDevice();
    }

    /// <inheritdoc />
    protected override void OnLostTouchCapture(TouchEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        base.OnLostTouchCapture(e);

        if (currentDevice == null)
        {
            return;
        }

        CaptureCurrentDevice(e);
    }

    private void ReleaseCurrentDevice()
    {
        if (currentDevice == null)
        {
            return;
        }

        var temp = currentDevice;
        currentDevice = null;
        ReleaseTouchCapture(temp);
    }

    private void CaptureCurrentDevice(TouchEventArgs e)
    {
        var gotTouch = CaptureTouch(e.TouchDevice);
        if (gotTouch)
        {
            currentDevice = e.TouchDevice;
        }
    }
}