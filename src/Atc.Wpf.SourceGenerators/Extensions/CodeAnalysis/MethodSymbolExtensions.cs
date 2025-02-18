namespace Atc.Wpf.SourceGenerators.Extensions.CodeAnalysis;

internal static class MethodSymbolExtensions
{
    public static string? ExtractRelayCommandCanExecute(
        this IMethodSymbol methodSymbol)
    {
        var attribute = methodSymbol
            .GetAttributes()
            .FirstOrDefault(attr => attr.AttributeClass?.Name
                is NameConstants.RelayCommandAttribute
                or NameConstants.RelayCommand);

        var constructorArgumentValues = attribute?.ExtractConstructorArgumentValues();

        if (constructorArgumentValues is { Length: > 0 } &&
            constructorArgumentValues[0].StartsWith(NameConstants.CanExecute, StringComparison.Ordinal))
        {
            return constructorArgumentValues[0]
                .Replace(NameConstants.CanExecute, string.Empty)
                .Replace("=", string.Empty)
                .Trim();
        }

        return null;
    }
}