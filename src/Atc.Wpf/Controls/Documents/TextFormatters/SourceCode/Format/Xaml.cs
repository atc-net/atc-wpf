namespace Atc.Wpf.Controls.Documents.TextFormatters.SourceCode.Format;

/// <summary>
/// XML tokenizer, tokens are designed to match Visual Studio syntax highlighting.
/// </summary>
[SuppressMessage("Design", "MA0048:File name must match type name", Justification = "OK.")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "OK.")]
internal class XmlTokenizer
{
    private string input = string.Empty;
    private int position;
    private XmlTokenizerMode mode = XmlTokenizerMode.OutsideElement;

    /// <summary>
    /// Gets the remaining text.
    /// </summary>
    /// <value>
    /// The remaining text.
    /// </value>
    public string RemainingText => this.input.Substring(this.position);

    public static List<XmlToken> Tokenize(string input)
    {
        var tokenizerMode = XmlTokenizerMode.OutsideElement;
        XmlTokenizer tokenizer = new XmlTokenizer();
        return tokenizer.Tokenize(input, ref tokenizerMode);
    }

    public List<XmlToken> Tokenize(string inputValue, ref XmlTokenizerMode tokenizerMode)
    {
        this.input = inputValue;
        this.mode = tokenizerMode;
        this.position = 0;
        List<XmlToken> result = Tokenize();
        return result;
    }

    private List<XmlToken> Tokenize()
    {
        var list = new List<XmlToken>();
        XmlToken token;
        do
        {
            token = this.NextToken();
            list.Add(token);
        }
        while (token.Kind != XmlTokenKind.Eof);
        return list;
    }

    private XmlToken NextToken()
    {
        if (this.position >= this.input.Length)
        {
            return new XmlToken(XmlTokenKind.Eof, 0);
        }

        XmlToken token;
        switch (this.mode)
        {
            case XmlTokenizerMode.AfterAttributeEquals:
                token = this.TokenizeAttributeValue();
                break;
            case XmlTokenizerMode.AfterAttributeName:
                token = this.TokenizeSimple("=", XmlTokenKind.Equals, XmlTokenizerMode.AfterAttributeEquals);
                break;
            case XmlTokenizerMode.AfterOpen:
                token = this.TokenizeName(XmlTokenKind.ElementName, XmlTokenizerMode.InsideElement);
                break;
            case XmlTokenizerMode.InsideCData:
                token = this.TokenizeInsideCData();
                break;
            case XmlTokenizerMode.InsideComment:
                token = this.TokenizeInsideComment();
                break;
            case XmlTokenizerMode.InsideElement:
                token = this.TokenizeInsideElement();
                break;
            case XmlTokenizerMode.InsideProcessingInstruction:
                token = this.TokenizeInsideProcessingInstruction();
                break;
            case XmlTokenizerMode.OutsideElement:
                token = this.TokenizeOutsideElement();
                break;
            default:
                token = new XmlToken(XmlTokenKind.Eof, 0);
                break;
        }

        return token;
    }

    private static bool IsNameCharacter(char character)
    {
        // XML rule: Letter | Digit | '.' | '-' | '_' | ':' | CombiningChar | Extender
        bool result = char.IsLetterOrDigit(character) || character == '.' || character == '-' || character == '_' || character == ':';
        return result;
    }

    private XmlToken TokenizeAttributeValue()
    {
        int closePosition = this.input.IndexOf(this.input[this.position], this.position + 1);
        var token = new XmlToken(XmlTokenKind.AttributeValue, closePosition + 1 - this.position);
        this.position = closePosition + 1;
        this.mode = XmlTokenizerMode.InsideElement;
        return token;
    }

    private XmlToken TokenizeName(XmlTokenKind kind, XmlTokenizerMode nextMode)
    {
        int i;
        for (i = this.position; i < this.input.Length; i++)
        {
            if (!IsNameCharacter(this.input[i]))
            {
                break;
            }
        }

        var token = new XmlToken(kind, i - this.position);
        this.mode = nextMode;
        this.position = i;
        return token;
    }

    private XmlToken TokenizeElementWhitespace()
    {
        int i;
        for (i = this.position; i < this.input.Length; i++)
        {
            if (!char.IsWhiteSpace(this.input[i]))
            {
                break;
            }
        }

        var token = new XmlToken(XmlTokenKind.ElementWhitespace, i - this.position);
        this.position = i;
        return token;
    }

