namespace Atc.Wpf.Forms.Tests.Controls;

public sealed class LabelPasswordBoxTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    // A password box in an input form must reach the form's values like a text box does.
    [StaFact]
    public void Form_GetKeyValues_includes_the_password()
    {
        var sut = new LabelPasswordBox
        {
            LabelText = "Password",
            Text = "s3cret",
        };
        var form = new LabelControlsForm();
        form.AddColumn(new List<ILabelControlBase> { sut });

        var keyValues = form.GetKeyValues();

        var entry = Assert.Single(keyValues);
        Assert.Equal("Password", entry.Key);
        Assert.Equal("s3cret", entry.Value);
    }
}