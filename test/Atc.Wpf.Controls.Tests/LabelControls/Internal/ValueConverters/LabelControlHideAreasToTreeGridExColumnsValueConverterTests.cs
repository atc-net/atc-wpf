namespace Atc.Wpf.Controls.Tests.LabelControls.Internal.ValueConverters;

public class LabelControlHideAreasToTreeGridExColumnsValueConverterTests
{
    [Theory]
    [InlineData("10,*,20", LabelControlHideAreasType.None)]
    [InlineData("0,*,20", LabelControlHideAreasType.Asterisk)]
    [InlineData("10,*,0", LabelControlHideAreasType.Information)]
    [InlineData("10,*,20", LabelControlHideAreasType.Validation)]
    [InlineData("0,*,0", LabelControlHideAreasType.AsteriskAndInformation)]
    [InlineData("0,*,20", LabelControlHideAreasType.AsteriskAndValidation)]
    [InlineData("10,*,0", LabelControlHideAreasType.InformationAndValidation)]
    [InlineData("0,*,0", LabelControlHideAreasType.All)]
    public void Convert(string expected, LabelControlHideAreasType hideAreasType)
    {
        // Arrange
        var sut = new LabelControlHideAreasToTreeGridExColumnsValueConverter();

        // Act
        var actual = sut.Convert(hideAreasType, null!, null!, null!);

        // Assert
        Assert.Equal(expected, actual);
    }
}