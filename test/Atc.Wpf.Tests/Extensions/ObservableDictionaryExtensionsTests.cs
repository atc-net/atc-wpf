namespace Atc.Wpf.Tests.Extensions;

public sealed class ObservableDictionaryExtensionsTests
{
    [Fact]
    public void ToDictionaryOfStrings_string_keys_passes_keys_through_unchanged()
    {
        var input = new ObservableDictionary<string, string>
        {
            { "a", "alpha" },
            { "b", "beta" },
        };

        var actual = input.ToDictionaryOfStrings();

        Assert.Equal(2, actual.Count);
        Assert.Equal("alpha", actual["a"]);
        Assert.Equal("beta", actual["b"]);
    }

    [Fact]
    public void ToDictionaryOfStrings_int_keys_uses_invariant_string_form()
    {
        var input = new ObservableDictionary<int, string>
        {
            { 1, "one" },
            { 100, "hundred" },
        };

        var actual = input.ToDictionaryOfStrings();

        Assert.Equal(2, actual.Count);
        Assert.Equal("one", actual["1"]);
        Assert.Equal("hundred", actual["100"]);
    }

    [Fact]
    public void ToDictionaryOfStrings_Guid_keys_uses_default_Guid_string_form()
    {
        var firstKey = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var secondKey = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var input = new ObservableDictionary<Guid, string>
        {
            { firstKey, "first" },
            { secondKey, "second" },
        };

        var actual = input.ToDictionaryOfStrings();

        Assert.Equal(2, actual.Count);
        Assert.Equal("first", actual[firstKey.ToString()]);
        Assert.Equal("second", actual[secondKey.ToString()]);
    }

    [Fact]
    public void ToDictionaryOfStrings_string_keys_throws_for_null_input()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ObservableDictionaryExtensions.ToDictionaryOfStrings(
                (ObservableDictionary<string, string>)null!));
    }

    [Fact]
    public void ToDictionaryOfStrings_int_keys_throws_for_null_input()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ObservableDictionaryExtensions.ToDictionaryOfStrings(
                (ObservableDictionary<int, string>)null!));
    }

    [Fact]
    public void ToDictionaryOfStrings_Guid_keys_throws_for_null_input()
    {
        Assert.Throws<ArgumentNullException>(() =>
            ObservableDictionaryExtensions.ToDictionaryOfStrings(
                (ObservableDictionary<Guid, string>)null!));
    }
}