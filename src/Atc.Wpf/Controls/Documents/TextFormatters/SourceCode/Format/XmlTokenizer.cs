namespace Atc.Wpf.Controls.Documents.TextFormatters.SourceCode.Format;

/// <summary>
/// XML tokenizer, tokens are designed to match Visual Studio syntax highlighting.
/// </summary>
internal sealed class XmlTokenizer
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
    public string RemainingText => input.Substring(position);

    public static List<XmlToken> Tokenize(string input)
    {
        var tokenizerMode = XmlTokenizerMode.OutsideElement;
        var tokenizer = new XmlTokenizer();
        return tokenizer.Tokenize(input, ref tokenizerMode);
    }

    public List<XmlToken> Tokenize(string inputValue, ref XmlTokenizerMode tokenizerMode)
    {
        input = inputValue;
        mode = tokenizerMode;
        position = 0;
        var result = Tokenize();
        return result;
    }

    private List<XmlToken> Tokenize()
    {
        var list = new List<XmlToken>();
        XmlToken token;
        do
        {
            token = NextToken();
            list.Add(token);
        }
        while (token.Kind != XmlTokenKind.Eof);
        return list;
    }

    private XmlToken NextToken()
    {
        if (position >= input.Length)
        {
            return new XmlToken(XmlTokenKind.Eof, 0);
        }

        var token = mode switch
        {
            XmlTokenizerMode.AfterAttributeEquals => TokenizeAttributeValue(),
            XmlTokenizerMode.AfterAttributeName => TokenizeSimple("=", XmlTokenKind.Equals, XmlTokenizerMode.AfterAttributeEquals),
            XmlTokenizerMode.AfterOpen => TokenizeName(XmlTokenKind.ElementName, XmlTokenizerMode.InsideElement),
            XmlTokenizerMode.InsideCData => TokenizeInsideCData(),
            XmlTokenizerMode.InsideComment => TokenizeInsideComment(),
            XmlTokenizerMode.InsideElement => TokenizeInsideElement(),
            XmlTokenizerMode.InsideProcessingInstruction => TokenizeInsideProcessingInstruction(),
            XmlTokenizerMode.OutsideElement => TokenizeOutsideElement(),
            _ => new XmlToken(XmlTokenKind.Eof, 0),
        };

        return token;
    }

    private static bool IsNameCharacter(char character)
    {
        // XML rule: Letter | Digit | '.' | '-' | '_' | ':' | CombiningChar | Extender
        var result = char.IsLetterOrDigit(character) || character == '.' || character == '-' || character == '_' || character == ':';
        return result;
    }

    private XmlToken TokenizeAttributeValue()
    {
        var closePosition = input.IndexOf(input[position], position + 1);
        var token = new XmlToken(XmlTokenKind.AttributeValue, closePosition + 1 - position);
        position = closePosition + 1;
        mode = XmlTokenizerMode.InsideElement;
        return token;
    }

    private XmlToken TokenizeName(XmlTokenKind kind, XmlTokenizerMode nextMode)
    {
        int i;
        for (i = position; i < input.Length; i++)
        {
            if (!IsNameCharacter(input[i]))
            {
                break;
            }
        }

        var token = new XmlToken(kind, i - position);
        mode = nextMode;
        position = i;
        return token;
    }

    private XmlToken TokenizeElementWhitespace()
    {
        int i;
        for (i = position; i < input.Length; i++)
        {
            if (!char.IsWhiteSpace(input[i]))
            {
                break;
            }
        }

        var token = new XmlToken(XmlTokenKind.ElementWhitespace, i - position);
        position = i;
        return token;
    }

    private bool StartsWith(string text)
    {
        if (position + text.Length > input.Length)
        {
            return false;
        }

        return input.Substring(position, text.Length) == text;
    }

    private XmlToken TokenizeInsideElement()
    {
        if (char.IsWhiteSpace(input[position]))
        {
            return TokenizeElementWhitespace();
        }

        if (StartsWith("/>"))
        {
            return TokenizeSimple("/>", XmlTokenKind.SelfClose, XmlTokenizerMode.OutsideElement);
        }

        return StartsWith(">")
            ? TokenizeSimple(">", XmlTokenKind.Close, XmlTokenizerMode.OutsideElement)
            : TokenizeName(XmlTokenKind.AttributeName, XmlTokenizerMode.AfterAttributeName);
    }

    private XmlToken TokenizeText()
    {
        int i;
        for (i = position; i < input.Length; i++)
        {
            if (input[i] == '<' || input[i] == '&')
            {
                break;
            }
        }

        var token = new XmlToken(XmlTokenKind.TextContent, i - position);
        position = i;
        return token;
    }

    private XmlToken TokenizeOutsideElement()
    {
        if (position >= input.Length)
        {
            return new XmlToken(XmlTokenKind.Eof, 0);
        }

        return input[position] switch
        {
            '<' => TokenizeOpen(),
            '&' => TokenizeEntity(),
            _ => TokenizeText(),
        };
    }

    private XmlToken TokenizeSimple(string text, XmlTokenKind kind, XmlTokenizerMode nextMode)
    {
        var token = new XmlToken(kind, text.Length);
        position += text.Length;
        mode = nextMode;
        return token;
    }

    private XmlToken TokenizeOpen()
    {
        if (StartsWith("<!--"))
        {
            return TokenizeSimple("<!--", XmlTokenKind.CommentBegin, XmlTokenizerMode.InsideComment);
        }

        if (StartsWith("<![CDATA["))
        {
            return TokenizeSimple("<![CDATA[", XmlTokenKind.CDataBegin, XmlTokenizerMode.InsideCData);
        }

        if (StartsWith("<?"))
        {
            return TokenizeSimple("<?", XmlTokenKind.OpenProcessingInstruction, XmlTokenizerMode.InsideProcessingInstruction);
        }

        return StartsWith("</")
            ? TokenizeSimple("</", XmlTokenKind.OpenClose, XmlTokenizerMode.AfterOpen)
            : TokenizeSimple("<", XmlTokenKind.Open, XmlTokenizerMode.AfterOpen);
    }

    private XmlToken TokenizeEntity()
    {
        var token = new XmlToken(XmlTokenKind.Entity, input.IndexOf(';', position) - position);
        position += token.Length;
        return token;
    }

    private XmlToken TokenizeInsideProcessingInstruction()
    {
        var index = input.IndexOf("?>", position, StringComparison.Ordinal);
        if (position == index)
        {
            position += "?>".Length;
            mode = XmlTokenizerMode.OutsideElement;
            return new XmlToken(XmlTokenKind.CloseProcessingInstruction, "?>".Length);
        }

        var token = new XmlToken(XmlTokenKind.TextContent, index - position);
        position = index;
        return token;
    }

    private XmlToken TokenizeInsideCData()
    {
        var index = input.IndexOf("]]>", position, StringComparison.Ordinal);
        if (position == index)
        {
            position += "]]>".Length;
            mode = XmlTokenizerMode.OutsideElement;
            return new XmlToken(XmlTokenKind.CDataEnd, "]]>".Length);
        }

        var token = new XmlToken(XmlTokenKind.TextContent, index - position);
        position = index;
        return token;
    }

    private XmlToken TokenizeInsideComment()
    {
        var index = input.IndexOf("-->", position, StringComparison.Ordinal);
        if (position == index)
        {
            position += "-->".Length;
            mode = XmlTokenizerMode.OutsideElement;
            return new XmlToken(XmlTokenKind.CommentEnd, "-->".Length);
        }

        var token = new XmlToken(XmlTokenKind.CommentText, index - position);
        position = index;
        return token;
    }
}