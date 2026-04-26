namespace Atc.Wpf.FontIcons.Tests;

public sealed class CodeComplianceTests
{
    private readonly Assembly sourceAssembly = typeof(IAssemblyMarkerAtcWpfFontIcons).Assembly;

    [Fact]
    public void AssemblyMarkerExposesFontIconsAssembly()
    {
        Assert.Equal("Atc.Wpf.FontIcons", sourceAssembly.GetName().Name);
    }
}