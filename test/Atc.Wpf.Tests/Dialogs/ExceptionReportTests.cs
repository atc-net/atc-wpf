namespace Atc.Wpf.Tests.Dialogs;

/// <summary>
/// The crash report: the full exception with its inner exceptions and stack traces, saved to a file and shown in the
/// native message box, without throwing.
/// </summary>
public sealed class ExceptionReportTests
{
    private const string HarmlessBindingError =
        "Cannot find source for binding. BindingExpression:Path=HorizontalContentAlignment; DataItem=null; target element is 'ComboBoxItem' (Name='')";

    [Fact]
    public void CreateText_HasTitleVersionsInnerExceptionsAndStackTrace()
    {
        var exception = CreateThrownException();

        var text = Atc.Wpf.Dialogs.ExceptionReport.CreateText(exception, "Dispatcher Unhandled Exception");

        Assert.StartsWith("Dispatcher Unhandled Exception", text, StringComparison.Ordinal);
        Assert.Contains("OS: ", text, StringComparison.Ordinal);
        Assert.Contains(".NET ", text, StringComparison.Ordinal);
        Assert.Contains("The camera wall could not start.", text, StringComparison.Ordinal);
        Assert.Contains("The stream did not answer.", text, StringComparison.Ordinal);
        Assert.Contains(nameof(CreateThrownException), text, StringComparison.Ordinal);
    }

    [Fact]
    public void TryWriteToFile_WritesTheReportToANewFile()
    {
        var folder = Path.Combine(Path.GetTempPath(), "atc-wpf-tests", Guid.NewGuid().ToString("N"));
        try
        {
            var file = Atc.Wpf.Dialogs.ExceptionReport.TryWriteToFile("report text", folder);

            Assert.NotNull(file);
            Assert.StartsWith(folder, file, StringComparison.Ordinal);
            Assert.Equal("report text", File.ReadAllText(file));
        }
        finally
        {
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, recursive: true);
            }
        }
    }

    [Fact]
    public void TryWriteToFile_AnInvalidFolder_ReturnsNullInsteadOfThrowing()
        => Assert.Null(Atc.Wpf.Dialogs.ExceptionReport.TryWriteToFile("report text", "Z:\\<invalid>|folder"));

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void CreateMessageBoxText_HasTheMessagesAndWhereTheReportIs(
        bool copied,
        bool saved)
    {
        var text = Atc.Wpf.Dialogs.ExceptionReport.CreateMessageBoxText(CreateThrownException(), saved ? "C:\\crash.txt" : null, copied);

        Assert.Contains("The camera wall could not start.", text, StringComparison.Ordinal);
        Assert.Contains("The stream did not answer.", text, StringComparison.Ordinal);
        Assert.Equal(copied, text.Contains("copied to the clipboard", StringComparison.Ordinal));
        Assert.Equal(!copied, text.Contains("Ctrl+C", StringComparison.Ordinal));
        Assert.Equal(saved, text.Contains("C:\\crash.txt", StringComparison.Ordinal));
    }

    [Fact]
    public void IsKnownHarmlessWpfBindingError_FindsTheComboBoxItemBindingError_AlsoAsInnerException()
    {
        Assert.True(Atc.Wpf.Dialogs.ExceptionReport.IsKnownHarmlessWpfBindingError(new InvalidOperationException(HarmlessBindingError)));
        Assert.True(Atc.Wpf.Dialogs.ExceptionReport.IsKnownHarmlessWpfBindingError(new InvalidOperationException("Outer", new InvalidOperationException(HarmlessBindingError))));
    }

    [Fact]
    public void IsKnownHarmlessWpfBindingError_OtherErrorsAndNull_AreFalse()
    {
        Assert.False(Atc.Wpf.Dialogs.ExceptionReport.IsKnownHarmlessWpfBindingError(new InvalidOperationException("Object reference not set to an instance of an object.")));
        Assert.False(Atc.Wpf.Dialogs.ExceptionReport.IsKnownHarmlessWpfBindingError(null));
    }

    private static InvalidOperationException CreateThrownException()
    {
        try
        {
            try
            {
                throw new TimeoutException("The stream did not answer.");
            }
            catch (TimeoutException inner)
            {
                throw new InvalidOperationException("The camera wall could not start.", inner);
            }
        }
        catch (InvalidOperationException ex)
        {
            return ex;
        }
    }
}