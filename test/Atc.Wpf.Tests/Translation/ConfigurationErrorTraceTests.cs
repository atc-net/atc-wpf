namespace Atc.Wpf.Tests.Translation;

// Missing resources, invalid cultures and failing converters used to be silent in Release; they are traced once each.
[Collection(nameof(TraceTestGroup))]
public sealed class ConfigurationErrorTraceTests
{
    [StaFact]
    public void ResxExtension_with_a_missing_key_traces_it_once()
    {
        using var trace = TraceCapture.Start();
        var key = "MissingKey" + Guid.NewGuid().ToString("N");

        Provide(key);
        Provide(key);

        Assert.Equal(1, trace.Count(key));
    }

    [Fact]
    public void SetCultures_with_an_invalid_name_traces_the_fallback()
    {
        var original = CultureManager.UiCulture;
        using var trace = TraceCapture.Start();
        try
        {
            CultureManager.SetCultures("not-a-culture!", synchronizeThreadCultures: false);

            Assert.Equal(1, trace.Count("not-a-culture!"));
        }
        finally
        {
            CultureManager.UiCulture = original;
        }
    }

    [Fact]
    public void A_failing_converter_returns_unset_and_traces_once()
    {
        using var trace = TraceCapture.Start();
        IValueConverter sut = new ThrowingConverter();

        var first = sut.Convert(1, typeof(string), parameter: null, CultureInfo.InvariantCulture);
        var second = sut.Convert(2, typeof(string), parameter: null, CultureInfo.InvariantCulture);

        Assert.Same(DependencyProperty.UnsetValue, first);
        Assert.Same(DependencyProperty.UnsetValue, second);
        Assert.Equal(1, trace.Count(nameof(ThrowingConverter)));
    }

    private static void Provide(string key)
    {
        var textBlock = new TextBlock();
        var sut = new ResxExtension("Atc.Wpf.Resources.Miscellaneous", key);
        try
        {
            var target = Substitute.For<IProvideValueTarget>();
            target.TargetObject.Returns(textBlock);
            target.TargetProperty.Returns(TextBlock.TextProperty);
            var services = Substitute.For<IServiceProvider>();
            services.GetService(typeof(IProvideValueTarget)).Returns(target);

            sut.ProvideValue(services);
        }
        finally
        {
            ResxExtension.MarkupManager.ActiveExtensions.Remove(sut);
        }
    }

    private sealed class ThrowingConverter : MarkupValueConverterBase
    {
        protected override object? Convert(
            object? value,
            Type targetType,
            object? parameter,
            CultureInfo culture)
            => throw new InvalidOperationException("broken converter");

        protected override object? ConvertBack(
            object? value,
            Type targetType,
            object? parameter,
            CultureInfo culture)
            => throw new InvalidOperationException("broken converter");
    }
}