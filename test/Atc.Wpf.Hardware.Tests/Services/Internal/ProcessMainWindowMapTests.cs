namespace Atc.Wpf.Hardware.Tests.Services.Internal;

public sealed class ProcessMainWindowMapTests
{
    [Fact]
    public void Build_SeveralMainWindowsForOneProcess_KeepsTheFirstInZOrder()
    {
        var map = ProcessMainWindowMap.Build(
            [1, 2, 3],
            hWnd => hWnd == 3 ? 200 : 100,
            _ => true);

        Assert.Equal(2, map.Count);
        Assert.Equal(1, map[100]);
        Assert.Equal(3, map[200]);
    }

    [Fact]
    public void Build_OwnedOrHiddenWindowsBeforeTheMainWindow_AreSkipped()
    {
        // Windows 1 and 2 are not main windows (owned or hidden); window 3 is.
        var map = ProcessMainWindowMap.Build(
            [1, 2, 3],
            _ => 100,
            hWnd => hWnd == 3);

        Assert.Equal(3, Assert.Single(map).Value);
    }

    [Fact]
    public void Build_ProcessWithoutAMainWindow_IsNotMapped()
    {
        var map = ProcessMainWindowMap.Build(
            [1, 2],
            _ => 100,
            _ => false);

        Assert.Empty(map);
    }

    [Fact]
    public void Build_ProcessAlreadyMapped_DoesNotCheckItsLaterWindows()
    {
        var checkedWindows = new List<IntPtr>();

        ProcessMainWindowMap.Build(
            [1, 2, 3],
            _ => 100,
            hWnd =>
            {
                checkedWindows.Add(hWnd);
                return true;
            });

        Assert.Equal([(IntPtr)1], checkedWindows);
    }

    [WpfFact]
    public void Capture_MatchesProcessMainWindowHandleAndTitle()
    {
        var window = new Window
        {
            Title = "Process main window probe",
            Width = 120,
            Height = 80,
            Left = -10000,
            Top = -10000,
            ShowActivated = false,
            WindowStartupLocation = WindowStartupLocation.Manual,
        };

        window.Show();
        try
        {
            var map = ProcessMainWindowMap.Capture();

            using var process = System.Diagnostics.Process.GetCurrentProcess();
            var expectedHandle = process.MainWindowHandle;
            Assert.NotEqual(IntPtr.Zero, expectedHandle);
            Assert.True(map.TryGetValue(process.Id, out var handle));
            Assert.Equal(expectedHandle, handle);
            Assert.Equal(process.MainWindowTitle, NativeWindowMethods.ReadWindowTitle(handle));
        }
        finally
        {
            window.Close();
        }
    }
}