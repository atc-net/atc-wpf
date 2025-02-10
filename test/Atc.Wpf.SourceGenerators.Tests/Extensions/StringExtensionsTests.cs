namespace Atc.Wpf.SourceGenerators.Tests.Extensions;

public sealed class StringExtensionsTests
{
    [Theory]
    [InlineData("Attribute(Param1, Param2, Param3)", new[] { "Param1", "Param2", "Param3" })]
    [InlineData("Attribute()", new string[] { })]
    [InlineData("Attribute(Param1)", new[] { "Param1" })]
    [InlineData("Attribute", new[] { "Attribute" })]
    [InlineData("Attribute(\"Param1\", [Param2, Param3])", new[] { "Param1", "[Param2, Param3]" })]
    public void ExtractAttributeParameters_WorksCorrectly(string input, string[] expected)
    {
        var result = input.ExtractAttributeParameters();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Param=Value", "Value")]
    [InlineData("Param=[Value]", "Value")]
    [InlineData("Param=[Value1, Value2]", "Value1, Value2")]
    [InlineData("Param=  Value ", "Value")]
    [InlineData("Param", "Param")]
    public void ExtractParameterValue_WorksCorrectly(string input, string expected)
    {
        var result = input.ExtractParameterValue();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("m_fieldName", "fieldName")]
    [InlineData("_fieldName", "fieldName")]
    [InlineData("fieldName", "fieldName")]
    [InlineData("m_", "")]
    [InlineData("_", "")]
    public void StripPrefixFromField_WorksCorrectly(string input, string expected)
    {
        var result = input.StripPrefixFromField();
        Assert.Equal(expected, result);
    }
}