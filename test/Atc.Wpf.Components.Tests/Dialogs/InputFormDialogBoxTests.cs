namespace Atc.Wpf.Components.Tests.Dialogs;

public sealed class InputFormDialogBoxTests
{
    [StaFact]
    public void RendersTheGivenForm()
    {
        var form = Form(out var name);

        var sut = new InputFormDialogBox(owningWindow: null!, form);

        Assert.Same(form, sut.Data);
        Assert.True(Contains(sut.LabelInputFormPanel.ContentControl, name));
    }

    [StaFact]
    public void TitleAndHeader_AreShown()
    {
        var sut = new InputFormDialogBox(owningWindow: null!, "Edit person", "Fill in the fields", Form(out _));

        Assert.Equal("Edit person", sut.Settings.TitleBarText);
        Assert.NotNull(sut.HeaderControl);
    }

    [StaFact]
    public void OkCancel_IsTheDefault()
    {
        var sut = new InputFormDialogBox(owningWindow: null!, Form(out _));

        Assert.True(sut.Settings.ShowNegativeButton);
        Assert.False(sut.Settings.ShowCancelButton);
    }

    [StaTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void GroupBoxes_AreUsedOnlyForAFormWithSeveralGroups(
        bool severalGroups)
    {
        var form = new LabelControlsForm();
        form.AddColumn(new List<ILabelControlBase>
        {
            new LabelTextBox { LabelText = "A", GroupIdentifier = "First" },
            new LabelTextBox { LabelText = "B", GroupIdentifier = severalGroups ? "Second" : "First" },
        });

        var sut = new InputFormDialogBox(owningWindow: null!, DialogBoxSettings.Create(DialogBoxType.OkCancel), form);

        Assert.Equal(severalGroups, sut.Settings.Form.UseGroupBox);
    }

    [StaFact]
    public void FollowsTheOwnersFlowDirection()
    {
        var owner = new Window { FlowDirection = FlowDirection.RightToLeft };

        var sut = new InputFormDialogBox(owner, Form(out _));

        Assert.Equal(FlowDirection.RightToLeft, sut.FlowDirection);
    }

    [StaFact]
    public void SizesToTheFormsHeight_AndKeepsItsWidthWithinTheFormMaximum()
    {
        var settings = DialogBoxSettings.Create(DialogBoxType.OkCancel);
        settings.Form.MaxSize = new Size(150, 300);

        var sut = new InputFormDialogBox(owningWindow: null!, settings, Form(out _));

        Assert.Equal(SizeToContent.Height, sut.SizeToContent);
        Assert.True(sut.Width <= 150, $"Width {sut.Width} is wider than the form maximum.");
        Assert.True(sut.MaxHeight <= 300, $"MaxHeight {sut.MaxHeight} is taller than the form maximum.");
    }

    // The dialog needs the application's theme resources to be shown, which a unit test does not have, so
    // the buttons are clicked on a dialog that is not shown: setting DialogResult then throws, which shows
    // the dialog tried to close; no exception means it stayed open.
    [StaFact]
    public void Ok_WithAnInvalidForm_KeepsTheDialogOpen()
    {
        var form = Form(out var name);
        name.IsMandatory = true;
        var sut = new InputFormDialogBox(owningWindow: null!, form);

        var exception = Record.Exception(() => Click(sut, DialogBoxButton.Affirmative));

        Assert.Null(exception);
        Assert.False(form.IsValid());
    }

    [StaFact]
    public void Ok_WithAValidForm_ClosesTheDialog()
    {
        var form = Form(out var name);
        name.IsMandatory = true;
        name.Text = "Ada";
        var sut = new InputFormDialogBox(owningWindow: null!, form);

        var exception = Record.Exception(() => Click(sut, DialogBoxButton.Affirmative));

        Assert.IsType<InvalidOperationException>(exception);
    }

    [StaFact]
    public void Cancel_ClosesTheDialog_EvenWhenTheFormIsInvalid()
    {
        var form = Form(out var name);
        name.IsMandatory = true;
        var sut = new InputFormDialogBox(owningWindow: null!, form);

        var exception = Record.Exception(() => Click(sut, DialogBoxButton.Negative));

        Assert.IsType<InvalidOperationException>(exception);
    }

    private static LabelControlsForm Form(out LabelTextBox name)
    {
        name = new LabelTextBox { LabelText = "Name" };
        var form = new LabelControlsForm();
        form.AddColumn(new List<ILabelControlBase> { name });
        return form;
    }

    private static void Click(
        DependencyObject dialog,
        DialogBoxButton button)
    {
        // Lets the button bar's Settings binding deliver its value, as it would before the dialog is shown.
        Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);

        var bar = Descendants(dialog).OfType<DialogBoxButtonBar>().Single();
        Assert.NotNull(bar.Settings);
        bar.GetButton(button)!.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
    }

    private static bool Contains(
        DependencyObject root,
        DependencyObject element)
        => Descendants(root).Contains(element);

    private static IEnumerable<DependencyObject> Descendants(
        DependencyObject root)
    {
        yield return root;
        foreach (var child in LogicalTreeHelper.GetChildren(root).OfType<DependencyObject>())
        {
            foreach (var descendant in Descendants(child))
            {
                yield return descendant;
            }
        }
    }
}