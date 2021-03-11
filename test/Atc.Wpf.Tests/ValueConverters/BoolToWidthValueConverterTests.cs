using System;
using System.Windows.Data;
using Atc.Wpf.ValueConverters;
using Xunit;

namespace Atc.Wpf.Tests.ValueConverters
{
    public class BoolToWidthValueConverterTests
    {
        private readonly IValueConverter converter = new BoolToWidthValueConverter();

        [Theory]
        [InlineData(0, false, null)]
        [InlineData(double.NaN, true, null)]
        [InlineData(10, true, 10)]
        [InlineData(double.NaN, true, "Auto")]
        public void Convert(double expected, bool input, object parameter)
            => Assert.Equal(
                expected,
                converter.Convert(input, targetType: null, parameter, culture: null));

        [Fact]
        public void ConvertBack_Should_Throw_Exception()
        {
            // Act
            var exception = Record.Exception(() => converter.ConvertBack(value: null, targetType: null, parameter: null, culture: null));

            // Assert
            Assert.IsType<NotSupportedException>(exception);
            Assert.Equal("Specified method is not supported.", exception.Message);
        }
    }
}