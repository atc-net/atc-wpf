namespace Atc.Wpf.UiTests;

/// <summary>
/// E2E test: on the Long text DialogBox sample page, every dialog that shows a message wraps it to the dialog width
/// (no horizontal scrolling, the text inside the dialog and next to the icon), in the light and dark theme and in
/// both flow directions. A short message keeps the dialog at its set height; a long one grows it, centred on the
/// owner, up to 80 % of the work area and then scrolls.
/// </summary>
[Collection("SampleApp")]
public sealed class DialogLongTextTests : IDisposable
{
    private const double DefaultWidth = 500;
    private const double DefaultHeight = 200;
    private const double ContentPadding = 30;
    private const double IconAndGap = 32 + 20;
    private const double Tolerance = 6;
    private const string HeaderText = "A header that is long enough to wrap onto a second line";

    private static readonly string[] TextKinds = ["Short", "OneLongLine", "SeveralLongLines", "LongPath", "VeryLong"];

    private readonly Application application;
    private readonly UIA3Automation automation;
    private readonly Window mainWindow;

    public DialogLongTextTests()
    {
        application = AutoScrollTestHelpers.LaunchSample(out automation, out mainWindow);
        AutoScrollTestHelpers.NavigateToSample(
            mainWindow,
            tabAutomationId: "TabWpfComponents",
            parentTreeNodeName: "Dialogs",
            leafTreeNodeName: "Long text DialogBox");
    }

    public void Dispose()
        => AutoScrollTestHelpers.CleanupApplication(application, automation);

    [Trait("Category", "UI")]
    [Theory]
    [InlineData("Light", false)]
    [InlineData("Light", true)]
    [InlineData("Dark", false)]
    [InlineData("Dark", true)]
    public void Text_dialogs_wrap_every_text_without_horizontal_scrolling(
        string theme,
        bool rightToLeft)
    {
        PickTheme(theme);
        SetChecked("LongTextRightToLeft", rightToLeft);

        foreach (var textKind in TextKinds)
        {
            SelectTextKind(textKind);
            AssertTextDialog("LongTextInfoInformation", textKind, rightToLeft, hasIcon: true);
            AssertTextDialog("LongTextQuestionHeader", textKind, rightToLeft, hasIcon: true);
        }

        SelectTextKind("OneLongLine");
        AssertTextDialog("LongTextInfoNone", "OneLongLine", rightToLeft, hasIcon: false);
        AssertTextDialog("LongTextInfoWithHeader", "OneLongLine", rightToLeft, hasIcon: true);
        AssertTextDialog("LongTextQuestionText", "OneLongLine", rightToLeft, hasIcon: true);
        AssertTextDialog("LongTextQuestionTitle", "OneLongLine", rightToLeft, hasIcon: true);
        AssertTextDialog("LongTextQuestionSettings", "OneLongLine", rightToLeft, hasIcon: true);

        SetChecked("LongTextNarrow", value: true);
        AssertTextDialog("LongTextQuestionSettings", "OneLongLine", rightToLeft, hasIcon: true, width: 350);
        AssertTextDialog("LongTextInfoInformation", "LongPath", rightToLeft, hasIcon: true, width: 350);
        SetChecked("LongTextNarrow", value: false);
    }

    [Trait("Category", "UI")]
    [Fact]
    public void Info_dialog_wraps_the_text_with_every_icon()
    {
        SelectTextKind("OneLongLine");
        foreach (var icon in new[] { "Critical", "Error", "Warning", "Security", "Audit", "Service", "UI", "Information", "Debug", "Trace" })
        {
            AssertTextDialog($"LongTextInfo{icon}", "OneLongLine", rightToLeft: false, hasIcon: true);
        }
    }

    [Trait("Category", "UI")]
    [Fact]
    public void Input_dialogs_do_not_scroll_horizontally_and_show_the_whole_header()
    {
        SelectTextKind("OneLongLine");
        foreach (var buttonId in new[] { "LongTextInput", "LongTextInputForm" })
        {
            var dialog = OpenDialog(buttonId);
            try
            {
                var viewer = ContentScrollViewer(dialog);
                Assert.False(viewer.Patterns.Scroll.Pattern.HorizontallyScrollable.Value, $"{buttonId} should not scroll horizontally.");

                // The Input sample sets its header after construction; the input form passes it to the constructor.
                var header = Retry.WhileNull(
                    () => dialog.FindFirstDescendant(cf => cf.ByControlType(ControlType.Text).And(cf.ByName(HeaderText))),
                    timeout: TimeSpan.FromSeconds(5),
                    interval: TimeSpan.FromMilliseconds(200)).Result;
                Assert.NotNull(header);
                Assert.False(header.IsOffscreen, $"{buttonId}: the header should be shown.");
                Assert.True(
                    header.BoundingRectangle.Bottom <= viewer.BoundingRectangle.Top + Tolerance,
                    $"{buttonId}: the header {header.BoundingRectangle} should end above the content {viewer.BoundingRectangle}, not be cut off.");
                Assert.True(
                    header.BoundingRectangle.Height > header.BoundingRectangle.Width / 20,
                    $"{buttonId}: the long header {header.BoundingRectangle} should wrap onto more than one line.");
            }
            finally
            {
                CloseDialogs();
            }
        }
    }

