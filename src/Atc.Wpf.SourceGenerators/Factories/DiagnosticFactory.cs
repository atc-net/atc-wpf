namespace Atc.Wpf.SourceGenerators.Factories;

internal static class DiagnosticFactory
{
    public static Diagnostic CreateNoViewModelIsFound()
    {
        return Diagnostic.Create(
            new DiagnosticDescriptor(
                "AtcWpfSG001",
                "No classes of ViewModel is found",
                "No classes of ViewModel is declared",
                "Hint",
                DiagnosticSeverity.Info,
                isEnabledByDefault: true),
            Location.None);
    }
}