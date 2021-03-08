using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Media;

namespace Atc.Wpf.Controls.Documents.SourceCode.Format
{
    /// <summary>
    /// Provides a base class for formatting most programming languages.
    /// </summary>
    [SuppressMessage("Security", "MA0009:Add regex evaluation timeout", Justification = "OK.")]
    [SuppressMessage("Design", "MA0056:Do not call overridable members in constructor", Justification = "OK.")]
    [SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "OK.")]
    public abstract class Code : Source
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Code"/> class.
        /// </summary>
        protected Code()
        {
            // generate the keyword and preprocessor regexes from the keyword lists
            var r = new Regex(@"\w+|-\w+|#\w+|@@\w+|#(?:\\(?:s|w)(?:\*|\+)?\w+)+|@\\w\*+");
            var regKeyword = r.Replace(this.Keywords, @"(?<=^|\W)$0(?=\W)");
            var regPreproc = r.Replace(this.Preprocessors, @"(?<=^|\s)$0(?=\s|$)");
            r = new Regex(@" +");
            regKeyword = r.Replace(regKeyword, @"|");
            regPreproc = r.Replace(regPreproc, @"|");

            if (regPreproc.Length == 0)
            {
                // use something quite impossible...
                regPreproc = "(?!.*)_{37}(?<!.*)";
            }

            // build a master regex with capturing groups
            StringBuilder regAll = new StringBuilder();
            regAll.Append('(');
            regAll.Append(this.CommentRegEx);
            regAll.Append(")|(");
            regAll.Append(this.StringRegEx);
            if (regPreproc.Length > 0)
            {
                regAll.Append(")|(");
                regAll.Append(regPreproc);
            }

            regAll.Append(")|(");
            regAll.Append(regKeyword);
            regAll.Append(')');

            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            RegexOptions caseInsensitive = this.CaseSensitive ? 0 : RegexOptions.IgnoreCase;
            this.CodeRegex = new Regex(regAll.ToString(), RegexOptions.Singleline | caseInsensitive);
            this.CodeParagraphGlobal = new List<Run>();
        }

        /// <summary>
        /// Determines if the language is case sensitive.
        /// </summary>
        /// <value><b>true</b> if the language is case sensitive, <b>false</b>
        /// otherwise. The default is true.</value>
        /// <remarks>
        /// A case-insensitive language formatter must override this
        /// property to return false.
        /// </remarks>
        public virtual bool CaseSensitive => true;

        /// <summary>
        /// Must be overridden to provide a list of keywords defined in
        /// each language.
        /// </summary>
        /// <remarks>
        /// Keywords must be separated with spaces.
        /// </remarks>
        protected abstract string Keywords { get; }

        /// <summary>
        /// Can be overridden to provide a list of preprocessors defined in
        /// each language.
        /// </summary>
        /// <remarks>
        /// Preprocessors must be separated with spaces.
        /// </remarks>
        protected virtual string Preprocessors => string.Empty;

        /// <summary>
        /// Must be overridden to provide a regular expression string
        /// to match strings literals.
        /// </summary>
        protected abstract string StringRegEx { get; }

        /// <summary>
        /// Must be overridden to provide a regular expression string
        /// to match comments.
        /// </summary>
        protected abstract string CommentRegEx { get; }

        /// <summary>
        /// Called to evaluate the HTML fragment corresponding to each
        /// matching token in the code.
        /// </summary>
        /// <param name="match">The <see cref="Match" /> resulting from a
        /// single regular expression match.</param>
        /// <returns>A string containing the HTML code fragment.</returns>
        [SuppressMessage("Design", "MA0051:Method is too long", Justification = "OK.")]
        protected override string MatchEval(Match match)
        {
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }

            if (match.Groups[1].Success)
            {
                var reader = new StringReader(match.ToString());
                var sb = new StringBuilder();
                string line;
                bool firstLineRead = false;
                while ((line = reader.ReadLine()!) != null)
                {
                    if (firstLineRead)
                    {
                        sb.Append('\r');
                    }

                    sb.Append(line);
                    firstLineRead = true;
                }

                if (!string.IsNullOrEmpty(sb.ToString()))
                {
                    Run run = new Run(sb.ToString())
                    {
                        Foreground = new SolidColorBrush(Color.FromRgb(0, 128, 0)),
                    };

                    this.CodeParagraphGlobal.Add(run);
                }

                return "::::::";
            }

            // string literal
            if (match.Groups[2].Success)
            {
                var run = new Run(match.ToString())
                {
                    Foreground = new SolidColorBrush(Color.FromRgb(0, 96, 128)),
                };

                this.CodeParagraphGlobal.Add(run);
                return "::::::";
            }

            // preprocessor keyword
            if (match.Groups[3].Success)
            {
                var run = new Run(match.ToString())
                {
                    Foreground = new SolidColorBrush(Color.FromRgb(204, 102, 51)),
                };

                this.CodeParagraphGlobal.Add(run);
                return "::::::";
            }

            // keyword
            if (match.Groups[4].Success)
            {
                var run = new Run(match.ToString())
                {
                    Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 255)),
                };

                this.CodeParagraphGlobal.Add(run);
                return "::::::";
            }

            return string.Empty;
        }
    }
}