namespace Atc.Wpf.Components.Tests.Dialogs;

/// <summary>
/// <see cref="InWindowDialogService"/> shows the standard dialogs as centred modal flyouts in the window's
/// <see cref="FlyoutHost"/>, answers like the windowed dialogs, and falls back to them without a host.
/// </summary>
[Collection(nameof(PackResourceTestGroup))]
public sealed class InWindowDialogServiceTests
{
    [StaFact]
    public void Information_ShowsTheMessageInACentredModalFlyout_AndOkAnswersTrue()
    {
        using var window = FlyoutWindow.Show();
        var sut = new InWindowDialogService(new FlyoutService(window.Host));

        var task = sut.ShowInformation("About", "Version 4.5");
        FlyoutWindow.Pump();

        var flyout = TopFlyout(window);
        Assert.Equal("About", flyout.Header);
        Assert.Equal(FlyoutPosition.Center, flyout.Position);
        Assert.False(flyout.IsLightDismissEnabled);
        Assert.True(flyout.ShowOverlay);
        Assert.Contains(TextsIn(flyout), t => t == "Version 4.5");
        Assert.NotNull(Content(flyout).ButtonBar.GetButton(DialogBoxButton.Affirmative));
        Assert.Null(Content(flyout).ButtonBar.GetButton(DialogBoxButton.Negative));

        Click(flyout, DialogBoxButton.Affirmative);

        Assert.True(Result(task));
        Assert.Empty(window.Host.Items);
    }

    [StaTheory]
    [InlineData("warning")]
    [InlineData("error")]
    [InlineData("information")]
    public void MessageDialogs_ClosedWithTheCloseButton_AnswerFalse(string kind)
    {
        using var window = FlyoutWindow.Show();
        var sut = new InWindowDialogService(new FlyoutService(window.Host));

        var task = kind switch
        {
            "warning" => sut.ShowWarning("Disk", "Almost full"),
            "error" => sut.ShowError("Disk", "Full"),
            _ => sut.ShowInformation("Disk", "Fine"),
        };
        FlyoutWindow.Pump();
        CloseWithTheCloseButton(TopFlyout(window));

        Assert.False(Result(task));
    }

    [StaTheory]
    [InlineData(DialogBoxButton.Affirmative, true)]
    [InlineData(DialogBoxButton.Negative, false)]
    public void Confirmation_YesAnswersTrue_NoAnswersFalse(
        DialogBoxButton button,
        bool expected)
    {
        using var window = FlyoutWindow.Show();
        var sut = new InWindowDialogService(new FlyoutService(window.Host));

        var task = sut.ShowConfirmation("Delete", "Delete the file?");
        FlyoutWindow.Pump();
        var flyout = TopFlyout(window);
        var expectedSettings = DialogService.CreateConfirmationSettings("Delete");
        Assert.Equal(expectedSettings.AffirmativeButtonText, Content(flyout).ButtonBar.GetButton(DialogBoxButton.Affirmative)!.Content);
        Assert.Equal(expectedSettings.NegativeButtonText, Content(flyout).ButtonBar.GetButton(DialogBoxButton.Negative)!.Content);

        Click(flyout, button);

        Assert.Equal(expected, Result(task));
    }

    [StaFact]
    public void OkCancel_CancelAnswersFalse()
    {
        using var window = FlyoutWindow.Show();
        var sut = new InWindowDialogService(new FlyoutService(window.Host));

        var task = sut.ShowOkCancel("Save", "Save the changes?");
        FlyoutWindow.Pump();
        Click(TopFlyout(window), DialogBoxButton.Negative);

        Assert.False(Result(task));
    }

    [StaFact]
    public void Confirmation_FocusesTheDefaultButtonWhenOpened()
    {
        using var window = FlyoutWindow.Show();
        var sut = new InWindowDialogService(new FlyoutService(window.Host));

        _ = sut.ShowConfirmation("Delete", "Delete the file?");
        var flyout = TopFlyout(window);
        var yes = Content(flyout).ButtonBar.GetButton(DialogBoxButton.Affirmative)!;

        Assert.True(FlyoutWindow.WaitUntil(() => yes.IsKeyboardFocused), "The default button did not get the focus.");
        Content(flyout).ButtonBar.GetButton(DialogBoxButton.Negative)!.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
    }

    [StaFact]
    public void Input_ShowsTheDefaultValue_FocusesTheTextBox_AndOkReturnsTheText()
    {
        using var window = FlyoutWindow.Show();
        var sut = new InWindowDialogService(new FlyoutService(window.Host));

        var task = sut.ShowInput("Rename", "Name", "old.txt");
        var flyout = TopFlyout(window);
        var textBox = Assert.Single(Descendants(flyout).OfType<TextBox>());
        Assert.Equal("old.txt", textBox.Text);
        Assert.True(FlyoutWindow.WaitUntil(() => textBox.IsKeyboardFocusWithin), "The text box did not get the focus.");

        textBox.Text = "new.txt";
        Click(flyout, DialogBoxButton.Affirmative);

        Assert.Equal("new.txt", Result(task));
    }

