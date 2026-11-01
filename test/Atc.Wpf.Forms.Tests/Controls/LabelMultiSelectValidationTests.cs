namespace Atc.Wpf.Forms.Tests.Controls;

/// <summary>
/// Every Label* multi-select wrapper validates the same way: a limit is checked whoever set the keys, an in-place
/// change of <c>SelectedKeys</c> re-validates, and a limit below one is rejected.
/// </summary>
public sealed class LabelMultiSelectValidationTests : IDisposable
{
    public static TheoryData<string> WrapperNames
        => new(
            nameof(LabelCheckListBox),
            nameof(LabelMultiSelectComboBox),
            nameof(LabelChipGroup),
            nameof(LabelTokenBox),
            nameof(LabelTreeSelectComboBox),
            nameof(LabelCascaderComboBox),
            nameof(LabelMultiColumnComboBox));

    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaTheory]
    [MemberData(nameof(WrapperNames))]
    public void More_keys_than_the_limit_set_in_code_is_not_valid(
        string wrapperName)
    {
        UseEnglishUi();
        var sut = Create(wrapperName);
        SetLimit(sut, 2);

        var keys = GetKeys(sut);
        keys.Add("a");
        keys.Add("b");
        keys.Add("c");

        Assert.False(sut.IsValid());
        Assert.Equal("Select at most 2", sut.ValidationText);
    }

    [StaTheory]
    [MemberData(nameof(WrapperNames))]
    public void Mandatory_keys_cleared_in_place_sets_the_validation_text(
        string wrapperName)
    {
        UseEnglishUi();
        var sut = Create(wrapperName);
        sut.IsMandatory = true;
        GetKeys(sut).Add("a");
        Assert.True(sut.IsValid());

        GetKeys(sut).Clear();

        Assert.Equal("Field is required", sut.ValidationText);
    }

    [StaTheory]
    [MemberData(nameof(WrapperNames))]
    public void A_limit_below_one_is_rejected(string wrapperName)
    {
        var sut = Create(wrapperName);

        Assert.Throws<ArgumentException>(() => SetLimit(sut, -1));
        Assert.Throws<ArgumentException>(() => SetLimit(sut, 0));

        SetLimit(sut, 1);
        Assert.Equal(1, sut.GetValue(LimitProperty(sut)));
    }

    private static LabelControl Create(string wrapperName)
        => wrapperName switch
        {
            nameof(LabelCheckListBox) => new LabelCheckListBox { LabelText = "X", Items = Items() },
            nameof(LabelMultiSelectComboBox) => new LabelMultiSelectComboBox { LabelText = "X", Items = Items() },
            nameof(LabelChipGroup) => new LabelChipGroup { LabelText = "X", Items = Items() },
            nameof(LabelTokenBox) => new LabelTokenBox { LabelText = "X", Items = Items() },
            nameof(LabelTreeSelectComboBox) => new LabelTreeSelectComboBox { LabelText = "X", Nodes = Nodes() },
            nameof(LabelCascaderComboBox) => new LabelCascaderComboBox { LabelText = "X", Nodes = Nodes() },
            nameof(LabelMultiColumnComboBox) => new LabelMultiColumnComboBox
            {
                LabelText = "X",
                KeyMemberPath = nameof(Row.Key),
                DisplayMemberPath = nameof(Row.Name),
                ItemsSource = Items().Select(x => new Row(x.Key, x.Value)).ToList(),
            },
            _ => throw new ArgumentOutOfRangeException(nameof(wrapperName), wrapperName, message: null),
        };

    private static DependencyProperty LimitProperty(LabelControl sut)
        => sut switch
        {
            LabelCheckListBox => LabelCheckListBox.MaxSelectionCountProperty,
            LabelMultiSelectComboBox => LabelMultiSelectComboBox.MaxSelectionCountProperty,
            LabelChipGroup => LabelChipGroup.MaxSelectionCountProperty,
            LabelTokenBox => LabelTokenBox.MaxSelectionCountProperty,
            LabelTreeSelectComboBox => LabelTreeSelectComboBox.MaxSelectionCountProperty,
            LabelCascaderComboBox => LabelCascaderComboBox.MaxSelectionCountProperty,
            _ => LabelMultiColumnComboBox.MaxSelectionCountProperty,
        };

    private static void SetLimit(
        LabelControl sut,
        int? limit)
        => sut.SetValue(LimitProperty(sut), limit);

    private static System.Collections.ObjectModel.ObservableCollection<string> GetKeys(
        LabelControl sut)
        => sut switch
        {
            LabelCheckListBox x => x.SelectedKeys,
            LabelMultiSelectComboBox x => x.SelectedKeys,
            LabelChipGroup x => x.SelectedKeys,
            LabelTokenBox x => x.SelectedKeys,
            LabelTreeSelectComboBox x => x.SelectedKeys,
            LabelCascaderComboBox x => x.SelectedKeys,
            LabelMultiColumnComboBox x => x.SelectedKeys,
            _ => throw new ArgumentOutOfRangeException(nameof(sut)),
        };

    private static Dictionary<string, string> Items()
        => new(StringComparer.Ordinal)
        {
            ["a"] = "Alpha",
            ["b"] = "Beta",
            ["c"] = "Gamma",
        };

    private static System.Collections.ObjectModel.ObservableCollection<CheckTreeNode> Nodes()
        => new(Items().Select(x => new CheckTreeNode(x.Key, x.Value)));

    private static void UseEnglishUi()
        => Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");

    private sealed record Row(string Key, string Name);
}