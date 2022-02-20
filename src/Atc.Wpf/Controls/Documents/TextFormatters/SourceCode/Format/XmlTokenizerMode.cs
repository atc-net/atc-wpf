namespace Atc.Wpf.Controls.Documents.TextFormatters.SourceCode.Format;

internal enum XmlTokenizerMode
{
    /// <summary>
    /// The inside comment
    /// </summary>
    InsideComment,

    /// <summary>
    /// The inside processing instruction
    /// </summary>
    InsideProcessingInstruction,

    /// <summary>
    /// The after open
    /// </summary>
    AfterOpen,

    /// <summary>
    /// The after attribute name
    /// </summary>
    AfterAttributeName,

    /// <summary>
    /// The after attribute equals
    /// </summary>
    AfterAttributeEquals,

    /// <summary>
    /// The inside element
    /// </summary>
    InsideElement,

    /// <summary>
    /// The outside element
    /// </summary>
    OutsideElement,

    /// <summary>
    /// The inside c data
    /// </summary>
    InsideCData,
}