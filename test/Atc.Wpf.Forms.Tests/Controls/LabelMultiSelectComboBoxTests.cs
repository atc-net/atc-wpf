namespace Atc.Wpf.Forms.Tests.Controls;

public sealed class LabelMultiSelectComboBoxTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void IsValid_MandatoryWithNothingTicked_ReportsFieldIsRequired()
    {
        UseEnglishUi();
        var sut = Create();
        sut.IsMandatory = true;

        Assert.False(sut.IsValid());
        Assert.Equal("Field is required", sut.ValidationText);

        sut.SelectedKeys.Add("b");

        Assert.True(sut.IsValid());
        Assert.Equal(string.Empty, sut.ValidationText);
    }

    [StaFact]
    public void IsValid_NotMandatory_IsValidWithNothingTicked()
    {
        var sut = Create();

        Assert.True(sut.IsValid());
    }

    [StaFact]
    public void Items_and_SelectedKeys_reach_the_inner_combo_box()
    {
        var sut = Create();
        sut.SelectedKeys.Add("c");

        Assert.Same(sut.Items, sut.ComboBox.Items);
        Assert.Same(sut.SelectedKeys, sut.ComboBox.SelectedKeys);
        Assert.Equal("Gamma", sut.ComboBox.SummaryText);
    }

    [StaFact]
    public void Options_are_passed_through_to_the_inner_combo_box()
    {
        var sut = Create();
        var template = new DataTemplate();

        sut.SelectNewItems = true;
        sut.ShowSelectAllRow = false;
        sut.ShowClearButton = false;
        sut.IsFilterVisible = true;
        sut.FilterText = "Al";
        sut.ItemGroups = new Dictionary<string, string>(StringComparer.Ordinal) { ["a"] = "G" };
        sut.IsGroupCheckable = false;
        sut.MaxSelectionCount = 2;
        sut.DisabledKeys.Add("a");
        sut.ItemTemplate = template;
        sut.MaxNamesInSummary = 1;
        sut.AllText = "Everything";
        sut.NoneText = "Nothing";
        sut.CountFormat = "{0}/{1}";
        sut.MaxDropDownHeight = 123;
        sut.WatermarkText = "Pick some";

        var inner = sut.ComboBox;
        Assert.True(inner.SelectNewItems);
        Assert.False(inner.ShowSelectAllRow);
        Assert.False(inner.ShowClearButton);
        Assert.True(inner.IsFilterVisible);
        Assert.Equal("Al", inner.FilterText);
        Assert.Same(sut.ItemGroups, inner.ItemGroups);
        Assert.False(inner.IsGroupCheckable);
        Assert.Equal(2, inner.MaxSelectionCount);
        Assert.Same(sut.DisabledKeys, inner.DisabledKeys);
        Assert.Same(template, inner.ItemTemplate);
        Assert.Equal(1, inner.MaxNamesInSummary);
        Assert.Equal("Everything", inner.AllText);
        Assert.Equal("Nothing", inner.NoneText);
        Assert.Equal("{0}/{1}", inner.CountFormat);
        Assert.Equal(123, inner.MaxDropDownHeight);
        Assert.Equal("Pick some", Atc.Wpf.Helpers.TextBoxHelper.GetWatermark(inner));
    }

    [StaFact]
    public void Ticking_updates_SelectedKeys_and_raises_SelectedKeysChanged_with_the_identifier()
    {
        var sut = Create();
        ValueChangedEventArgs<IReadOnlyList<string>>? raised = null;
        sut.SelectedKeysChanged += (_, e) => raised = e;

        sut.ComboBox.GetItem("b")!.IsChecked = true;

        Assert.Equal(["b"], sut.SelectedKeys);
        Assert.NotNull(raised);
        Assert.Equal(sut.Identifier, raised.Identifier);
        Assert.Empty(raised.OldValue);
        Assert.Equal(["b"], raised.NewValue);
    }

    [StaFact]
    public void Items_dropping_a_selected_key_raises_one_event()
    {
        var sut = Create();
        sut.ComboBox.SelectAll();
        var events = new List<ValueChangedEventArgs<IReadOnlyList<string>>>();
        sut.SelectedKeysChanged += (_, e) => events.Add(e);

        sut.Items = new Dictionary<string, string>(StringComparer.Ordinal) { ["a"] = "Alpha" };

        Assert.Equal(["a"], sut.SelectedKeys);
        Assert.Single(events);
    }

    [StaFact]
    public void Unticking_the_last_key_while_mandatory_raises_the_invalid_event()
    {
        UseEnglishUi();
        var sut = Create();
        sut.IsMandatory = true;
        sut.ComboBox.GetItem("a")!.IsChecked = true;
        var changed = 0;
        var invalid = 0;
        sut.SelectedKeysChanged += (_, _) => changed++;
        sut.SelectedKeysChangedInvalid += (_, _) => invalid++;

        sut.ComboBox.GetItem("a")!.IsChecked = false;

        Assert.Equal(0, changed);
        Assert.Equal(1, invalid);
        Assert.Equal("Field is required", sut.ValidationText);
    }

    [StaFact]
    public void A_new_SelectedKeys_instance_is_picked_up()
    {
        var sut = Create();

        sut.SelectedKeys = ["a", "c"];

        Assert.Same(sut.SelectedKeys, sut.ComboBox.SelectedKeys);
        Assert.Equal("Alpha, Gamma", sut.ComboBox.SummaryText);
    }

    [StaFact]
    public void Implements_ILabelMultiSelectComboBox()
    {
        ILabelMultiSelectComboBox sut = Create();
        var raised = 0;
        sut.SelectedKeysChanged += (_, _) => raised++;

        ((LabelMultiSelectComboBox)sut).ComboBox.SelectAll();

        Assert.Equal(["a", "b", "c"], sut.SelectedKeys);
        Assert.Equal(1, raised);
    }

    [StaFact]
    public void Factory_creates_a_labeled_combo_box_with_items_and_ticks()
    {
        var sut = LabelControlFactory.CreateLabelMultiSelectComboBox(
            groupIdentifier: "Group",
            labelText: "SourceSystems",
            isReadOnly: false,
            isMandatory: true,
            items: Items(),
            selectedKeys: ["b"]);

        Assert.Equal("Group", sut.GroupIdentifier);
        Assert.Equal("Source systems", sut.LabelText);
        Assert.True(sut.IsMandatory);
        Assert.Equal(3, sut.Items.Count);
        Assert.Equal(["b"], sut.SelectedKeys);
        Assert.True(sut.IsValid());
    }

    [StaFact]
    public void Form_GetKeyValues_returns_the_ticked_keys()
    {
        var sut = LabelControlFactory.CreateLabelMultiSelectComboBox(
            groupIdentifier: "Export",
            labelText: "Columns",
            isReadOnly: false,
            isMandatory: false,
            items: Items(),
            selectedKeys: ["a", "c"]);
        var form = new LabelControlsForm();
        form.AddColumn(new List<ILabelControlBase> { sut });

        var keyValues = form.GetKeyValues();

        var value = Assert.Single(keyValues).Value;
        Assert.Equal(["a", "c"], Assert.IsAssignableFrom<IEnumerable<string>>(value), StringComparer.Ordinal);
    }

    [StaFact]
    public void Form_values_write_the_ticked_keys_back_to_a_model()
    {
        var sut = LabelControlFactory.CreateLabelMultiSelectComboBox(
            groupIdentifier: "ExportSettings",
            labelText: "Columns",
            isReadOnly: false,
            isMandatory: false,
            items: Items(),
            selectedKeys: ["a", "c"]);
        var form = new LabelControlsForm();
        form.AddColumn(new List<ILabelControlBase> { sut });

        var actual = LabelControlsFormToModelWriter.Update(new ExportSettings(), form);

        Assert.Equal(["a", "c"], actual.Columns, StringComparer.Ordinal);
    }

    private static LabelMultiSelectComboBox Create()
        => new()
        {
            LabelText = "Systems",
            Items = Items(),
        };

    private static Dictionary<string, string> Items()
        => new(StringComparer.Ordinal)
        {
            ["a"] = "Alpha",
            ["b"] = "Beta",
            ["c"] = "Gamma",
        };

    private static void UseEnglishUi()
        => Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
}