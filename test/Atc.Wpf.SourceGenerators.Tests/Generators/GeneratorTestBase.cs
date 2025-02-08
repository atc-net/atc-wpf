// ReSharper disable NotNullOrRequiredMemberIsNotInitialized
namespace Atc.Wpf.SourceGenerators.Tests.Generators;

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

    internal static GeneratorRunResult GeneratorViewModel(
        string inputCode)
    {
        var inputCompilation = CreateCompilation(inputCode);

        ViewModelGenerator generator = new();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver = driver.RunGeneratorsAndUpdateCompilation(
            inputCompilation,
            out _,
            out _);

        var runResult = driver.GetRunResult();

        var generatorResult = runResult.Results[0];

        return generatorResult;
    }

    internal void AssertGeneratorRunResultAsEqual(
        string expectedCode,
        GeneratorRunResult generatorResult)
    {
        var generatedCode = string.Empty;

        if (generatorResult.GeneratedSources.Length == 1)
        {
            generatedCode = generatorResult.GeneratedSources[0].SourceText.ToString();
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
        GeneratorRunResult generatorResult)
    {
        if (generatorResult.GeneratedSources.Length == 0)
        {
            return;
        }

        var generatedCode = generatorResult.GeneratedSources[0].SourceText.ToString();

        var testResults = new List<TestResult>
        {
            new(true, 0, $"Expected no code!{Environment.NewLine}{Environment.NewLine}"),
            new(true, 0, $"Generated code:{Environment.NewLine}{generatedCode}{Environment.NewLine}{Environment.NewLine}"),
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