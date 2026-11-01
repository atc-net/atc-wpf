namespace Atc.Wpf.Forms.Tests.Controls;

public sealed class LabelMultiColumnComboBoxTests : IDisposable
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

        sut.SelectedKeys.Add("1002");

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

        sut.SelectedKeys = ["1001", "1002"];

        Assert.False(sut.IsValid());
        Assert.Equal(2, sut.SelectedKeys.Count);
    }

    [StaFact]
    public void Items_columns_and_SelectedKeys_reach_the_inner_combo_box()
    {
        var sut = Create();
        sut.SelectedKeys.Add("1003");

        Assert.Same(sut.ItemsSource, sut.ComboBox.ItemsSource);
        Assert.Same(sut.SelectedKeys, sut.ComboBox.SelectedKeys);
        Assert.Same(sut.Columns, sut.ComboBox.Columns);
        Assert.Equal("Charlie", sut.ComboBox.SummaryText);
    }

    [StaFact]
    public void Options_are_passed_through_to_the_inner_combo_box()
    {
        var sut = Create();
        var chipTemplate = new DataTemplate();

        sut.AutoGenerateColumns = true;
        sut.MaxSelectionCount = 4;
        sut.IsEnabledMemberPath = "IsActive";
        sut.IsFilterVisible = true;
        sut.FilterText = "ber";
        sut.FilterMemberPaths = "Name,City";
        sut.KeepSelectedOnTop = true;
        sut.DropDownWidth = 480;
        sut.CommitMode = MultiSelectCommitMode.Deferred;
        sut.DisplayMode = MultiSelectDisplayMode.Chips;
        sut.MaxVisibleChips = 2;
        sut.ChipsWrap = true;
        sut.ChipTemplate = chipTemplate;
        sut.MaxNamesInSummary = 1;
        sut.AllText = "Everyone";
        sut.NoneText = "Nobody";
        sut.CountFormat = "{0}/{1}";
        sut.MaxDropDownHeight = 123;
        sut.WatermarkText = "Pick customers";

        var inner = sut.ComboBox;
        Assert.True(inner.AutoGenerateColumns);
        Assert.Equal(4, inner.MaxSelectionCount);
        Assert.Equal("IsActive", inner.IsEnabledMemberPath);
        Assert.True(inner.IsFilterVisible);
        Assert.Equal("ber", inner.FilterText);
        Assert.Equal("Name,City", inner.FilterMemberPaths);
        Assert.True(inner.KeepSelectedOnTop);
        Assert.Equal(480, inner.DropDownWidth);
        Assert.Equal(MultiSelectCommitMode.Deferred, inner.CommitMode);
        Assert.Equal(MultiSelectDisplayMode.Chips, inner.DisplayMode);
        Assert.Equal(2, inner.MaxVisibleChips);
        Assert.True(inner.ChipsWrap);
        Assert.Same(chipTemplate, inner.ChipTemplate);
        Assert.Equal(1, inner.MaxNamesInSummary);
        Assert.Equal("Everyone", inner.AllText);
        Assert.Equal("Nobody", inner.NoneText);
        Assert.Equal("{0}/{1}", inner.CountFormat);
        Assert.Equal(123, inner.MaxDropDownHeight);
        Assert.Equal("Pick customers", Atc.Wpf.Helpers.TextBoxHelper.GetWatermark(inner));
    }

    [StaFact]
    public void Ticking_raises_SelectedKeysChanged_with_the_identifier()
    {
        var sut = Create();
        var events = new List<ValueChangedEventArgs<IReadOnlyList<string>>>();
        sut.SelectedKeysChanged += (_, e) => events.Add(e);

        sut.ComboBox.ToggleItem(((List<Customer>)sut.ItemsSource!)[1]);

        Assert.Equal(["1002"], sut.SelectedKeys);
        var change = Assert.Single(events);
        Assert.Equal(sut.Identifier, change.Identifier);
    }

    [StaFact]
    public void Unticking_the_last_key_while_mandatory_raises_the_invalid_event()
    {
        UseEnglishUi();
        var sut = Create();
        sut.IsMandatory = true;
        var item = ((List<Customer>)sut.ItemsSource!)[2];
        sut.ComboBox.ToggleItem(item);
        var changed = 0;
        var invalid = 0;
        sut.SelectedKeysChanged += (_, _) => changed++;
        sut.SelectedKeysChangedInvalid += (_, _) => invalid++;

        sut.ComboBox.ToggleItem(item);

        Assert.Equal(0, changed);
        Assert.Equal(1, invalid);
        Assert.Equal("Field is required", sut.ValidationText);
    }

    [StaFact]
    public void Implements_ILabelMultiColumnComboBox()
    {
        ILabelMultiColumnComboBox sut = Create();
        var raised = 0;
        sut.SelectedKeysChanged += (_, _) => raised++;

        ((LabelMultiColumnComboBox)sut).ComboBox.SelectAll();

        Assert.Equal(["1001", "1002", "1003"], sut.SelectedKeys);
        Assert.Equal("Number", sut.KeyMemberPath);
        Assert.Equal("Name", sut.DisplayMemberPath);
        Assert.Equal(1, raised);
    }

    [StaFact]
    public void Factory_creates_a_labeled_multi_column_combo_box_with_items_and_ticks()
    {
        var sut = LabelControlFactory.CreateLabelMultiColumnComboBox(
            groupIdentifier: "Group",
            labelText: "KeyAccounts",
            isReadOnly: false,
            isMandatory: true,
            itemsSource: Customers(),
            keyMemberPath: "Number",
            displayMemberPath: "Name",
            selectedKeys: ["1002"]);

        Assert.Equal("Group", sut.GroupIdentifier);
        Assert.Equal("Key accounts", sut.LabelText);
        Assert.True(sut.IsMandatory);
        Assert.True(sut.AutoGenerateColumns);
        Assert.Equal("Bravo", sut.ComboBox.SummaryText);
        Assert.True(sut.IsValid());
    }

    [StaFact]
    public void Form_GetKeyValues_returns_the_ticked_keys()
    {
        var sut = LabelControlFactory.CreateLabelMultiColumnComboBox(
            groupIdentifier: "Sales",
            labelText: "Customers",
            isReadOnly: false,
            isMandatory: false,
            itemsSource: Customers(),
            keyMemberPath: "Number",
            displayMemberPath: "Name",
            selectedKeys: ["1003", "1001"]);
        var form = new LabelControlsForm();
        form.AddColumn(new List<ILabelControlBase> { sut });

        var keyValues = form.GetKeyValues();

        var value = Assert.Single(keyValues).Value;
        Assert.Equal(["1003", "1001"], Assert.IsAssignableFrom<IEnumerable<string>>(value), StringComparer.Ordinal);
    }

    private static LabelMultiColumnComboBox Create()
    {
        var sut = new LabelMultiColumnComboBox
        {
            LabelText = "Customers",
            KeyMemberPath = "Number",
            DisplayMemberPath = "Name",
            ItemsSource = Customers(),
        };
        sut.Columns.Add(new DataGridTextColumn { Header = "No.", Binding = new Binding("Number") });
        sut.Columns.Add(new DataGridTextColumn { Header = "Name", Binding = new Binding("Name") });
        return sut;
    }

    private static List<Customer> Customers()
        =>
        [
            new("1001", "Alpha", "Aarhus", IsActive: true),
            new("1002", "Bravo", "Berlin", IsActive: true),
            new("1003", "Charlie", "Copenhagen", IsActive: true),
        ];

    private static void UseEnglishUi()
        => Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");

    private sealed record Customer(
        string Number,
        string Name,
        string City,
        bool IsActive);
}