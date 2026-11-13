namespace Atc.Wpf.Controls.Sample;

/// <summary>
/// The sample <c>.xaml</c> files under a folder, read once, so selecting a sample does not scan the solution tree.
/// </summary>
internal sealed class SampleFileIndex
{
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, SampleFileIndex> ByFolder =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly ILookup<string, string> filesByName;

    internal SampleFileIndex(IEnumerable<string> xamlFiles)
        => filesByName = xamlFiles.ToLookup(Path.GetFileNameWithoutExtension, StringComparer.Ordinal)!;

    public static SampleFileIndex For(string baseFolder)
        => ByFolder.GetOrAdd(baseFolder, static folder => new SampleFileIndex(EnumerateXamlFiles(folder)));

    /// <summary>
    /// Finds the folder of the view's <c>.xaml</c> file; when several views share the name, the one whose path
    /// matches the type's namespace.
    /// </summary>
    public DirectoryInfo? FindFolder(
        string classViewName,
        string typeFullName)
    {
        var files = filesByName[classViewName].ToList();
        if (files.Count == 1)
        {
            return new FileInfo(files[0]).Directory;
        }

        var match = files.Find(file => file
            .Replace('\\', '.')
            .Replace('/', '.')
            .Contains(typeFullName, StringComparison.OrdinalIgnoreCase));
        return match is null
            ? null
            : new FileInfo(match).Directory;
    }

    private static IEnumerable<string> EnumerateXamlFiles(string folder)
    {
        var separator = Path.DirectorySeparatorChar;
        return Directory
            .EnumerateFiles(folder, "*.xaml", SearchOption.AllDirectories)
            .Where(file => !file.Contains($"{separator}bin{separator}", StringComparison.OrdinalIgnoreCase) &&
                           !file.Contains($"{separator}obj{separator}", StringComparison.OrdinalIgnoreCase));
    }
}