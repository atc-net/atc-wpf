namespace Atc.Wpf.Displays.Tests;

public sealed class CodeComplianceTests
{
    private readonly Assembly sourceAssembly = typeof(IAssemblyMarkerAtcWpfDisplays).Assembly;

    [Fact]
    public void AssertAssemblyHasExpectedName()
    {
        // Arrange & Act
        var name = sourceAssembly.GetName().Name;

        // Assert
        name.Should().Be("Atc.Wpf.Displays");
    }
}