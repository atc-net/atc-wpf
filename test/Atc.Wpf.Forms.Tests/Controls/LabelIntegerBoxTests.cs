namespace Atc.Wpf.Forms.Tests.Controls;

public sealed class LabelIntegerBoxTests
{
    [StaFact]
    public void Value_Changed_RaisesValueLostFocusWithOldAndNewValue()
    {
        var sut = new LabelIntegerBox
        {
            LabelText = "Quantity",
            Value = 3,
        };
        ValueChangedEventArgs<int?>? raised = null;
        sut.ValueLostFocus += (_, e) => raised = e;

        sut.Value = 7;

        Assert.NotNull(raised);
        Assert.Equal(sut.Identifier, raised.Identifier);
        Assert.Equal(3, raised.OldValue);
        Assert.Equal(7, raised.NewValue);
    }

    [StaFact]
    public void IsValid_WithoutValidationMessage_IsValid()
    {
        var sut = new LabelIntegerBox { Value = 42 };

        Assert.True(sut.IsValid());
    }
}