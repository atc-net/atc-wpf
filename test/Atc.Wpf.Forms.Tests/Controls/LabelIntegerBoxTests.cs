namespace Atc.Wpf.Forms.Tests.Controls;

public sealed class LabelIntegerBoxTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Value_ChangedWithoutFocus_DoesNotRaiseValueLostFocus()
    {
        var sut = new LabelIntegerBox { Value = 3 };
        var raised = false;
        sut.ValueLostFocus += (_, _) => raised = true;

        sut.Value = 7;

        Assert.False(raised);
    }

    [StaFact]
    public void Value_EditedThenFocusLost_RaisesValueLostFocusOnceWithOldAndNewValue()
    {
        var sut = new LabelIntegerBox
        {
            LabelText = "Quantity",
            Value = 3,
        };
        var raised = new List<ValueChangedEventArgs<int?>>();
        sut.ValueLostFocus += (_, e) => raised.Add(e);
        FocusEvents.GotFocus(sut);
        sut.Value = 5;
        sut.Value = 7;

        FocusEvents.LostFocus(sut);

        var e = Assert.Single(raised);
        Assert.Equal(sut.Identifier, e.Identifier);
        Assert.Equal(3, e.OldValue);
        Assert.Equal(7, e.NewValue);
    }

    [StaFact]
    public void IsValid_WithoutValidationMessage_IsValid()
    {
        var sut = new LabelIntegerBox { Value = 42 };

        Assert.True(sut.IsValid());
    }
}