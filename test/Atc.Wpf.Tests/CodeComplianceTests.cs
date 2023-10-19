namespace Atc.Wpf.Tests;

public class CodeComplianceTests
{
    private readonly Assembly sourceAssembly = typeof(IAssemblyMarkerAtcWpf).Assembly;

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