    private bool StartsWith(string text)
    {
        if (this.position + text.Length > this.input.Length)
        {
            return false;
        }

        return this.input.Substring(this.position, text.Length) == text;
    }

    private XmlToken TokenizeInsideElement()
    {
        if (char.IsWhiteSpace(this.input[this.position]))
        {
            return this.TokenizeElementWhitespace();
        }

        if (this.StartsWith("/>"))
        {
            return this.TokenizeSimple("/>", XmlTokenKind.SelfClose, XmlTokenizerMode.OutsideElement);
        }

        return this.StartsWith(">")
            ? this.TokenizeSimple(">", XmlTokenKind.Close, XmlTokenizerMode.OutsideElement)
            : this.TokenizeName(XmlTokenKind.AttributeName, XmlTokenizerMode.AfterAttributeName);
    }

    private XmlToken TokenizeText()
    {
        int i;
        for (i = this.position; i < this.input.Length; i++)
        {
            if (this.input[i] == '<' || this.input[i] == '&')
            {
                break;
            }
        }

        var token = new XmlToken(XmlTokenKind.TextContent, i - this.position);
        this.position = i;
        return token;
    }

    private XmlToken TokenizeOutsideElement()
    {
        if (this.position >= this.input.Length)
        {
            return new XmlToken(XmlTokenKind.Eof, 0);
        }

        switch (this.input[this.position])
        {
            case '<':
                return this.TokenizeOpen();
            case '&':
                return this.TokenizeEntity();
            default:
                return this.TokenizeText();
        }
    }

    private XmlToken TokenizeSimple(string text, XmlTokenKind kind, XmlTokenizerMode nextMode)
    {
        var token = new XmlToken(kind, text.Length);
        this.position += text.Length;
        this.mode = nextMode;
        return token;
    }

    private XmlToken TokenizeOpen()
    {
        if (this.StartsWith("<!--"))
        {
            return this.TokenizeSimple("<!--", XmlTokenKind.CommentBegin, XmlTokenizerMode.InsideComment);
        }

        if (this.StartsWith("<![CDATA["))
        {
            return this.TokenizeSimple("<![CDATA[", XmlTokenKind.CDataBegin, XmlTokenizerMode.InsideCData);
        }

        if (this.StartsWith("<?"))
        {
            return this.TokenizeSimple("<?", XmlTokenKind.OpenProcessingInstruction, XmlTokenizerMode.InsideProcessingInstruction);
        }

        return this.StartsWith("</")
            ? this.TokenizeSimple("</", XmlTokenKind.OpenClose, XmlTokenizerMode.AfterOpen)
            : this.TokenizeSimple("<", XmlTokenKind.Open, XmlTokenizerMode.AfterOpen);
    }

    private XmlToken TokenizeEntity()
    {
        var token = new XmlToken(XmlTokenKind.Entity, this.input.IndexOf(';', this.position) - this.position);
        this.position += token.Length;
        return token;
    }

    private XmlToken TokenizeInsideProcessingInstruction()
    {
        int index = this.input.IndexOf("?>", this.position, StringComparison.Ordinal);
        if (this.position == index)
        {
            this.position += "?>".Length;
            this.mode = XmlTokenizerMode.OutsideElement;
            return new XmlToken(XmlTokenKind.CloseProcessingInstruction, "?>".Length);
        }

        var token = new XmlToken(XmlTokenKind.TextContent, index - this.position);
        this.position = index;
        return token;
    }

    private XmlToken TokenizeInsideCData()
    {
        int index = this.input.IndexOf("]]>", this.position, StringComparison.Ordinal);
        if (this.position == index)
        {
            this.position += "]]>".Length;
            this.mode = XmlTokenizerMode.OutsideElement;
            return new XmlToken(XmlTokenKind.CDataEnd, "]]>".Length);
        }

        var token = new XmlToken(XmlTokenKind.TextContent, index - this.position);
        this.position = index;
        return token;
    }

    private XmlToken TokenizeInsideComment()
    {
        int index = this.input.IndexOf("-->", this.position, StringComparison.Ordinal);
        if (this.position == index)
        {
            this.position += "-->".Length;
            this.mode = XmlTokenizerMode.OutsideElement;
            return new XmlToken(XmlTokenKind.CommentEnd, "-->".Length);
        }

        var token = new XmlToken(XmlTokenKind.CommentText, index - this.position);
        this.position = index;
        return token;
    }
}