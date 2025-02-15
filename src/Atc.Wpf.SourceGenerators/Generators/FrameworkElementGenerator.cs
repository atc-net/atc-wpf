// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable ReturnTypeCanBeNotNullable
namespace Atc.Wpf.SourceGenerators.Generators;

[Generator]
public sealed class FrameworkElementGenerator : IIncrementalGenerator
{
    public void Initialize(
        IncrementalGeneratorInitializationContext context)
    {
        //// #if DEBUG
        ////         if (!System.Diagnostics.Debugger.IsAttached)
        ////         {
        ////             System.Diagnostics.Debugger.Launch();
        ////         }
        //// #endif

        var viewModelsToGenerate = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (syntaxNode, _) => IsSyntaxTargetViewModel(syntaxNode),
                transform: static (context, _) => GetSemanticTargetFrameworkElementToGenerate(context))
            .Where(target => target is not null);

        context.RegisterSourceOutput(
            viewModelsToGenerate,
            static (spc, source)
                => Execute(spc, source));
    }

    private static bool IsSyntaxTargetViewModel(
        SyntaxNode syntaxNode)
    {
        if (!syntaxNode.HasPublicPartialClassDeclaration())
        {
            return false;
        }

        if (syntaxNode.HasClassDeclarationWithValidDependencyProperties())
        {
            return true;
        }

        if (syntaxNode.HasClassDeclarationWithValidAttachedProperties())
        {
            return true;
        }

        return false;
    }

    private static FrameworkElementToGenerate? GetSemanticTargetFrameworkElementToGenerate(
        GeneratorSyntaxContext context)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
        var frameworkElementClassSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax)!;

        var frameworkElementInspectorResult = FrameworkElementInspector.Inspect(frameworkElementClassSymbol);

        if (!frameworkElementInspectorResult.FoundAnythingToGenerate)
        {
            return null;
        }

        var frameworkElementToGenerate = new FrameworkElementToGenerate(
            namespaceName: frameworkElementClassSymbol.ContainingNamespace.ToDisplayString(),
            className: frameworkElementClassSymbol.Name,
            accessModifier: frameworkElementClassSymbol.GetAccessModifier(),
            isStatic: frameworkElementInspectorResult.IsStatic)
        {
            DependencyPropertiesToGenerate = frameworkElementInspectorResult.DependencyPropertiesToGenerate,
            AttachedPropertiesToGenerate = frameworkElementInspectorResult.AttachedPropertiesToGenerate,
        };

        return frameworkElementToGenerate;
    }

    private static void Execute(
        SourceProductionContext context,
        FrameworkElementToGenerate? frameworkElementToGenerate)
    {
        if (frameworkElementToGenerate is null)
        {
            return;
        }

        var frameworkElementBuilder = new FrameworkElementBuilder();

        if (frameworkElementToGenerate.DependencyPropertiesToGenerate?.Count > 0)
        {
            frameworkElementBuilder.GenerateStart(frameworkElementToGenerate);

            frameworkElementBuilder.GenerateDependencyProperties(frameworkElementToGenerate.DependencyPropertiesToGenerate);
        }
        else
        {
            frameworkElementBuilder.GenerateStart(frameworkElementToGenerate);

            frameworkElementBuilder.GenerateAttachedProperties(frameworkElementToGenerate.AttachedPropertiesToGenerate);
        }

        frameworkElementBuilder.GenerateEnd();

        var sourceText = frameworkElementBuilder.ToSourceText();

        context.AddSource(
            frameworkElementToGenerate.GeneratedFileName,
            sourceText);
    }
}