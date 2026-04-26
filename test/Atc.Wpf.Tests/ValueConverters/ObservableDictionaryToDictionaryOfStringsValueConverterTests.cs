namespace Atc.Wpf.Tests.ValueConverters;

public sealed class ObservableDictionaryToDictionaryOfStringsValueConverterTests
{
    [Fact]
    public void Convert_returns_empty_dictionary_for_null_input()
    {
        var actual = ObservableDictionaryToDictionaryOfStringsValueConverter.Instance.Convert(
            value: null,
            typeof(Dictionary<string, string>),
            parameter: null,
            CultureInfo.InvariantCulture);

        var dictionary = Assert.IsType<Dictionary<string, string>>(actual);
        Assert.Empty(dictionary);
    }

    [Fact]
    public void Convert_throws_for_unsupported_dictionary_type()
    {
        var unsupported = new ObservableDictionary<double, string>
        {
            { 1.5, "value" },
        };

        Assert.ThrowsAny<Exception>(() =>
            ObservableDictionaryToDictionaryOfStringsValueConverter.Instance.Convert(
                unsupported,
                typeof(Dictionary<string, string>),
                parameter: null,
                CultureInfo.InvariantCulture));
    }

    [Fact]
    public void Convert_supports_ObservableDictionary_of_string_to_string()
    {
        var input = new ObservableDictionary<string, string>
        {
            { "a", "alpha" },
            { "b", "beta" },
        };

        var actual = ObservableDictionaryToDictionaryOfStringsValueConverter.Instance.Convert(
            input,
            typeof(Dictionary<string, string>),
            parameter: null,
            CultureInfo.InvariantCulture);

        var dictionary = Assert.IsType<Dictionary<string, string>>(actual);
        Assert.Equal(2, dictionary.Count);
        Assert.Equal("alpha", dictionary["a"]);
        Assert.Equal("beta", dictionary["b"]);
    }

    [Fact]
    public void Convert_supports_ObservableDictionary_of_int_to_string()
    {
        var input = new ObservableDictionary<int, string>
        {
            { 1, "one" },
            { 2, "two" },
        };

        var actual = ObservableDictionaryToDictionaryOfStringsValueConverter.Instance.Convert(
            input,
            typeof(Dictionary<string, string>),
            parameter: null,
            CultureInfo.InvariantCulture);

        var dictionary = Assert.IsType<Dictionary<string, string>>(actual);
        Assert.Equal(2, dictionary.Count);
        Assert.Equal("one", dictionary["1"]);
        Assert.Equal("two", dictionary["2"]);
    }

    [Fact]
    public void ConvertBack_throws_NotSupportedException()
    {
        Assert.Throws<NotSupportedException>(() =>
            ObservableDictionaryToDictionaryOfStringsValueConverter.Instance.ConvertBack(
                new Dictionary<string, string>(StringComparer.Ordinal),
                typeof(ObservableDictionary<string, string>),
                parameter: null,
                CultureInfo.InvariantCulture));
    }
}