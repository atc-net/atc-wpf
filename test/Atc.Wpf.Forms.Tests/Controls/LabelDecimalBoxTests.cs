namespace Atc.Wpf.Forms.Tests.Controls;

public sealed class LabelDecimalBoxTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void LabelDecimalBox_IsValid_WithAValidValue_IsValid()
    {
        var sut = new LabelDecimalBox { Value = 5.5m };

        Assert.True(sut.IsValid());
        Assert.Equal(string.Empty, sut.ValidationText);
    }

    [StaFact]
    public void LabelDecimalBox_IsValid_WithAStaleMessage_ClearsTheMessage()
    {
        var sut = new LabelDecimalBox { Value = 5.5m, ValidationText = "stale" };

        Assert.True(sut.IsValid());
        Assert.Equal(string.Empty, sut.ValidationText);
    }

    [StaFact]
    public void LabelCurrencyBox_IsValid_WithAValidValue_IsValid()
    {
        var sut = new LabelCurrencyBox { Value = 9.95m };

        Assert.True(sut.IsValid());
        Assert.Equal(string.Empty, sut.ValidationText);
    }

    [StaFact]
    public void LabelCurrencyBox_IsValid_WithAStaleMessage_ClearsTheMessage()
    {
        var sut = new LabelCurrencyBox { Value = 9.95m, ValidationText = "stale" };

        Assert.True(sut.IsValid());
        Assert.Equal(string.Empty, sut.ValidationText);
    }
}