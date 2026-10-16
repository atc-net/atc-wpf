namespace Atc.Wpf.Tests.ValueConverters;

public sealed class ObservableDictionaryToDictionaryOfStringsValueConverterGuidTests
{
    [Fact]
    public void Convert_supports_ObservableDictionary_of_Guid_to_string()
    {
        var first = Guid.NewGuid();
        var second = Guid.NewGuid();
        var input = new ObservableDictionary<Guid, string>
        {
            { first, "alpha" },
            { second, "beta" },
        };

        var actual = ObservableDictionaryToDictionaryOfStringsValueConverter.Instance.Convert(
            input,
            typeof(Dictionary<string, string>),
            parameter: null,
            CultureInfo.InvariantCulture);

        var dictionary = Assert.IsType<Dictionary<string, string>>(actual);
        Assert.Equal(2, dictionary.Count);
        Assert.Equal("alpha", dictionary[first.ToString()]);
        Assert.Equal("beta", dictionary[second.ToString()]);
    }

    [Fact]
    public void Convert_returns_the_same_entries_as_the_Guid_extension_method()
    {
        var input = new ObservableDictionary<Guid, string>
        {
            { Guid.NewGuid(), "alpha" },
            { Guid.NewGuid(), "beta" },
            { Guid.NewGuid(), "gamma" },
        };

        var actual = (Dictionary<string, string>)ObservableDictionaryToDictionaryOfStringsValueConverter.Instance.Convert(
            input,
            typeof(Dictionary<string, string>),
            parameter: null,
            CultureInfo.InvariantCulture)!;

        Assert.Equal(input.ToDictionaryOfStrings(), actual);
    }
}