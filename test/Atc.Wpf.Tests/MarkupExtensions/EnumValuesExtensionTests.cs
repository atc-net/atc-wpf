// ReSharper disable once CheckNamespace
namespace Atc.Wpf.Tests.MarkupExtensions;

public sealed class EnumValuesExtensionTests
{
    [Fact]
    public void ProvideValue_WithSinglePositionalValue_ReturnsSingleItemArray()
    {
        var extension = new EnumValuesExtension(DayOfWeek.Monday);

        var result = (Enum[])extension.ProvideValue(serviceProvider: null!);

        Assert.Equal(new Enum[] { DayOfWeek.Monday }, result);
    }

    [Fact]
    public void ProvideValue_WithThreePositionalValues_ReturnsThreeItemArray()
    {
        var extension = new EnumValuesExtension(
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday);

        var result = (Enum[])extension.ProvideValue(serviceProvider: null!);

        Assert.Equal(
            new Enum[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday },
            result);
    }

    [Fact]
    public void ProvideValue_WithTypeAndValues_ParsesCommaSeparatedNames()
    {
        var extension = new EnumValuesExtension
        {
            Type = typeof(DayOfWeek),
            Values = "Monday,Tuesday",
        };

        var result = (Enum[])extension.ProvideValue(serviceProvider: null!);

        Assert.Equal(
            new Enum[] { DayOfWeek.Monday, DayOfWeek.Tuesday },
            result);
    }

    [Fact]
    public void ProvideValue_WithTypeAndValues_TrimsWhitespace()
    {
        var extension = new EnumValuesExtension
        {
            Type = typeof(DayOfWeek),
            Values = " Monday , Tuesday ",
        };

        var result = (Enum[])extension.ProvideValue(serviceProvider: null!);

        Assert.Equal(
            new Enum[] { DayOfWeek.Monday, DayOfWeek.Tuesday },
            result);
    }

    [Fact]
    public void ProvideValue_WithTypeAndValues_IsCaseInsensitive()
    {
        var extension = new EnumValuesExtension
        {
            Type = typeof(DayOfWeek),
            Values = "monday,TUESDAY",
        };

        var result = (Enum[])extension.ProvideValue(serviceProvider: null!);

        Assert.Equal(
            new Enum[] { DayOfWeek.Monday, DayOfWeek.Tuesday },
            result);
    }

    [Fact]
    public void Type_SetToNonEnum_Throws()
        => Assert.Throws<UnexpectedTypeException>(() =>
            new EnumValuesExtension { Type = typeof(string) });

    [Fact]
    public void ProvideValue_NoPositionalAndNoTypeOrValues_Throws()
    {
        var extension = new EnumValuesExtension();

        Assert.Throws<InvalidOperationException>(() =>
            extension.ProvideValue(serviceProvider: null!));
    }

    [Fact]
    public void ProvideValue_TypeSetButValuesMissing_Throws()
    {
        var extension = new EnumValuesExtension
        {
            Type = typeof(DayOfWeek),
        };

        Assert.Throws<InvalidOperationException>(() =>
            extension.ProvideValue(serviceProvider: null!));
    }
}