namespace Atc.Wpf.Hardware.Models;

public sealed record UsbCameraFormat(
    uint Width,
    uint Height,
    double FrameRate,
    string Subtype)
{
    public override string ToString()
        => $"{Width}×{Height} @ {FrameRate.ToString("0.#", CultureInfo.InvariantCulture)} fps ({Subtype})";
}