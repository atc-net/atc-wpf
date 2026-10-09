namespace Atc.Wpf.Forms.Tests.Controls;

public sealed class LabelXyBoxLostFocusTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void LabelDecimalXyBox_ValueChangedWithoutFocus_DoesNotRaiseLostFocusEvents()
    {
        var sut = new LabelDecimalXyBox { ValueX = 1m, ValueY = 2m };
        var raised = false;
        sut.ValueXLostFocus += (_, _) => raised = true;
        sut.ValueYLostFocus += (_, _) => raised = true;

        sut.ValueX = 1.5m;
        sut.ValueY = 2.5m;

        Assert.False(raised);
    }

    [StaFact]
    public void LabelDecimalXyBox_EditedThenFocusLost_RaisesValueXLostFocusOnce()
    {
        var sut = new LabelDecimalXyBox { ValueX = 1m, ValueY = 2m };
        var raisedX = new List<ValueChangedEventArgs<decimal?>>();
        var raisedY = false;
        sut.ValueXLostFocus += (_, e) => raisedX.Add(e);
        sut.ValueYLostFocus += (_, _) => raisedY = true;
        FocusEvents.GotFocus(sut);
        sut.ValueX = 1.25m;
        sut.ValueX = 1.5m;

        FocusEvents.LostFocus(sut);

        var e = Assert.Single(raisedX);
        Assert.Equal(1m, e.OldValue);
        Assert.Equal(1.5m, e.NewValue);
        Assert.False(raisedY);
    }

    [StaFact]
    public void LabelIntegerXyBox_ValueChangedWithoutFocus_DoesNotRaiseLostFocusEvents()
    {
        var sut = new LabelIntegerXyBox { ValueX = 1, ValueY = 2 };
        var raised = false;
        sut.ValueXLostFocus += (_, _) => raised = true;
        sut.ValueYLostFocus += (_, _) => raised = true;

        sut.ValueX = 10;
        sut.ValueY = 20;

        Assert.False(raised);
    }

    [StaFact]
    public void LabelIntegerXyBox_EditedThenFocusLost_RaisesValueYLostFocusOnce()
    {
        var sut = new LabelIntegerXyBox { ValueX = 1, ValueY = 2 };
        var raised = new List<ValueChangedEventArgs<int?>>();
        sut.ValueYLostFocus += (_, e) => raised.Add(e);
        FocusEvents.GotFocus(sut);
        sut.ValueY = 5;
        sut.ValueY = 9;

        FocusEvents.LostFocus(sut);

        var e = Assert.Single(raised);
        Assert.Equal(2, e.OldValue);
        Assert.Equal(9, e.NewValue);
    }

    [StaFact]
    public void LabelPixelSizeBox_ValueChangedWithoutFocus_DoesNotRaiseLostFocusEvents()
    {
        var sut = new LabelPixelSizeBox { ValueWidth = 100, ValueHeight = 50 };
        var raised = false;
        sut.ValueWidthLostFocus += (_, _) => raised = true;
        sut.ValueHeightLostFocus += (_, _) => raised = true;

        sut.ValueWidth = 200;
        sut.ValueHeight = 80;

        Assert.False(raised);
    }

    [StaFact]
    public void LabelPixelSizeBox_EditedThenFocusLost_RaisesValueHeightLostFocusOnce()
    {
        var sut = new LabelPixelSizeBox { ValueWidth = 100, ValueHeight = 50 };
        var raised = new List<ValueChangedEventArgs<int?>>();
        sut.ValueHeightLostFocus += (_, e) => raised.Add(e);
        FocusEvents.GotFocus(sut);
        sut.ValueHeight = 60;
        sut.ValueHeight = 75;

        FocusEvents.LostFocus(sut);

        var e = Assert.Single(raised);
        Assert.Equal(50, e.OldValue);
        Assert.Equal(75, e.NewValue);
    }
}