namespace Atc.Wpf.Tests.Collections;

public sealed class ObservableDictionaryTests
{
    [Fact]
    public void Add_increases_Count_and_makes_ContainsKey_return_true()
    {
        var sut = new ObservableDictionary<string, int> { { "a", 1 } };

        Assert.Single(sut);
        Assert.True(sut.ContainsKey("a"));
    }

    [Fact]
    public void Add_throws_when_key_already_exists()
    {
        var sut = new ObservableDictionary<string, int> { { "a", 1 } };

        Assert.Throws<ArgumentException>(() => sut.Add("a", 2));
    }

    [Fact]
    public void Indexer_returns_existing_value_and_throws_for_missing_key()
    {
        var sut = new ObservableDictionary<string, int> { { "a", 1 } };

        Assert.Equal(1, sut["a"]);
        Assert.Throws<ArgumentException>(() => _ = sut["missing"]);
    }

    [Fact]
    public void Indexer_setter_inserts_new_keys_and_updates_existing_keys()
    {
        var sut = new ObservableDictionary<string, int>();

        sut["a"] = 1;
        Assert.Equal(1, sut["a"]);

        sut["a"] = 99;
        Assert.Equal(99, sut["a"]);
        Assert.Single(sut);
    }

    [Fact]
    public void TryGetValue_returns_false_when_key_is_missing()
    {
        var sut = new ObservableDictionary<string, int>();

        var found = sut.TryGetValue("missing", out var value);

        Assert.False(found);
        Assert.Equal(0, value);
    }

    [Fact]
    public void TryGetValue_returns_true_and_the_value_when_key_exists()
    {
        var sut = new ObservableDictionary<string, int> { { "answer", 42 } };

        var found = sut.TryGetValue("answer", out var value);

        Assert.True(found);
        Assert.Equal(42, value);
    }

    [Fact]
    public void Remove_returns_true_and_decreases_Count_when_key_exists()
    {
        var sut = new ObservableDictionary<string, int> { { "a", 1 }, { "b", 2 } };

        Assert.True(sut.Remove("a"));
        Assert.Single(sut);
        Assert.False(sut.ContainsKey("a"));
    }

    [Fact]
    public void Remove_returns_false_when_key_is_missing()
    {
        var sut = new ObservableDictionary<string, int> { { "a", 1 } };

        Assert.False(sut.Remove("missing"));
        Assert.Single(sut);
    }

    [Fact]
    public void Keys_and_Values_reflect_the_current_contents()
    {
        var sut = new ObservableDictionary<string, int>
        {
            { "a", 1 },
            { "b", 2 },
        };

        Assert.Equal(new[] { "a", "b" }, sut.Keys);
        Assert.Equal(new[] { 1, 2 }, sut.Values);
    }

    [Fact]
    public void CopyTo_copies_pairs_into_the_destination_array_starting_at_arrayIndex()
    {
        var sut = new ObservableDictionary<string, int>
        {
            { "a", 1 },
            { "b", 2 },
        };
        var destination = new KeyValuePair<string, int>[5];

        sut.CopyTo(destination, arrayIndex: 1);

        Assert.Equal(default, destination[0]);
        Assert.Equal(new KeyValuePair<string, int>("a", 1), destination[1]);
        Assert.Equal(new KeyValuePair<string, int>("b", 2), destination[2]);
        Assert.Equal(default, destination[3]);
        Assert.Equal(default, destination[4]);
    }

    [Fact]
    public void CopyTo_throws_for_null_array()
    {
        var sut = new ObservableDictionary<string, int>();

        Assert.Throws<ArgumentNullException>(() =>
            sut.CopyTo(array: null!, arrayIndex: 0));
    }

    [Fact]
    public void CopyTo_throws_for_negative_arrayIndex()
    {
        var sut = new ObservableDictionary<string, int>();
        var destination = new KeyValuePair<string, int>[2];

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            sut.CopyTo(destination, arrayIndex: -1));
    }

    [Fact]
    public void CopyTo_throws_when_destination_is_too_small()
    {
        var sut = new ObservableDictionary<string, int>
        {
            { "a", 1 },
            { "b", 2 },
        };
        var destination = new KeyValuePair<string, int>[1];

        Assert.Throws<ArgumentException>(() =>
            sut.CopyTo(destination, arrayIndex: 0));
    }
}