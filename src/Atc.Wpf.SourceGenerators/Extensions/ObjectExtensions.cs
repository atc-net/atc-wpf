namespace Atc.Wpf.SourceGenerators.Extensions;

internal static class ObjectExtensions
{
    public static object? TransformDefaultValueIfNeeded(
        this object? defaultValue)
    {
        if (defaultValue is null)
        {
            return null;
        }

        defaultValue = defaultValue.ToString() switch
        {
            "" => "string.Empty",
            "true" => "BooleanBoxes.TrueBox",
            "True" => "BooleanBoxes.TrueBox",
            "false" => "BooleanBoxes.FalseBox",
            "False" => "BooleanBoxes.FalseBox",
            _ => defaultValue,
        };

        return defaultValue;
    }
}