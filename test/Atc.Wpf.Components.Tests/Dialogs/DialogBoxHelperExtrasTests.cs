namespace Atc.Wpf.Components.Tests.Dialogs;

/// <summary>
/// The optional details area and "Don't ask again" check box under a dialog message.
/// </summary>
public sealed class DialogBoxHelperExtrasTests
{
    [StaFact]
    public void WithoutExtras_TheTextIsTheOnlyContentNextToTheIcon()
    {
        var settings = new Atc.Wpf.Forms.Dialogs.DialogBoxSettings(Atc.Wpf.Forms.Dialogs.DialogBoxType.Ok);

        var content = DialogBoxHelper.CreateContentControl("Saved.", contentImage: null, settings);

        var grid = (Grid)content.Content;
        Assert.Single(grid.Children.OfType<TextBlock>());
        Assert.Empty(grid.Children.OfType<StackPanel>());
        Assert.Null(DialogBoxHelper.FindDontAskAgainCheckBox(content));
    }

    [StaFact]
    public void DetailsText_AddsACollapsedReadOnlyDetailsArea()
    {
        var settings = new Atc.Wpf.Forms.Dialogs.DialogBoxSettings(Atc.Wpf.Forms.Dialogs.DialogBoxType.Ok)
        {
            DetailsText = "System.InvalidOperationException: boom\n   at Foo.Bar()",
        };

        var content = DialogBoxHelper.CreateContentControl("It failed.", contentImage: null, settings);

        var expander = ((Grid)content.Content).Children.OfType<StackPanel>().Single().Children.OfType<Expander>().Single();
        Assert.False(expander.IsExpanded);
        var details = Assert.IsType<TextBox>(expander.Content);
        Assert.True(details.IsReadOnly);
        Assert.Equal(settings.DetailsText, details.Text);
    }

    [StaFact]
    public void ShowDontAskAgain_AddsTheCheckBox_WithTheCustomText()
    {
        var settings = new Atc.Wpf.Forms.Dialogs.DialogBoxSettings(Atc.Wpf.Forms.Dialogs.DialogBoxType.YesNo)
        {
            ShowDontAskAgain = true,
            DontAskAgainText = "Always close without asking",
        };

        var content = DialogBoxHelper.CreateContentControl("Close?", contentImage: null, settings);

        var checkBox = DialogBoxHelper.FindDontAskAgainCheckBox(content);
        Assert.NotNull(checkBox);
        Assert.Equal("Always close without asking", checkBox.Content);
        Assert.False(checkBox.IsChecked);
    }

    [StaFact]
    public void ShowDontAskAgain_WithoutText_UsesTheLocalizedText()
    {
        var settings = new Atc.Wpf.Forms.Dialogs.DialogBoxSettings(Atc.Wpf.Forms.Dialogs.DialogBoxType.YesNo)
        {
            ShowDontAskAgain = true,
        };

        var checkBox = DialogBoxHelper.FindDontAskAgainCheckBox(
            DialogBoxHelper.CreateContentControl("Close?", contentImage: null, settings));

        Assert.Equal(Atc.Wpf.Components.Resources.Miscellaneous.DontAskAgain, checkBox!.Content);
    }

    [StaFact]
    public void ExpandedDetails_WithAVeryLongLine_DoNotMakeTheDialogScrollSideways()
    {
        var settings = new Atc.Wpf.Forms.Dialogs.DialogBoxSettings(Atc.Wpf.Forms.Dialogs.DialogBoxType.Ok)
        {
            DetailsText = new string('x', 2000),
            ShowDontAskAgain = true,
        };

        var content = DialogBoxHelper.CreateContentControl("It failed.", new Border { Width = 32, Height = 32 }, settings);
        var expander = ((Grid)content.Content).Children.OfType<StackPanel>().Single().Children.OfType<Expander>().Single();
        expander.IsExpanded = true;

        var viewer = new ScrollViewer
        {
            Padding = new Thickness(30),
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Content = content,
        };

        var window = new Window
        {
            Width = 500,
            Height = 400,
            Left = -10000,
            Top = -10000,
            WindowStyle = WindowStyle.None,
            ShowInTaskbar = false,
            ShowActivated = false,
            Content = viewer,
        };

        window.Show();
        try
        {
            window.UpdateLayout();
            Assert.Equal(0, viewer.ScrollableWidth);
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void CreateClipboardText_IncludesTheDetails()
    {
        var settings = new Atc.Wpf.Forms.Dialogs.DialogBoxSettings(Atc.Wpf.Forms.Dialogs.DialogBoxType.Ok)
        {
            AffirmativeButtonText = "OK",
            DetailsText = "Stack trace",
        };

        var text = DialogBoxHelper.CreateClipboardText("Error", header: null, "It failed.", settings);

        Assert.Contains("It failed." + Environment.NewLine + "---------------------------" + Environment.NewLine + "Stack trace", text, StringComparison.Ordinal);
    }
}