namespace Atc.Wpf.Forms.Tests.Controls;

public sealed class LabelChipGroupTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void IsValid_MandatoryWithNothingSelected_ReportsFieldIsRequired()
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
    public void IsValid_NotMandatory_IsValidWithNothingSelected()
    {
        var sut = Create();

        Assert.True(sut.IsValid());
    }

    [StaFact]
    public void IsValid_MoreKeysThanTheLimit_ReportsSelectAtMost()
    {
        UseEnglishUi();
        var sut = Create();
        sut.MaxSelectionCount = 2;

        sut.SelectedKeys = ["a", "b", "c"];

        Assert.False(sut.IsValid());
        Assert.Equal("Select at most 2", sut.ValidationText);

        sut.SelectedKeys.Remove("c");

        Assert.True(sut.IsValid());
    }

    [StaFact]
    public void Items_and_SelectedKeys_reach_the_inner_group()
    {
        var sut = Create();
        sut.SelectedKeys.Add("c");

        Assert.Same(sut.Items, sut.ChipGroup.Items);
        Assert.Same(sut.SelectedKeys, sut.ChipGroup.SelectedKeys);
        Assert.True(FindChip(sut, "Gamma").IsSelected);
    }

    [StaFact]
    public void Options_are_passed_through_to_the_inner_group()
    {
        var sut = Create();
        var icons = new Dictionary<string, object>(StringComparer.Ordinal) { ["a"] = "*" };
        var counts = new Dictionary<string, int>(StringComparer.Ordinal) { ["a"] = 3 };

        sut.SelectionMode = ChipSelectionMode.SingleOrNone;
        sut.SelectNewItems = true;
        sut.ChipVariant = ChipGroupVariant.Outlined;
        sut.ChipSize = ChipSize.Large;
        sut.ShowCheckMark = false;
        sut.ChipOrientation = Orientation.Vertical;
        sut.Wrap = false;
        sut.ItemSpacing = 10;
        sut.ShowAllChip = true;
        sut.MaxSelectionCount = 2;
        sut.DisabledKeys.Add("a");
        sut.ItemIcons = icons;
        sut.ItemCounts = counts;

        var inner = sut.ChipGroup;
        Assert.Equal(ChipSelectionMode.SingleOrNone, inner.SelectionMode);
        Assert.True(inner.SelectNewItems);
        Assert.Equal(ChipGroupVariant.Outlined, inner.ChipVariant);
        Assert.Equal(ChipSize.Large, inner.ChipSize);
        Assert.False(inner.ShowCheckMark);
        Assert.Equal(Orientation.Vertical, inner.Orientation);
        Assert.False(inner.Wrap);
        Assert.Equal(10d, inner.ItemSpacing);
        Assert.True(inner.ShowAllChip);
        Assert.Equal(2, inner.MaxSelectionCount);
        Assert.Same(sut.DisabledKeys, inner.DisabledKeys);
        Assert.Same(icons, inner.ItemIcons);
        Assert.Same(counts, inner.ItemCounts);
    }

    [StaFact]
    public void Toggling_a_chip_updates_SelectedKeys_and_raises_SelectedKeysChanged_with_the_identifier()
    {
        var sut = Create();
        ValueChangedEventArgs<IReadOnlyList<string>>? raised = null;
        sut.SelectedKeysChanged += (_, e) => raised = e;

        Toggle(FindChip(sut, "Beta"));

        Assert.Equal(["b"], sut.SelectedKeys);
        Assert.NotNull(raised);
        Assert.Equal(sut.Identifier, raised.Identifier);
        Assert.Empty(raised.OldValue);
        Assert.Equal(["b"], raised.NewValue);
    }

    [StaFact]
    public void Untoggling_the_last_key_while_mandatory_raises_the_invalid_event()
    {
        UseEnglishUi();
        var sut = Create();
        sut.IsMandatory = true;
        Toggle(FindChip(sut, "Alpha"));
        var changed = 0;
        var invalid = 0;
        sut.SelectedKeysChanged += (_, _) => changed++;
        sut.SelectedKeysChangedInvalid += (_, _) => invalid++;

        Toggle(FindChip(sut, "Alpha"));

        Assert.Equal(0, changed);
        Assert.Equal(1, invalid);
        Assert.Equal("Field is required", sut.ValidationText);
    }

    [StaFact]
    public void A_new_SelectedKeys_instance_is_picked_up()
    {
        var sut = Create();

        sut.SelectedKeys = ["a", "c"];

        Assert.Same(sut.SelectedKeys, sut.ChipGroup.SelectedKeys);
        Assert.True(FindChip(sut, "Alpha").IsSelected);
        Assert.False(FindChip(sut, "Beta").IsSelected);
        Assert.True(FindChip(sut, "Gamma").IsSelected);
    }

    [StaFact]
    public void Implements_ILabelChipGroup()
    {
        ILabelChipGroup sut = Create();
        var raised = 0;
        sut.SelectedKeysChanged += (_, _) => raised++;

        ((LabelChipGroup)sut).ChipGroup.SelectAll();

        Assert.Equal(["a", "b", "c"], sut.SelectedKeys);
        Assert.Equal(ChipSelectionMode.Multiple, sut.SelectionMode);
        Assert.Equal(1, raised);
    }

    [StaFact]
    public void Factory_creates_a_labeled_chip_group_with_items_and_selection()
    {
        var sut = LabelControlFactory.CreateLabelChipGroup(
            groupIdentifier: "Group",
            labelText: "LogLevels",
            isReadOnly: false,
            isMandatory: true,
            items: Items(),
            selectedKeys: ["b"]);

        Assert.Equal("Group", sut.GroupIdentifier);
        Assert.Equal("Log levels", sut.LabelText);
        Assert.True(sut.IsMandatory);
        Assert.Equal(3, sut.Items.Count);
        Assert.Equal(["b"], sut.SelectedKeys);
        Assert.True(sut.IsValid());
    }

    [StaFact]
    public void Form_GetKeyValues_returns_the_selected_keys()
    {
        var sut = LabelControlFactory.CreateLabelChipGroup(
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
    public void Form_values_write_the_selected_keys_back_to_a_model()
    {
        var sut = LabelControlFactory.CreateLabelChipGroup(
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

    private static LabelChipGroup Create()
        => new()
        {
            LabelText = "Levels",
            Items = Items(),
        };

    private static Dictionary<string, string> Items()
        => new(StringComparer.Ordinal)
        {
            ["a"] = "Alpha",
            ["b"] = "Beta",
            ["c"] = "Gamma",
        };

    private static Atc.Wpf.Controls.DataDisplay.Chip FindChip(
        LabelChipGroup sut,
        string name)
    {
        var chip = FindLogical<Atc.Wpf.Controls.DataDisplay.Chip>(
            sut.ChipGroup,
            x => string.Equals(System.Windows.Automation.AutomationProperties.GetName(x), name, StringComparison.Ordinal));
        Assert.NotNull(chip);
        return chip;
    }

    private static T? FindLogical<T>(
        DependencyObject parent,
        Func<T, bool> predicate)
        where T : DependencyObject
    {
        foreach (var child in LogicalTreeHelper.GetChildren(parent).OfType<DependencyObject>())
        {
            if (child is T match && predicate(match))
            {
                return match;
            }

            var descendant = FindLogical(child, predicate);
            if (descendant is not null)
            {
                return descendant;
            }
        }

        return null;
    }

    private static void Toggle(UIElement chip)
        => ((System.Windows.Automation.Provider.IToggleProvider)System.Windows.Automation.Peers.UIElementAutomationPeer
            .CreatePeerForElement(chip)
            .GetPattern(System.Windows.Automation.Peers.PatternInterface.Toggle)).Toggle();

    private static void UseEnglishUi()
        => Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
}