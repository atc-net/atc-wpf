namespace Atc.Wpf.Controls.Layouts;

/// <summary>
/// Stores <see cref="DockPanelPro"/> layouts as JSON files in a folder, one file per layout identifier.
/// <see cref="Default"/> uses <c>%LocalAppData%\&lt;application name&gt;\DockLayouts</c>.
/// A layout that cannot be read or written is treated as missing, so auto-save never fails the application.
/// </summary>
public sealed class FileDockLayoutStore : IDockLayoutStore
{
    private static readonly Lazy<FileDockLayoutStore> DefaultStore = new(() => new FileDockLayoutStore(GetDefaultDirectory()));

    /// <summary>
    /// Initializes a new instance of the <see cref="FileDockLayoutStore"/> class.
    /// </summary>
    /// <param name="directory">The folder of the layout files; it is created on the first save.</param>
    public FileDockLayoutStore(string directory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);
        Directory = directory;
    }

    /// <summary>
    /// Gets the store used when <see cref="DockPanelPro.LayoutStore"/> is not set:
    /// <c>%LocalAppData%\&lt;application name&gt;\DockLayouts</c>.
    /// </summary>
    public static FileDockLayoutStore Default => DefaultStore.Value;

    /// <summary>
    /// Gets the folder of the layout files.
    /// </summary>
    public string Directory { get; }

    /// <inheritdoc />
    public string? Load(string layoutId)
    {
        ArgumentException.ThrowIfNullOrEmpty(layoutId);

        var path = GetPath(layoutId);
        try
        {
            return File.Exists(path)
                ? File.ReadAllText(path)
                : null;
        }
        catch (IOException)
        {
            return null;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
    }

    /// <inheritdoc />
    public void Save(
        string layoutId,
        string layout)
    {
        ArgumentException.ThrowIfNullOrEmpty(layoutId);
        ArgumentNullException.ThrowIfNull(layout);

        try
        {
            System.IO.Directory.CreateDirectory(Directory);
            File.WriteAllText(GetPath(layoutId), layout);
        }
        catch (IOException)
        {
            // Auto-save is best effort.
        }
        catch (UnauthorizedAccessException)
        {
            // Auto-save is best effort.
        }
    }

    /// <summary>
    /// Gets the file of a layout: the identifier with the characters a file name cannot hold replaced by <c>_</c>.
    /// </summary>
    /// <param name="layoutId">The layout identifier.</param>
    /// <returns>The full path of the layout file.</returns>
    public string GetPath(string layoutId)
    {
        ArgumentException.ThrowIfNullOrEmpty(layoutId);

        var invalid = System.IO.Path.GetInvalidFileNameChars();
        var name = new string(layoutId.Select(c => Array.IndexOf(invalid, c) >= 0 ? '_' : c).ToArray());
        return System.IO.Path.Combine(Directory, name + ".json");
    }

    private static string GetDefaultDirectory()
    {
        var application = Assembly.GetEntryAssembly()?.GetName().Name ?? "Atc.Wpf";
        return System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            application,
            "DockLayouts");
    }
}