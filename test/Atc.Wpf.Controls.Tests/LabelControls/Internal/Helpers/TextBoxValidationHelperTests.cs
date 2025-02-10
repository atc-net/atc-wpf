// ReSharper disable StringLiteralTypo
namespace Atc.Wpf.Controls.Tests.LabelControls.Internal.Helpers;

public sealed class TextBoxValidationHelperTests
{
    [Theory]
    [InlineData(TextBoxValidationRuleType.Email, true, "", "john.doe@example.com")]
    [InlineData(TextBoxValidationRuleType.Email, false, "Invalid email address", "johndoe")]
    [InlineData(TextBoxValidationRuleType.Http, true, "", "http://example.com")]
    [InlineData(TextBoxValidationRuleType.Http, false, "Invalid HTTP url", "https://example.com")]
    [InlineData(TextBoxValidationRuleType.Https, true, "", "https://example.com")]
    [InlineData(TextBoxValidationRuleType.Https, false, "Invalid HTTPS url", "http://example.com")]
    [InlineData(TextBoxValidationRuleType.HexRGB, true, "", "#00FF00")]
    [InlineData(TextBoxValidationRuleType.HexRGB, false, "Invalid Hex RGB format", "test")]
    [InlineData(TextBoxValidationRuleType.HexARGB, true, "", "#FF00FF00")]
    [InlineData(TextBoxValidationRuleType.HexARGB, false, "Invalid Hex ARGB format", "test")]
    public void Validate(
        TextBoxValidationRuleType ruleType,
        bool expectedIsValid,
        string expectedErrorMessage,
        string input)
    {
        var result = TextBoxValidationHelper.Validate(ruleType, input);

        Assert.Equal(expectedIsValid, result.IsValid);
        Assert.Equal(expectedErrorMessage, result.ErrorMessage);
    }
}