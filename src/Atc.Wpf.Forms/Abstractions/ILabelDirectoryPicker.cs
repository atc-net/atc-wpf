namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Represents a labeled directory picker control.
/// </summary>
public interface ILabelDirectoryPicker : ILabelControl
{
    /// <summary>
    /// Gets or sets the selected directory.
    /// </summary>
    DirectoryInfo? Value { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a clear button is shown inside the text box.
    /// </summary>
    bool ShowClearTextButton { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether only existing directories are accepted.
    /// </summary>
    bool AllowOnlyExisting { get; set; }

    /// <summary>
    /// Gets or sets the default directory used by the folder dialog.
    /// </summary>
    string DefaultDirectory { get; set; }

    /// <summary>
    /// Gets or sets the initial directory used by the folder dialog.
    /// </summary>
    string InitialDirectory { get; set; }

    /// <summary>
    /// Gets or sets the root directory used by the folder dialog.
    /// </summary>
    string RootDirectory { get; set; }
}