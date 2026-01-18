namespace Atc.Wpf.Forms.Tests.Internal.ValueConverters;

public sealed class LabelControlHideAreasToShowToolTipValueConverterTests
{
    [Theory]
    [InlineData(false, LabelControlHideAreasType.None)]
    [InlineData(false, LabelControlHideAreasType.Asterisk)]
    [InlineData(false, LabelControlHideAreasType.Information)]
    [InlineData(true, LabelControlHideAreasType.Validation)]
    [InlineData(false, LabelControlHideAreasType.AsteriskAndInformation)]
    [InlineData(true, LabelControlHideAreasType.AsteriskAndValidation)]
    [InlineData(true, LabelControlHideAreasType.InformationAndValidation)]
    [InlineData(true, LabelControlHideAreasType.All)]
    public void Convert(
        bool expected,
        LabelControlHideAreasType hideAreasType)
    {
        // Arrange
        var sut = new LabelControlHideAreasToShowToolTipValueConverter();

        // Act
        var actual = sut.Convert(
            hideAreasType,
            null!,
            null!,
            null!);

        // Assert
        Assert.Equal(expected, actual);
    }
}