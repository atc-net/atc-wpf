namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

/// <summary>
/// One step of a 4-wire SPI capture of a graphic display: a run of command bytes (D/C low) or data bytes (D/C high).
/// </summary>
/// <param name="IsData"><see langword="true"/> for display data; <see langword="false"/> for command bytes.</param>
/// <param name="Bytes">The bytes of the step.</param>
[SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "OK - a capture step is a plain byte run.")]
public sealed record GraphicDisplayCaptureStep(
    bool IsData,
    byte[] Bytes);