namespace Atc.Wpf.Hardware.Tests.Models;

public sealed class RunningProcessInfoTests
{
    [Fact]
    public void Constructor_AssignsAllProperties()
    {
        var info = new RunningProcessInfo(
            processId: 1234,
            processName: "notepad",
            mainWindowTitle: "Untitled - Notepad",
            mainModulePath: @"C:\Windows\notepad.exe");

        Assert.Equal(1234, info.ProcessId);
        Assert.Equal("notepad", info.ProcessName);
        Assert.Equal("Untitled - Notepad", info.MainWindowTitle);
        Assert.Equal(@"C:\Windows\notepad.exe", info.MainModulePath);
        Assert.Equal("1234", info.DeviceId);
        Assert.Equal(DeviceState.Unknown, info.State);
    }

    [Theory]
    [InlineData("notepad", "", "notepad")]
    [InlineData("notepad", "Untitled", "notepad — Untitled")]
    public void FriendlyName_FormatsByTitle(
        string processName,
        string title,
        string expected)
    {
        var info = new RunningProcessInfo(
            processId: 1,
            processName: processName,
            mainWindowTitle: title,
            mainModulePath: null);

        Assert.Equal(expected, info.FriendlyName);
    }

    [Fact]
    public void ToString_IncludesPid()
    {
        var info = new RunningProcessInfo(
            processId: 42,
            processName: "chrome",
            mainWindowTitle: "Google",
            mainModulePath: null);

        Assert.Contains("PID 42", info.ToString(), StringComparison.Ordinal);
    }
}