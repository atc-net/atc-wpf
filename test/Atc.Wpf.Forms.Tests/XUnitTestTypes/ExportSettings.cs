namespace Atc.Wpf.Forms.Tests.XUnitTestTypes;

public sealed class ExportSettings
{
    [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Deserialized by the form writer.")]
    public IList<string> Columns { get; set; } = [];
}