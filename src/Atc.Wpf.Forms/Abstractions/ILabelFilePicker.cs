namespace Atc.Wpf.Forms.Abstractions;

/// <summary>
/// Represents a labeled file picker control.
/// </summary>
public interface ILabelFilePicker : ILabelControl
{
    /// <summary>
    /// Gets or sets the selected file.
    /// </summary>
    FileInfo? Value { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a clear button is shown inside the text box.
    /// </summary>
    bool ShowClearTextButton { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether only existing files are accepted.
    /// </summary>
    bool AllowOnlyExisting { get; set; }

    /// <summary>
    /// Gets or sets the file type filter used by the file dialog.
    /// </summary>
    string Filter { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the file dialog shows a preview pane.
    /// </summary>
    bool UsePreviewPane { get; set; }

    /// <summary>
    /// Gets or sets the default directory used by the file dialog.
    /// </summary>
    string DefaultDirectory { get; set; }

    /// <summary>
    /// Gets or sets the initial directory used by the file dialog.
    /// </summary>
    string InitialDirectory { get; set; }

    /// <summary>
    /// Gets or sets the root directory used by the file dialog.
    /// </summary>
    string RootDirectory { get; set; }
}