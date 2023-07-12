namespace Atc.Wpf.Controls.Tests.LabelControls.Helpers;

public class ModelToLabelControlHelperTests
{
    [StaFact]
    public void GetLabelControls_PrimitiveTypesModel()
    {
        // Arrange
        var model = new PrimitiveTypesModel();

        // Act
        var actual = ModelToLabelControlHelper.GetLabelControls(
            model,
            includeReadOnly: true);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(13, actual.Count);
    }

    [StaFact]
    public void GetLabelControls_Address()
    {
        // Arrange
        var model = new Address();

        // Act
        var labelControls = ModelToLabelControlHelper.GetLabelControls(
            model,
            includeReadOnly: true);

        // Assert
        Assert.NotNull(labelControls);
        Assert.Equal(4, labelControls.Count);
    }

    [StaTheory]
    [InlineData(true, "", "My city name")]
    [InlineData(false, "Field is required", "")]
    [InlineData(false, "Min. length: 2", "M")]
    [InlineData(false, "Max. length: 16", "My city name - too long")]
    public void GetLabelControls_Address_Validate_CityName(
        bool expectedIsValid,
        string expectedValidationText,
        string? cityName)
    {
        // Arrange
        var model = new Address
        {
            CityName = cityName,
        };

        // Act
        var actual = ModelToLabelControlHelper.GetLabelControls(
            model,
            includeReadOnly: true);

        // Assert
        Assert.NotNull(actual);
        var controlPostalCode = actual.Single(x => x.Identifier == "CityName");
        Assert.Equal(expectedIsValid, controlPostalCode.IsValid());
        Assert.Equal(expectedValidationText, ((ILabelControl)controlPostalCode).ValidationText);
    }

    [StaTheory]
    [InlineData(true, "", "1234")]
    [InlineData(false, "Field is required", "")]
    [InlineData(false, "Regular expression don't match", "12345")]
    public void GetLabelControls_Address_Validate_PostalCode(
        bool expectedIsValid,
        string expectedValidationText,
        string? postalCode)
    {
        // Arrange
        var model = new Address
        {
            PostalCode = postalCode,
        };

        // Act
        var actual = ModelToLabelControlHelper.GetLabelControls(
            model,
            includeReadOnly: true);

        // Assert
        Assert.NotNull(actual);
        var controlPostalCode = actual.Single(x => x.Identifier == "PostalCode");
        Assert.Equal(expectedIsValid, controlPostalCode.IsValid());
        Assert.Equal(expectedValidationText, ((ILabelControl)controlPostalCode).ValidationText);
    }

    [StaFact]
    public void GetLabelControls_Person()
    {
        // Arrange
        var model = new Person();

        // Act
        var actual = ModelToLabelControlHelper.GetLabelControls(
            model,
            includeReadOnly: true);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(3, actual.Count);
    }
}