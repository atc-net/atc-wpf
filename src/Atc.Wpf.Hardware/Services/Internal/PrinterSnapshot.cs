namespace Atc.Wpf.Hardware.Services.Internal;

/// <summary>
/// One polled observation of a print queue.
/// </summary>
internal sealed record PrinterSnapshot(
    string Name,
    string FullName,
    bool IsLocal,
    bool IsShared,
    bool IsDefault,
    string QueueStatus);