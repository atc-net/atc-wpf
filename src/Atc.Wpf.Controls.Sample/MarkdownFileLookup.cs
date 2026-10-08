namespace Atc.Wpf.Controls.Sample;

/// <summary>
/// Finds a markdown file by the end of its path.
/// </summary>
internal static class MarkdownFileLookup
{
    /// <summary>
    /// Returns the first file whose path ends with <paramref name="endPath"/> (".md" is added when missing),
    /// starting at a folder boundary: "ScrollViewer_Readme" does not match "ZoomScrollViewer_Readme.md".
    /// </summary>
    /// <param name="files">The markdown files to search.</param>
    /// <param name="endPath">The file name, optionally with leading folders, to match.</param>
    /// <returns>The matching file, or null.</returns>
    public static FileInfo? Find(
        IEnumerable<FileInfo> files,
        string endPath)
    {
        ArgumentNullException.ThrowIfNull(files);
        ArgumentNullException.ThrowIfNull(endPath);

        if (!endPath.EndsWith(
                ".md",
                StringComparison.Ordinal))
        {
            endPath += ".md";
        }

        var suffix = Path.DirectorySeparatorChar + endPath;
        return files.FirstOrDefault(x => x.FullName.EndsWith(
            suffix,
            StringComparison.OrdinalIgnoreCase));
    }
}