    [StaFact]
    public void Input_CancelReturnsNull()
    {
        using var window = FlyoutWindow.Show();
        var sut = new InWindowDialogService(new FlyoutService(window.Host));

        var task = sut.ShowInput("Rename", "Name", "old.txt");
        FlyoutWindow.Pump();
        Click(TopFlyout(window), DialogBoxButton.Negative);

        Assert.Null(Result(task));
    }

    [StaFact]
    public void ColorPicker_OkReturnsTheColour_CancelReturnsNull()
    {
        using var window = FlyoutWindow.Show(width: 1000, height: 900);
        var sut = new InWindowDialogService(new FlyoutService(window.Host));

        var ok = sut.ShowColorPicker("Colour", Colors.Teal);
        FlyoutWindow.Pump();
        Click(TopFlyout(window), DialogBoxButton.Affirmative);
        Assert.Equal(Colors.Teal, Result(ok));

        var cancel = sut.ShowColorPicker("Colour", Colors.Teal);
        FlyoutWindow.Pump();
        Click(TopFlyout(window), DialogBoxButton.Negative);
        Assert.Null(Result(cancel));
    }

    [StaFact]
    public void FontPicker_OkReturnsTheFont_CancelReturnsNull()
    {
        using var window = FlyoutWindow.Show(width: 1100, height: 900);
        var sut = new InWindowDialogService(new FlyoutService(window.Host));
        var initial = new FontDescription(new FontFamily("Courier New"), 18, FontWeights.Bold, FontStyles.Normal, FontStretches.Normal);

        var ok = sut.ShowFontPicker("Font", initial);
        FlyoutWindow.Pump();
        Click(TopFlyout(window), DialogBoxButton.Affirmative);
        var font = Result(ok);
        Assert.NotNull(font);
        Assert.Equal("Courier New", font.Family.Source);
        Assert.Equal(18, font.Size);

        var cancel = sut.ShowFontPicker("Font", initial);
        FlyoutWindow.Pump();
        Click(TopFlyout(window), DialogBoxButton.Negative);
        Assert.Null(Result(cancel));
    }

    [StaFact]
    public void InputForm_OkWithAnInvalidForm_StaysOpen_AndOkWhenValidAnswersTrue()
    {
        using var window = FlyoutWindow.Show();
        var sut = new InWindowDialogService(new FlyoutService(window.Host));
        var name = new LabelTextBox { LabelText = "Name", IsMandatory = true };
        var form = new LabelControlsForm();
        form.AddColumn(new List<ILabelControlBase> { name });

        var task = sut.ShowInputForm("Person", form);
        FlyoutWindow.Pump();
        var flyout = TopFlyout(window);
        Assert.Contains(name, Descendants(flyout));

        Content(flyout).ButtonBar.GetButton(DialogBoxButton.Affirmative)!.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        FlyoutWindow.Pump();

        // Closing clears IsOpen at once (the close animation runs afterwards).
        Assert.True(flyout.IsOpen);
        Assert.False(task.IsCompleted);

        name.Text = "Ada";
        Click(flyout, DialogBoxButton.Affirmative);

        Assert.True(Result(task));
        Assert.Equal("Ada", form.GetKeyValues().Values.Single());
    }

    [StaFact]
    public void InputForm_CancelAnswersFalse()
    {
        using var window = FlyoutWindow.Show();
        var sut = new InWindowDialogService(new FlyoutService(window.Host));
        var form = new LabelControlsForm();
        form.AddColumn(new List<ILabelControlBase> { new LabelTextBox { LabelText = "Name", IsMandatory = true } });

        var task = sut.ShowInputForm("Person", form);
        FlyoutWindow.Pump();
        Click(TopFlyout(window), DialogBoxButton.Negative);

        Assert.False(Result(task));
    }

    [StaFact]
    public void AnAlreadyCancelledToken_NeverShowsTheDialog()
    {
        using var window = FlyoutWindow.Show();
        var sut = new InWindowDialogService(new FlyoutService(window.Host));
        var task = sut.ShowConfirmation("Delete", "Delete?", new CancellationToken(canceled: true));

        Assert.True(task.IsCanceled);
        Assert.Empty(window.Host.Items);
    }

    [StaFact]
    public void CancellingTheToken_ClosesTheOpenDialog_AndCancelsTheTask()
    {
        using var window = FlyoutWindow.Show();
        var sut = new InWindowDialogService(new FlyoutService(window.Host));
        var cts = new CancellationTokenSource();
        try
        {
            var task = sut.ShowConfirmation("Delete", "Delete?", cts.Token);
            _ = TopFlyout(window);

            // Cancelled on the UI thread, as a view model would.
            cts.Cancel();

            Assert.True(FlyoutWindow.WaitUntil(() => task.IsCompleted && window.Host.Items.Count == 0), "The dialog did not close.");
            Assert.True(task.IsCanceled);
        }
        finally
        {
            cts.Dispose();
        }
    }

