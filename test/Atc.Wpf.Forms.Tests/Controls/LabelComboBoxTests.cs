namespace Atc.Wpf.Forms.Tests.Controls;

public sealed class LabelComboBoxTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void IsValid_MandatoryWithoutSelection_ReportsFieldIsRequired()
    {
        UseEnglishUi();
        var sut = new LabelComboBox { IsMandatory = true };

        Assert.False(sut.IsValid());
        Assert.Equal("Field is required", sut.ValidationText);
    }

    [StaFact]
    public void SelectedKey_Set_RaisesSelectorChangedWithOldAndNewKey()
    {
        UseEnglishUi();
        var sut = new LabelComboBox
        {
            LabelText = "Country",
            Items = new Dictionary<string, string>(StringComparer.Ordinal) { ["DK"] = "Denmark", ["SE"] = "Sweden" },
            SelectedKey = "DK",
        };
        ValueChangedEventArgs<string?>? raised = null;
        sut.SelectorChanged += (_, e) => raised = e;

        sut.SelectedKey = "SE";

        Assert.NotNull(raised);
        Assert.Equal(sut.Identifier, raised.Identifier);
        Assert.Equal("DK", raised.OldValue);
        Assert.Equal("SE", raised.NewValue);
        Assert.True(sut.IsValid());
    }

    [StaFact]
    public void SelectedKey_ClearedWhileMandatory_RaisesSelectorLostFocusInvalid()
    {
        UseEnglishUi();
        var sut = new LabelComboBox
        {
            IsMandatory = true,
            Items = new Dictionary<string, string>(StringComparer.Ordinal) { ["DK"] = "Denmark" },
            SelectedKey = "DK",
        };
        var invalidRaised = false;
        var changedRaised = false;
        sut.SelectorLostFocusInvalid += (_, _) => invalidRaised = true;
        sut.SelectorChanged += (_, _) => changedRaised = true;

        sut.SelectedKey = string.Empty;

        Assert.True(invalidRaised);
        Assert.False(changedRaised);
        Assert.Equal("Field is required", sut.ValidationText);
    }

    private static void UseEnglishUi()
        => Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
}