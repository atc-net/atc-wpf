namespace Atc.Wpf.Theming.Primitives;

public class NiceThumb : Thumb, INiceThumb
{
    private TouchDevice? currentDevice;

    protected override void OnPreviewTouchDown(
        TouchEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);
        ArgumentNullException.ThrowIfNull(e);
        ReleaseCurrentDevice();
        CaptureCurrentDevice(e);
    }

    protected override void OnPreviewTouchUp(
        TouchEventArgs e)
    {
        ReleaseCurrentDevice();
    }

    protected override void OnLostTouchCapture(
        TouchEventArgs e)
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

    private void CaptureCurrentDevice(
        TouchEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        var gotTouch = CaptureTouch(e.TouchDevice);
        if (gotTouch)
        {
            currentDevice = e.TouchDevice;
        }
    }
}