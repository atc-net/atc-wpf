namespace Atc.Wpf.Forms.Tests.Controls;

public sealed class LabelTreeSelectComboBoxTests : IDisposable
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

        sut.SelectedKeys.Add("dk-cph");

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
    public void IsValid_KeysBeyondTheLimit_AreKeptButInvalid()
    {
        UseEnglishUi();
        var sut = Create();
        sut.MaxSelectionCount = 1;

        sut.SelectedKeys = ["dk-cph", "se-sto"];

        Assert.False(sut.IsValid());
        Assert.Equal(2, sut.SelectedKeys.Count);
    }

    [StaFact]
    public void Nodes_and_SelectedKeys_reach_the_inner_combo_box()
    {
        var sut = Create();
        sut.SelectedKeys.Add("dk-cph");

        Assert.Same(sut.Nodes, sut.ComboBox.Nodes);
        Assert.Same(sut.SelectedKeys, sut.ComboBox.SelectedKeys);
        Assert.Equal("Copenhagen", sut.ComboBox.SummaryText);
        Assert.True(sut.ComboBox.FindNode("dk-cph")!.IsChecked);
    }

    [StaFact]
    public void Options_are_passed_through_to_the_inner_combo_box()
    {
        var sut = Create();
        var template = new DataTemplate();
        var chipTemplate = new DataTemplate();
        Func<CheckTreeNode, CancellationToken, Task<IReadOnlyList<CheckTreeNode>>> loader =
            (_, _) => Task.FromResult<IReadOnlyList<CheckTreeNode>>([]);

        sut.CheckedKeysStrategy = CheckedKeysStrategy.Parents;
        sut.IsCheckLinked = false;
        sut.IsFilterVisible = true;
        sut.FilterText = "den";
        sut.FilterMode = TreeFilterMode.FlatResults;
        sut.ShowSelectAllRow = true;
        sut.MaxSelectionCount = 4;
        sut.ExpandDepth = 0;
        sut.ItemTemplate = template;
        sut.DisplayMode = MultiSelectDisplayMode.Chips;
        sut.MaxVisibleChips = 2;
        sut.ChipsWrap = true;
        sut.ChipTemplate = chipTemplate;
        sut.MaxNamesInSummary = 1;
        sut.AllText = "Everything";
        sut.NoneText = "Nothing";
        sut.CountFormat = "{0}/{1}";
        sut.MaxDropDownHeight = 123;
        sut.WatermarkText = "Pick regions";
        sut.LoadChildrenAsync = loader;

        var inner = sut.ComboBox;
        Assert.Equal(CheckedKeysStrategy.Parents, inner.CheckedKeysStrategy);
        Assert.False(inner.IsCheckLinked);
        Assert.True(inner.IsFilterVisible);
        Assert.Equal("den", inner.FilterText);
        Assert.Equal(TreeFilterMode.FlatResults, inner.FilterMode);
        Assert.True(inner.ShowSelectAllRow);
        Assert.Equal(4, inner.MaxSelectionCount);
        Assert.Equal(0, inner.ExpandDepth);
        Assert.Same(template, inner.ItemTemplate);
        Assert.Equal(MultiSelectDisplayMode.Chips, inner.DisplayMode);
        Assert.Equal(2, inner.MaxVisibleChips);
        Assert.True(inner.ChipsWrap);
        Assert.Same(chipTemplate, inner.ChipTemplate);
        Assert.Equal(1, inner.MaxNamesInSummary);
        Assert.Equal("Everything", inner.AllText);
        Assert.Equal("Nothing", inner.NoneText);
        Assert.Equal("{0}/{1}", inner.CountFormat);
        Assert.Equal(123, inner.MaxDropDownHeight);
        Assert.Equal("Pick regions", Atc.Wpf.Helpers.TextBoxHelper.GetWatermark(inner));
        Assert.Same(loader, inner.LoadChildrenAsync);
    }

    [StaFact]
    public void Ticking_raises_SelectedKeysChanged_with_the_identifier()
    {
        var sut = Create();
        sut.LabelText = "Regions";
        var events = new List<ValueChangedEventArgs<IReadOnlyList<string>>>();
        sut.SelectedKeysChanged += (_, e) => events.Add(e);

        sut.ComboBox.ToggleNode(sut.ComboBox.FindNode("dk")!);

        Assert.Equal(["dk-aarhus", "dk-cph", "dk-odense"], sut.SelectedKeys);
        var change = Assert.Single(events);
        Assert.Equal(sut.Identifier, change.Identifier);
    }

    [StaFact]
    public void Unticking_the_last_key_while_mandatory_raises_the_invalid_event()
    {
        UseEnglishUi();
        var sut = Create();
        sut.IsMandatory = true;
        sut.ComboBox.ToggleNode(sut.ComboBox.FindNode("dk-cph")!);
        var changed = 0;
        var invalid = 0;
        sut.SelectedKeysChanged += (_, _) => changed++;
        sut.SelectedKeysChangedInvalid += (_, _) => invalid++;

        sut.ComboBox.ToggleNode(sut.ComboBox.FindNode("dk-cph")!);

        Assert.Equal(0, changed);
        Assert.Equal(1, invalid);
        Assert.Equal("Field is required", sut.ValidationText);
    }

    [StaFact]
    public void Implements_ILabelTreeSelectComboBox()
    {
        ILabelTreeSelectComboBox sut = Create();
        var raised = 0;
        sut.SelectedKeysChanged += (_, _) => raised++;

        ((LabelTreeSelectComboBox)sut).ComboBox.SelectAll();

        Assert.Equal(5, sut.SelectedKeys.Count);
        Assert.Equal(1, raised);
    }

    [StaFact]
    public void Factory_creates_a_labeled_tree_drop_down_with_nodes_and_ticks()
    {
        var sut = LabelControlFactory.CreateLabelTreeSelectComboBox(
            groupIdentifier: "Group",
            labelText: "SalesRegions",
            isReadOnly: false,
            isMandatory: true,
            nodes: Nodes(),
            selectedKeys: ["dk"],
            checkedKeysStrategy: CheckedKeysStrategy.Parents);

        Assert.Equal("Group", sut.GroupIdentifier);
        Assert.Equal("Sales regions", sut.LabelText);
        Assert.True(sut.IsMandatory);
        Assert.Equal(CheckedKeysStrategy.Parents, sut.CheckedKeysStrategy);
        Assert.Single(sut.Nodes);
        Assert.True(sut.ComboBox.FindNode("dk-odense")!.IsChecked);
        Assert.True(sut.IsValid());
    }

    [StaFact]
    public void Form_GetKeyValues_returns_the_ticked_keys()
    {
        var sut = LabelControlFactory.CreateLabelTreeSelectComboBox(
            groupIdentifier: "Sales",
            labelText: "Regions",
            isReadOnly: false,
            isMandatory: false,
            nodes: Nodes(),
            selectedKeys: ["dk-cph", "se-sto"]);
        var form = new LabelControlsForm();
        form.AddColumn(new List<ILabelControlBase> { sut });

        var keyValues = form.GetKeyValues();

        var value = Assert.Single(keyValues).Value;
        Assert.Equal(["dk-cph", "se-sto"], Assert.IsAssignableFrom<IEnumerable<string>>(value), StringComparer.Ordinal);
    }

    private static LabelTreeSelectComboBox Create()
        => new()
        {
            LabelText = "Regions",
            Nodes = new System.Collections.ObjectModel.ObservableCollection<CheckTreeNode>(Nodes()),
        };

    private static List<CheckTreeNode> Nodes()
    {
        var dk = new CheckTreeNode("dk", "Denmark");
        dk.Children.Add(new CheckTreeNode("dk-aarhus", "Aarhus"));
        dk.Children.Add(new CheckTreeNode("dk-cph", "Copenhagen"));
        dk.Children.Add(new CheckTreeNode("dk-odense", "Odense"));

        var se = new CheckTreeNode("se", "Sweden");
        se.Children.Add(new CheckTreeNode("se-sto", "Stockholm"));
        se.Children.Add(new CheckTreeNode("se-got", "Gothenburg"));

        var nordics = new CheckTreeNode("nordics", "Nordics");
        nordics.Children.Add(dk);
        nordics.Children.Add(se);
        return [nordics];
    }

    private static void UseEnglishUi()
        => Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
}