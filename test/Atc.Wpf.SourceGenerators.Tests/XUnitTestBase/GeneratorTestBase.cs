// ReSharper disable NotNullOrRequiredMemberIsNotInitialized
namespace Atc.Wpf.SourceGenerators.Tests.XUnitTestBase;

public abstract class GeneratorTestBase
{
    private static MetadataReference[] metadataReferences;

    protected GeneratorTestBase()
    {
        System.Reflection.Assembly.Load("Atc.Wpf.SourceGenerators");

        metadataReferences = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic)
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .ToArray<MetadataReference>();
    }

    internal static (GeneratorRunResult GeneratorResult, ImmutableArray<Diagnostic> Diagnostics) RunGenerator<T>(
        string inputCode)
        where T : IIncrementalGenerator, new()
    {
        var inputCompilation = CreateCompilation(inputCode);

        T generator = new();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);

        driver = driver.RunGeneratorsAndUpdateCompilation(
            inputCompilation,
            out _,
            out var diagnostics);

        var runResult = driver.GetRunResult();

        var generatorResult = runResult.Results[0];

        return (generatorResult, diagnostics);
    }

    internal void AssertGeneratorRunResultAsEqual(
        string expectedCode,
        (GeneratorRunResult GeneratorResult, ImmutableArray<Diagnostic> Diagnostics) generatorResult)
    {
        var generatedCode = string.Empty;

        if (generatorResult.GeneratorResult.GeneratedSources.Length == 1)
        {
            generatedCode = generatorResult.GeneratorResult.GeneratedSources[0].SourceText.ToString();
            if (expectedCode == generatedCode)
            {
                return;
            }
        }

        var testResults = new List<TestResult>
        {
            new(true, 0, $"Expected code:{Environment.NewLine}{expectedCode}{Environment.NewLine}{Environment.NewLine}"),
            new(true, 0, $"Generated code:{Environment.NewLine}{generatedCode}{Environment.NewLine}{Environment.NewLine}"),
        };

        TestResultHelper.AssertOnTestResults(testResults);
    }

    internal void AssertGeneratorRunResultIsEmpty(
        (GeneratorRunResult GeneratorResult, ImmutableArray<Diagnostic> Diagnostics) generatorResult)
    {
        if (generatorResult.GeneratorResult.GeneratedSources.Length == 0)
        {
            return;
        }

        var generatedCode = generatorResult.GeneratorResult.GeneratedSources[0].SourceText.ToString();

        var testResults = new List<TestResult>
        {
            new(true, 0, $"Expected no code:{Environment.NewLine}{Environment.NewLine}"),
            new(true, 0, $"Generated code:{Environment.NewLine}{generatedCode}{Environment.NewLine}{Environment.NewLine}"),
        };

        TestResultHelper.AssertOnTestResults(testResults);
    }

    internal void AssertGeneratorRunResultHasDiagnostics(
        string[] diagnosticCodes,
        (GeneratorRunResult GeneratorResult, ImmutableArray<Diagnostic> Diagnostics) generatorResult)
    {
        var collectedCodes = generatorResult.Diagnostics
            .Select(diagnostic => diagnostic.Id)
            .Order(StringComparer.Ordinal)
            .ToList();

        var orderedDiagnosticCodes = diagnosticCodes
            .Order(StringComparer.Ordinal)
            .ToList();

        if (collectedCodes.SequenceEqual(orderedDiagnosticCodes, StringComparer.Ordinal))
        {
            return;
        }

        var testResults = new List<TestResult>
        {
            new(true, 0, $"Expected error codes:{Environment.NewLine}{string.Join(", ", diagnosticCodes)}{Environment.NewLine}{Environment.NewLine}"),
            new(true, 0, $"Collected error codes:{Environment.NewLine}{string.Join(", ", collectedCodes)}{Environment.NewLine}{Environment.NewLine}"),
        };

        TestResultHelper.AssertOnTestResults(testResults);
    }

    private static Compilation CreateCompilation(
        string source)
        => CSharpCompilation.Create(
            "compilation",
            [CSharpSyntaxTree.ParseText(source)],
            metadataReferences,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
}