namespace Atc.Wpf.SourceGenerators.Extensions.CodeAnalysis;

public static class FieldSymbolExtensions
{
    public static bool HasObservablePropertyName(
        this IFieldSymbol fieldSymbol,
        string name)
    {
        if (fieldSymbol is null)
        {
            throw new ArgumentNullException(nameof(fieldSymbol));
        }

        var attribute = fieldSymbol
            .GetAttributes()
            .FirstOrDefault(attr => attr.AttributeClass?.Name
                is NameConstants.ObservablePropertyAttribute
                or NameConstants.ObservableProperty);

        if (attribute is null)
        {
            return false;
        }

        if (fieldSymbol.Name == name)
        {
            return true;
        }

        var parameterValues = attribute.ExtractConstructorArgumentValues();

        return parameterValues is not null &&
               parameterValues.Length > 0 &&
               parameterValues[0] == name;
    }

    public static bool HasObservableFieldName(
        this IFieldSymbol fieldSymbol,
        string name)
    {
        if (fieldSymbol is null)
        {
            throw new ArgumentNullException(nameof(fieldSymbol));
        }

        return fieldSymbol.Name == name.EnsureFirstCharacterToLower() &&
               fieldSymbol
                   .GetAttributes()
                   .FirstOrDefault(attr => attr.AttributeClass?.Name
                       is NameConstants.ObservablePropertyAttribute
                       or NameConstants.ObservableProperty) is not null;
    }
}