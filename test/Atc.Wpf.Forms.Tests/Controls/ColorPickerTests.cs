namespace Atc.Wpf.Forms.Tests.Controls;

public sealed class ColorPickerTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void PickedColor_RaisesColorChangedWithThePreviousColorAsOldValue()
    {
        var sut = new ColorPicker { ColorValue = Colors.Red };
        ValueChangedEventArgs<Color>? raised = null;
        sut.ColorChanged += (_, e) => raised = e;

        sut.ApplyPickedColor(Colors.Blue);

        Assert.NotNull(raised);
        Assert.Equal(Colors.Red, raised.OldValue);
        Assert.Equal(Colors.Blue, raised.NewValue);
    }

    [StaFact]
    public void PickedColor_UpdatesColorAndBrush()
    {
        var sut = new ColorPicker { ColorValue = Colors.Red };

        sut.ApplyPickedColor(Colors.Blue);

        Assert.Equal(Colors.Blue, sut.ColorValue);
        Assert.Equal(Colors.Blue, sut.BrushValue?.Color);
    }

    [StaFact]
    public void PickedColor_KeepsAOneWayBindingSoLaterSourceChangesStillArrive()
    {
        var source = new ColorPicker { ColorValue = Colors.Green };
        var sut = new ColorPicker();
        BindingOperations.SetBinding(
            sut,
            ColorPicker.ColorValueProperty,
            new Binding(nameof(ColorPicker.ColorValue)) { Source = source });

        sut.ApplyPickedColor(Colors.Blue);
        source.ColorValue = Colors.Yellow;

        Assert.Equal(Colors.Yellow, sut.ColorValue);
    }

    [StaFact]
    public void LabelColorPicker_PickedColor_RaisesColorChangedWithItsPreviousColorAsOldValue()
    {
        var sut = new LabelColorPicker { LabelText = "Accent", ColorValue = Colors.Red };
        ValueChangedEventArgs<Color>? raised = null;
        sut.ColorChanged += (_, e) => raised = e;

        sut.InnerColorPicker.ApplyPickedColor(Colors.Blue);

        Assert.NotNull(raised);
        Assert.Equal(sut.Identifier, raised.Identifier);
        Assert.Equal(Colors.Red, raised.OldValue);
        Assert.Equal(Colors.Blue, raised.NewValue);
        Assert.Equal(Colors.Blue, sut.ColorValue);
    }
}