namespace Atc.Wpf.SourceGenerators.Tests.Generators;

[SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
public sealed partial class ViewModelGeneratorTests : GeneratorTestBase
{
    [Fact]
    public void ClassName_Invalid()
    {
        const string inputCode =
            """
            namespace TestNamespace;

            public partial class Test : ViewModelBase
            {
                [ObservableProperty]
                private string name;
            }
            """;

        var generatorResult = RunGenerator<ViewModelGenerator>(inputCode);

        AssertGeneratorRunResultIsEmpty(generatorResult);
    }

    [Fact]
    public void ClassAccessor_Invalid()
    {
        const string inputCode =
            """
            namespace TestNamespace;

            public class TestViewModel : ViewModelBase
            {
                [ObservableProperty]
                private string name;
            }
            """;

        var generatorResult = RunGenerator<ViewModelGenerator>(inputCode);

        AssertGeneratorRunResultIsEmpty(generatorResult);
    }
}