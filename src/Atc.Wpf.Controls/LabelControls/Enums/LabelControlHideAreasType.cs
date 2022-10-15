// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls.LabelControls;

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