namespace Atc.Wpf.SourceGenerators.Generators;

[Generator]
public sealed class ViewModelGenerator : IIncrementalGenerator
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
                transform: static (context, _) => GetSemanticTargetViewModelToGenerate(context))
            .Where(target => target is not null)
            .Collect()
            .Select((viewModels, _) => viewModels
                .GroupBy(vm => vm!.GeneratedFileName, StringComparer.Ordinal)
                .Select(group => group.First())
                .ToImmutableArray());

        context.RegisterSourceOutput(
            viewModelsToGenerate,
            static (spc, sources) =>
            {
                foreach (var source in sources)
                {
                    Execute(spc, source);
                }
            });
    }

    private static bool IsSyntaxTargetViewModel(
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

        if (!classDeclarationSyntax.HasBaseClassWithName(
                NameConstants.ViewModelBase,
                NameConstants.MainWindowViewModelBase,
                NameConstants.ViewModelDialogBase,
                NameConstants.ObservableObject))
        {
            return false;
        }

        return syntaxNode.HasClassDeclarationWithValidObservableFields() ||
               syntaxNode.HasClassDeclarationWithValidRelayCommandMethods();
    }

    private static ViewModelToGenerate? GetSemanticTargetViewModelToGenerate(
        GeneratorSyntaxContext context)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
        var viewModelClassSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax)!;

        if (!viewModelClassSymbol.IsInheritsFromIObservableObject())
        {
            return null;
        }

        var allPartialDeclarations = context
            .SemanticModel
            .Compilation
            .GetAllPartialClassDeclarations(viewModelClassSymbol);

        var allProperties = new List<ObservablePropertyToGenerate>();
        var allCommands = new List<RelayCommandToGenerate>();

        foreach (var partialClassSyntax in allPartialDeclarations)
        {
            if (context.SemanticModel.Compilation
                    .GetSemanticModel(partialClassSyntax.SyntaxTree)
                    .GetDeclaredSymbol(partialClassSyntax) is not { } partialClassSymbol)
            {
                continue;
            }

            var result = ViewModelInspector.Inspect(partialClassSymbol);

            result.ApplyCommandsAndProperties(
                allCommands,
                allProperties);
        }

        if (!allProperties.Any() && !allCommands.Any())
        {
            return null;
        }

        var viewModelToGenerate = new ViewModelToGenerate(
            namespaceName: viewModelClassSymbol.ContainingNamespace.ToDisplayString(),
            className: viewModelClassSymbol.Name,
            accessModifier: viewModelClassSymbol.GetAccessModifier())
        {
            PropertiesToGenerate = allProperties,
            RelayCommandsToGenerate = allCommands,
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

        if (viewModelToGenerate.ContainsRelayCommandNameDuplicates)
        {
            context.ReportDiagnostic(DiagnosticFactory.CreateContainsDuplicateNamesForRelayCommand());
        }
        else
        {
            viewModelBuilder.GenerateRelayCommands(viewModelBuilder, viewModelToGenerate.RelayCommandsToGenerate);
        }

        viewModelBuilder.GenerateProperties(viewModelToGenerate.PropertiesToGenerate);

        viewModelBuilder.GenerateEnd();

        var sourceText = viewModelBuilder.ToSourceText();

        context.AddSource(
            viewModelToGenerate.GeneratedFileName,
            sourceText);
    }
}