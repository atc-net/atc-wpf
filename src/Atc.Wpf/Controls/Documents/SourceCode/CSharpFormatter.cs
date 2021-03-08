using System;
using System.Windows.Documents;
using Atc.Wpf.Controls.Documents.SourceCode.Format;
using Atc.Wpf.Controls.RichTextBoxFormatters;

namespace Atc.Wpf.Controls.Documents.SourceCode
{
    /// <summary>
    /// Formats the RichTextBox text as colored C#.
    /// </summary>
    public class CSharpFormatter : ITextFormatter
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly CSharpFormatter Instance = new CSharpFormatter();

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>The text.</returns>
        public string GetText(FlowDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            return new TextRange(document.ContentStart, document.ContentEnd).Text;
        }

        /// <summary>
        /// Sets the text.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="text">The text.</param>
        public void SetText(FlowDocument document, string text)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            document.Blocks.Clear();
            document.SetCurrentValue(FlowDocument.PageWidthProperty, 2500D);
            var cSharp = new CSharp();
            var p = cSharp.FormatCode(text);
            document.Blocks.Add(p);
        }
    }
}