namespace Atc.Wpf.Forms.Tests.XUnitTestTypes;

[SuppressMessage("Naming", "CA1056:URI-like properties should not be strings", Justification = "Test model.")]
[SuppressMessage("Minor Code Smell", "S100:Methods and properties should be named in PascalCase", Justification = "Consecutive capitals on purpose.")]
public sealed class AcronymModel
{
    public uint UIntValue { get; set; }

    public string IPAddressText { get; set; } = string.Empty;

    public int HTTPPort { get; set; }
}