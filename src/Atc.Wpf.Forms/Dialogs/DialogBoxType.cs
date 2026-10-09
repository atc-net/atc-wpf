namespace Atc.Wpf.Forms.Dialogs;

/// <summary>
/// Specifies which buttons a dialog box shows.
/// </summary>
public enum DialogBoxType
{
    /// <summary>
    /// No type is set.
    /// </summary>
    Unknown,

    /// <summary>
    /// An OK button only.
    /// </summary>
    Ok,

    /// <summary>
    /// OK and Cancel buttons.
    /// </summary>
    OkCancel,

    /// <summary>
    /// Yes and No buttons.
    /// </summary>
    YesNo,
}