// ReSharper disable StringLiteralTypo
namespace Atc.Wpf.Forms.Tests.Internal.Helpers;

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
    [InlineData(TextBoxValidationRuleType.PasswordWeak, true, "", "abc123")]
    [InlineData(TextBoxValidationRuleType.PasswordWeak, true, "", "password")]
    [InlineData(TextBoxValidationRuleType.PasswordWeak, false, "Password is too short", "abc")]
    [InlineData(TextBoxValidationRuleType.PasswordWeak, false, "Password is too short", "12345")]
    [InlineData(TextBoxValidationRuleType.PasswordWeak, false, "Password is too short", "")]
    [InlineData(TextBoxValidationRuleType.PasswordMedium, true, "", "Password")]
    [InlineData(TextBoxValidationRuleType.PasswordMedium, true, "", "MySecret1")]
    [InlineData(TextBoxValidationRuleType.PasswordMedium, false, "Password is too short", "Pass")]
    [InlineData(TextBoxValidationRuleType.PasswordMedium, false, "Password must contain upper and lower case letters", "password")]
    [InlineData(TextBoxValidationRuleType.PasswordMedium, false, "Password must contain upper and lower case letters", "PASSWORD")]
    [InlineData(TextBoxValidationRuleType.PasswordMedium, false, "Password must contain upper and lower case letters", "12345678")]
    [InlineData(TextBoxValidationRuleType.PasswordMedium, false, "Password is too short", "")]
    [InlineData(TextBoxValidationRuleType.PasswordStrong, true, "", "MyPassword1!")]
    [InlineData(TextBoxValidationRuleType.PasswordStrong, true, "", "Str0ng@Pass!2")]
    [InlineData(TextBoxValidationRuleType.PasswordStrong, true, "", "C0mplex#Pass!")]
    [InlineData(TextBoxValidationRuleType.PasswordStrong, false, "Password is too short", "Short1!")]
    [InlineData(TextBoxValidationRuleType.PasswordStrong, false, "Password must contain upper and lower case letters", "mypassword1!")]
    [InlineData(TextBoxValidationRuleType.PasswordStrong, false, "Password must contain upper and lower case letters", "MYPASSWORD1!")]
    [InlineData(TextBoxValidationRuleType.PasswordStrong, false, "Password must contain at least one digit", "MyPasswordOnly!")]
    [InlineData(TextBoxValidationRuleType.PasswordStrong, false, "Password must contain at least one special character", "MyPassword123")]
    [InlineData(TextBoxValidationRuleType.PasswordStrong, false, "Password is too short", "")]
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