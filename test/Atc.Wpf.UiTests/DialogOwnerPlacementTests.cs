namespace Atc.Wpf.UiTests;

/// <summary>
/// E2E test: on the Standard DialogBox sample page, the Info dialog opens as a separate modal window owned by
/// the main window, centred over it, and closing it gives focus back to the main window.
/// </summary>
/// <remarks>
/// The main window is moved away from the middle of the screen first, so a dialog centred on the screen
/// instead of on its owner fails the test.
/// </remarks>
[Collection("SampleApp")]
public sealed class DialogOwnerPlacementTests : IDisposable
{
    private const double CentreTolerance = 5;
    private const double MinimumOffsetFromScreenCentre = 150;

    private readonly Application application;
    private readonly UIA3Automation automation;
    private readonly Window mainWindow;

    public DialogOwnerPlacementTests()
    {
        application = AutoScrollTestHelpers.LaunchSample(out automation, out mainWindow);
    }

    public void Dispose()
        => AutoScrollTestHelpers.CleanupApplication(application, automation);

    [Trait("Category", "UI")]
    [Fact]
    public void Info_dialog_is_centred_on_the_main_window_and_focus_returns_when_it_closes()
    {
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            tabAutomationId: "TabWpfComponents",
            parentTreeNodeName: "Dialogs",
            leafTreeNodeName: "Standard DialogBox");

        // Maximized, the main window is centred on the work area, which is where CenterScreen would put the dialog too.
        var screenCentre = Centre(mainWindow.BoundingRectangle);
        MoveMainWindowToTopLeft();
        var ownerCentre = Centre(mainWindow.BoundingRectangle);
        Assert.True(
            Distance(ownerCentre, screenCentre) >= MinimumOffsetFromScreenCentre,
            $"The main window centre {ownerCentre} must be away from the screen centre {screenCentre} for the test to tell owner and screen centring apart.");

        var view = Retry.WhileNull(
            () => mainWindow.FindFirstDescendant(cf => cf.ByClassName("StandardDialogBoxView")),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(view);

        FindButton(view, "Info").Patterns.Invoke.Pattern.Invoke();
        var dialog = Retry.WhileNull(
            () => mainWindow.ModalWindows.FirstOrDefault(),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(dialog);

        try
        {
            Assert.Equal("This is a information", dialog.Title);
            Assert.NotEqual(
                mainWindow.Properties.NativeWindowHandle.Value,
                dialog.Properties.NativeWindowHandle.Value);

            var dialogCentre = Centre(dialog.BoundingRectangle);
            Assert.True(
                System.Math.Abs(dialogCentre.X - ownerCentre.X) <= CentreTolerance &&
                System.Math.Abs(dialogCentre.Y - ownerCentre.Y) <= CentreTolerance,
                $"Dialog centre {dialogCentre} should be within {CentreTolerance}px of the main window centre {ownerCentre}.");

            FindButton(dialog, "OK").Patterns.Invoke.Pattern.Invoke();
            WaitUntil(() => mainWindow.ModalWindows.Length == 0);
            Assert.Empty(mainWindow.ModalWindows);
        }
        finally
        {
            CloseDialogIfStillOpen();
        }

        var mainWindowHandle = mainWindow.Properties.NativeWindowHandle.Value;
        WaitUntil(() => FocusedWindowHandle() == mainWindowHandle);
        Assert.Equal(mainWindowHandle, FocusedWindowHandle());
    }

    /// <summary>
    /// Returns the handle of the nearest window that holds the keyboard focus. The dialog is a child of the main
    /// window in the automation tree, so focus left in the dialog does not count as focus in the main window.
    /// </summary>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "The focused element can go away while the tree is walked; the caller retries.")]
    private IntPtr FocusedWindowHandle()
    {
        try
        {
            var walker = automation.TreeWalkerFactory.GetControlViewWalker();
            var element = automation.FocusedElement();
            while (element is not null &&
                   element.ControlType != ControlType.Window)
            {
                element = walker.GetParent(element);
            }

            return element?.Properties.NativeWindowHandle.ValueOrDefault ?? IntPtr.Zero;
        }
        catch (Exception)
        {
            return IntPtr.Zero;
        }
    }

    private void MoveMainWindowToTopLeft()
    {
        var windowPattern = mainWindow.Patterns.Window.Pattern;
        windowPattern.SetWindowVisualState(WindowVisualState.Normal);
        WaitUntil(() => windowPattern.WindowVisualState.Value == WindowVisualState.Normal);
        Assert.Equal(WindowVisualState.Normal, windowPattern.WindowVisualState.Value);

        // Shrink to the minimum size first (the window clamps to its MinWidth/MinHeight), so moving it to the
        // corner takes its centre well away from the screen centre on smaller screens too.
        var transformPattern = mainWindow.Patterns.Transform.Pattern;
        Assert.True(transformPattern.CanResize.Value);
        var normalBounds = mainWindow.BoundingRectangle;
        transformPattern.Resize(800, 600);
        WaitUntil(() => mainWindow.BoundingRectangle.Width < normalBounds.Width);
        Assert.True(mainWindow.BoundingRectangle.Width < normalBounds.Width);

        Assert.True(transformPattern.CanMove.Value);
        transformPattern.Move(0, 0);
        WaitUntil(() => mainWindow.BoundingRectangle.X <= 10 && mainWindow.BoundingRectangle.Y <= 10);
        Assert.True(mainWindow.BoundingRectangle.X <= 10 && mainWindow.BoundingRectangle.Y <= 10);

        mainWindow.Focus();
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Best-effort cleanup; a dialog left open would block closing the sample app.")]
    private void CloseDialogIfStillOpen()
    {
        try
        {
            foreach (var modalWindow in mainWindow.ModalWindows)
            {
                modalWindow.Close();
            }
        }
        catch (Exception)
        {
            // Best-effort cleanup.
        }
    }

    private static AutomationElement FindButton(
        AutomationElement parent,
        string name)
    {
        var element = Retry.WhileNull(
            () => parent.FindFirstDescendant(cf => cf.ByControlType(ControlType.Button).And(cf.ByName(name))),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(element);
        return element;
    }

    private static System.Drawing.PointF Centre(System.Drawing.Rectangle bounds)
        => new(
            bounds.X + (bounds.Width / 2f),
            bounds.Y + (bounds.Height / 2f));

    private static double Distance(
        System.Drawing.PointF a,
        System.Drawing.PointF b)
        => System.Math.Sqrt(System.Math.Pow(a.X - b.X, 2) + System.Math.Pow(a.Y - b.Y, 2));

    private static void WaitUntil(Func<bool> condition)
        => Retry.WhileFalse(
            condition,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
}