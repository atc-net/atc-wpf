namespace Atc.Wpf.Forms.Tests.Controls;

// The colour picker can be empty, so IsMandatory must be enforced like on the text fields. (LabelFontPicker has the
// same check but cannot be created without the application theme: its template uses theme brushes in a converter parameter.)
public sealed class LabelPickerMandatoryTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void LabelColorPicker_MandatoryWithoutAColor_ReportsFieldIsRequired()
    {
        UseEnglishUi();
        var sut = new LabelColorPicker { IsMandatory = true, ColorValue = null };

        Assert.False(sut.IsValid());
        Assert.Equal("Field is required", sut.ValidationText);
    }

    [StaFact]
    public void LabelColorPicker_MandatoryWithAColor_IsValid()
    {
        UseEnglishUi();
        var sut = new LabelColorPicker { IsMandatory = true, ColorValue = Colors.Teal };

        Assert.True(sut.IsValid());
        Assert.Equal(string.Empty, sut.ValidationText);
    }

    [StaFact]
    public void LabelColorPicker_NotMandatoryWithoutAColor_IsValid()
    {
        var sut = new LabelColorPicker { ColorValue = null };

        Assert.True(sut.IsValid());
    }

    private static void UseEnglishUi()
        => Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
}