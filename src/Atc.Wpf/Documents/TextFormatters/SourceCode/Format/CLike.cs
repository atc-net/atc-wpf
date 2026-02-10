namespace Atc.Wpf.Documents.TextFormatters.SourceCode.Format;

/// <summary>
/// Provides a base class for formatting languages similar to C.
/// </summary>
public abstract class CLike : Code
{
    /// <summary>
    /// Regular expression string to match single line and multi-line
    /// comments (// and /* */).
    /// </summary>
    protected override string CommentRegEx => @"/\*.*?\*/|//.*?(?=\r|\n)";

    /// <summary>
    /// Regular expression string to match string and character literals.
    /// </summary>
    protected override string StringRegEx
        => @"@?""""|@?"".*?(?!\\).""|''|'.*?(?!\\).'";
}