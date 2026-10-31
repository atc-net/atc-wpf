namespace Atc.Wpf.Controls.Tests.Selectors;

/// <summary>
/// A view model that restores <c>SelectedKeys</c> and fills <c>Items</c> later (empty first, loaded afterwards) must
/// keep its keys in every control of the family: only keys of items that disappeared are dropped.
/// </summary>
[Collection(nameof(PackResourceTestGroup))]
public sealed class MultiSelectPresetKeysTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void CheckListBox_keeps_preset_keys_across_an_empty_first_Items()
    {
        var sut = new CheckListBox { SelectedKeys = ["a"] };
        var events = 0;
        sut.SelectedKeysChanged += (_, _) => events++;

        sut.Items = Empty();
        sut.Items = Loaded();

        Assert.Equal(["a"], sut.SelectedKeys);
        Assert.Equal(0, events);
    }

    [StaFact]
    public void MultiSelectComboBox_keeps_preset_keys_across_an_empty_first_Items()
    {
        var sut = new MultiSelectComboBox { SelectedKeys = ["a"] };
        var events = 0;
        sut.SelectedKeysChanged += (_, _) => events++;

        sut.Items = Empty();
        sut.Items = Loaded();

        Assert.Equal(["a"], sut.SelectedKeys);
        Assert.Equal(0, events);
    }

    [StaFact]
    public void ChipGroup_keeps_preset_keys_across_an_empty_first_Items()
    {
        var sut = new ChipGroup { SelectedKeys = ["a"] };
        var events = 0;
        sut.SelectedKeysChanged += (_, _) => events++;

        sut.Items = Empty();
        sut.Items = Loaded();

        Assert.Equal(["a"], sut.SelectedKeys);
        Assert.Equal(0, events);
    }

    [StaFact]
    public void TokenBox_keeps_preset_keys_across_an_empty_first_Items()
    {
        var sut = new TokenBox { AllowCreate = false, SelectedKeys = ["a"] };
        var events = 0;
        sut.SelectedKeysChanged += (_, _) => events++;

        sut.Items = Empty();
        sut.Items = Loaded();

        Assert.Equal(["a"], sut.SelectedKeys);
        Assert.Equal(0, events);
    }

    [StaFact]
    public void MultiColumnComboBox_keeps_preset_keys_across_an_empty_first_Items()
    {
        var sut = new MultiColumnComboBox
        {
            KeyMemberPath = "Key",
            DisplayMemberPath = "Value",
            SelectedKeys = ["a"],
        };
        var events = 0;
        sut.SelectedKeysChanged += (_, _) => events++;

        sut.ItemsSource = new List<KeyValuePair<string, string>>();
        sut.ItemsSource = Loaded().ToList();

        Assert.Equal(["a"], sut.SelectedKeys);
        Assert.Equal(0, events);
    }

    [StaFact]
    public void Keys_of_items_that_disappear_are_still_dropped()
    {
        var sut = new CheckListBox { Items = Loaded(), SelectedKeys = ["a", "b"] };
        var events = 0;
        sut.SelectedKeysChanged += (_, _) => events++;

        sut.Items = new Dictionary<string, string>(StringComparer.Ordinal) { ["b"] = "B" };

        Assert.Equal(["b"], sut.SelectedKeys);
        Assert.Equal(1, events);
    }

    private static Dictionary<string, string> Empty()
        => new(StringComparer.Ordinal);

    private static Dictionary<string, string> Loaded()
        => new(StringComparer.Ordinal)
        {
            ["a"] = "A",
            ["b"] = "B",
        };
}