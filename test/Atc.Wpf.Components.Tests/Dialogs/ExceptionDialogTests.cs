namespace Atc.Wpf.Components.Tests.Dialogs;

/// <summary>
/// <see cref="ExceptionDialog"/> falls back to the native message box when there is no application to show the themed
/// dialog in, and lists every exception message once.
/// </summary>
[Collection(nameof(ExceptionDialogTestGroup))]
public sealed class ExceptionDialogTests
{
    [StaFact]
    public void Show_WithoutAnApplication_UsesTheFallback()
    {
        Assert.Null(Application.Current);
        var original = ExceptionDialog.Fallback;
        Exception? shownException = null;
        string? shownTitle = null;
        ExceptionDialog.Fallback = (exception, title) =>
        {
            shownException = exception;
            shownTitle = title;
        };

        try
        {
            var exception = new InvalidOperationException("boom");

            ExceptionDialog.Show(exception, "Dispatcher Unhandled Exception");

            Assert.Same(exception, shownException);
            Assert.Equal("Dispatcher Unhandled Exception", shownTitle);
        }
        finally
        {
            ExceptionDialog.Fallback = original;
        }
    }

    [StaFact]
    public void CreateMessage_ListsEachMessageOnce()
    {
        var exception = new InvalidOperationException(
            "The camera wall could not start.",
            new TimeoutException("The stream did not answer.", new TimeoutException("The stream did not answer.")));

        var message = ExceptionDialog.CreateMessage(exception);

        Assert.Equal("The camera wall could not start." + Environment.NewLine + "The stream did not answer.", message);
    }
}