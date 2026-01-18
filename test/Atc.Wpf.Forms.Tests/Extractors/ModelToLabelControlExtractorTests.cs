namespace Atc.Wpf.Forms.Tests.Extractors;

[Collection("TestCollection")]
[Trait(Traits.Category, Traits.Categories.Integration)]
[Trait(Traits.Category, Traits.Categories.SkipWhenLiveUnitTesting)]
public sealed class ModelToLabelControlExtractorTests
{
    [StaFact]
    public void GetLabelControls_PrimitiveTypesModel()
    {
        // Arrange
        var model = new PrimitiveTypesModel();

        // Act
        var actual = ModelToLabelControlExtractor.Extract(
            model,
            includeReadOnly: true);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(40, actual.Count);
    }

    [StaFact]
    public void GetLabelControls_Address()
    {
        // Arrange
        var model = new Address();

        // Act
        var labelControls = ModelToLabelControlExtractor.Extract(
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
        var actual = ModelToLabelControlExtractor.Extract(
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
        var actual = ModelToLabelControlExtractor.Extract(
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
        var actual = ModelToLabelControlExtractor.Extract(
            model,
            includeReadOnly: true);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(7, actual.Count);
    }

    [StaFact]
    public void GetLabelControls_Account()
    {
        // Arrange
        var model = new Account();

        // Act
        var actual = ModelToLabelControlExtractor.Extract(
            model,
            includeReadOnly: true);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(11, actual.Count);
    }

    [StaFact]
    public void GetLabelControls_DriveItem()
    {
        // Arrange
        var model = new DriveItem();

        // Act
        var actual = ModelToLabelControlExtractor.Extract(
            model,
            includeReadOnly: true);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(3, actual.Count);
    }
}