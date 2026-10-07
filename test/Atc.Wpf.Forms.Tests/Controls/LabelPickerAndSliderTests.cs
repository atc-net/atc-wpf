namespace Atc.Wpf.Forms.Tests.Controls;

public sealed class LabelPickerAndSliderTests
{
    [StaFact]
    public void LabelSlider_IsValid_WithAValidValue_IsValid()
    {
        var sut = new LabelSlider { Value = 5 };

        Assert.True(sut.IsValid());
        Assert.Equal(string.Empty, sut.ValidationText);
    }
}