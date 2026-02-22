namespace Atc.Wpf.Sample.ScreenshotGeneration;

/// <summary>
/// Represents a single control to capture a screenshot for.
/// </summary>
internal sealed record ScreenshotItem(
    string AssemblyArea,
    string Category,
    string ControlName,
    string SamplePath);