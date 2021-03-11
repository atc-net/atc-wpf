using System.Windows.Data;
using System.Windows.Media;
using Atc.Wpf.ValueConverters;
using Xunit;

// ReSharper disable PossibleNullReferenceException
namespace Atc.Wpf.Tests.ValueConverters
{
    public class ColorToBrushValueConverterTests
    {
        private readonly IValueConverter converter = new ColorToBrushValueConverter();

        [Theory]
        [InlineData("#FFFF1493", null)]
        [InlineData("#FFFF1493", "#FFFF1493")]
        [InlineData("#FF000000", "#FF000000")]
        public void Convert(string expectedHex, string? inputHex)
        {
            // Arrange
            Color? input = null;
            if (inputHex != null)
            {
                input = (Color)ColorConverter.ConvertFromString(inputHex);
            }

            // Act
            var actual = converter.Convert(input, targetType: null, parameter: null, culture: null);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<SolidColorBrush>(actual);
            Assert.Equal(expectedHex, actual.ToString());
        }

        [Theory]
        [InlineData("#FFFF1493", null)]
        [InlineData("#FFFF1493", "#FFFF1493")]
        [InlineData("#FF000000", "#FF000000")]
        public void ConvertBack(string expectedHex, string? inputHex)
        {
            // Arrange
            SolidColorBrush? input = null;
            if (inputHex != null)
            {
                input = (SolidColorBrush)new BrushConverter().ConvertFrom(inputHex);
            }

            // Act
            var actual = converter.ConvertBack(input, targetType: null, parameter: null, culture: null);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<Color>(actual);
            Assert.Equal(expectedHex, actual.ToString());
        }
    }
}