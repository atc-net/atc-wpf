namespace Atc.Wpf.Forms.Tests.XUnitTestTypes;

[Flags]
[SuppressMessage("Design", "CA1028:Enum Storage should be Int32", Justification = "Tests the signed byte storage path.")]
public enum AccessRights : sbyte
{
    None = 0,
    Read = 1,
    Write = 2,
    Delete = 4,
    ReadWrite = Read | Write,
}