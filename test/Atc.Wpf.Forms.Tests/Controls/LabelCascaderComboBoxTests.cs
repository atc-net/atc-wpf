namespace Atc.Wpf.Forms.Tests.Controls;

public sealed class LabelCascaderComboBoxTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void Defaults_use_the_Parents_strategy_inside_and_out()
    {
        var sut = Create();

        Assert.Equal(CheckedKeysStrategy.Parents, sut.CheckedKeysStrategy);
        Assert.Equal(CheckedKeysStrategy.Parents, sut.ComboBox.CheckedKeysStrategy);

        sut.ComboBox.ToggleNode(sut.ComboBox.FindNode("dk-jut")!);

        Assert.Equal(["dk-jut"], sut.SelectedKeys);
    }

    [StaFact]
    public void IsValid_MandatoryWithNothingTicked_ReportsFieldIsRequired()
    {
        UseEnglishUi();
        var sut = Create();
        sut.IsMandatory = true;

        Assert.False(sut.IsValid());
        Assert.Equal("Field is required", sut.ValidationText);

        sut.SelectedKeys.Add("dk-jut-aar");

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

        sut.SelectedKeys = ["dk-jut-aar", "se"];

        Assert.False(sut.IsValid());
        Assert.Equal(2, sut.SelectedKeys.Count);
    }

    [StaFact]
    public void Nodes_and_SelectedKeys_reach_the_inner_combo_box()
    {
        var sut = Create();
        sut.SelectedKeys.Add("dk-jut-aar");

        Assert.Same(sut.Nodes, sut.ComboBox.Nodes);
        Assert.Same(sut.SelectedKeys, sut.ComboBox.SelectedKeys);
        Assert.Equal("Aarhus", sut.ComboBox.SummaryText);
        Assert.Null(sut.ComboBox.FindNode("dk-jut")!.IsChecked);
    }

    [StaFact]
    public void Options_are_passed_through_to_the_inner_combo_box()
    {
        var sut = Create();
        var template = new DataTemplate();
        var chipTemplate = new DataTemplate();
        Func<CheckTreeNode, CancellationToken, Task<IReadOnlyList<CheckTreeNode>>> loader =
            (_, _) => Task.FromResult<IReadOnlyList<CheckTreeNode>>([]);

        sut.CheckedKeysStrategy = CheckedKeysStrategy.Leaves;
        sut.IsCheckLinked = false;
        sut.IsMultiSelect = false;
        sut.ExpandTrigger = CascaderExpandTrigger.Hover;
        sut.ColumnWidth = 220;
        sut.IsFilterVisible = true;
        sut.FilterText = "den";
        sut.MaxSelectionCount = 4;
        sut.PathSeparator = " > ";
        sut.ShowFullPath = true;
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
        Assert.Equal(CheckedKeysStrategy.Leaves, inner.CheckedKeysStrategy);
        Assert.False(inner.IsCheckLinked);
        Assert.False(inner.IsMultiSelect);
        Assert.Equal(CascaderExpandTrigger.Hover, inner.ExpandTrigger);
        Assert.Equal(220, inner.ColumnWidth);
        Assert.True(inner.IsFilterVisible);
        Assert.Equal("den", inner.FilterText);
        Assert.Equal(4, inner.MaxSelectionCount);
        Assert.Equal(" > ", inner.PathSeparator);
        Assert.True(inner.ShowFullPath);
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

        Assert.Equal(["dk"], sut.SelectedKeys);
        var change = Assert.Single(events);
        Assert.Equal(sut.Identifier, change.Identifier);
    }

    [StaFact]
    public void Unticking_the_last_key_while_mandatory_raises_the_invalid_event()
    {
        UseEnglishUi();
        var sut = Create();
        sut.IsMandatory = true;
        sut.ComboBox.ToggleNode(sut.ComboBox.FindNode("dk-jut-aar")!);
        var changed = 0;
        var invalid = 0;
        sut.SelectedKeysChanged += (_, _) => changed++;
        sut.SelectedKeysChangedInvalid += (_, _) => invalid++;

        sut.ComboBox.ToggleNode(sut.ComboBox.FindNode("dk-jut-aar")!);

        Assert.Equal(0, changed);
        Assert.Equal(1, invalid);
        Assert.Equal("Field is required", sut.ValidationText);
    }

    [StaFact]
    public void Implements_ILabelCascaderComboBox()
    {
        ILabelCascaderComboBox sut = Create();
        var raised = 0;
        sut.SelectedKeysChanged += (_, _) => raised++;

        ((LabelCascaderComboBox)sut).ComboBox.SelectAll();

        Assert.Equal(["dk", "se"], sut.SelectedKeys);
        Assert.Equal(1, raised);
    }

    [StaFact]
    public void Factory_creates_a_labeled_cascader_with_nodes_and_ticks()
    {
        var sut = LabelControlFactory.CreateLabelCascaderComboBox(
            groupIdentifier: "Group",
            labelText: "SalesRegions",
            isReadOnly: false,
            isMandatory: true,
            nodes: Nodes(),
            selectedKeys: ["dk-jut"]);

        Assert.Equal("Group", sut.GroupIdentifier);
        Assert.Equal("Sales regions", sut.LabelText);
        Assert.True(sut.IsMandatory);
        Assert.Equal(CheckedKeysStrategy.Parents, sut.CheckedKeysStrategy);
        Assert.Equal(2, sut.Nodes.Count);
        Assert.True(sut.ComboBox.FindNode("dk-jut-aal")!.IsChecked);
        Assert.True(sut.IsValid());
    }

    [StaFact]
    public void Form_GetKeyValues_returns_the_ticked_keys()
    {
        var sut = LabelControlFactory.CreateLabelCascaderComboBox(
            groupIdentifier: "Sales",
            labelText: "Regions",
            isReadOnly: false,
            isMandatory: false,
            nodes: Nodes(),
            selectedKeys: ["dk-jut-aar", "se"]);
        var form = new LabelControlsForm();
        form.AddColumn(new List<ILabelControlBase> { sut });

        var keyValues = form.GetKeyValues();

        var value = Assert.Single(keyValues).Value;
        Assert.Equal(["dk-jut-aar", "se"], Assert.IsAssignableFrom<IEnumerable<string>>(value), StringComparer.Ordinal);
    }

    private static LabelCascaderComboBox Create()
        => new()
        {
            LabelText = "Regions",
            Nodes = new System.Collections.ObjectModel.ObservableCollection<CheckTreeNode>(Nodes()),
        };

    private static List<CheckTreeNode> Nodes()
    {
        var jutland = new CheckTreeNode("dk-jut", "Jutland");
        jutland.Children.Add(new CheckTreeNode("dk-jut-aar", "Aarhus"));
        jutland.Children.Add(new CheckTreeNode("dk-jut-aal", "Aalborg"));

        var zealand = new CheckTreeNode("dk-zea", "Zealand");
        zealand.Children.Add(new CheckTreeNode("dk-zea-cph", "Copenhagen"));

        var dk = new CheckTreeNode("dk", "Denmark");
        dk.Children.Add(jutland);
        dk.Children.Add(zealand);

        var svealand = new CheckTreeNode("se-sve", "Svealand");
        svealand.Children.Add(new CheckTreeNode("se-sve-sto", "Stockholm"));

        var se = new CheckTreeNode("se", "Sweden");
        se.Children.Add(svealand);
        return [dk, se];
    }

    private static void UseEnglishUi()
        => Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
}