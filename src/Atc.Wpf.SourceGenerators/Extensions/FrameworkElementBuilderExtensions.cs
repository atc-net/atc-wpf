namespace Atc.Wpf.SourceGenerators.Extensions;

[SuppressMessage("Design", "CA1308:Teplace the call to 'ToLowerInvariant' with 'ToUpperInvariant'", Justification = "OK.")]
internal static class FrameworkElementBuilderExtensions
{
    public static void GenerateStart(
        this FrameworkElementBuilder builder,
        FrameworkElementToGenerate frameworkElementToGenerate)
    {
        builder.AppendLine("// <auto-generated>");
        builder.AppendLine("#nullable enable");
        if (frameworkElementToGenerate.DependencyPropertiesToGenerate is not null &&
            frameworkElementToGenerate.DependencyPropertiesToGenerate.Any(x => x.DefaultUpdateSourceTrigger is not null))
        {
            builder.AppendLine("using System.Windows.Data;");
        }

        builder.AppendLine();
        builder.AppendLine($"namespace {frameworkElementToGenerate.NamespaceName};");
        builder.AppendLine();
        builder.AppendLine($"{frameworkElementToGenerate.ClassAccessModifier} partial class {frameworkElementToGenerate.ClassName}");
        builder.AppendLine("{");
        builder.IncreaseIndent();
    }

    public static void GenerateDependencyProperties(
        this FrameworkElementBuilder builder,
        IEnumerable<DependencyPropertyToGenerate>? dependencyPropertiesToGenerate)
    {
        if (dependencyPropertiesToGenerate is null)
        {
            return;
        }

        foreach (var propertyToGenerate in dependencyPropertiesToGenerate)
        {
            GenerateDependencyProperty(builder, propertyToGenerate);
        }
    }

    private static void GenerateDependencyProperty(
        FrameworkElementBuilder builder,
        DependencyPropertyToGenerate p)
    {
        builder.AppendLineBeforeMember();
        builder.AppendLine($"public static readonly DependencyProperty {p.Name}Property = DependencyProperty.Register(");
        builder.IncreaseIndent();
        builder.AppendLine($"nameof({p.Name}),");
        builder.AppendLine($"typeof({p.Type}),");
        builder.AppendLine($"typeof({p.OwnerType}),");

        if (string.IsNullOrEmpty(p.PropertyChangedCallback) && string.IsNullOrEmpty(p.CoerceValueCallback))
        {
            builder.AppendLine($"new PropertyMetadata(defaultValue: {p.DefaultValue}));");
        }
        else
        {
            if (string.IsNullOrEmpty(p.Flags) &&
                string.IsNullOrEmpty(p.DefaultUpdateSourceTrigger) &&
                p.IsAnimationProhibited is null)
            {
                GeneratePropertyMetadataExtended(builder, p);
            }
            else
            {
                GenerateFrameworkPropertyMetadata(builder, p);
            }

            builder.DecreaseIndent();
        }

        builder.DecreaseIndent();

        GenerateClrProperty(builder, p);
    }

    private static void GeneratePropertyMetadataExtended(
        FrameworkElementBuilder builder,
        DependencyPropertyToGenerate p)
    {
        builder.AppendLine("new PropertyMetadata(");
        builder.IncreaseIndent();
        builder.AppendLine($"defaultValue: {p.DefaultValue},");

        if (string.IsNullOrEmpty(p.PropertyChangedCallback))
        {
            builder.AppendLine($"coerceValueCallback: {p.CoerceValueCallback}));");
        }
        else if (string.IsNullOrEmpty(p.CoerceValueCallback))
        {
            builder.AppendLine($"propertyChangedCallback: {p.PropertyChangedCallback}));");
        }
        else
        {
            builder.AppendLine($"propertyChangedCallback: {p.PropertyChangedCallback},");
            builder.AppendLine($"coerceValueCallback: {p.CoerceValueCallback}));");
        }
    }

    private static void GenerateFrameworkPropertyMetadata(
        FrameworkElementBuilder builder,
        DependencyPropertyToGenerate p)
    {
        builder.AppendLine("new FrameworkPropertyMetadata(");
        builder.IncreaseIndent();
        builder.AppendLine($"defaultValue: {p.DefaultValue},");

        if (!string.IsNullOrEmpty(p.PropertyChangedCallback))
        {
            builder.AppendLine($"propertyChangedCallback: {p.PropertyChangedCallback},");
        }

        if (!string.IsNullOrEmpty(p.CoerceValueCallback))
        {
            builder.AppendLine($"coerceValueCallback: {p.CoerceValueCallback},");
        }

        var hasAddedLine = false;
        if (!string.IsNullOrEmpty(p.Flags))
        {
            builder.Append($"flags: {p.Flags}");
            hasAddedLine = true;
        }

        if (!string.IsNullOrEmpty(p.DefaultUpdateSourceTrigger))
        {
            if (hasAddedLine)
            {
                builder.AppendLine(",");
            }

            builder.Append($"defaultUpdateSourceTrigger: {p.DefaultUpdateSourceTrigger}");
        }

        if (p.IsAnimationProhibited.HasValue)
        {
            if (hasAddedLine)
            {
                builder.AppendLine(",");
            }

            builder.Append($"isAnimationProhibited: {p.IsAnimationProhibited.Value.ToString().ToLowerInvariant()}");
        }

        builder.AppendLine("));");
    }

    private static void GenerateClrProperty(
        FrameworkElementBuilder builder,
        DependencyPropertyToGenerate p)
    {
        builder.AppendLine();

        if (!string.IsNullOrEmpty(p.Category))
        {
            builder.AppendLine($"[Category(\"{p.Category}\")]");
        }

        if (!string.IsNullOrEmpty(p.Description))
        {
            builder.AppendLine($"[Description(\"{p.Description}\")]");
        }

        builder.AppendLine($"public {p.Type} {p.Name}");
        builder.AppendLine("{");
        builder.IncreaseIndent();
        builder.AppendLine("get => (bool)GetValue(IsRunningProperty);");
        builder.AppendLine("set => SetValue(IsRunningProperty, value);");
        builder.DecreaseIndent();
        builder.AppendLine("}");
    }
}