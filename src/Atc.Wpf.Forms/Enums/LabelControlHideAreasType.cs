// ReSharper disable CheckNamespace
namespace Atc.Wpf.Forms;

[SuppressMessage("Maintainability", "S2342:Enumeration types should comply with a naming convention", Justification = "OK.")]
[Flags]
public enum LabelControlHideAreasType
{
    None = 0x00,
    Asterisk = 0x01,
    Information = 0x02,
    Validation = 0x04,
    AsteriskAndInformation = Asterisk | Information,
    AsteriskAndValidation = Asterisk | Validation,
    InformationAndValidation = Information | Validation,
    All = Asterisk | Information | Validation,
}