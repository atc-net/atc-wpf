namespace Atc.Wpf.SourceGenerators.Tests.Generators;

[SuppressMessage("Design", "MA0048:File name must match type name", Justification = "OK.")]
public sealed partial class FrameworkElementGeneratorTests
{
    [Fact]
    public void RelayCommand_NoParameter()
    {
        const string inputCode =
            """
            namespace TestNamespace;

            public partial class TestControl : UserControl
            {
                [RelayCommand]
                public void DoCommandHandler()
                {
                }
            }
            """;

        const string expectedCode =
            """
            // <auto-generated>
            #nullable enable
            using Atc.Wpf.Command;

            namespace TestNamespace;

            public partial class TestControl
            {
                public IRelayCommand DoCommand => new RelayCommand(DoCommandHandler);
            }

            #nullable disable
            """;

        var generatorResult = RunGenerator<FrameworkElementGenerator>(inputCode);

        AssertGeneratorRunResultAsEqual(expectedCode, generatorResult);
    }
}