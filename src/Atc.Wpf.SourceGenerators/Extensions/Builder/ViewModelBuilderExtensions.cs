namespace Atc.Wpf.SourceGenerators.Extensions.Builder;

[SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
internal static class ViewModelBuilderExtensions
{
    public static void GenerateStart(
        this ViewModelBuilder builder,
        ViewModelToGenerate viewModelToGenerate)
    {
        builder.AppendLine("// <auto-generated>");
        builder.AppendLine("#nullable enable");
        if (viewModelToGenerate.RelayCommandsToGenerate?.Count > 0)
        {
            builder.AppendLine("using Atc.Wpf.Command;");
        }

        builder.AppendLine();
        builder.AppendLine($"namespace {viewModelToGenerate.NamespaceName};");
        builder.AppendLine();
        builder.AppendLine($"{viewModelToGenerate.ClassAccessModifier} partial class {viewModelToGenerate.ClassName}");
        builder.AppendLine("{");
        builder.IncreaseIndent();
    }

    public static void GenerateProperties(
        this ViewModelBuilder builder,
        IEnumerable<ObservablePropertyToGenerate>? propertiesToGenerate)
    {
        if (propertiesToGenerate is null)
        {
            return;
        }

        foreach (var propertyToGenerate in propertiesToGenerate)
        {
            GenerateProperty(builder, propertyToGenerate);
        }
    }

    private static void GenerateProperty(
        ViewModelBuilder builder,
        ObservablePropertyToGenerate p)
    {
        builder.AppendLineBeforeMember();
        builder.AppendLine($"public {p.Type} {p.Name}");
        builder.AppendLine("{");
        builder.IncreaseIndent();
        builder.AppendLine($"get => {p.BackingFieldName};");
        builder.AppendLine("set");
        builder.AppendLine("{");
        builder.IncreaseIndent();
        builder.AppendLine($"if ({p.BackingFieldName} == value)");
        builder.AppendLine("{");
        builder.IncreaseIndent();
        builder.AppendLine("return;");
        builder.DecreaseIndent();
        builder.AppendLine("}");
        builder.AppendLine();
        builder.AppendLine($"{p.BackingFieldName} = value;");
        builder.AppendLine($"RaisePropertyChanged(nameof({p.Name}));");
        if (p.PropertyNamesToInvalidate is not null)
        {
            foreach (var propertyNameToInvalidate in p.PropertyNamesToInvalidate)
            {
                builder.AppendLine($"RaisePropertyChanged(nameof({propertyNameToInvalidate}));");
            }
        }

        builder.DecreaseIndent();
        builder.AppendLine("}");
        builder.DecreaseIndent();
        builder.AppendLine("}");
    }
}