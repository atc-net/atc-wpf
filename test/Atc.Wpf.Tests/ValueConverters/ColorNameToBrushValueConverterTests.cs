using System.Windows.Data;
using System.Windows.Media;
using Atc.Wpf.ValueConverters;
using Xunit;

namespace Atc.Wpf.Tests.ValueConverters
{
    public class ColorNameToBrushValueConverterTests
    {
        private readonly IValueConverter converter = new ColorNameToBrushValueConverter();

        [Theory]
        [InlineData("#FFFF1493", null)]
        [InlineData("#FFFF0000", "Red")]
        [InlineData("#FF008000", "Green")]
        [InlineData("#FF0000FF", "Blue")]
        public void Convert(string expectedHex, string? input)
        {
            // Act
            var actual = converter.Convert(input, targetType: null, parameter: null, culture: null);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<SolidColorBrush>(actual);
            Assert.Equal(expectedHex, actual.ToString());
        }

        [Theory]
        [InlineData("DeepPink", null)]
        [InlineData("Red", "#FFFF0000")]
        [InlineData("Green", "#FF008000")]
        [InlineData("Blue", "#FF0000FF")]
        [InlineData("#FF0000F0", "#FF0000F0")]
        public void ConvertBack(string expectedHex, string? inputName)
        {
            // Arrange
            SolidColorBrush? input = null;
            if (inputName is not null)
            {
                input = (SolidColorBrush)new BrushConverter().ConvertFrom(inputName);
            }

            // Act
            var actual = converter.ConvertBack(input, targetType: null, parameter: null, culture: null);

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<string>(actual);
            Assert.Equal(expectedHex, actual.ToString());
        }
    }
}