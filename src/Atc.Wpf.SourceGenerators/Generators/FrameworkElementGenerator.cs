namespace Atc.Wpf.SourceGenerators.Generators;

/// <summary>
/// Source generator for generating framework element properties and commands.
/// </summary>
[Generator]
public sealed class FrameworkElementGenerator : IIncrementalGenerator
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
                transform: static (context, _) => GetSemanticTargetFrameworkElementToGenerate(context))
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
    /// Extracts the semantic target framework element to generate.
    /// </summary>
    /// <param name="context">The generator syntax context.</param>
    /// <returns>A FrameworkElementToGenerate object if valid; otherwise, null.</returns>
    /// <remarks>
    /// This method uses <c>HasBaseClassFromFrameworkElementOrEndsOnAttachOrBehavior</c> to ensure the element
    /// inherits from a valid base class or follows a recognized naming convention.
    ///
    /// The valid base classes or naming conventions are:
    /// <list type="bullet">
    /// <item><description>UserControl</description></item>
    /// <item><description>DependencyObject</description></item>
    /// <item><description>FrameworkElement</description></item>
    /// <item><description>Class name ending with "Attach"</description></item>
    /// <item><description>Class name ending with "Behavior"</description></item>
    /// </list>
    /// </remarks>
    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
    private static FrameworkElementToGenerate? GetSemanticTargetFrameworkElementToGenerate(
        GeneratorSyntaxContext context)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
        var frameworkElementClassSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax);

        if (frameworkElementClassSymbol is null)
        {
            return null;
        }

        var allPartialDeclarations = context
            .SemanticModel
            .Compilation
            .GetAllPartialClassDeclarations(frameworkElementClassSymbol);

        if (!allPartialDeclarations.HasBaseClassFromFrameworkElementOrEndsOnAttachOrBehavior(context))
        {
            return null;
        }

        var allAttachedProperties = new List<AttachedPropertyToGenerate>();
        var allDependencyProperties = new List<DependencyPropertyToGenerate>();
        var allRelayCommands = new List<RelayCommandToGenerate>();
        var isStatic = false;

        foreach (var partialClassSyntax in allPartialDeclarations)
        {
            if (context.SemanticModel.Compilation
                    .GetSemanticModel(partialClassSyntax.SyntaxTree)
                    .GetDeclaredSymbol(partialClassSyntax) is not { } partialClassSymbol)
            {
                continue;
            }

            var result = FrameworkElementInspector.Inspect(partialClassSymbol);

            if (!result.FoundAnythingToGenerate)
            {
                continue;
            }

            result.ApplyCommandsAndProperties(
                allAttachedProperties,
                allDependencyProperties,
                allRelayCommands);

            if (!isStatic)
            {
                isStatic = result.IsStatic;
            }
        }

        if (!allAttachedProperties.Any() &&
            !allDependencyProperties.Any() &&
            !allRelayCommands.Any())
        {
            return null;
        }

        var frameworkElementToGenerate = new FrameworkElementToGenerate(
            namespaceName: frameworkElementClassSymbol.ContainingNamespace.ToDisplayString(),
            className: frameworkElementClassSymbol.Name,
            accessModifier: frameworkElementClassSymbol.GetAccessModifier(),
            isStatic: isStatic)
        {
            AttachedPropertiesToGenerate = allAttachedProperties,
            DependencyPropertiesToGenerate = allDependencyProperties,
            RelayCommandsToGenerate = allRelayCommands,
        };

        return frameworkElementToGenerate;
    }

    /// <summary>
    /// Executes the source generation process.
    /// </summary>
    /// <param name="context">The source production context.</param>
    /// <param name="frameworkElementToGenerate">The framework element to generate.</param>
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