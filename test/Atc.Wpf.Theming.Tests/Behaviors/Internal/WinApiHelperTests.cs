namespace Atc.Wpf.Theming.Tests.Behaviors.Internal;

public sealed class WinApiHelperTests
{
    [StaFact]
    public void SetWindowPlacement_AppliesTheSavedNormalPosition()
    {
        var window = new Window
        {
            WindowStyle = WindowStyle.None,
            ShowInTaskbar = false,
            ShowActivated = false,
            Left = 10,
            Top = 10,
            Width = 200,
            Height = 150,
        };

        try
        {
            new WindowInteropHelper(window).EnsureHandle();
            var saved = new WINDOWPLACEMENT
            {
                showCmd = SHOW_WINDOW_CMD.SW_SHOWNORMAL,
                rcNormalPosition = new RECT
                {
                    left = 123,
                    top = 87,
                    right = 123 + 345,
                    bottom = 87 + 234,
                },
            };

            WinApiHelper.SetWindowPlacement(window, saved);

            var actual = WinApiHelper.GetWindowPlacement(window);

            Assert.Equal(123, actual.rcNormalPosition.left);
            Assert.Equal(87, actual.rcNormalPosition.top);
            Assert.Equal(123 + 345, actual.rcNormalPosition.right);
            Assert.Equal(87 + 234, actual.rcNormalPosition.bottom);
        }
        finally
        {
            window.Close();
        }
    }
}