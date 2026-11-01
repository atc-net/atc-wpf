namespace Atc.Wpf.Forms.Tests.Controls;

public sealed class LabelCheckListBoxTests : IDisposable
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
    public void Items_and_SelectedKeys_reach_the_inner_list()
    {
        var sut = Create();
        sut.SelectedKeys.Add("c");

        Assert.Same(sut.Items, sut.CheckList.Items);
        Assert.Same(sut.SelectedKeys, sut.CheckList.SelectedKeys);
        Assert.True(sut.CheckList.GetItem("c")!.IsChecked);
    }

    [StaFact]
    public void Options_are_passed_through_to_the_inner_list()
    {
        var sut = Create();

        sut.ItemStyle = CheckListItemStyle.Switch;
        sut.MaxSelectionCount = 2;
        sut.ShowSelectAllRow = false;
        sut.ShowClearButton = true;
        sut.IsFilterVisible = true;
        sut.Columns = 2;
        sut.IsGroupCheckable = false;
        sut.SelectNewItems = true;
        sut.ItemGroups = new Dictionary<string, string>(StringComparer.Ordinal) { ["a"] = "G" };
        sut.DisabledKeys.Add("a");
        sut.MaxListHeight = 123;

        Assert.Equal(CheckListItemStyle.Switch, sut.CheckList.ItemStyle);
        Assert.Equal(2, sut.CheckList.MaxSelectionCount);
        Assert.False(sut.CheckList.ShowSelectAllRow);
        Assert.True(sut.CheckList.ShowClearButton);
        Assert.True(sut.CheckList.IsFilterVisible);
        Assert.Equal(2, sut.CheckList.Columns);
        Assert.False(sut.CheckList.IsGroupCheckable);
        Assert.True(sut.CheckList.SelectNewItems);
        Assert.Same(sut.ItemGroups, sut.CheckList.ItemGroups);
        Assert.Same(sut.DisabledKeys, sut.CheckList.DisabledKeys);
        Assert.Equal(123, sut.CheckList.MaxHeight);
    }

    [StaFact]
    public void Ticking_in_the_list_updates_SelectedKeys_and_raises_SelectedKeysChanged_with_the_identifier()
    {
        UseEnglishUi();
        var sut = Create();
        ValueChangedEventArgs<IReadOnlyList<string>>? raised = null;
        sut.SelectedKeysChanged += (_, e) => raised = e;

        sut.CheckList.GetItem("b")!.IsChecked = true;

        Assert.Equal(["b"], sut.SelectedKeys);
        Assert.NotNull(raised);
        Assert.Equal(sut.Identifier, raised.Identifier);
        Assert.Empty(raised.OldValue);
        Assert.Equal(["b"], raised.NewValue);
    }

    [StaFact]
    public void Unticking_the_last_key_while_mandatory_raises_the_invalid_event()
    {
        UseEnglishUi();
        var sut = Create();
        sut.IsMandatory = true;
        sut.CheckList.GetItem("a")!.IsChecked = true;
        var changed = 0;
        var invalid = 0;
        sut.SelectedKeysChanged += (_, _) => changed++;
        sut.SelectedKeysChangedInvalid += (_, _) => invalid++;

        sut.CheckList.GetItem("a")!.IsChecked = false;

        Assert.Equal(0, changed);
        Assert.Equal(1, invalid);
        Assert.Equal("Field is required", sut.ValidationText);
    }

    [StaFact]
    public void A_new_SelectedKeys_instance_is_picked_up()
    {
        var sut = Create();

        sut.SelectedKeys = ["a", "c"];

        Assert.True(sut.CheckList.GetItem("a")!.IsChecked);
        Assert.True(sut.CheckList.GetItem("c")!.IsChecked);
    }

    [StaFact]
    public void Implements_ILabelCheckListBox()
    {
        ILabelCheckListBox sut = Create();
        var raised = 0;
        sut.SelectedKeysChanged += (_, _) => raised++;

        ((LabelCheckListBox)sut).CheckList.SelectAll();

        Assert.Equal(["a", "b", "c"], sut.SelectedKeys);
        Assert.Equal(1, raised);
    }

    [StaFact]
    public void Factory_creates_a_labeled_list_with_items_and_ticks()
    {
        var sut = LabelControlFactory.CreateLabelCheckListBox(
            groupIdentifier: "Group",
            labelText: "ColumnsToExport",
            isReadOnly: false,
            isMandatory: true,
            items: Items(),
            selectedKeys: ["b"]);

        Assert.Equal("Group", sut.GroupIdentifier);
        Assert.Equal("Columns to export", sut.LabelText);
        Assert.True(sut.IsMandatory);
        Assert.Equal(3, sut.Items.Count);
        Assert.Equal(["b"], sut.SelectedKeys);
        Assert.True(sut.IsValid());
    }

    [StaFact]
    public void Form_GetKeyValues_returns_the_ticked_keys()
    {
        var sut = LabelControlFactory.CreateLabelCheckListBox(
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
        var sut = LabelControlFactory.CreateLabelCheckListBox(
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

    [StaFact]
    public void IsValid_MoreKeysThanTheLimitSetInCode_ReportsSelectAtMost()
    {
        UseEnglishUi();
        var sut = Create();
        sut.MaxSelectionCount = 2;

        sut.SelectedKeys.Add("a");
        sut.SelectedKeys.Add("b");
        sut.SelectedKeys.Add("c");

        Assert.False(sut.IsValid());
        Assert.Equal("Select at most 2", sut.ValidationText);
    }

    [StaFact]
    public void Mandatory_SelectedKeysClearedInPlace_SetsValidationText()
    {
        UseEnglishUi();
        var sut = Create();
        sut.IsMandatory = true;
        sut.SelectedKeys.Add("a");
        Assert.True(sut.IsValid());

        sut.SelectedKeys.Clear();

        Assert.Equal("Field is required", sut.ValidationText);
    }

    [StaFact]
    public void Mandatory_OnlyAStaleKey_IsNotValid()
    {
        UseEnglishUi();
        var sut = Create();
        sut.IsMandatory = true;

        sut.SelectedKeys = ["gone"];

        Assert.False(sut.IsValid());
        Assert.Equal("Field is required", sut.ValidationText);
    }

    [StaTheory]
    [InlineData(-1)]
    [InlineData(0)]
    public void MaxSelectionCount_BelowOne_IsRejected(int limit)
    {
        var sut = Create();

        Assert.Throws<ArgumentException>(() => sut.MaxSelectionCount = limit);
        Assert.Null(sut.MaxSelectionCount);
    }

    private static LabelCheckListBox Create()
        => new()
        {
            LabelText = "Columns",
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