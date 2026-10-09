namespace Atc.Wpf.Controls.Tests.Inputs;

public sealed class XyBoxLostFocusTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void DecimalXyBox_InnerBoxChangedWhileEditing_DoesNotRaiseValueXLostFocus()
    {
        var sut = CreateMeasured(new DecimalXyBox { DecimalPlaces = 2, ValueX = 1m });
        var raised = false;
        sut.ValueXLostFocus += (_, _) => raised = true;
        var boxX = InnerBoxes(sut)[0];
        GotFocus(boxX);

        boxX.Value = 3.25;
        BindingOperations.GetBindingExpression(boxX, NumericBox.ValueProperty)?.UpdateSource();

        Assert.Equal(3.25m, sut.ValueX);
        Assert.False(raised);
    }

    [StaFact]
    public void DecimalXyBox_InnerBoxLosesFocusAfterAChange_RaisesValueXLostFocusOnceWithOldAndNewValue()
    {
        var sut = CreateMeasured(new DecimalXyBox { DecimalPlaces = 2, ValueX = 1m, ValueY = 5m });
        var raisedX = new List<ValueChangedEventArgs<decimal?>>();
        var raisedY = false;
        sut.ValueXLostFocus += (_, e) => raisedX.Add(e);
        sut.ValueYLostFocus += (_, _) => raisedY = true;
        var boxX = InnerBoxes(sut)[0];
        GotFocus(boxX);
        sut.ValueX = 2m;
        sut.ValueX = 3m;

        LostFocus(boxX);

        var e = Assert.Single(raisedX);
        Assert.Equal(1m, e.OldValue);
        Assert.Equal(3m, e.NewValue);
        Assert.False(raisedY);
    }

    [StaFact]
    public void IntegerXyBox_ValueChangedWithoutFocus_DoesNotRaiseLostFocusEvents()
    {
        var sut = new IntegerXyBox { ValueX = 1, ValueY = 2 };
        var raised = false;
        sut.ValueXLostFocus += (_, _) => raised = true;
        sut.ValueYLostFocus += (_, _) => raised = true;

        sut.ValueX = 10;
        sut.ValueY = 20;

        Assert.False(raised);
    }

    [StaFact]
    public void IntegerXyBox_InnerBoxLosesFocusAfterAChange_RaisesValueYLostFocus()
    {
        var sut = CreateMeasured(new IntegerXyBox { ValueX = 1, ValueY = 2 });
        ValueChangedEventArgs<int?>? raised = null;
        sut.ValueYLostFocus += (_, e) => raised = e;
        var boxY = InnerBoxes(sut)[1];
        GotFocus(boxY);
        sut.ValueY = 7;

        LostFocus(boxY);

        Assert.NotNull(raised);
        Assert.Equal(2, raised.OldValue);
        Assert.Equal(7, raised.NewValue);
    }

    [StaFact]
    public void PixelSizeBox_ValueChangedWithoutFocus_DoesNotRaiseLostFocusEvents()
    {
        var sut = new PixelSizeBox { ValueWidth = 100, ValueHeight = 50 };
        var raised = false;
        sut.ValueWidthLostFocus += (_, _) => raised = true;
        sut.ValueHeightLostFocus += (_, _) => raised = true;

        sut.ValueWidth = 200;
        sut.ValueHeight = 80;

        Assert.False(raised);
    }

    [StaFact]
    public void PixelSizeBox_InnerBoxLosesFocusAfterAChange_RaisesValueWidthLostFocus()
    {
        var sut = CreateMeasured(new PixelSizeBox { ValueWidth = 100, ValueHeight = 50 });
        ValueChangedEventArgs<int?>? raised = null;
        sut.ValueWidthLostFocus += (_, e) => raised = e;
        var boxWidth = InnerBoxes(sut)[0];
        GotFocus(boxWidth);
        sut.ValueWidth = 640;

        LostFocus(boxWidth);

        Assert.NotNull(raised);
        Assert.Equal(100, raised.OldValue);
        Assert.Equal(640, raised.NewValue);
    }

    [StaFact]
    public void PixelSizeBox_InnerBoxLosesFocusWithoutAChange_RaisesNothing()
    {
        var sut = CreateMeasured(new PixelSizeBox { ValueWidth = 100, ValueHeight = 50 });
        var raised = false;
        sut.ValueWidthLostFocus += (_, _) => raised = true;
        sut.ValueHeightLostFocus += (_, _) => raised = true;
        var boxWidth = InnerBoxes(sut)[0];
        GotFocus(boxWidth);

        LostFocus(boxWidth);

        Assert.False(raised);
    }

    // The Label* XY wrappers get their value through a binding that the inner box updates on lost focus. The
    // wrapper's tracker sees the same LostFocus event later in its route, so it must already see the new value.
    [StaFact]
    public void WrapperBoundToAnXyBox_InnerBoxEditedThenFocusLost_RaisesWithTheEditedValue()
    {
        var sut = CreateMeasured(new XyBoxHost { ValueX = 1m });
        ValueChangedEventArgs<decimal?>? raised = null;
        sut.ValueXLostFocus += (_, e) => raised = e;
        var boxX = InnerBoxes(sut.Inner)[0];
        GotFocus(boxX);
        boxX.Value = 3.25;
        BindingOperations.GetBindingExpression(boxX, NumericBox.ValueProperty)?.UpdateSource();
        Assert.Equal(1m, sut.ValueX);

        LostFocus(boxX);

        Assert.Equal(3.25m, sut.ValueX);
        Assert.NotNull(raised);
        Assert.Equal(1m, raised.OldValue);
        Assert.Equal(3.25m, raised.NewValue);
    }

    private static T CreateMeasured<T>(T control)
        where T : UserControl
    {
        control.Measure(new Size(400, 100));
        control.Arrange(new Rect(0, 0, 400, 100));
        Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Action(() => { }));
        return control;
    }

    private static NumericBox[] InnerBoxes(UserControl control)
        => LogicalTreeHelper
            .GetChildren((DependencyObject)control.Content)
            .OfType<NumericBox>()
            .ToArray();

    private static void GotFocus(UIElement editor)
        => editor.RaiseEvent(new RoutedEventArgs(UIElement.GotFocusEvent, editor));

    private static void LostFocus(UIElement editor)
        => editor.RaiseEvent(new RoutedEventArgs(UIElement.LostFocusEvent, editor));
}