namespace Atc.Wpf.Controls.Tests;

public sealed class CodeComplianceTests
{
    private readonly Assembly sourceAssembly = typeof(IAssemblyMarkerAtcWpfControls).Assembly;

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