    [StaFact]
    public void TwoDialogs_StackAndAnswerIndependently()
    {
        using var window = FlyoutWindow.Show();
        var sut = new InWindowDialogService(new FlyoutService(window.Host));

        var first = sut.ShowConfirmation("First", "First?");
        var second = sut.ShowOkCancel("Second", "Second?");
        FlyoutWindow.Pump();
        Assert.Equal(2, window.Host.Items.Count);

        Click(TopFlyout(window), DialogBoxButton.Negative);
        Assert.False(Result(second));
        Assert.False(first.IsCompleted);

        Click(TopFlyout(window), DialogBoxButton.Affirmative);
        Assert.True(Result(first));
    }

    [StaFact]
    public void RightToLeftWindow_GivesARightToLeftDialog()
    {
        using var window = FlyoutWindow.Show(flowDirection: FlowDirection.RightToLeft);
        var sut = new InWindowDialogService(new FlyoutService(window.Host));

        _ = sut.ShowInformation("About", "Version");
        FlyoutWindow.Pump();

        Assert.Equal(FlowDirection.RightToLeft, Content(TopFlyout(window)).FlowDirection);
        Click(TopFlyout(window), DialogBoxButton.Affirmative);
    }

    [StaFact]
    public void WithoutAHost_UsesTheFallbackService_AndTracesOnce()
    {
        var fallback = Substitute.For<IDialogService>();
        fallback.ShowConfirmation("Delete", "Delete?", Arg.Any<CancellationToken>()).Returns(true);
        fallback.ShowInput("Rename", "Name", "a", Arg.Any<CancellationToken>()).Returns("b");
        var sut = new InWindowDialogService(new FlyoutService(), fallback);
        var listener = new RecordingTraceListener();
        Trace.Listeners.Add(listener);
        try
        {
            var confirmation = sut.ShowConfirmation("Delete", "Delete?");
            var input = sut.ShowInput("Rename", "Name", "a");

            Assert.True(confirmation.Result);
            Assert.Equal("b", input.Result);
            Assert.Single(listener.Messages, m => m.Contains(nameof(InWindowDialogService), StringComparison.Ordinal));
        }
        finally
        {
            Trace.Listeners.Remove(listener);
        }
    }

    // The top flyout once its open animation has finished: a flyout ignores a close while it is animating.
    private static Flyout TopFlyout(FlyoutWindow window)
    {
        var flyout = Assert.IsType<Flyout>(window.Host.Items[^1]);
        var isAnimating = typeof(Flyout).GetField("isAnimating", BindingFlags.Instance | BindingFlags.NonPublic)!;
        Assert.True(
            FlyoutWindow.WaitUntil(() => flyout.IsOpen && !(bool)isAnimating.GetValue(flyout)!),
            "The dialog did not open.");
        return flyout;
    }

    private static InWindowDialogContent Content(Flyout flyout)
        => Assert.IsType<InWindowDialogContent>(flyout.Content);

    private static void Click(
        Flyout flyout,
        DialogBoxButton button)
    {
        var count = ((FlyoutHost)ItemsControl.ItemsControlFromItemContainer(flyout) ?? throw new InvalidOperationException("No host.")).Items.Count;
        Content(flyout).ButtonBar.GetButton(button)!.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        var host = (FlyoutHost)ItemsControl.ItemsControlFromItemContainer(flyout)!;
        Assert.True(FlyoutWindow.WaitUntil(() => host.Items.Count < count), "The dialog did not close.");
    }

    private static void CloseWithTheCloseButton(Flyout flyout)
    {
        var host = (FlyoutHost)ItemsControl.ItemsControlFromItemContainer(flyout)!;
        var count = host.Items.Count;
        var close = Assert.IsType<Button>(flyout.Template.FindName("PART_CloseButton", flyout));
        close.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        Assert.True(FlyoutWindow.WaitUntil(() => host.Items.Count < count), "The dialog did not close.");
    }

    private static T Result<T>(Task<T> task)
    {
        Assert.True(FlyoutWindow.WaitUntil(() => task.IsCompleted), "The dialog task did not complete.");
        return task.Result;
    }

    private static IEnumerable<string> TextsIn(DependencyObject root)
        => Descendants(root).OfType<TextBlock>().Select(t => t.Text);

    private static IEnumerable<DependencyObject> Descendants(
        DependencyObject root)
    {
        yield return root;
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
        {
            foreach (var descendant in Descendants(VisualTreeHelper.GetChild(root, i)))
            {
                yield return descendant;
            }
        }
    }

    private sealed class RecordingTraceListener : TraceListener
    {
        public List<string> Messages { get; } = [];

        public override void Write(string? message)
            => Messages.Add(message ?? string.Empty);

        public override void WriteLine(string? message)
            => Messages.Add(message ?? string.Empty);
    }
}