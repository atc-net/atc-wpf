using System;
using System.Windows.Documents;

namespace Atc.Wpf.Controls.RichTextBoxFormatters
{
    /// <summary>
    /// Formats the RichTextBox text as plain text.
    /// </summary>
    public class PlainTextFormatter : ITextFormatter
    {
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

            new TextRange(document.ContentStart, document.ContentEnd).Text = text;
        }
    }
}