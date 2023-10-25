namespace Atc.Wpf.Theming.Tests;

public class CodeComplianceTests
{
    private readonly Assembly sourceAssembly = typeof(IAssemblyMarkerAtcWpfTheming).Assembly;

    [Fact]
    public void AssertLocalizationResources()
    {
        // Arrange
        var cultureNames = new List<string>
        {
            "da-DK",
            "de-DE",
        };

        // Act & Assert
        CodeComplianceHelper.AssertLocalizationResources(
            sourceAssembly,
            cultureNames);
    }
}