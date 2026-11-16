namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

/// <summary>
/// One step of a logical HD44780 capture: an instruction (RS low) or a data byte (RS high).
/// </summary>
/// <param name="IsData"><see langword="true"/> for a data byte; <see langword="false"/> for an instruction.</param>
/// <param name="Value">The byte.</param>
public readonly record struct Hd44780CaptureStep(
    bool IsData,
    byte Value);