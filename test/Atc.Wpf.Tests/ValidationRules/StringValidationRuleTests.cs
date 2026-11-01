namespace Atc.Wpf.Tests.ValidationRules;

public sealed class StringValidationRuleTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_RequiredAndEmptyOrNull_IsInvalidWithAMessage(
        string? value)
    {
        // Arrange
        var sut = new StringValidationRule
        {
            Required = true,
            MinLength = 0,
        };

        // Act
        var result = sut.Validate(value, CultureInfo.InvariantCulture);

        // Assert
        Assert.False(result.IsValid);
        Assert.False(string.IsNullOrEmpty(result.ErrorContent as string));
    }

    [Fact]
    public void Validate_NotRequiredWithoutMinLengthAndNull_IsValid()
    {
        // Arrange
        var sut = new StringValidationRule { MinLength = 0 };

        // Act
        var result = sut.Validate(value: null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Same(ValidationResult.ValidResult, result);
    }

    [Theory]
    [InlineData("ab", false)]
    [InlineData("abc", true)]
    [InlineData("abcde", true)]
    [InlineData("abcdef", false)]
    public void Validate_LengthBounds(
        string value,
        bool expectedValid)
    {
        // Arrange
        var sut = new StringValidationRule
        {
            MinLength = 3,
            MaxLength = 5,
        };

        // Act
        var result = sut.Validate(value, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(expectedValid, result.IsValid);
    }

    [Theory]
    [InlineData("report.txt", true)]
    [InlineData("re:port", false)]
    [InlineData("a*b", false)]
    public void Validate_InvalidCharacters_FromAString(
        string value,
        bool expectedValid)
    {
        // Arrange
        var sut = new StringValidationRule { InvalidCharacters = ":*?" };

        // Act
        var result = sut.Validate(value, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(expectedValid, result.IsValid);
        Assert.Equal(":*?", sut.InvalidCharacters);
    }

    [Fact]
    public void InvalidPrefixStrings_SplitsCommaSeparatedValues_TrimmingAndDroppingEmptyOnes()
    {
        // Arrange
        var sut = new StringValidationRule();

        // Act
        sut.InvalidPrefixStrings = " tmp_ , ,test_,";

        // Assert
        Assert.Equal(["tmp_", "test_"], sut.Constraints.InvalidPrefixStrings, StringComparer.Ordinal);
        Assert.Equal("tmp_,test_", sut.InvalidPrefixStrings);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("   ")]
    public void InvalidPrefixStrings_BlankOrNull_ClearsThePrefixes(string? csv)
    {
        // Arrange
        var sut = new StringValidationRule { InvalidPrefixStrings = "tmp_" };

        // Act
        sut.InvalidPrefixStrings = csv!;

        // Assert
        Assert.Empty(sut.Constraints.InvalidPrefixStrings);
    }

    [Theory]
    [InlineData("tmp_file", false)]
    [InlineData("test_file", false)]
    [InlineData("file_tmp_", true)]
    public void Validate_InvalidPrefixStrings(
        string value,
        bool expectedValid)
    {
        // Arrange
        var sut = new StringValidationRule { InvalidPrefixStrings = "tmp_,test_" };

        // Act
        var result = sut.Validate(value, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(expectedValid, result.IsValid);
    }

    [Theory]
    [InlineData("AB-123", true)]
    [InlineData("ab-123", false)]
    public void Validate_RegularExpression(
        string value,
        bool expectedValid)
    {
        // Arrange
        var sut = new StringValidationRule { RegularExpression = "^[A-Z]{2}-[0-9]{3}$" };

        // Act
        var result = sut.Validate(value, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(expectedValid, result.IsValid);
    }

    [Fact]
    public void Validate_NonStringValue_IsValidatedThroughToString()
    {
        // Arrange
        var sut = new StringValidationRule { MaxLength = 3 };

        // Act
        var result = sut.Validate(12345, CultureInfo.InvariantCulture);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public void Properties_WriteThroughToTheConstraints()
    {
        // Arrange
        var sut = new StringValidationRule();

        // Act
        sut.Required = true;
        sut.MinLength = 2;
        sut.MaxLength = 9;
        sut.RegularExpression = "^x";

        // Assert
        Assert.True(sut.Constraints.Required);
        Assert.Equal(2u, sut.Constraints.MinLength);
        Assert.Equal(9u, sut.Constraints.MaxLength);
        Assert.Equal("^x", sut.Constraints.RegularExpression);
    }
}