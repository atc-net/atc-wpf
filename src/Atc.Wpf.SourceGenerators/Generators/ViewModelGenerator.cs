namespace Atc.Wpf.SourceGenerators.Generators;

/// <summary>
/// Source generator for generating view model properties and commands.
/// </summary>
[Generator]
public sealed class ViewModelGenerator : IIncrementalGenerator
{
    /// <summary>
    /// Initializes the source generator.
    /// </summary>
    /// <param name="context">The initialization context.</param>
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
                predicate: static (syntaxNode, _) => IsSyntaxTargetPartialClass(syntaxNode),
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

    /// <summary>
    /// Determines if a given syntax node is a valid target for processing as a partial class.
    /// </summary>
    /// <param name="syntaxNode">The syntax node to check.</param>
    /// <returns>True if the node is a valid target; otherwise, false.</returns>
    /// <remarks>
    /// This method checks for partial class declarations to allow the generator to correctly
    /// collect and process class definitions that are split across multiple files. Partial
    /// classes are essential for scenarios where different aspects of a ViewModel or
    /// FrameworkElement are defined separately but should be combined into a single generated output.
    /// </remarks>
    private static bool IsSyntaxTargetPartialClass(
        SyntaxNode syntaxNode)
        => syntaxNode.HasPartialClassDeclaration();

    /// <summary>
    /// Extracts the semantic target ViewModel to generate.
    /// </summary>
    /// <param name="context">The generator syntax context.</param>
    /// <returns>A ViewModelToGenerate object if valid; otherwise, null.</returns>
    /// <remarks>
    /// This method uses <c>HasBaseClassFromList</c> to ensure the ViewModel inherits
    /// from a valid base class. This check is necessary to correctly identify ViewModels
    /// even if their base class is defined in another file.
    ///
    /// The valid base classes are:
    /// <list type="bullet">
    /// <item><description>ViewModelBase</description></item>
    /// <item><description>MainWindowViewModelBase</description></item>
    /// <item><description>ViewModelDialogBase</description></item>
    /// <item><description>ObservableObject</description></item>
    /// </list>
    /// </remarks>
    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
    private static ViewModelToGenerate? GetSemanticTargetViewModelToGenerate(
        GeneratorSyntaxContext context)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
        var viewModelClassSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax);

        if (viewModelClassSymbol is null)
        {
            return null;
        }

        var allPartialDeclarations = context
            .SemanticModel
            .Compilation
            .GetAllPartialClassDeclarations(viewModelClassSymbol);

        if (!allPartialDeclarations.HasBaseClassFromList(
                context,
                NameConstants.ViewModelBase,
                NameConstants.MainWindowViewModelBase,
                NameConstants.ViewModelDialogBase,
                NameConstants.ObservableObject))
        {
            return null;
        }

        var allObservableProperties = new List<ObservablePropertyToGenerate>();
        var allRelayCommands = new List<RelayCommandToGenerate>();

        foreach (var partialClassSyntax in allPartialDeclarations)
        {
            if (context.SemanticModel.Compilation
                    .GetSemanticModel(partialClassSyntax.SyntaxTree)
                    .GetDeclaredSymbol(partialClassSyntax) is not { } partialClassSymbol)
            {
                continue;
            }

            var result = ViewModelInspector.Inspect(partialClassSymbol);

            if (!result.FoundAnythingToGenerate)
            {
                continue;
            }

            result.ApplyCommandsAndProperties(
                allObservableProperties,
                allRelayCommands);
        }

        if (!allObservableProperties.Any() &&
            !allRelayCommands.Any())
        {
            return null;
        }

        var viewModelToGenerate = new ViewModelToGenerate(
            namespaceName: viewModelClassSymbol.ContainingNamespace.ToDisplayString(),
            className: viewModelClassSymbol.Name,
            accessModifier: viewModelClassSymbol.GetAccessModifier())
        {
            PropertiesToGenerate = allObservableProperties,
            RelayCommandsToGenerate = allRelayCommands,
        };

        return viewModelToGenerate;
    }

    /// <summary>
    /// Executes the source generation process.
    /// </summary>
    /// <param name="context">The source production context.</param>
    /// <param name="viewModelToGenerate">The ViewModel to generate.</param>
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