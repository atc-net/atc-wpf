namespace Atc.Wpf.NetworkControls.Tests;

public sealed class CodeComplianceTests
{
    private readonly Assembly sourceAssembly = typeof(IAssemblyMarkerAtcWpfNetworkControls).Assembly;

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
        XUnit.CodeComplianceHelper.AssertLocalizationResources(
            sourceAssembly,
            cultureNames);
    }
}