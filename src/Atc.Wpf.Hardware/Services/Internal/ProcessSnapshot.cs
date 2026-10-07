namespace Atc.Wpf.Hardware.Services.Internal;

/// <summary>
/// One polled observation of a running process.
/// </summary>
internal sealed record ProcessSnapshot(
    int ProcessId,
    string ProcessName,
    string MainWindowTitle,
    bool HasMainWindow);