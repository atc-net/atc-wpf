namespace Atc.Wpf.Controls.Documents.SourceCode.Format
{
    /*
    * this file implements a mostly correct XML tokenizer.  The token boundaries
    * have been chosen to match Visual Studio syntax highlighting, so a few of
    * the boundaries are little weird.  (Especially comments) known issues:
    *
    * Doesn't handle DTD's
    * mediocre handling of processing instructions <? ?> -- it won't crash,
    *      but the token boundaries are wrong
    * Doesn't enforce correct XML
    * there's probably a few cases where it will die if given in valid XML
    *
    *
    * This tokenizer has been designed to be restartable, so you can tokenize
    * one line of XML at a time.
    */
    internal enum XmlTokenKind
    {
        /// <summary>
        /// The open.
        /// </summary>
        Open,

        /// <summary>
        /// The close
        /// </summary>
        Close,

        /// <summary>
        /// The self close.
        /// </summary>
        SelfClose,

        /// <summary>
        /// The open close.
        /// </summary>
        OpenClose,

        /// <summary>
        /// The element name.
        /// </summary>
        ElementName,

        /// <summary>
        /// The element whitespace.
        /// </summary>
        ElementWhitespace,

        /// <summary>
        /// The attribute name.
        /// </summary>
        AttributeName,

        /// <summary>
        /// The equals.
        /// </summary>
        Equals,

        /// <summary>
        /// The attribute value.
        /// </summary>
        AttributeValue,

        /// <summary>
        /// The comment begin.
        /// </summary>
        CommentBegin,

        /// <summary>
        /// The comment text.
        /// </summary>
        CommentText,

        /// <summary>
        /// The comment end.
        /// </summary>
        CommentEnd,

        /// <summary>
        /// The entity.
        /// </summary>
        Entity,

        /// <summary>
        /// The open processing instruction.
        /// </summary>
        OpenProcessingInstruction,

        /// <summary>
        /// The close processing instruction.
        /// </summary>
        CloseProcessingInstruction,

        /// <summary>
        /// The c data begin.
        /// </summary>
        CDataBegin,

        /// <summary>
        /// The c data end.
        /// </summary>
        CDataEnd,

        /// <summary>
        /// The text content.
        /// </summary>
        TextContent,

        /// <summary>
        /// The Eof.
        /// </summary>
        Eof,
    }
}