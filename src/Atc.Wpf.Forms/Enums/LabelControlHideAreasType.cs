// ReSharper disable CheckNamespace
namespace Atc.Wpf.Forms;

/// <summary>
/// Specifies which optional areas of a label control are hidden.
/// </summary>
[SuppressMessage("Maintainability", "S2342:Enumeration types should comply with a naming convention", Justification = "OK.")]
[Flags]
public enum LabelControlHideAreasType
{
    /// <summary>
    /// No areas are hidden.
    /// </summary>
    None = 0x00,

    /// <summary>
    /// Hides the mandatory asterisk.
    /// </summary>
    Asterisk = 0x01,

    /// <summary>
    /// Hides the information area.
    /// </summary>
    Information = 0x02,

    /// <summary>
    /// Hides the validation message area.
    /// </summary>
    Validation = 0x04,

    /// <summary>
    /// Hides the mandatory asterisk and the information area.
    /// </summary>
    AsteriskAndInformation = Asterisk | Information,

    /// <summary>
    /// Hides the mandatory asterisk and the validation message area.
    /// </summary>
    AsteriskAndValidation = Asterisk | Validation,

    /// <summary>
    /// Hides the information area and the validation message area.
    /// </summary>
    InformationAndValidation = Information | Validation,

    /// <summary>
    /// Hides the mandatory asterisk, the information area and the validation message area.
    /// </summary>
    All = Asterisk | Information | Validation,
}