    /// <summary>
    /// The colour picker hosts a fixed-size picker, not text, so it keeps its own scrolling. It sets no owner, so it
    /// is found among the application's top-level windows.
    /// </summary>
    [Trait("Category", "UI")]
    [Fact]
    public void Color_picker_dialog_still_opens_with_its_picker()
    {
        var mainHandle = mainWindow.Properties.NativeWindowHandle.Value;
        FindByAutomationId(mainWindow, "LongTextColorPicker").Patterns.Invoke.Pattern.Invoke();
        var dialog = Retry.WhileNull(
            () => application
                .GetAllTopLevelWindows(automation)
                .FirstOrDefault(x => x.Properties.NativeWindowHandle.Value != mainHandle),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(dialog);

        try
        {
            Assert.NotNull(ContentScrollViewer(dialog));
        }
        finally
        {
            dialog.Close();
            WaitUntil(() => application.GetAllTopLevelWindows(automation).Length == 1);
        }
    }

    [Trait("Category", "UI")]
    [Fact]
    public void Long_text_grows_the_dialog_centred_on_the_owner_and_short_text_keeps_the_height()
    {
        MoveMainWindowToTopLeft();
        var ownerCentre = Centre(mainWindow.BoundingRectangle);

        SelectTextKind("Short");
        var shortDialog = OpenDialog("LongTextQuestionSettings");
        double scale;
        try
        {
            scale = shortDialog.BoundingRectangle.Width / DefaultWidth;
            Assert.Equal(DefaultHeight * scale, shortDialog.BoundingRectangle.Height, Tolerance * scale);
        }
        finally
        {
            CloseDialogs();
        }

        SelectTextKind("SeveralLongLines");
        var grownDialog = OpenDialog("LongTextQuestionSettings");
        try
        {
            Assert.True(
                grownDialog.BoundingRectangle.Height > (DefaultHeight + 20) * scale,
                $"The dialog ({grownDialog.BoundingRectangle}) should grow taller than {DefaultHeight} for several wrapped lines.");
            Assert.False(ContentScrollViewer(grownDialog).Patterns.Scroll.Pattern.VerticallyScrollable.Value);
            AssertCentredOn(grownDialog, ownerCentre);
        }
        finally
        {
            CloseDialogs();
        }

        mainWindow.Patterns.Window.Pattern.SetWindowVisualState(WindowVisualState.Maximized);
        WaitUntil(() => mainWindow.Patterns.Window.Pattern.WindowVisualState.Value == WindowVisualState.Maximized);
        var maximizedOwnerCentre = Centre(mainWindow.BoundingRectangle);
        var grownOnMaximizedOwner = OpenDialog("LongTextQuestionSettings");
        try
        {
            AssertCentredOn(grownOnMaximizedOwner, maximizedOwnerCentre);
        }
        finally
        {
            CloseDialogs();
        }

        SelectTextKind("VeryLong");
        var maxDialog = OpenDialog("LongTextQuestionSettings");
        try
        {
            var screen = System.Windows.Forms.Screen.FromRectangle(mainWindow.BoundingRectangle);
            Assert.True(
                maxDialog.BoundingRectangle.Height <= (screen.WorkingArea.Height * 0.8) + (Tolerance * scale),
                $"The dialog ({maxDialog.BoundingRectangle}) should stop at 80 % of the work area ({screen.WorkingArea}).");
            Assert.True(ContentScrollViewer(maxDialog).Patterns.Scroll.Pattern.VerticallyScrollable.Value);
            AssertCentredOn(maxDialog, maximizedOwnerCentre);
        }
        finally
        {
            CloseDialogs();
        }
    }

    private void AssertTextDialog(
        string buttonId,
        string textKind,
        bool rightToLeft,
        bool hasIcon,
        double width = DefaultWidth)
    {
        var dialog = OpenDialog(buttonId);
        try
        {
            var context = $"{buttonId} with {textKind}";
            var bounds = dialog.BoundingRectangle;
            var scale = bounds.Width / width;

            var viewer = ContentScrollViewer(dialog);
            var scroll = viewer.Patterns.Scroll.Pattern;
            Assert.False(scroll.HorizontallyScrollable.Value, $"{context}: the content should not scroll horizontally.");

            var text = MessageText(viewer);
            var viewerBounds = viewer.BoundingRectangle;
            var textBounds = text.BoundingRectangle;
            Assert.True(
                textBounds.Left >= viewerBounds.Left - Tolerance && textBounds.Right <= viewerBounds.Right + Tolerance,
                $"{context}: the text {textBounds} should lie inside the content area {viewerBounds}.");

            var expectedInset = (ContentPadding + (hasIcon ? IconAndGap : 0)) * scale;
            var inset = rightToLeft
                ? viewerBounds.Right - textBounds.Right
                : textBounds.Left - viewerBounds.Left;
            Assert.True(
                System.Math.Abs(inset - expectedInset) <= Tolerance * scale,
                $"{context}: the text {textBounds} should start {expectedInset}px from the {(rightToLeft ? "right" : "left")} edge of {viewerBounds} (was {inset}).");

            if (textKind != "VeryLong")
            {
                Assert.False(scroll.VerticallyScrollable.Value, $"{context}: the dialog should grow to fit the text.");
            }
        }
        finally
        {
            CloseDialogs();
        }
    }

    private Window OpenDialog(string buttonId)
    {
        FindByAutomationId(mainWindow, buttonId).Patterns.Invoke.Pattern.Invoke();
        var dialog = Retry.WhileNull(
            () => mainWindow.ModalWindows.FirstOrDefault(),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(dialog);
        return dialog;
    }

    /// <summary>
    /// The dialog's content ScrollViewer: the first scrollable pane (the input controls inside it come later).
    /// </summary>
    private static AutomationElement ContentScrollViewer(
        AutomationElement dialog)
    {
        var viewer = Retry.WhileNull(
            () => dialog
                .FindAllDescendants(cf => cf.ByControlType(ControlType.Pane))
                .FirstOrDefault(x => x.Patterns.Scroll.IsSupported),
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(viewer);
        return viewer;
    }

    /// <summary>
    /// The message: the longest text element in the content area (the header lies outside it).
    /// </summary>
    private static AutomationElement MessageText(AutomationElement viewer)
    {
        var texts = viewer.FindAllDescendants(cf => cf.ByControlType(ControlType.Text));
        var text = texts.MaxBy(x => x.Name?.Length ?? 0);
        Assert.NotNull(text);
        return text;
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Best-effort cleanup; a dialog left open would block the next step.")]
    private void CloseDialogs()
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

        WaitUntil(() => mainWindow.ModalWindows.Length == 0);
    }

    private void SelectTextKind(string textKind)
    {
        var comboBox = FindByAutomationId(mainWindow, "LongTextKind").AsComboBox();
        comboBox.Select(Array.IndexOf(TextKinds, textKind));
        comboBox.Collapse();
        WaitUntil(() => comboBox.SelectedItem?.Text == textKind);
    }

    private void SetChecked(
        string automationId,
        bool value)
    {
        var checkBox = FindByAutomationId(mainWindow, automationId).AsCheckBox();
        if (checkBox.IsChecked != value)
        {
            checkBox.Toggle();
        }

        WaitUntil(() => checkBox.IsChecked == value);
    }

    /// <summary>
    /// ThemeSelector lists the base themes ordered by display name: Dark, then Light.
    /// </summary>
    [SuppressMessage("Major Bug", "S2925:Do not use Thread.Sleep", Justification = "The theme is applied asynchronously; give it time to render.")]
    private void PickTheme(string mode)
    {
        var comboBox = FindByAutomationId(mainWindow, "LongTextThemeSelector")
            .FindFirstDescendant(cf => cf.ByControlType(ControlType.ComboBox))
            .AsComboBox();
        comboBox.Select(mode == "Dark" ? 0 : 1);
        comboBox.Collapse();
        Thread.Sleep(500);
    }

    private void MoveMainWindowToTopLeft()
    {
        var windowPattern = mainWindow.Patterns.Window.Pattern;
        windowPattern.SetWindowVisualState(WindowVisualState.Normal);
        WaitUntil(() => windowPattern.WindowVisualState.Value == WindowVisualState.Normal);

        var transformPattern = mainWindow.Patterns.Transform.Pattern;
        transformPattern.Resize(1000, 700);
        transformPattern.Move(0, 0);
        WaitUntil(() => mainWindow.BoundingRectangle.X <= 10 && mainWindow.BoundingRectangle.Y <= 10);
        mainWindow.Focus();
    }

    private static void AssertCentredOn(
        AutomationElement dialog,
        System.Drawing.PointF ownerCentre)
    {
        var dialogCentre = Centre(dialog.BoundingRectangle);
        Assert.True(
            System.Math.Abs(dialogCentre.X - ownerCentre.X) <= Tolerance &&
            System.Math.Abs(dialogCentre.Y - ownerCentre.Y) <= Tolerance,
            $"Dialog centre {dialogCentre} should be within {Tolerance}px of the owner centre {ownerCentre}.");
    }

    private static AutomationElement FindByAutomationId(
        AutomationElement parent,
        string automationId)
    {
        var element = Retry.WhileNull(
            () => parent.FindFirstDescendant(cf => cf.ByAutomationId(automationId)),
            timeout: TimeSpan.FromSeconds(10),
            interval: TimeSpan.FromMilliseconds(200)).Result;
        Assert.NotNull(element);
        return element;
    }

    private static System.Drawing.PointF Centre(System.Drawing.Rectangle bounds)
        => new(
            bounds.X + (bounds.Width / 2f),
            bounds.Y + (bounds.Height / 2f));

    private static void WaitUntil(Func<bool> condition)
        => Retry.WhileFalse(
            condition,
            timeout: TimeSpan.FromSeconds(5),
            interval: TimeSpan.FromMilliseconds(200));
}