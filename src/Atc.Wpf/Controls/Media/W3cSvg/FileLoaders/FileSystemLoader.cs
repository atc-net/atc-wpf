namespace Atc.Wpf.Controls.Media.W3cSvg.FileLoaders;

public sealed class FileSystemLoader : IExternalFileLoader
{
    static FileSystemLoader()
    {
        Instance = new FileSystemLoader();
    }

    public static FileSystemLoader Instance { get; }

    public Stream? LoadFile(
        string hRef,
        string svgFilename)
    {
        var path = Environment.CurrentDirectory;
        if (!string.IsNullOrEmpty(svgFilename))
        {
            path = Path.GetDirectoryName(svgFilename);
        }

        if (path is not null)
        {
            var fileName = Path.Combine(path, hRef);
            if (File.Exists(fileName))
            {
                return File.OpenRead(fileName);
            }
        }

        Trace.TraceWarning("Unresolved URI: " + hRef);

        return null;
    }
}