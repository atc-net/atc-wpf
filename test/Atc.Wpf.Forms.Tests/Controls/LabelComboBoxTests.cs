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

    // The valid counterpart of SelectorLostFocusInvalid, like the Valid/Invalid pair of the text fields.
    [StaFact]
    public void SelectedKey_SetToAValidKey_RaisesSelectorLostFocusValid()
    {
        UseEnglishUi();
        var sut = new LabelComboBox
        {
            IsMandatory = true,
            Items = new Dictionary<string, string>(StringComparer.Ordinal) { ["DK"] = "Denmark", ["SE"] = "Sweden" },
            SelectedKey = "DK",
        };
        ValueChangedEventArgs<string?>? valid = null;
        var invalidRaised = false;
        sut.SelectorLostFocusValid += (_, e) => valid = e;
        sut.SelectorLostFocusInvalid += (_, _) => invalidRaised = true;

        sut.SelectedKey = "SE";

        Assert.NotNull(valid);
        Assert.Equal("DK", valid.OldValue);
        Assert.Equal("SE", valid.NewValue);
        Assert.False(invalidRaised);
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

    [StaFact]
    public void SelectedKey_SetThroughTheInterface_RaisesTheInterfaceEvents()
    {
        UseEnglishUi();
        ILabelComboBoxBase sut = new LabelComboBox
        {
            IsMandatory = true,
            Items = new Dictionary<string, string>(StringComparer.Ordinal) { ["DK"] = "Denmark", ["SE"] = "Sweden" },
            SelectedKey = "DK",
        };
        var raised = new List<string>();
        sut.SelectorChanged += (_, e) => raised.Add($"changed:{e.NewValue}");
        sut.SelectorLostFocusInvalid += (_, _) => raised.Add("invalid");

        sut.SelectedKey = "SE";
        sut.SelectedKey = string.Empty;

        Assert.Equal(["changed:SE", "invalid"], raised);
    }

    [StaFact]
    public void EnumPicker_MandatoryWithThePlaceholderSelected_ReportsFieldIsRequired()
    {
        UseEnglishUi();
        var sut = LabelControlFactory.CreateLabelEnumPicker(
            groupIdentifier: "Group",
            labelText: "Day",
            isReadOnly: false,
            isMandatory: true,
            selectedKey: nameof(DayOfWeek.Monday),
            typeof(DayOfWeek));
        var placeholderKey = Atc.Helpers.DropDownFirstItemTypeHelper
            .GetEnumGuid(Atc.DropDownFirstItemType.PleaseSelect)
            .ToString();
        Assert.True(sut.Items.ContainsKey(placeholderKey));
        var invalidRaised = false;
        var changedRaised = false;
        sut.SelectorLostFocusInvalid += (_, _) => invalidRaised = true;
        sut.SelectorChanged += (_, _) => changedRaised = true;

        sut.SelectedKey = placeholderKey;

        Assert.True(invalidRaised);
        Assert.False(changedRaised);
        Assert.False(sut.IsValid());
        Assert.Equal("Field is required", sut.ValidationText);
    }

    private static void UseEnglishUi()
        => Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
}