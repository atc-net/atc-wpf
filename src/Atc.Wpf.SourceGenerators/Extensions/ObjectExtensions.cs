namespace Atc.Wpf.SourceGenerators.Extensions;

internal static class ObjectExtensions
{
    [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
    public static string? TransformDefaultValueIfNeeded(
        this object? defaultValue,
        string type)
    {
        if (defaultValue is null)
        {
            return null;
        }

        var strDefaultValue = defaultValue
            .ToString()?
            .EnsureNoNameof()?
            .ToString() ?? string.Empty;

        if (!type.IsSimpleType())
        {
            return strDefaultValue;
        }

        switch (type)
        {
            case "bool":
                strDefaultValue = strDefaultValue switch
                {
                    "true" => "BooleanBoxes.TrueBox",
                    "True" => "BooleanBoxes.TrueBox",
                    "false" => "BooleanBoxes.FalseBox",
                    "False" => "BooleanBoxes.FalseBox",
                    _ => strDefaultValue,
                };
                break;
            case "decimal":
                strDefaultValue = strDefaultValue.Length == 0
                    ? SimpleTypeFactory.CreateDefaultValueAsStrForType(type)
                    : strDefaultValue.Replace(',', '.') + "m";
                break;
            case "double":
                strDefaultValue = strDefaultValue.Length == 0
                    ? SimpleTypeFactory.CreateDefaultValueAsStrForType(type)
                    : strDefaultValue.Replace(',', '.') + "d";
                break;
            case "float":
                strDefaultValue = strDefaultValue.Length == 0
                    ? SimpleTypeFactory.CreateDefaultValueAsStrForType(type)
                    : strDefaultValue.Replace(',', '.') + "f";
                break;
            case "int" or "long":
                if (strDefaultValue.Length == 0)
                {
                    strDefaultValue = "0";
                }

                break;
            case "string":
            {
                if (strDefaultValue.Length == 0)
                {
                    strDefaultValue = "string.Empty";
                }

                break;
            }
        }

        return strDefaultValue;
    }

    public static object? EnsureNoNameof(
        this object? value)
    {
        if (value is null ||
            !value.ToString().StartsWith("nameof", StringComparison.Ordinal))
        {
            return value;
        }

        return value
            .ToString()
            .Replace("nameof(", string.Empty)
            .Replace(")", string.Empty);
    }
}