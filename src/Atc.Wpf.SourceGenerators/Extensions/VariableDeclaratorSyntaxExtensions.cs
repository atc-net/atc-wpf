namespace Atc.Wpf.SourceGenerators.Extensions;

internal static class VariableDeclaratorSyntaxExtensions
{
    public static string GetFieldName(
        this VariableDeclaratorSyntax variableDeclaratorSyntax)
    {
        var fieldName = variableDeclaratorSyntax.Identifier.Text;
        if (fieldName.StartsWith("m_", StringComparison.Ordinal))
        {
            fieldName = fieldName.Substring(2);
        }
        else if (fieldName.StartsWith("_", StringComparison.Ordinal))
        {
            fieldName = fieldName.Substring(1);
        }

        return fieldName;
    }

    public static bool HasValidBackingFieldName(
        this VariableDeclaratorSyntax variableDeclaratorSyntax)
    {
        var fieldName = variableDeclaratorSyntax.GetFieldName();
        return fieldName.Length > 0 &&
               char.IsLower(fieldName[0]);
    }
}