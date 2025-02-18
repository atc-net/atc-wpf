namespace Atc.Wpf.SourceGenerators.Tests.Generators;

[SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
public sealed partial class FrameworkElementGeneratorTests : GeneratorTestBase
{
    [Fact]
    public void ClassAccessor_Invalid()
    {
        const string inputCode =
            """
            namespace TestNamespace;

            public class TestControl : UserControl
            {
                [RelayCommand]
                public void Save()
                {
                }
            }
            """;

        var generatorResult = RunGenerator<ViewModelGenerator>(inputCode);

        AssertGeneratorRunResultIsEmpty(generatorResult);
    }
}