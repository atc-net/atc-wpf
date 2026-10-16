namespace Atc.Wpf.Forms.Tests.Controls;

public sealed class LabelToggleSwitchTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void IsOn_TurnedOn_RaisesIsOnChangedWithOldFalseAndNewTrue()
    {
        var sut = new LabelToggleSwitch { LabelText = "Power" };
        ValueChangedEventArgs<bool>? raised = null;
        sut.IsOnChanged += (_, e) => raised = e;

        sut.IsOn = true;

        Assert.NotNull(raised);
        Assert.Equal(sut.Identifier, raised.Identifier);
        Assert.False(raised.OldValue);
        Assert.True(raised.NewValue);
    }

    [StaFact]
    public void IsOn_TurnedOff_RaisesIsOnChangedWithOldTrueAndNewFalse()
    {
        var sut = new LabelToggleSwitch { IsOn = true };
        ValueChangedEventArgs<bool>? raised = null;
        sut.IsOnChanged += (_, e) => raised = e;

        sut.IsOn = false;

        Assert.NotNull(raised);
        Assert.True(raised.OldValue);
        Assert.False(raised.NewValue);
    }
}