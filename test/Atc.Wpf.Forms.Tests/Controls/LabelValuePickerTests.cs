namespace Atc.Wpf.Forms.Tests.Controls;

public sealed class LabelValuePickerTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void LabelFilePicker_IsValid_DoesNotRaiseLostFocusValid()
    {
        var sut = new LabelFilePicker();
        var raised = false;
        sut.LostFocusValid += (_, _) => raised = true;

        var isValid = sut.IsValid();

        Assert.True(isValid);
        Assert.False(raised);
    }

    [StaFact]
    public void LabelDirectoryPicker_IsValid_DoesNotRaiseLostFocusValid()
    {
        var sut = new LabelDirectoryPicker();
        var raised = false;
        sut.LostFocusValid += (_, _) => raised = true;

        var isValid = sut.IsValid();

        Assert.True(isValid);
        Assert.False(raised);
    }

    [StaFact]
    public void LabelTimeZonePicker_IsValid_DoesNotRaiseLostFocusValid()
    {
        var sut = new LabelTimeZonePicker();
        var raised = false;
        sut.LostFocusValid += (_, _) => raised = true;

        var isValid = sut.IsValid();

        Assert.True(isValid);
        Assert.False(raised);
    }
}