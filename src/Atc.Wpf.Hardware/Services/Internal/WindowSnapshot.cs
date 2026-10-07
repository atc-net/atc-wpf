namespace Atc.Wpf.Hardware.Services.Internal;

/// <summary>
/// One polled observation of a top-level window.
/// </summary>
internal sealed record WindowSnapshot(
    IntPtr Handle,
    string Title,
    string ClassName,
    int ProcessId);