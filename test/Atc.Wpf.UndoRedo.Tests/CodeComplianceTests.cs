namespace Atc.Wpf.UndoRedo.Tests;

public sealed class CodeComplianceTests
{
    private readonly Assembly sourceAssembly = typeof(IAssemblyMarkerAtcWpfUndoRedo).Assembly;

    [Fact]
    public void AssertAssemblyHasExpectedName()
    {
        // Arrange & Act
        var name = sourceAssembly.GetName().Name;

        // Assert
        name.Should().Be("Atc.Wpf.UndoRedo");
    }
}