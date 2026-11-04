namespace Atc.Wpf.Forms.Tests.Dialogs;

/// <summary>
/// <see cref="DialogBoxButtonBar"/> shows the buttons the settings ask for, makes the chosen one the default (Enter),
/// maps Escape to Cancel or the negative button, and reports clicks.
/// </summary>
public sealed class DialogBoxButtonBarTests
{
    [StaTheory]
    [InlineData(DialogBoxType.Ok, true, false, false)]
    [InlineData(DialogBoxType.OkCancel, true, true, false)]
    [InlineData(DialogBoxType.YesNo, true, true, false)]
    [InlineData(DialogBoxType.YesNoCancel, true, true, true)]
    public void ShowsTheButtonsOfTheDialogType(
        DialogBoxType type,
        bool affirmative,
        bool negative,
        bool cancel)
    {
        var sut = new DialogBoxButtonBar { Settings = new DialogBoxSettings(type) };

        Assert.Equal(affirmative, sut.GetButton(DialogBoxButton.Affirmative) is not null);
        Assert.Equal(negative, sut.GetButton(DialogBoxButton.Negative) is not null);
        Assert.Equal(cancel, sut.GetButton(DialogBoxButton.Cancel) is not null);
    }

    [StaFact]
    public void YesNoCancel_EscapeClicksCancel_AndEnterClicksYes()
    {
        var sut = new DialogBoxButtonBar { Settings = new DialogBoxSettings(DialogBoxType.YesNoCancel) };

        Assert.True(sut.GetButton(DialogBoxButton.Affirmative)!.IsDefault);
        Assert.True(sut.GetButton(DialogBoxButton.Cancel)!.IsCancel);
        Assert.False(sut.GetButton(DialogBoxButton.Negative)!.IsCancel);
    }

    [StaFact]
    public void YesNo_EscapeClicksNo()
    {
        var sut = new DialogBoxButtonBar { Settings = new DialogBoxSettings(DialogBoxType.YesNo) };

        Assert.True(sut.GetButton(DialogBoxButton.Negative)!.IsCancel);
    }

    [StaFact]
    public void DefaultButtonNegative_MakesNoTheDefault()
    {
        var settings = new DialogBoxSettings(DialogBoxType.YesNo) { DefaultButton = DialogBoxButton.Negative };

        var sut = new DialogBoxButtonBar { Settings = settings };

        Assert.True(sut.GetButton(DialogBoxButton.Negative)!.IsDefault);
        Assert.False(sut.GetButton(DialogBoxButton.Affirmative)!.IsDefault);
    }

    [StaFact]
    public void ADefaultButtonThatIsNotShown_FallsBackToTheAffirmativeButton()
    {
        var settings = new DialogBoxSettings(DialogBoxType.Ok) { DefaultButton = DialogBoxButton.Cancel };

        var sut = new DialogBoxButtonBar { Settings = settings };

        Assert.True(sut.GetButton(DialogBoxButton.Affirmative)!.IsDefault);
    }

    [StaFact]
    public void CustomTexts_AreShown()
    {
        var settings = new DialogBoxSettings(DialogBoxType.YesNoCancel)
        {
            AffirmativeButtonText = "Save",
            NegativeButtonText = "Don't save",
            CancelButtonText = "Keep editing",
        };

        var sut = new DialogBoxButtonBar { Settings = settings };

        Assert.Equal("Save", sut.GetButton(DialogBoxButton.Affirmative)!.Content);
        Assert.Equal("Don't save", sut.GetButton(DialogBoxButton.Negative)!.Content);
        Assert.Equal("Keep editing", sut.GetButton(DialogBoxButton.Cancel)!.Content);
    }

    [StaFact]
    public void ClickingAButton_RaisesButtonClickedWithIt()
    {
        var sut = new DialogBoxButtonBar { Settings = new DialogBoxSettings(DialogBoxType.YesNoCancel) };
        DialogBoxButton? clicked = null;
        sut.ButtonClicked += (_, e) => clicked = e.Button;

        sut.GetButton(DialogBoxButton.Cancel)!.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));

        Assert.Equal(DialogBoxButton.Cancel, clicked);
    }
}