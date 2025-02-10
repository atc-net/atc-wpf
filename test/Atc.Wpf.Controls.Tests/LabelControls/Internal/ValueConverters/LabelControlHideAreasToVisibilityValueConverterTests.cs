namespace Atc.Wpf.Controls.Tests.LabelControls.Internal.ValueConverters;

public sealed class LabelControlHideAreasToVisibilityValueConverterTests
{
    [Theory]
    [InlineData(Visibility.Visible, LabelControlHideAreasType.None, LabelControlHideAreasType.None)]
    [InlineData(Visibility.Visible, LabelControlHideAreasType.Asterisk, LabelControlHideAreasType.None)]
    [InlineData(Visibility.Visible, LabelControlHideAreasType.Information, LabelControlHideAreasType.None)]
    [InlineData(Visibility.Visible, LabelControlHideAreasType.Validation, LabelControlHideAreasType.None)]
    [InlineData(Visibility.Visible, LabelControlHideAreasType.AsteriskAndInformation, LabelControlHideAreasType.None)]
    [InlineData(Visibility.Visible, LabelControlHideAreasType.AsteriskAndValidation, LabelControlHideAreasType.None)]
    [InlineData(Visibility.Visible, LabelControlHideAreasType.InformationAndValidation, LabelControlHideAreasType.None)]
    [InlineData(Visibility.Visible, LabelControlHideAreasType.All, LabelControlHideAreasType.None)]

    [InlineData(Visibility.Visible, LabelControlHideAreasType.None, LabelControlHideAreasType.Asterisk)]
    [InlineData(Visibility.Collapsed, LabelControlHideAreasType.Asterisk, LabelControlHideAreasType.Asterisk)]
    [InlineData(Visibility.Visible, LabelControlHideAreasType.Information, LabelControlHideAreasType.Asterisk)]
    [InlineData(Visibility.Visible, LabelControlHideAreasType.Validation, LabelControlHideAreasType.Asterisk)]
    [InlineData(Visibility.Collapsed, LabelControlHideAreasType.AsteriskAndInformation, LabelControlHideAreasType.Asterisk)]
    [InlineData(Visibility.Collapsed, LabelControlHideAreasType.AsteriskAndValidation, LabelControlHideAreasType.Asterisk)]
    [InlineData(Visibility.Visible, LabelControlHideAreasType.InformationAndValidation, LabelControlHideAreasType.Asterisk)]
    [InlineData(Visibility.Collapsed, LabelControlHideAreasType.All, LabelControlHideAreasType.Asterisk)]

    [InlineData(Visibility.Visible, LabelControlHideAreasType.None, LabelControlHideAreasType.Information)]
    [InlineData(Visibility.Visible, LabelControlHideAreasType.Asterisk, LabelControlHideAreasType.Information)]
    [InlineData(Visibility.Collapsed, LabelControlHideAreasType.Information, LabelControlHideAreasType.Information)]
    [InlineData(Visibility.Visible, LabelControlHideAreasType.Validation, LabelControlHideAreasType.Information)]
    [InlineData(Visibility.Collapsed, LabelControlHideAreasType.AsteriskAndInformation, LabelControlHideAreasType.Information)]
    [InlineData(Visibility.Visible, LabelControlHideAreasType.AsteriskAndValidation, LabelControlHideAreasType.Information)]
    [InlineData(Visibility.Collapsed, LabelControlHideAreasType.InformationAndValidation, LabelControlHideAreasType.Information)]
    [InlineData(Visibility.Collapsed, LabelControlHideAreasType.All, LabelControlHideAreasType.Information)]

    [InlineData(Visibility.Visible, LabelControlHideAreasType.None, LabelControlHideAreasType.Validation)]
    [InlineData(Visibility.Visible, LabelControlHideAreasType.Asterisk, LabelControlHideAreasType.Validation)]
    [InlineData(Visibility.Visible, LabelControlHideAreasType.Information, LabelControlHideAreasType.Validation)]
    [InlineData(Visibility.Collapsed, LabelControlHideAreasType.Validation, LabelControlHideAreasType.Validation)]
    [InlineData(Visibility.Visible, LabelControlHideAreasType.AsteriskAndInformation, LabelControlHideAreasType.Validation)]
    [InlineData(Visibility.Collapsed, LabelControlHideAreasType.AsteriskAndValidation, LabelControlHideAreasType.Validation)]
    [InlineData(Visibility.Collapsed, LabelControlHideAreasType.InformationAndValidation, LabelControlHideAreasType.Validation)]
    [InlineData(Visibility.Collapsed, LabelControlHideAreasType.All, LabelControlHideAreasType.Validation)]
    public void Convert(Visibility expected, LabelControlHideAreasType hideAreasType, LabelControlHideAreasType requiredHideAreasType)
    {
        // Arrange
        var sut = new LabelControlHideAreasToVisibilityValueConverter();

        // Act
        var actual = sut.Convert(hideAreasType, null!, requiredHideAreasType, null!);

        // Assert
        Assert.Equal(expected, actual);
    }
}