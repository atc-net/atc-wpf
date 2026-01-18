namespace Atc.Wpf.Forms.Tests.Internal.ValueConverters;

public sealed class LabelControlHideAreasToBoolValueConverterTests
{
    [Theory]
    [InlineData(true, LabelControlHideAreasType.None, LabelControlHideAreasType.None)]
    [InlineData(true, LabelControlHideAreasType.Asterisk, LabelControlHideAreasType.None)]
    [InlineData(true, LabelControlHideAreasType.Information, LabelControlHideAreasType.None)]
    [InlineData(true, LabelControlHideAreasType.Validation, LabelControlHideAreasType.None)]
    [InlineData(true, LabelControlHideAreasType.AsteriskAndInformation, LabelControlHideAreasType.None)]
    [InlineData(true, LabelControlHideAreasType.AsteriskAndValidation, LabelControlHideAreasType.None)]
    [InlineData(true, LabelControlHideAreasType.InformationAndValidation, LabelControlHideAreasType.None)]
    [InlineData(true, LabelControlHideAreasType.All, LabelControlHideAreasType.None)]

    [InlineData(true, LabelControlHideAreasType.None, LabelControlHideAreasType.Asterisk)]
    [InlineData(false, LabelControlHideAreasType.Asterisk, LabelControlHideAreasType.Asterisk)]
    [InlineData(true, LabelControlHideAreasType.Information, LabelControlHideAreasType.Asterisk)]
    [InlineData(true, LabelControlHideAreasType.Validation, LabelControlHideAreasType.Asterisk)]
    [InlineData(false, LabelControlHideAreasType.AsteriskAndInformation, LabelControlHideAreasType.Asterisk)]
    [InlineData(false, LabelControlHideAreasType.AsteriskAndValidation, LabelControlHideAreasType.Asterisk)]
    [InlineData(true, LabelControlHideAreasType.InformationAndValidation, LabelControlHideAreasType.Asterisk)]
    [InlineData(false, LabelControlHideAreasType.All, LabelControlHideAreasType.Asterisk)]

    [InlineData(true, LabelControlHideAreasType.None, LabelControlHideAreasType.Information)]
    [InlineData(true, LabelControlHideAreasType.Asterisk, LabelControlHideAreasType.Information)]
    [InlineData(false, LabelControlHideAreasType.Information, LabelControlHideAreasType.Information)]
    [InlineData(true, LabelControlHideAreasType.Validation, LabelControlHideAreasType.Information)]
    [InlineData(false, LabelControlHideAreasType.AsteriskAndInformation, LabelControlHideAreasType.Information)]
    [InlineData(true, LabelControlHideAreasType.AsteriskAndValidation, LabelControlHideAreasType.Information)]
    [InlineData(false, LabelControlHideAreasType.InformationAndValidation, LabelControlHideAreasType.Information)]
    [InlineData(false, LabelControlHideAreasType.All, LabelControlHideAreasType.Information)]

    [InlineData(true, LabelControlHideAreasType.None, LabelControlHideAreasType.Validation)]
    [InlineData(true, LabelControlHideAreasType.Asterisk, LabelControlHideAreasType.Validation)]
    [InlineData(true, LabelControlHideAreasType.Information, LabelControlHideAreasType.Validation)]
    [InlineData(false, LabelControlHideAreasType.Validation, LabelControlHideAreasType.Validation)]
    [InlineData(true, LabelControlHideAreasType.AsteriskAndInformation, LabelControlHideAreasType.Validation)]
    [InlineData(false, LabelControlHideAreasType.AsteriskAndValidation, LabelControlHideAreasType.Validation)]
    [InlineData(false, LabelControlHideAreasType.InformationAndValidation, LabelControlHideAreasType.Validation)]
    [InlineData(false, LabelControlHideAreasType.All, LabelControlHideAreasType.Validation)]
    public void Convert(
        bool expected,
        LabelControlHideAreasType hideAreasType,
        LabelControlHideAreasType requiredHideAreasType)
    {
        // Arrange
        var sut = new LabelControlHideAreasToBoolValueConverter();

        // Act
        var actual = sut.Convert(
            hideAreasType,
            null!,
            requiredHideAreasType,
            null!);

        // Assert
        Assert.Equal(expected, actual);
    }
}