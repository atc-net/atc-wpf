namespace Atc.Wpf.Helpers;

public static class AssemblyHelper
{
    public static Assembly? FindResourceAssembly(string resxName)
    {
        ArgumentNullException.ThrowIfNull(resxName);

        var assembly = Assembly.GetEntryAssembly();

        // Check the entry assembly first - this will short circuit a lot of searching
        if (assembly is not null && HasEmbeddedResx(assembly, resxName))
        {
            return assembly;
        }

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // ReSharper disable StringLiteralTypo
        var searchAssemblies = (
                from searchAssembly
                    in assemblies
                let name = searchAssembly.FullName
                where
                    !name.StartsWith("System", StringComparison.Ordinal) &&
                    !name.StartsWith("mscorlib", StringComparison.Ordinal) &&
                    !name.StartsWith("Presentation", StringComparison.Ordinal) &&
                    !name.StartsWith("WindowsBase", StringComparison.Ordinal) &&
                    !name.StartsWith("Microsoft", StringComparison.Ordinal) &&
                    !name.StartsWith("netstandard", StringComparison.Ordinal) &&
                    !name.StartsWith("UIAutomation", StringComparison.Ordinal) &&
                    !name.StartsWith("SMDiagnostics", StringComparison.Ordinal) &&
                    !name.StartsWith("DirectWriteForwarder", StringComparison.Ordinal) &&
                    !name.StartsWith("Fluent", StringComparison.Ordinal) &&
                    !name.StartsWith("ControlzEx", StringComparison.Ordinal) &&
                    !name.StartsWith("MahApps", StringComparison.Ordinal) &&
                    !name.StartsWith("WindowsFormsIntegration", StringComparison.Ordinal) &&
                    !name.StartsWith("vshost", StringComparison.Ordinal)
                select searchAssembly)
            .ToArray();

        // ReSharper restore StringLiteralTypo
        return searchAssemblies.FirstOrDefault(x => HasEmbeddedResx(x, resxName));
    }

    /// <summary>
    /// Check if the assembly contains an embedded resx of the given name.
    /// </summary>
    /// <param name="assembly">The assembly to check.</param>
    /// <param name="resxName">The name of the resource we are looking for.</param>
    /// <returns>True if the assembly contains the resource.</returns>
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
    private static bool HasEmbeddedResx(Assembly assembly, string resxName)
    {
        try
        {
            var resources = assembly.GetManifestResourceNames();
            var searchName = resxName + ".resources";
            return resources.Any(resource => searchName.Equals(resource, StringComparison.OrdinalIgnoreCase));
        }
        catch
        {
            // GetManifestResourceNames throws an exception for some
            // dynamic assemblies - just ignore these assemblies.
        }

        return false;
    }
}