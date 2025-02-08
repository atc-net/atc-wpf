namespace Atc.Wpf.SourceGenerators.Generators;

[Generator]
public class ViewModelGenerator : IIncrementalGenerator
{
    public void Initialize(
        IncrementalGeneratorInitializationContext context)
    {
        //// #if DEBUG
        ////         if (!Debugger.IsAttached)
        ////         {
        ////             Debugger.Launch();
        ////         }
        //// #endif

        var viewModelsToGenerate = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (syntaxNode, _) => IsSyntaxTargetViewModel(syntaxNode),
                transform: static (context, _) => GetSemanticTargetViewModelToGenerate(context))
            .Where(target => target is not null);

        context.RegisterSourceOutput(
            viewModelsToGenerate,
            static (spc, source)
                => Execute(spc, source));
    }

    private static bool IsSyntaxTargetViewModel(
        SyntaxNode syntaxNode)
        => syntaxNode is ClassDeclarationSyntax classDeclarationSyntax &&
            classDeclarationSyntax.Identifier.Text.Contains(NameConstants.ViewModel);

    private static ViewModelToGenerate? GetSemanticTargetViewModelToGenerate(
        GeneratorSyntaxContext context)
    {
        var viewModelClassSymbol = context.GetNamedTypeSymbolFromClassDeclarationSyntax();

        var viewModelMemberInspectorResult = ViewModelInspector.Inspect(viewModelClassSymbol);

        if (!viewModelMemberInspectorResult.FoundAnythingToGenerate)
        {
            return null;
        }

        var viewModelToGenerate = new ViewModelToGenerate(
            namespaceName: viewModelClassSymbol.ContainingNamespace.ToDisplayString(),
            className: viewModelClassSymbol.Name,
            accessModifier: viewModelClassSymbol.GetAccessModifier())
        {
            PropertiesToGenerate = viewModelMemberInspectorResult.PropertiesToGenerate,
        };

        return viewModelToGenerate;
    }

    private static void Execute(
        SourceProductionContext context,
        ViewModelToGenerate? viewModelToGenerate)
    {
        if (viewModelToGenerate is null)
        {
            return;
        }

        var viewModelBuilder = new ViewModelBuilder();

        viewModelBuilder.GenerateStart(viewModelToGenerate);

        viewModelBuilder.GenerateProperties(viewModelToGenerate.PropertiesToGenerate);

        viewModelBuilder.GenerateEnd();

        var sourceText = viewModelBuilder.ToSourceText();

        context.AddSource(
            viewModelToGenerate.GeneratedFileName,
            sourceText);
    }
}