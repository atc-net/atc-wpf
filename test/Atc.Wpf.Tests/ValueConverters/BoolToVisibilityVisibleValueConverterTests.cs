using System.Windows;
using System.Windows.Data;
using Atc.Wpf.ValueConverters;
using Xunit;

namespace Atc.Wpf.Tests.ValueConverters
{
    public class BoolToVisibilityVisibleValueConverterTests
    {
        private readonly IValueConverter converter = new BoolToVisibilityVisibleValueConverter();

        [Theory]
        [InlineData(Visibility.Visible, true)]
        [InlineData(Visibility.Collapsed, false)]
        public void Convert(Visibility expected, bool input)
            => Assert.Equal(
                expected,
                converter.Convert(input, targetType: null, parameter: null, culture: null));

        [Theory]
        [InlineData(Visibility.Visible, true)]
        [InlineData(Visibility.Collapsed, false)]
        public void ConvertBack(Visibility expected, bool input)
            => Assert.Equal(
                expected,
                converter.ConvertBack(input, targetType: null, parameter: null, culture: null));
    }
}
