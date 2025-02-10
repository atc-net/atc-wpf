namespace Atc.Wpf.Tests.ValueConverters;

public sealed class MultiObjectNullToVisibilityCollapsedValueConverterTests
{
    private readonly IMultiValueConverter converter = new MultiObjectNullToVisibilityCollapsedValueConverter();

    [Theory]
    [InlineData(Visibility.Collapsed, null)]
    [InlineData(Visibility.Collapsed, new object[] { null })]
    [InlineData(Visibility.Visible, new object[] { false })]
    [InlineData(Visibility.Visible, new object[] { true })]
    [InlineData(Visibility.Visible, new object[] { "Hello" })]
    [InlineData(Visibility.Collapsed, new object[] { false, null })]
    [InlineData(Visibility.Collapsed, new object[] { true, null })]
    [InlineData(Visibility.Collapsed, new object[] { "Hello", null })]
    public void Convert(Visibility expected, object[]? input)
        => Assert.Equal(
            expected,
            converter.Convert(input, targetType: null, parameter: null, culture: null));
}