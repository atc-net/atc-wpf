// ReSharper disable FieldCanBeMadeReadOnly.Global
namespace Atc.Wpf.Controls.Documents.TextFormatters.SourceCode.Format;

/// <summary>
/// Xml Token.
/// </summary>
[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "OK.")]
[StructLayout(LayoutKind.Auto)]
internal struct XmlToken
{
    /// <summary>
    /// The kind.
    /// </summary>
    public XmlTokenKind Kind;

    /// <summary>
    /// The length.
    /// </summary>
    public short Length;

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlToken" /> struct.
    /// </summary>
    /// <param name="kind">The kind.</param>
    /// <param name="length">The length.</param>
    public XmlToken(XmlTokenKind kind, int length)
    {
        this.Kind = kind;
        this.Length = (short)length;
    }
}