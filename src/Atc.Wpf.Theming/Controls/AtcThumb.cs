namespace Atc.Wpf.Theming.Controls;

public class AtcThumb : Thumb, IAtcThumb
{
    private TouchDevice? currentDevice;

    protected override void OnPreviewTouchDown(
        TouchEventArgs e)
    {
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