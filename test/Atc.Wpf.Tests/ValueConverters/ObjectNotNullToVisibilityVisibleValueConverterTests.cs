using System.Windows;
using System.Windows.Data;
using Atc.Wpf.ValueConverters;
using Xunit;

namespace Atc.Wpf.Tests.ValueConverters
{
    public class ObjectNotNullToVisibilityVisibleValueConverterTests
    {
        private readonly IValueConverter converter = new ObjectNotNullToVisibilityVisibleValueConverter();

        [Theory]
        [InlineData(Visibility.Collapsed, null)]
        [InlineData(Visibility.Visible, true)]
        [InlineData(Visibility.Visible, "Hallo")]
        public void Convert(Visibility expected, object input)
            => Assert.Equal(
                expected,
                converter.Convert(input, targetType: null, parameter: null, culture: null));

        [Theory]
        [InlineData(Visibility.Visible, null)]
        [InlineData(Visibility.Collapsed, true)]
        [InlineData(Visibility.Collapsed, "Hallo")]
        public void ConvertBack(Visibility expected, object input)
            => Assert.Equal(
                expected,
                converter.ConvertBack(input, targetType: null, parameter: null, culture: null));
    }
}