namespace Atc.Wpf.SourceGenerators.Extensions;

internal static class ViewModelBuilderExtensions
{
    internal static void GenerateStart(
        this ViewModelBuilder vmBuilder,
        ViewModelToGenerate viewModelToGenerate)
    {
        vmBuilder.AppendLine("#nullable enable");
        vmBuilder.AppendLine();
        vmBuilder.AppendLine($"namespace {viewModelToGenerate.NamespaceName};");
        vmBuilder.AppendLine();
        vmBuilder.AppendLine($"{viewModelToGenerate.ClassAccessModifier} partial class {viewModelToGenerate.ClassName}");
        vmBuilder.AppendLine("{");
        vmBuilder.IncreaseIndent();
    }

    internal static void GenerateEnd(
        this ViewModelBuilder vmBuilder)
    {
        vmBuilder.DecreaseIndent();
        vmBuilder.AppendLine("}");
    }

    internal static void GenerateProperties(
        this ViewModelBuilder vmBuilder,
        IEnumerable<PropertyToGenerate>? propertiesToGenerate)
    {
        if (propertiesToGenerate is null)
        {
            return;
        }

        foreach (var propertyToGenerate in propertiesToGenerate)
        {
            GenerateProperty(vmBuilder, propertyToGenerate);
        }
    }

    internal static SourceText ToSourceText(
        this ViewModelBuilder vmBuilder)
    {
        var str = vmBuilder.ToString();
        str = str.Substring(0, str.Length - 2);
        return SourceText.From(
            str,
            Encoding.UTF8);
    }

    private static void GenerateProperty(
        ViewModelBuilder vmBuilder,
        PropertyToGenerate p)
    {
        vmBuilder.AppendLineBeforeMember();
        vmBuilder.AppendLine($"public {p.Type} {p.Name}");
        vmBuilder.AppendLine("{");
        vmBuilder.IncreaseIndent();
        vmBuilder.AppendLine($"get => {p.BackingFieldName};");
        vmBuilder.AppendLine("set");
        vmBuilder.AppendLine("{");
        vmBuilder.IncreaseIndent();
        vmBuilder.AppendLine($"if ({p.BackingFieldName} == value)");
        vmBuilder.AppendLine("{");
        vmBuilder.IncreaseIndent();
        vmBuilder.AppendLine("return;");
        vmBuilder.DecreaseIndent();
        vmBuilder.AppendLine("}");
        vmBuilder.AppendLine();
        vmBuilder.AppendLine($"{p.BackingFieldName} = value;");
        vmBuilder.AppendLine($"RaisePropertyChanged(nameof({p.Name}));");
        if (p.PropertyNamesToInvalidate is not null)
        {
            foreach (var propertyNameToInvalidate in p.PropertyNamesToInvalidate)
            {
                vmBuilder.AppendLine($"RaisePropertyChanged(nameof({propertyNameToInvalidate}));");
            }
        }

        vmBuilder.DecreaseIndent();
        vmBuilder.AppendLine("}");
        vmBuilder.DecreaseIndent();
        vmBuilder.AppendLine("}");
    }
}