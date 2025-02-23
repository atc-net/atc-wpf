#pragma warning disable xUnit1042
namespace Atc.Wpf.SourceGenerators.Tests.Extensions;

public sealed class StringExtensionsTests
{
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

    [Theory]
    [MemberData(nameof(GetExtractAttributeConstructorParametersTestData))]
    public void ExtractAttributeConstructorParameters(
        string input,
        Dictionary<string, string?> expected)
    {
        var result = input.ExtractAttributeConstructorParameters();
        Assert.NotNull(result);
        Assert.Equal(expected, result);
    }

    public static IEnumerable<object[]> GetExtractAttributeConstructorParametersTestData()
    {
        yield return [
            "[RelayCommand]",
            new Dictionary<string, string?>(StringComparer.Ordinal),
        ];

        yield return [
            "[RelayCommand(\"MyCommand\")]",
            new Dictionary<string, string?>(StringComparer.Ordinal)
            {
                { "Name", "MyCommand" },
            },
        ];

        yield return [
            "[RelayCommand(\"MyCommand\", CanExecute = nameof(MyCanExecute))]",
            new Dictionary<string, string?>(StringComparer.Ordinal)
            {
                { "Name", "MyCommand" },
                { "CanExecute", "nameof(MyCanExecute)" },
            },
        ];

        yield return [
            "[RelayCommand(CanExecute = nameof(MyCanExecute))]",
            new Dictionary<string, string?>(StringComparer.Ordinal)
            {
                { "CanExecute", "nameof(MyCanExecute)" },
            },
        ];

        yield return [
            "[RelayCommand(\"MyTestLeft\", ParameterValues = [LeftTopRightBottomType.Left, 1])]",
            new Dictionary<string, string?>(StringComparer.Ordinal)
            {
                { "Name", "MyTestLeft" },
                { "ParameterValues", "LeftTopRightBottomType.Left, 1" },
            },
        ];

        yield return [
            "[ObservableProperty(nameof(MyName), DependentProperties = [nameof(FullName), nameof(Age)])]",
            new Dictionary<string, string?>(StringComparer.Ordinal)
            {
                { "Name", "MyName" },
                { "DependentProperties", "nameof(FullName), nameof(Age)" },
            },
        ];

        yield return [
            "[ObservableProperty(BeforeChangedCallback = \"DoStuffA();\", AfterChangedCallback = \"EntrySelected?.Invoke(this, selectedEntry); DoStuffB();\")]",
            new Dictionary<string, string?>(StringComparer.Ordinal)
            {
                { "BeforeChangedCallback", "DoStuffA();" },
                { "AfterChangedCallback", "EntrySelected?.Invoke(this, selectedEntry); DoStuffB();" },
            },
        ];
    }
}