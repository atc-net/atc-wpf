namespace Atc.Wpf.Controls.Media.W3cSvg.FileLoaders;

/// <summary>
/// Loads files referenced by an SVG from the file system, restricted to the SVG file's directory.
/// </summary>
public sealed class FileSystemLoader : IExternalFileLoader
{
    static FileSystemLoader()
    {
        Instance = new FileSystemLoader();
    }

    /// <summary>
    /// Gets the shared <see cref="FileSystemLoader"/> instance.
    /// </summary>
    public static FileSystemLoader Instance { get; }

    /// <inheritdoc />
    public Stream? LoadFile(
        string hRef,
        string svgFilename)
    {
        var path = Environment.CurrentDirectory;
        if (!string.IsNullOrEmpty(svgFilename))
        {
            path = Path.GetDirectoryName(svgFilename);
        }

        if (path is not null &&
            TryResolveInsideDirectory(path, hRef, out var fileName) &&
            File.Exists(fileName))
        {
            return File.OpenRead(fileName);
        }

        Trace.TraceWarning("Unresolved URI: " + hRef);

        return null;
    }

    /// <summary>
    /// Resolves <paramref name="hRef"/> against <paramref name="baseDirectory"/> and only accepts the result
    /// when it stays inside that directory. Rooted hrefs (drive paths, UNC shares, device paths) and
    /// <c>..</c> segments that climb out are rejected, so an untrusted SVG cannot read arbitrary local files
    /// or trigger an outbound SMB connection.
    /// </summary>
    private static bool TryResolveInsideDirectory(
        string baseDirectory,
        string hRef,
        out string fileName)
    {
        fileName = string.Empty;

        if (string.IsNullOrWhiteSpace(hRef) ||
            Path.IsPathRooted(hRef))
        {
            return false;
        }

        var baseFullPath = Path.GetFullPath(baseDirectory);
        if (!Path.EndsInDirectorySeparator(baseFullPath))
        {
            baseFullPath += Path.DirectorySeparatorChar;
        }

        var candidate = Path.GetFullPath(Path.Combine(baseFullPath, hRef));
        if (!candidate.StartsWith(baseFullPath, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        fileName = candidate;
        return true;
    }
}