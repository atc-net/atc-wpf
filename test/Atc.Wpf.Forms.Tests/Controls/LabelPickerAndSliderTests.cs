namespace Atc.Wpf.Forms.Tests.Controls;

public sealed class LabelPickerAndSliderTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void LabelSlider_IsValid_WithAValidValue_IsValid()
    {
        var sut = new LabelSlider { Value = 5 };

        Assert.True(sut.IsValid());
        Assert.Equal(string.Empty, sut.ValidationText);
    }

    [StaFact]
    public void LabelDatePicker_Text_Changed_RaisesTextChangedWithOldAndNewValue()
    {
        var sut = new LabelDatePicker { Text = "01-02-2026" };
        var textBefore = sut.Text;
        var raised = new List<RoutedPropertyChangedEventArgs<string>>();
        sut.AddHandler(
            LabelDatePicker.TextChangedEvent,
            new RoutedPropertyChangedEventHandler<string>((_, e) => raised.Add(e)));

        sut.Text = "03-04-2026";

        // The picker may re-format the parsed date afterwards, which is a further (legitimate) text change.
        Assert.NotEmpty(raised);
        Assert.Equal(textBefore, raised[0].OldValue);
        Assert.Equal("03-04-2026", raised[0].NewValue);
    }

    [StaFact]
    public void LabelTimePicker_Text_Changed_RaisesTextChangedWithOldAndNewValue()
    {
        // en-US re-formats "09:30" to "9:30 AM", so the run doesn't depend on the machine's culture.
        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
        var sut = new LabelTimePicker { Text = "08:00" };
        var textBefore = sut.Text;
        var raised = new List<RoutedPropertyChangedEventArgs<string>>();
        sut.AddHandler(
            LabelTimePicker.TextChangedEvent,
            new RoutedPropertyChangedEventHandler<string>((_, e) => raised.Add(e)));

        sut.Text = "09:30";

        // The picker re-formats the parsed time in the UI culture afterwards, which is a further (legitimate) text change.
        Assert.NotEmpty(raised);
        Assert.Equal(textBefore, raised[0].OldValue);
        Assert.Equal("09:30", raised[0].NewValue);
    }
}