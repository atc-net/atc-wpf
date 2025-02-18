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
                predicate: static (syntaxNode, _) => IsSyntaxTargetFrameworkElement(syntaxNode),
                transform: static (context, _) => GetSemanticTargetFrameworkElementToGenerate(context))
            .Where(target => target is not null);

        context.RegisterSourceOutput(
            viewModelsToGenerate,
            static (spc, source)
                => Execute(spc, source));
    }

    private static bool IsSyntaxTargetFrameworkElement(
        SyntaxNode syntaxNode)
    {
        if (!syntaxNode.HasPartialClassDeclaration())
        {
            return false;
        }

        if (syntaxNode is not ClassDeclarationSyntax classDeclarationSyntax)
        {
            return false;
        }

        if (classDeclarationSyntax.BaseList is null ||
            classDeclarationSyntax.HasBaseClassWithName(NameConstants.UserControl) ||
            classDeclarationSyntax.Identifier.ToString().EndsWith(NameConstants.Attach, StringComparison.Ordinal) ||
            classDeclarationSyntax.Identifier.ToString().EndsWith(NameConstants.Behavior, StringComparison.Ordinal))
        {
            return syntaxNode.HasClassDeclarationWithValidAttachedProperties() ||
                   syntaxNode.HasClassDeclarationWithValidDependencyProperties() ||
                   syntaxNode.HasClassDeclarationWithValidRelayCommandMethods();
        }

        return false;
    }

    private static FrameworkElementToGenerate? GetSemanticTargetFrameworkElementToGenerate(
        GeneratorSyntaxContext context)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
        var frameworkElementClassSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax)!;

        if (frameworkElementClassSymbol is null)
        {
            return null;
        }

        if (!classDeclarationSyntax.HasBaseClassWithName(NameConstants.UserControl) &&
            !frameworkElementClassSymbol.InheritsFrom(
                NameConstants.DependencyObject,
                NameConstants.FrameworkElement) &&
            !frameworkElementClassSymbol.Name.EndsWith(NameConstants.Attach, StringComparison.Ordinal) &&
            !frameworkElementClassSymbol.Name.EndsWith(NameConstants.Behavior, StringComparison.Ordinal))
        {
            return null;
        }

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
            AttachedPropertiesToGenerate = frameworkElementInspectorResult.AttachedPropertiesToGenerate,
            DependencyPropertiesToGenerate = frameworkElementInspectorResult.DependencyPropertiesToGenerate,
            RelayCommandsToGenerate = frameworkElementInspectorResult.RelayCommandsToGenerate,
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

        frameworkElementBuilder.GenerateStart(frameworkElementToGenerate);

        if (frameworkElementToGenerate.AttachedPropertiesToGenerate?.Count > 0)
        {
            frameworkElementBuilder.GenerateAttachedProperties(frameworkElementToGenerate.AttachedPropertiesToGenerate);
        }

        if (frameworkElementToGenerate.DependencyPropertiesToGenerate?.Count > 0)
        {
            frameworkElementBuilder.GenerateDependencyProperties(frameworkElementToGenerate.DependencyPropertiesToGenerate);
        }

        if (frameworkElementToGenerate.RelayCommandsToGenerate?.Count > 0)
        {
            frameworkElementBuilder.GenerateRelayCommands(frameworkElementBuilder, frameworkElementToGenerate.RelayCommandsToGenerate);
        }

        frameworkElementBuilder.GenerateEnd();

        var sourceText = frameworkElementBuilder.ToSourceText();

        context.AddSource(
            frameworkElementToGenerate.GeneratedFileName,
            sourceText);
    }
}