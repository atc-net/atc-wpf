namespace Atc.Wpf.Tests.ValueConverters;

public sealed class MethodToValueConverterTests
{
    private sealed class Sample
    {
        public int CallCount { get; private set; }

        public string Greet() => "hello";

        public int Tick()
        {
            CallCount++;
            return CallCount;
        }
    }

    [Fact]
    public void Convert_invokes_named_parameterless_method_and_returns_its_result()
    {
        var sample = new Sample();

        var actual = MethodToValueConverter.Instance.Convert(
            sample,
            typeof(string),
            "Greet",
            CultureInfo.InvariantCulture);

        Assert.Equal("hello", actual);
    }

    [Fact]
    public void Convert_returns_null_when_value_is_null()
    {
        var actual = MethodToValueConverter.Instance.Convert(
            value: null,
            typeof(string),
            "Greet",
            CultureInfo.InvariantCulture);

        Assert.Null(actual);
    }

    [Fact]
    public void Convert_returns_null_when_parameter_is_not_a_string()
    {
        var actual = MethodToValueConverter.Instance.Convert(
            new Sample(),
            typeof(string),
            parameter: 42,
            CultureInfo.InvariantCulture);

        Assert.Null(actual);
    }

    [Fact]
    public void Convert_returns_null_when_method_does_not_exist()
    {
        var actual = MethodToValueConverter.Instance.Convert(
            new Sample(),
            typeof(string),
            "DoesNotExist",
            CultureInfo.InvariantCulture);

        Assert.Null(actual);
    }

    [Fact]
    public void Convert_invokes_each_call_against_the_provided_instance()
    {
        var sample = new Sample();

        MethodToValueConverter.Instance.Convert(sample, typeof(int), "Tick", CultureInfo.InvariantCulture);
        MethodToValueConverter.Instance.Convert(sample, typeof(int), "Tick", CultureInfo.InvariantCulture);
        var third = MethodToValueConverter.Instance.Convert(sample, typeof(int), "Tick", CultureInfo.InvariantCulture);

        Assert.Equal(3, third);
        Assert.Equal(3, sample.CallCount);
    }

    [Fact]
    public void ConvertBack_throws_NotSupportedException()
    {
        Assert.Throws<NotSupportedException>(() =>
            MethodToValueConverter.Instance.ConvertBack(
                "anything",
                typeof(object),
                parameter: null,
                CultureInfo.InvariantCulture));
    }
}